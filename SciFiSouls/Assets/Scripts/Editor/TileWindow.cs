using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Sprites;
using UnityEditor;
using UnityEditor.Sprites;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;

public class TileWindow : EditorWindow {
	[MenuItem ("Tile/Tile Maker")]
	public static void  ShowWindow () {
		TileWindow window = (TileWindow)EditorWindow.GetWindow(typeof(TileWindow));
		window.titleContent.text = "Tile Maker";
		window.Show();

		window.data = (GameData)Resources.Load("GameData", typeof(GameData));
		window.minSize = new Vector2(360, 500);
		window.window = window;
		//window.CurrentTile = (Tile)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/TestTile.prefab", typeof(Tile));

		window.LoadPlayerPrefs();
		window.LoadTextureAtlases();
		window.SetBackgroundTexture(window.TextureSelectionColor);
	}

	GameData data;
	TileWindow window;
	Tile CurrentTile;

	int SpriteSize = 16;
	int PixelPadding = 2;
	float SpriteScale = 2f;
	float LastSpriteScale = 1f;
	GUIStyle SelectionGridStyle;

	int SelectedTextureIndex = 0;
	int SublayerIndex = 0;
	int SelectedLayerIndex = 0;
	List<Texture2D> DisplayedTextures = new List<Texture2D>();
	List<Sprite> TextureAtlases = new List<Sprite>();
	List<Texture2D> DisplayedTextureAtlases = new List<Texture2D>();
	Sprite[] DisplayedSprites;
	Texture2D SelectionTexture;
	string[] AtlasPaths;
	Sprite CurrentAtlas;
	public Vector2 ScrollPosition = Vector2.zero;
	Texture2D TextureSelectionBackground;
	Color TextureSelectionColor;
	Color TempTextureSelectionColor;
	List<string> layersDisplayed;

	int lastBoxWidth;
	int lastBoxHeight;
	int boxWidth;
	int boxHeight;

	void OnGUI () {
		if (CurrentTile != null) {
			DrawTileEditor();
		} else {
			DrawNullTileEditor();
		}

		if (SelectionGridStyle == null)
			SetSelectionGridStyle();

		boxWidth = (int)window.position.width/2;
		boxHeight = (int)window.position.height/2;

		if (boxWidth != lastBoxWidth || boxHeight != lastBoxHeight) {
			SetBackgroundTexture(TextureSelectionColor, true);
		}

		lastBoxWidth = boxWidth;
		lastBoxHeight = boxHeight;

		GUILayout.BeginArea(new Rect(0, boxHeight, window.position.width, 40));
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Scale", GUILayout.MaxWidth(35));
		SpriteScale = EditorGUILayout.Slider(SpriteScale, 1f, 4f, GUILayout.MaxWidth(200));
		EditorGUILayout.LabelField("Padding", GUILayout.MaxWidth(50));
		PixelPadding = EditorGUILayout.IntSlider(PixelPadding, 2, 10, GUILayout.MaxWidth(200));
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.BeginHorizontal();
		if (DisplayedTextures.Count > 0 && GUILayout.Button("Back", GUILayout.MinWidth(100), GUILayout.MaxWidth(200))) {
			GoToAtlasSelection();
		}
		EditorGUILayout.LabelField("Background Color", GUILayout.MaxWidth(120));
		SetBackgroundTexture(EditorGUILayout.ColorField(TextureSelectionColor, GUILayout.MinWidth(100)));
		EditorGUILayout.EndHorizontal();
		GUILayout.EndArea();
		GUILayout.BeginArea(new Rect(0, boxHeight + 40, window.position.width, window.position.height), TextureSelectionBackground);
		ScrollPosition = EditorGUILayout.BeginScrollView(ScrollPosition, false, true, GUILayout.Height(boxHeight-40));
		ShowSpriteThumbnails();
		EditorGUILayout.EndScrollView();
		GUILayout.EndArea();
	}

