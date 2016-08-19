using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;

public static class SpriteManager {

	public static Dictionary<string, List<GameObject>> LoadedTilesets = new Dictionary<string, List<GameObject>>();

	public static string TilesetBaseDirectory = "Assets/Resources/Prefabs/";

    public static void UpdateMaterial<T>(Sprite s, Material m) {
        CacheTileset<T>(s.name);
        string spriteName = s.name;
        if (spriteName.Contains("_0"))
            spriteName = spriteName.Substring(0, spriteName.Length - 2);

        List<GameObject> prefabs = new List<GameObject>();
        LoadedTilesets.TryGetValue(spriteName, out prefabs);
        if (prefabs != null) {
            for (int i = 0; i < prefabs.Count; i++) {
                GameObject g = prefabs[i];
                g.GetComponent<SpriteRenderer>().sharedMaterial = m;
            }
        }
    }

    public static void ForceUpdateTiles() {
        GameData data = (GameData)Resources.Load("GameData", typeof(GameData));

        List<string> createdTilesets = new List<string>();
        createdTilesets = Directory.GetDirectories(TilesetBaseDirectory).OfType<string>().ToList<string>();
        for (int i = 0; i < createdTilesets.Count; i++) {
            createdTilesets[i] = createdTilesets[i].Remove(0, TilesetBaseDirectory.Length);
        }

        for (int i = 0; i < createdTilesets.Count; i++) {
            GameObject[] tiles = Resources.LoadAll("Prefabs/Tiles/" + createdTilesets[i]).OfType<GameObject>().ToArray();
            for (int j = 0; j < tiles.Length; j++) {
                tiles[j].GetComponent<Tile>().ForceUpdateTile(data);
            }
        }
    }

	private static void LoadTileset(string path, string folderName) {
		if (!LoadedTilesets.ContainsKey(path)) {
            GameObject[] tiles = Resources.LoadAll("Prefabs/" + folderName + path).OfType<GameObject>().ToArray();
			List<GameObject> tileset = new List<GameObject>();
			foreach (GameObject g in tiles) {
				tileset.Add(g);
			}
			LoadedTilesets.Add(path, tileset);
		}
	}

    public static void CacheTileset<T>(string atlasName) {
        if (atlasName.Contains("_0")) //Sort of hard coded for a _0 that exists for no reason i know
            atlasName = atlasName.Substring(0, atlasName.Length - 2);

        string folderName = "Tiles/";
        string prefabName = "TileBase.prefab";
        Type type = typeof(T);
        if (typeof(Tile) == type) {
            folderName = "Tiles/";
            prefabName = "TileBase.prefab";
        } else if (typeof(Pipe) == type) {
            folderName = "Pipes/";
            prefabName = "PipeBase.prefab";
        } else if (typeof(SpriteObject) == type) {
            folderName = "Objects/";
            prefabName = "SpriteObjectBase.prefab";
        }

        if (!DoesTilesetExist(atlasName, folderName)) {
            //Debug.Log("Tileset doesnt exist, building");
            CreateTileset(atlasName, folderName, prefabName);
        }

        if (DoesTilesetExist(atlasName, folderName) && !LoadedTilesets.ContainsKey(atlasName)) {
            //Debug.Log("Tileset exists and is not loaded already");
            LoadTileset(atlasName, folderName);
        }
    }

	public static T LoadTileInTileset<T>(string tileName, string atlasName, int depth = 0) { //Atlas name is like "tiles_masonry_1x1_d_0"
        if (atlasName.Contains("_0")) //Sort of hard coded for a _0 that exists for no reason i know
            atlasName = atlasName.Substring(0, atlasName.Length - 2);
        CacheTileset<T>(atlasName);

		List<GameObject> tileset;
		LoadedTilesets.TryGetValue(atlasName, out tileset);
		if (tileset != null && tileset.Count > 0) {
			GameObject tile = tileset.FindByName(tileName);
            if (tile != null) {
                T temp = tile.GetComponent<T>();
                if (temp != null)
                    return temp;
            }
		}
        LoadedTilesets.Clear();
        if (depth == 0)
            return LoadTileInTileset<T>(tileName, atlasName, depth+1);
        Debug.LogErrorFormat("[SpriteManager] Failed to load tile {0} in tileset {1}", tileName, atlasName);
		return default(T);
	}

    //"Assets/Resources/Prefabs/" + "Tiles/" + "tiles_masonry_1x1_d"
    //"Assets/Resources/Prefabs/Tiles/tiles_masonry_1x1_d"
    public static bool DoesTilesetExist(string atlasName, string folderName) {
		//TODO: check that tileset is up to date/has all sprites/has up to date base prefab/etc
		return AssetDatabase.IsValidFolder(TilesetBaseDirectory + folderName + atlasName);
	}

	public static void CreateTileset(string atlasName, string folderName, string prefabName) {
		string[] paths = Directory.GetFiles(Application.dataPath + "/assets/" + folderName, "*.png", SearchOption.AllDirectories);
		Sprite[] tileSprites = new Sprite[0];
		for (int i = 0; i < paths.Length; i++) {
			string p = "Assets" + paths[i].Replace(Application.dataPath, "").Replace('\\', '/');
			if (p.Contains(atlasName)) {
				tileSprites = AssetDatabase.LoadAllAssetsAtPath(p).OfType<Sprite>().ToArray();
				break;
			}
		}
		string guid = AssetDatabase.CreateFolder(TilesetBaseDirectory + folderName.Substring(0, folderName.Length-1), atlasName);
		string folderPath = AssetDatabase.GUIDToAssetPath(guid);

		for (int i = 0; i < tileSprites.Length; i++) {
			Sprite s = tileSprites[i];
			var prefab = PrefabUtility.CreateEmptyPrefab(folderPath + "/" + s.name + ".prefab");
			var basePrefab = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/" + prefabName, typeof(GameObject));
            GameObject g = PrefabUtility.ReplacePrefab(basePrefab, prefab, ReplacePrefabOptions.ConnectToPrefab);
            if (g.GetComponent<ISprite>() != null)
                g.GetComponent<ISprite>().ChangeSprite(s);
		}
	}

	public static void UnloadAllTilesets() {
		LoadedTilesets.Clear();
	}
}
