using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;

public static class SpriteManager {

	public static Dictionary<string, List<GameObject>> LoadedTilesets = new Dictionary<string, List<GameObject>>();

	public static string TilesetBaseDirectory = "Assets/Resources/Prefabs/Tiles/";

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

	private static void LoadTileset(string path) {
		if (!LoadedTilesets.ContainsKey(path)) {
			GameObject[] tiles = Resources.LoadAll("Prefabs/Tiles/" + path).OfType<GameObject>().ToArray();
			List<GameObject> tileset = new List<GameObject>();
			foreach (GameObject g in tiles) {
				tileset.Add(g);
			}
			LoadedTilesets.Add(path, tileset);
		}
	}

    public static void CacheTileset(string atlasName) {
        if (atlasName.Contains("_0")) //Sort of hard coded for a _0 that exists for no reason i know
            atlasName = atlasName.Substring(0, atlasName.Length - 2);

        if (!DoesTilesetExist(atlasName)) {
            //Debug.Log("Tileset doesnt exist, building");
            CreateTileset(atlasName);
        }

        if (DoesTilesetExist(atlasName) && !LoadedTilesets.ContainsKey(atlasName)) {
            //Debug.Log("Tileset exists and is not loaded already");
            LoadTileset(atlasName);
        }
    }

	public static Tile LoadTileInTileset(string tileName, string atlasName, int depth = 0) { //Atlas name is like "tiles_masonry_1x1_d_0"
        if (atlasName.Contains("_0")) //Sort of hard coded for a _0 that exists for no reason i know
            atlasName = atlasName.Substring(0, atlasName.Length - 2);

        CacheTileset(atlasName);

		List<GameObject> tileset;
		LoadedTilesets.TryGetValue(atlasName, out tileset);
		if (tileset != null && tileset.Count > 0) {
			GameObject tile = tileset.FindByName(tileName);
			if (tile != null)
				return tile.GetComponent<Tile>();
		}
        LoadedTilesets.Clear();
        if (depth == 0)
            return LoadTileInTileset(tileName, atlasName, depth++);
        Debug.LogErrorFormat("[SpriteManager] Failed to load tile {0} in tileset {1}", tileName, atlasName);
		return null;
	}

	public static bool DoesTilesetExist(string atlasName) {
		//TODO: check that tileset is up to date/has all sprites/has up to date base prefab/etc
		return AssetDatabase.IsValidFolder(TilesetBaseDirectory + atlasName);
	}

	public static void CreateTileset(string atlasName) {
		string[] paths = Directory.GetFiles(Application.dataPath + "/assets/tiles/", "*.png", SearchOption.AllDirectories);
		Sprite[] tileSprites = new Sprite[0];
		for (int i = 0; i < paths.Length; i++) {
			string p = "Assets" + paths[i].Replace(Application.dataPath, "").Replace('\\', '/');
			if (p.Contains(atlasName)) {
				tileSprites = AssetDatabase.LoadAllAssetsAtPath(p).OfType<Sprite>().ToArray();
				break;
			}
		}

		string guid = AssetDatabase.CreateFolder(TilesetBaseDirectory.Substring(0, TilesetBaseDirectory.Length-1), atlasName);
		string folderPath = AssetDatabase.GUIDToAssetPath(guid);

		for (int i = 0; i < tileSprites.Length; i++) {
			Sprite s = tileSprites[i];
			var prefab = PrefabUtility.CreateEmptyPrefab(folderPath + "/" + s.name + ".prefab");
			var basePrefab = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/TileBase.prefab", typeof(GameObject));
			GameObject g = PrefabUtility.ReplacePrefab(basePrefab, prefab, ReplacePrefabOptions.ConnectToPrefab);
            g.GetComponent<ISprite>().ChangeSprite(s);
		}
	}

	public static void UnloadAllTilesets() {
		LoadedTilesets.Clear();
	}
}