    void SelectTile(int i) {
        if (CurrentAtlas == null || DisplayedSprites.Count() <= 0)
            return;

        Sprite s = DisplayedSprites[i];
        string tileName = s.name;

        CurrentTile = SpriteManager.LoadTileInTileset(tileName, CurrentAtlas.name);
        //Debug.Log(CurrentTile);
        if (CurrentTile == null) { 
            return;
        }
		TileHelper._instance.UpdateSelectedTile(CurrentTile.gameObject);
		if (CurrentTile != null) {
			SelectedLayerIndex = data.GetLayerIndex(CurrentTile);
			SublayerIndex = CurrentTile.SubLayer;
		} else {
			CurrentTile = null;
			SelectedLayerIndex = 0;
			SublayerIndex = 0;
		}

	}

	void UpdateTile() {
		SelectTile(SelectedTextureIndex);
	}

	void DrawTileEditor() {
		GUILayout.BeginArea(new Rect(0, 0, boxWidth, boxHeight));
		EditorGUILayout.LabelField("Tile Name: " + CurrentTile.name);

		if (GUILayout.Button("Select Prefab")) {
			Selection.activeObject = CurrentTile.gameObject;
		}

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Destructable", GUILayout.MinWidth(boxWidth/2), GUILayout.MaxWidth(120));
		CurrentTile.destructable = EditorGUILayout.Toggle(CurrentTile.destructable);
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.BeginHorizontal();
		if (CurrentTile.destructable) {
			EditorGUILayout.LabelField("Health", GUILayout.MaxWidth(40));
			CurrentTile.MaxHealth = EditorGUILayout.IntSlider(CurrentTile.MaxHealth, 1, 100);
		}
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Trip Threshold", GUILayout.MinWidth(boxWidth/2), GUILayout.MaxWidth(120));
		CurrentTile.TripThresholdType = (Tile.TripThreshold)EditorGUILayout.EnumPopup(CurrentTile.TripThresholdType, GUILayout.MaxWidth(150));
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Walking Noise", GUILayout.MinWidth(boxWidth/2), GUILayout.MaxWidth(120));
		CurrentTile.WalkingNoiseType = (Tile.WalkingNoise)EditorGUILayout.EnumPopup(CurrentTile.WalkingNoiseType, GUILayout.MaxWidth(150));
		EditorGUILayout.EndHorizontal();
		GUILayout.EndArea();

		DrawRightBox();
	}

	void DrawRightBox() {
		GUILayout.BeginArea(new Rect(window.position.width/2f, 0, window.window.position.width/2f, window.window.position.height/2f));
		DrawLayerEditor ();
		DrawCollisionEditor();
		GUILayout.EndArea();
	}

	void DrawLayerEditor () {
		EditorGUILayout.LabelField ("Main Layers");
		if (layersDisplayed != data.layers) {
			UpdateTile ();
			string[] newLayers = new string[data.layers.Count];
			data.layers.CopyTo (newLayers);
			layersDisplayed = newLayers.ToList ();
		}
		SelectedLayerIndex = GUILayout.SelectionGrid (SelectedLayerIndex, layersDisplayed.ToArray (), 1, GUILayout.MaxWidth (boxWidth));
		EditorGUILayout.LabelField ("Sub-Layer (Can be ANY integer value)");
		SublayerIndex = EditorGUILayout.IntField (SublayerIndex);

        CurrentTile.BaseLayerName = data.GetLayerName(SelectedLayerIndex);
        CurrentTile.SubLayer = SublayerIndex;
        CurrentTile.ForceUpdateTile(data);
	}

	void DrawCollisionEditor() {
		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Tile Collision");

		EditorGUI.indentLevel++;

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Collision Enabled?", GUILayout.MaxWidth(150));
		PolygonCollider2D collider = CurrentTile.GetComponent<PolygonCollider2D>();
		bool colliding = collider.enabled;
		CurrentTile.GetComponent<PolygonCollider2D>().enabled = EditorGUILayout.Toggle(colliding);
		EditorGUILayout.EndHorizontal();
		if (colliding) {
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Is Trigger?", GUILayout.MaxWidth(150));
			collider.isTrigger = EditorGUILayout.Toggle(collider.isTrigger);
			EditorGUILayout.EndHorizontal();
		}
		if (colliding && GUILayout.Button("(NYI) Modify Collision (NYI)")) {
			Selection.activeObject = CurrentTile;
		}

		EditorGUI.indentLevel--;
	}

	void DrawNullTileEditor() {
		GUILayout.BeginArea(new Rect(0, 0, window.position.width, boxHeight));
		EditorGUILayout.LabelField("No Tile selected, select one below to edit tile");
		GUILayout.EndArea();
	}

	void ShowSpriteThumbnails() {
		UpdateSelectionGridStyle();

		if (DisplayedTextures.Count > 0) {
			if (LastSpriteScale != SpriteScale) {
				int imageSize = Mathf.FloorToInt(SpriteSize * Mathf.Max(SpriteScale, 1));
				LoadSpritesheets(imageSize, AtlasPaths[TextureAtlases.IndexOf(CurrentAtlas)]);
				LastSpriteScale = SpriteScale;
			}
			int size = (int)SelectionGridStyle.CalcSize(new GUIContent(DisplayedTextures[0])).x;
			int numColumns = Mathf.FloorToInt((window.position.width)/(size));
			int newIndex = GUILayout.SelectionGrid(SelectedTextureIndex, DisplayedTextures.ToArray(), numColumns, SelectionGridStyle, GUILayout.MaxWidth(window.position.width - 20));
			if (newIndex != SelectedTextureIndex) {
				SelectedTextureIndex = newIndex;
				SelectTile(SelectedTextureIndex);
			}
		} else if (TextureAtlases.Count > 0) {
			if (LastSpriteScale != SpriteScale) {
				LoadTextureAtlases();
				LastSpriteScale = SpriteScale;
			}
			for (int i = 0; i < TextureAtlases.Count; i++) {
				Sprite s = TextureAtlases[i];
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField(s.name, GUILayout.MaxWidth(window.position.width/3f));
				if (GUILayout.Button(DisplayedTextureAtlases[i], GUILayout.Width(window.position.width*(2f/3f) - 30))) {
					CurrentAtlas = s;
					ScrollPosition = Vector2.zero;
					SelectedTextureIndex = 0;
					LoadSpritesheets(Mathf.FloorToInt(SpriteSize * Mathf.Max(SpriteScale, 1)), AtlasPaths[i]);
					SelectTile(0);
				}
				EditorGUILayout.EndHorizontal();
			}
		}
	}

	void LoadTextureAtlases() {
		TextureAtlases.Clear();
		DisplayedTextureAtlases.Clear();

		string[] paths = Directory.GetFiles(Application.dataPath + "/assets/tiles/", "*.png", SearchOption.AllDirectories);
		List<string> validPaths = new List<string>();

		for (int i = 0; i < paths.Length-1; i++) {
			paths[i] = "Assets" + paths[i].Replace(Application.dataPath, "").Replace('\\', '/');
			Sprite s = (Sprite)AssetDatabase.LoadAssetAtPath(paths[i], typeof(Sprite));
			if (s is Sprite) {
				SpriteScale /= 2;
				Texture2D displayText = new Texture2D(s.texture.width, s.texture.height);
				displayText.SetPixels(s.texture.GetPixels());
				displayText.Apply();

				TextureScale.Bilinear(displayText, (int)(s.texture.width*SpriteScale), (int)(s.texture.height*SpriteScale));
				Color[] colors = displayText.GetPixels(0, 0, (int)(s.texture.width*SpriteScale), (int)(s.texture.height*SpriteScale));
				displayText = new Texture2D((int)(s.texture.width*SpriteScale), (int)(s.texture.height*SpriteScale), TextureFormat.ARGB32, false);
				displayText.SetPixels(colors);
				displayText.Apply();

				DisplayedTextureAtlases.Add(displayText);
				TextureAtlases.Add(s);
				validPaths.Add(paths[i]);
				SpriteScale *= 2;
			}
		}
		AtlasPaths = validPaths.ToArray();
		//Debug.Log("loaded " + DisplayedTextureAtlases.Count + " + atlases");
	}

	void LoadSpritesheets(int size, string path) {
		DisplayedSprites = AssetDatabase.LoadAllAssetsAtPath(path).OfType<Sprite>().ToArray();
		DisplayedTextures.Clear();

		for (int i = 0; i < DisplayedSprites.Length - 1; i++) {
			Sprite s = DisplayedSprites[i];
			//Debug.Log(s.name);
			Color[] colors = s.texture.GetPixels((int)s.rect.x, (int)s.rect.y, (int)s.rect.width, (int)s.rect.height);
			Texture2D t = new Texture2D((int)s.rect.width, (int)s.rect.height, TextureFormat.ARGB32, false);
			t.SetPixels(colors);
			t.Apply();

			TextureScale.Bilinear(t, size, size);
			Color[] pix = t.GetPixels(0, 0, size, size);
			t = new Texture2D(size, size, TextureFormat.ARGB32, false);
			t.SetPixels(pix);
			t.Apply();

			DisplayedTextures.Add(t);
		}

		SelectionTexture = new Texture2D(size, size);
		for (int i = 0; i < size; i++) { //Bottom
			SelectionTexture.SetPixel(i, 0, Color.yellow); 
		}
		for (int i = 0; i < size; i++) { //Top
			SelectionTexture.SetPixel(i, size-1, Color.yellow); 
		}
		for (int i = 0; i < size; i++) { //Left
			SelectionTexture.SetPixel(0, i, Color.yellow); 
		}
		for (int i = 0; i < size; i++) { //Right
			SelectionTexture.SetPixel(size-1, i, Color.yellow); 
		}
		SelectionTexture.Apply();
	}

	void SetSelectionGridStyle() {
		SelectionGridStyle = new GUIStyle();
//		SelectionGridStyle = new GUIStyle(GUI.skin.button);
		SelectionGridStyle.padding.bottom = PixelPadding;
		SelectionGridStyle.padding.right = PixelPadding;

		SelectionGridStyle.onNormal.background = SelectionTexture; //Selected background
//		Vector2 size = SelectionGridStyle.CalcScreenSize(new Vector2(SpriteSize, SpriteSize));
//		SelectionGridStyle.contentOffset = new Vector2(-.5f, 0);
	}

	void UpdateSelectionGridStyle() {
		SelectionGridStyle.padding.bottom = PixelPadding;
		SelectionGridStyle.padding.right = PixelPadding;
//		SelectionGridStyle.margin.bottom = PixelPadding;
//		SelectionGridStyle.margin.right = PixelPadding;

		SelectionGridStyle.onNormal.background = SelectionTexture;
//		Vector2 size = SelectionGridStyle.CalcScreenSize(new Vector2(SpriteSize, SpriteSize));
		SelectionGridStyle.contentOffset = new Vector2(PixelPadding/2f, PixelPadding/2f);
	}

	void GoToAtlasSelection() {
		DisplayedTextures.Clear();
		CurrentAtlas = null;
		ScrollPosition = Vector2.zero;
		SelectedTextureIndex = 0;
		LoadTextureAtlases();
	}

	void SetBackgroundTexture(Color c, bool forceUpdate = false) {
		if (TempTextureSelectionColor == c && !forceUpdate) {
			return;
		}
		
		TextureSelectionColor = TempTextureSelectionColor = c;
		TextureSelectionBackground = new Texture2D((int)window.position.width, (int)window.position.height);
		for (int i = 0; i < window.position.width; i++) {
			for (int j = 0; j < window.position.height; j++) {
				TextureSelectionBackground.SetPixel(i, j, c);
			}
		}
		TextureSelectionBackground.Apply();
	}

	void LoadPlayerPrefs() {
		if (PlayerPrefs.HasKey("TextureSelectionBackgroundColor")) {
			string color = PlayerPrefs.GetString("TextureSelectionBackgroundColor");
			TextureSelectionColor = m_Utility.ColorUtil.ParseColor(color);
		} else TextureSelectionColor = Color.white;
	}

	void OnDestroy() {
		PlayerPrefs.SetString("TextureSelectionBackgroundColor", m_Utility.ColorUtil.GetParsableString(TextureSelectionColor));
		PlayerPrefs.Save();
	}

	public GameObject GetSelectedTile() {
		return CurrentTile.gameObject;
	}
}
