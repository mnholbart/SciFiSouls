using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.Sprites;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System;

//TODO: still needs to support "generic" types based on SpriteBase rather than having all these object/tile/pipe modes, if done need to edit SpriteManager as well
//TODO: still needs quite a bit of refactoring lots of inefficiencies, needs comments and or renaming on several ambiguous variables

public class TileWindow : EditorWindow {
	[MenuItem ("Editor/Tile Maker")]
	public static void  ShowWindow () {
		TileWindow window = (TileWindow)EditorWindow.GetWindow(typeof(TileWindow));
		window.titleContent.text = "Tile Maker";
		window.Show();

		window.data = (GameData)Resources.Load("GameData", typeof(GameData));
		window.minSize = new Vector2(500, 600);
		window.window = window;

		window.LoadPlayerPrefs();
		window.SetBackgroundTexture(window.TextureSelectionColor);
        window.SetEditMode(EditMode.None);
	}

    public enum EditMode {
        None,
        Tile, 
        Object,
        Pipe
    }
    public EditMode CurrentEditMode = EditMode.None;
    
    Tile CurrentTile;
    Pipe CurrentPipe;
    SpriteObject CurrentObject;

    List<SpriteDirectoryData> DirectoryData = new List<SpriteDirectoryData>();
    SpriteDirectoryData CurrentDirectory;

    GameData data;
	TileWindow window;

	int SpriteSize = 16;
	int PixelPadding = 2;
	float SpriteScale = 2f;
	float LastSpriteScale = 1f;
	GUIStyle SelectionGridStyle;
    
    int SelectedTextureIndex = 0;
	int SublayerIndex = 0;
	int SelectedLayerIndex = 0;
	Sprite[] DisplayedSprites;
    List<Texture2D> DisplayedSpriteTextures = new List<Texture2D>();
    Texture2D SelectionTexture;
	Sprite CurrentAtlas;
    int CurrentAtlasIndex = -1;
	public Vector2 ScrollPosition = Vector2.zero;
	Texture2D TextureSelectionBackground;
	Color TextureSelectionColor;
	Color TempTextureSelectionColor;
	List<string> layersDisplayed;

	int lastBoxWidth;
	int lastBoxHeight;
	int boxWidth;
	int boxHeight;

    CollisionEditorWindow collisionWindow;

    public void SetEditMode(EditMode newMode) {
        CurrentTile = null;
        CurrentPipe = null;
        CurrentObject = null;
        CurrentDirectory = null;
        DirectoryData.Clear();
        DisplayedSpriteTextures.Clear();

        if (newMode == EditMode.Pipe)
            CurrentAssetPath = AssetPathPipes;
        else if (newMode == EditMode.Tile)
            CurrentAssetPath = AssetPathTiles;
        else if (newMode == EditMode.Object) {
            CurrentAssetPath = AssetPathObjects;
        } else CurrentAssetPath = string.Empty;
        CurrentEditMode = newMode;
        
        CurrentAtlas = null;
        CurrentAtlasIndex = -1;
        ScrollPosition = Vector2.zero;
        SelectedTextureIndex = 0;

        LoadTextureAtlases();
    }

    void OnGUI () {
        if (CurrentEditMode == EditMode.None) {

        } else if (CurrentEditMode == EditMode.Tile) {
            DrawTileEditor();
        } else if (CurrentEditMode == EditMode.Pipe) {
            DrawPipeEditor();
        } else if (CurrentEditMode == EditMode.Object) {
            DrawObjectEditor();
        }
        DrawRightBox(); //TODO draw for diff selected types

        UpdateWindow();

        DrawSelectionWindow();

        if (collisionWindow != null)
            collisionWindow.Repaint();

        if (CurrentTile != null)
            EditorUtility.SetDirty(CurrentTile);
    }

    void DrawSelectionWindow() {
        GUILayout.BeginArea(new Rect(0, boxHeight, window.position.width, 40));
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Scale", GUILayout.MaxWidth(35));
        SpriteScale = EditorGUILayout.Slider(SpriteScale, 1f, 4f, GUILayout.MaxWidth(200));
        EditorGUILayout.LabelField("Padding", GUILayout.MaxWidth(50));
        PixelPadding = EditorGUILayout.IntSlider(PixelPadding, 2, 10, GUILayout.MaxWidth(200));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        if (CurrentEditMode != EditMode.None && GUILayout.Button("Back", GUILayout.MinWidth(100), GUILayout.MaxWidth(200))) {
            SetEditMode(EditMode.None);
        }
        EditorGUILayout.LabelField("Background Color", GUILayout.MaxWidth(120));
        SetBackgroundTexture(EditorGUILayout.ColorField(TextureSelectionColor, GUILayout.MinWidth(100)));
        EditorGUILayout.EndHorizontal();
        GUILayout.EndArea();
        GUILayout.BeginArea(new Rect(0, boxHeight + 40, window.position.width, window.position.height), TextureSelectionBackground);
        ScrollPosition = EditorGUILayout.BeginScrollView(ScrollPosition, false, true, GUILayout.Height(boxHeight - 40));
        if (CurrentEditMode != EditMode.None)
            DrawSelectionBox();
        else DrawEditModeSelection();
        EditorGUILayout.EndScrollView();
        GUILayout.EndArea();

    }

    void UpdateWindow() {
        if (SelectionGridStyle == null)
            SetSelectionGridStyle();

        boxWidth = (int)window.position.width / 2;
        boxHeight = (int)window.position.height / 2;

        if (boxWidth != lastBoxWidth || boxHeight != lastBoxHeight) {
            SetBackgroundTexture(TextureSelectionColor, true);
        }

        lastBoxWidth = boxWidth;
        lastBoxHeight = boxHeight;
    }

    void SelectTile(int i) {
        if (CurrentAtlas == null || DisplayedSprites.Count() == 0)
            return;

        TileHelper.data = data;
        Sprite s = DisplayedSprites[i];
        string tileName = s.name;

        if (CurrentEditMode == EditMode.Tile) {
            CurrentTile = SpriteManager.LoadTileInTileset<Tile>(tileName, CurrentAtlas.name);
            TileHelper._instance.UpdateSelectedTile(CurrentTile == null ? null: CurrentTile.GetComponent<SpriteBase>());
        } else if (CurrentEditMode == EditMode.Pipe) {
            CurrentPipe = SpriteManager.LoadTileInTileset<Pipe>(tileName, CurrentAtlas.name);
    		TileHelper._instance.UpdateSelectedTile(CurrentPipe == null ? null : CurrentPipe.GetComponent<SpriteBase>());
        } else if (CurrentEditMode == EditMode.Object) {
            CurrentObject = SpriteManager.LoadTileInTileset<SpriteObject>(tileName, CurrentAtlas.name);
    		TileHelper._instance.UpdateSelectedTile(CurrentObject == null ? null : CurrentObject.GetComponent<SpriteBase>());
        }
	}

	void UpdateTile() {
		SelectTile(SelectedTextureIndex);
	}

    void DrawObjectEditor() {
        if (CurrentObject == null) {
            DrawNullSelectionEditor();
            return;
        }

        GUILayout.BeginArea(new Rect(0, 0, boxWidth, boxHeight));
        leftScrollPos = EditorGUILayout.BeginScrollView(leftScrollPos, GUIStyle.none, GUIStyle.none, GUILayout.Width(boxWidth), GUILayout.Height(boxHeight));
        EditorGUILayout.LabelField("Pipe Name: " + CurrentObject.name);
        EditorGUILayout.LabelField("Pipe Default Parameters: ");
        if (GUILayout.Button("Select Prefab")) {
            Selection.activeObject = CurrentObject.gameObject;
        }

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Destructable", GUILayout.MinWidth(boxWidth / 2), GUILayout.MaxWidth(120));
        CurrentObject.destructable = EditorGUILayout.Toggle(CurrentObject.destructable);
        EditorGUILayout.EndHorizontal();
        if (CurrentObject.destructable) {
            TileDestructable destructable = CurrentObject.GetComponentInChildren<TileDestructable>();
            EditorGUI.indentLevel++;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Health", GUILayout.MaxWidth(60));
            CurrentObject.MaxHealth = EditorGUILayout.IntSlider(CurrentObject.MaxHealth, 1, 1000);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.LabelField("Destruction Phases");
            EditorGUI.indentLevel++;
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(position.width / 2), GUILayout.Height(Mathf.Min(120, destructable.phases.Count * 90)));
            DrawDestructionPhases(destructable);
            EditorGUILayout.EndScrollView();
            EditorGUI.indentLevel--;
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Is Cover", GUILayout.MaxWidth(80));
        CurrentTile.cover = EditorGUILayout.Toggle(CurrentObject.cover);
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Edit Collider"))
            OpenCollisionWindow();

        if (GUILayout.Button("Edit Walk Collider"))
            OpenWalkCollisionWindow();

        if (GUILayout.Button("Edit Trigger Collider"))
            OpenTriggerCollisionWindow();

        DrawPipeConnectionEditor();

        EditorGUILayout.EndScrollView();
        GUILayout.EndArea();
    }

    void DrawPipeEditor() {
        if (CurrentPipe == null) {
            DrawNullSelectionEditor();
            return;
        }

        GUILayout.BeginArea(new Rect(0, 0, boxWidth, boxHeight));
        leftScrollPos = EditorGUILayout.BeginScrollView(leftScrollPos, GUIStyle.none, GUIStyle.none, GUILayout.Width(boxWidth), GUILayout.Height(boxHeight));
        EditorGUILayout.LabelField("Pipe Name: " + CurrentPipe.name);
        EditorGUILayout.LabelField("Pipe Default Parameters: ");
        if (GUILayout.Button("Select Prefab")) {
            Selection.activeObject = CurrentPipe.gameObject;
        }

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Destructable", GUILayout.MinWidth(boxWidth / 2), GUILayout.MaxWidth(120));
        CurrentPipe.destructable = EditorGUILayout.Toggle(CurrentPipe.destructable);
        EditorGUILayout.EndHorizontal();
        if (CurrentPipe.destructable) {
            TileDestructable destructable = CurrentPipe.GetComponentInChildren<TileDestructable>();
            EditorGUI.indentLevel++;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Health", GUILayout.MaxWidth(60));
            CurrentPipe.MaxHealth = EditorGUILayout.IntSlider(CurrentPipe.MaxHealth, 1, 1000);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.LabelField("Destruction Phases");
            EditorGUI.indentLevel++;
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(position.width / 2), GUILayout.Height(Mathf.Min(120, destructable.phases.Count * 90)));
            DrawDestructionPhases(destructable);
            EditorGUILayout.EndScrollView();
            EditorGUI.indentLevel--;
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Is Cover", GUILayout.MaxWidth(80));
        CurrentTile.cover = EditorGUILayout.Toggle(CurrentPipe.cover);
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Edit Collider"))
            OpenCollisionWindow();

        if (GUILayout.Button("Edit Walk Collider"))
            OpenWalkCollisionWindow();

        if (GUILayout.Button("Edit Trigger Collider"))
            OpenTriggerCollisionWindow();

        DrawPipeConnectionEditor();

        EditorGUILayout.EndScrollView();
        GUILayout.EndArea();
    }

    Vector2 scrollPos = new Vector2(0, 0);
    Vector2 leftScrollPos = new Vector2(0, 0);
    void DrawTileEditor() {
        if (CurrentTile == null) {
            DrawNullSelectionEditor();
            return;
        }

        GUILayout.BeginArea(new Rect(0, 0, boxWidth, boxHeight));
        leftScrollPos = EditorGUILayout.BeginScrollView(leftScrollPos, GUIStyle.none, GUIStyle.none, GUILayout.Width(boxWidth), GUILayout.Height(boxHeight));
		EditorGUILayout.LabelField("Tile Name: " + CurrentTile.name);
		EditorGUILayout.LabelField("Tile Default Parameters: ");

        if (GUILayout.Button("Select Prefab")) {
			Selection.activeObject = CurrentTile.gameObject;
		}
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Trip Threshold", GUILayout.MinWidth(boxWidth / 2), GUILayout.MaxWidth(120));
        CurrentTile.TripThresholdType = (Tile.TripThreshold)EditorGUILayout.EnumPopup(CurrentTile.TripThresholdType, GUILayout.MaxWidth(150));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Walking Noise", GUILayout.MinWidth(boxWidth / 2), GUILayout.MaxWidth(120));
        CurrentTile.WalkingNoiseType = (Tile.WalkingNoise)EditorGUILayout.EnumPopup(CurrentTile.WalkingNoiseType, GUILayout.MaxWidth(150));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Is Wall");
        bool temp = EditorGUILayout.Toggle(CurrentTile.isWall);
        if (temp != CurrentTile.isWall)
            CurrentTile.SetIsWall(temp);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Destructable", GUILayout.MinWidth(boxWidth/2), GUILayout.MaxWidth(120));
		CurrentTile.destructable = EditorGUILayout.Toggle(CurrentTile.destructable);
		EditorGUILayout.EndHorizontal();
		if (CurrentTile.destructable) {
            TileDestructable destructable = CurrentTile.GetComponentInChildren<TileDestructable>();
            EditorGUI.indentLevel++;
		    EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Health", GUILayout.MaxWidth(60));
			CurrentTile.MaxHealth = EditorGUILayout.IntSlider(CurrentTile.MaxHealth, 1, 1000);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.LabelField("Destruction Phases");
            EditorGUI.indentLevel++;
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(position.width/2), GUILayout.Height(Mathf.Min(120, destructable.phases.Count * 90)));
            DrawDestructionPhases(destructable);
            EditorGUILayout.EndScrollView();
            EditorGUI.indentLevel--;
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Is Cover", GUILayout.MaxWidth(80));
        CurrentTile.cover = EditorGUILayout.Toggle(CurrentTile.cover);
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Edit Collider"))
            OpenCollisionWindow();

        if (GUILayout.Button("Edit Walk Collider"))
            OpenWalkCollisionWindow();

        if (GUILayout.Button("Edit Trigger Collider"))
            OpenTriggerCollisionWindow();

        EditorGUILayout.EndScrollView();
        GUILayout.EndArea();
	}

    void DrawDestructionPhases(TileDestructable d) {
        for (int i = 0; i < d.phases.Count; i++) {
            if (d.phases[i] == null)
                return;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Phase " + i, GUILayout.MaxWidth(90));
            if (GUILayout.Button("Delete")) {
                d.phases.RemoveAt(i);
                i--;
                continue;
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Percent", GUILayout.MaxWidth(80));
            d.phases[i].percentActivate = EditorGUILayout.Slider (d.phases[i].percentActivate, 0f, 1f);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Sprite", GUILayout.MaxWidth(80));
            d.phases[i].sprite = EditorGUILayout.ObjectField(d.phases[i].sprite, typeof(Sprite), false) as Sprite;
            EditorGUILayout.EndHorizontal();
            if (CurrentTile.MyColliderData.data.CurrentColliderType != ColliderData.ColliderTypes.None) {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Height", GUILayout.MaxWidth(80));
                d.phases[i].height = EditorGUILayout.Slider(d.phases[i].height, 0f, 1f);
                EditorGUILayout.EndHorizontal();
            }
        }
        if (GUILayout.Button("Add Phase")) {
            d.phases.Add(new DestructionPhase());
        }
        EditorUtility.SetDirty(d);
    }

	void DrawRightBox() {
        GUILayout.BeginArea(new Rect(window.position.width/2f, 0, window.window.position.width/2f, window.window.position.height/2f));
        EditorGUILayout.LabelField("Tile Paste Settings");

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Grid Snap", GUILayout.MaxWidth(120));
        TileHelper.GridSnapEnabled = EditorGUILayout.Toggle(TileHelper.GridSnapEnabled);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Height Snap", GUILayout.MaxWidth(120));
        TileHelper.HeightSnapEnabled = EditorGUILayout.Toggle(TileHelper.HeightSnapEnabled);
        EditorGUILayout.EndHorizontal();
        if (TileHelper.HeightSnapEnabled) {
            EditorGUI.indentLevel++;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Paste Height", GUILayout.MaxWidth(100));
            TileHelper.PasteHeight = EditorGUILayout.Slider(TileHelper.PasteHeight, -10, 10);
            TileHelper.PasteHeight = Mathf.RoundToInt(TileHelper.PasteHeight * 10f) / 10f;
            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel--;
        }

        if (CurrentTile == null && CurrentPipe == null && CurrentObject == null) {
            GUILayout.EndArea();
            return;
        }

        DrawLayerEditor();

        GUILayout.EndArea();
	}

    void DrawPipeConnectionEditor() {

    }

    void OpenCollisionWindow() {
        if (collisionWindow == null) {
            CollisionEditorWindow window = CollisionEditorWindow.CreateWindow(this);
            collisionWindow = window;
        }
        collisionWindow.CurrentSprite = GetSelectedTile().GetComponent<SpriteBase>();
        collisionWindow.SetEditMode(CollisionEditorWindow.EditMode.Collider);
    }

    void OpenWalkCollisionWindow() {
        if (collisionWindow == null) {
            CollisionEditorWindow window = CollisionEditorWindow.CreateWindow(this);
            collisionWindow = window;
        }
        collisionWindow.CurrentSprite = GetSelectedTile().GetComponent<SpriteBase>();
        collisionWindow.SetEditMode(CollisionEditorWindow.EditMode.WalkCollider);
    }

    void OpenTriggerCollisionWindow() {
        if (collisionWindow == null) {
            CollisionEditorWindow window = CollisionEditorWindow.CreateWindow(this);
            collisionWindow = window;
        }
        collisionWindow.CurrentSprite = GetSelectedTile().GetComponent<SpriteBase>();
        collisionWindow.SetEditMode(CollisionEditorWindow.EditMode.TriggerCollider);
    }

    Vector2 layerScrollPos = Vector2.zero;
	void DrawLayerEditor () {
		//GUILayout.BeginArea(new Rect(0, 0, boxWidth, 120));
        EditorGUILayout.LabelField ("Main Layers");
        layerScrollPos = EditorGUILayout.BeginScrollView(layerScrollPos, GUILayout.Width(boxWidth - 10), GUILayout.Height(100));
		if (layersDisplayed != data.layers) {
            UpdateTile ();
			string[] newLayers = new string[data.layers.Count];
			data.layers.CopyTo (newLayers);
			layersDisplayed = newLayers.ToList ();
		}
		SelectedLayerIndex = GUILayout.SelectionGrid (SelectedLayerIndex, layersDisplayed.ToArray (), 1, GUILayout.MaxWidth (boxWidth));
        EditorGUILayout.EndScrollView();
        //GUILayout.EndArea();
		EditorGUILayout.LabelField ("Sub-Layer (Can be ANY integer value)");
		SublayerIndex = EditorGUILayout.IntField (SublayerIndex);

        TileHelper.LayerIndex = SelectedLayerIndex;
        TileHelper.SublayerIndex = SublayerIndex;
	}

    void DrawNullSelectionEditor() {
		GUILayout.BeginArea(new Rect(0, 0, window.position.width, boxHeight));
		EditorGUILayout.LabelField("Nothing selected, select something below to edit", GUILayout.MaxWidth(boxWidth));
		GUILayout.EndArea();
	}

    void DrawEditModeSelection() {
        if (GUILayout.Button("Tiles"))
            SetEditMode(EditMode.Tile);
        else if (GUILayout.Button("Pipes"))
            SetEditMode(EditMode.Pipe);
        else if (GUILayout.Button("Objects"))
            SetEditMode(EditMode.Object);
    }

    void DrawSelectionBox() {
		UpdateSelectionGridStyle();
        DrawDirectorySelection();
        UpdateTextureScale();
        DrawAtlasSelection();
        DrawSpriteSelection();
    }

    void DrawDirectorySelection() {
        if (CurrentDirectory != null)
            return;

        for (int i = 0; i < DirectoryData.Count; i++) {
            if (GUILayout.Button(DirectoryData[i].DirectoryName)) {
                CurrentDirectory = DirectoryData[i];
            }
        }
    }

    void DrawSpriteSelection() {
        if (CurrentAtlas == null || DisplayedSpriteTextures.Count == 0)
            return;

        int size = (int)SelectionGridStyle.CalcSize(new GUIContent(DisplayedSpriteTextures[0])).x;
        int numColumns = Mathf.FloorToInt((window.position.width) / (size));
        int newIndex = GUILayout.SelectionGrid(SelectedTextureIndex, DisplayedSpriteTextures.ToArray(), numColumns, SelectionGridStyle, GUILayout.MaxWidth(window.position.width - 20));
        if (newIndex != SelectedTextureIndex) {
            SelectedTextureIndex = newIndex;
            SelectTile(SelectedTextureIndex);
        }
    }

    void DrawAtlasSelection() {
        if (CurrentAtlas != null || CurrentDirectory == null || CurrentDirectory.sprites.Count == 0)
            return;

        for (int i = 0; i < CurrentDirectory.sprites.Count; i++) {
            Sprite s = CurrentDirectory.sprites[i];
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField(s.name, GUILayout.MaxWidth(window.position.width / 3f));
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Material", GUILayout.MaxWidth(80));
            Material m = (Material)EditorGUILayout.ObjectField(null, typeof(Material), false);
            if (m != null)
                UpdateSpriteMaterials(s, m);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndHorizontal();
            if (GUILayout.Button(CurrentDirectory.DisplayTextures[i], GUILayout.Width(window.position.width * (2f / 3f) - 30))) {
                CurrentAtlas = s;
                CurrentAtlasIndex = i;
                ScrollPosition = Vector2.zero;
                SelectedTextureIndex = 0;
                LoadSpritesheets();
                SelectTile(0);
            }
            EditorGUILayout.EndHorizontal();
        }
    }
    
    void UpdateSpriteMaterials(Sprite s, Material m) {
        if (CurrentEditMode == EditMode.Tile)
            SpriteManager.UpdateMaterial<Tile>(s, m);
        else if (CurrentEditMode == EditMode.Pipe)
            SpriteManager.UpdateMaterial<Pipe>(s, m);
    }

    const string AssetPathTiles = "/assets/Tiles/";
    const string AssetPathPipes = "/assets/Pipes";
    const string AssetPathObjects = "/assets/Objects";
    string CurrentAssetPath;

    /// <summary>
    /// Clears and sets TextureAtlases, DisplayedTextureAtlases
    /// Loads textures based on CurrentAssetPath
    /// Saves all valid paths to AtlasPaths
    /// Updates DisplayedTextureAtlases and TextureAtlases
    /// </summary>
    void LoadTextureAtlases() {
        string path = CurrentAssetPath;
        if (string.IsNullOrEmpty(path))
            return;
        
        DirectoryData.Clear();

        List<string> dirs = Directory.GetDirectories(Application.dataPath + path).ToList(); //Get all directories in Assets/assets/Tiles/
        dirs.Insert(0, Application.dataPath + path);

        for (int i = 0; i < dirs.Count; i++) { //Create new directory data for each directory
            SpriteDirectoryData d = new SpriteDirectoryData();
            d.DirectoryName = dirs[i].Replace(Application.dataPath + path, "");
            d.DirectoryPath = dirs[i]; //Full directory path
            d.SpritePaths = Directory.GetFiles(d.DirectoryPath, "*.png", SearchOption.TopDirectoryOnly);
            RemoveNonSpritePaths(d);
            //SpritePaths[0] == D:/Code/unity/SourceTree/ProjectTomato/SciFiSouls/Assets/assets/Objects\Doodads\objects_washroom_1x1_d.png
            for (int j = 0; j < d.SpritePaths.Count(); j++)
                GetDisplayTexturesAtPath(d, d.SpritePaths[j]);
            DirectoryData.Add(d);
        }
        DirectoryData[0].DirectoryName = "\\";
    }

    void UpdateTextureScale() {
        SpriteDirectoryData d = CurrentDirectory;
        if (LastSpriteScale == SpriteScale || d == null)
            return;
        d.DisplayTextures.Clear();
        d.sprites.Clear();
        for (int i = 0; i < d.SpritePaths.Count(); i++) {
            GetDisplayTexturesAtPath(d, d.SpritePaths[i]);
        }
        LoadSpritesheets();
        LastSpriteScale = SpriteScale;
    }

    void RemoveNonSpritePaths(SpriteDirectoryData d) {
        for (int i = 0; i < d.SpritePaths.Count(); i++) {
            string path = "Assets" + d.SpritePaths[i].Replace(Application.dataPath, "").Replace('\\', '/');
            Sprite s = (Sprite)AssetDatabase.LoadAssetAtPath(path, typeof(Sprite));
            if (!(s is Sprite)) {
                List<string> temp = d.SpritePaths.ToList();
                temp.RemoveAt(i);
                d.SpritePaths = temp.ToArray();
                i--;
            }
        }
    }

    //Get a sprite at the directory path
    void GetDisplayTexturesAtPath(SpriteDirectoryData data, string directoryPath) {
        string path = "Assets" + directoryPath.Replace(Application.dataPath, "").Replace('\\', '/');
        //path == Assets/assets/Objects/Doodads/objects_washroom_1x1_d.png
        Sprite s = (Sprite)AssetDatabase.LoadAssetAtPath(path, typeof(Sprite));
        if (s is Sprite) { //Ignore things like normal maps and load only sprites
            SpriteScale /= 2;
            Texture2D displayText = new Texture2D(s.texture.width, s.texture.height);
            displayText.SetPixels(s.texture.GetPixels());
            displayText.Apply();

            TextureScale.Bilinear(displayText, (int)(s.texture.width * SpriteScale), (int)(s.texture.height * SpriteScale));
            Color[] colors = displayText.GetPixels(0, 0, (int)(s.texture.width * SpriteScale), (int)(s.texture.height * SpriteScale));
            displayText = new Texture2D((int)(s.texture.width * SpriteScale), (int)(s.texture.height * SpriteScale), TextureFormat.ARGB32, false);
            displayText.SetPixels(colors);
            displayText.Apply();
            SpriteScale *= 2;

            data.DisplayTextures.Add(displayText);
            data.sprites.Add(s);
        } 
    }

	void LoadSpritesheets() {
        int index = CurrentAtlasIndex;

        if (index < 0)
            return;

        string path = "Assets" + CurrentDirectory.SpritePaths[index].Replace("\\", "/").Replace(Application.dataPath, "");
        DisplayedSprites = AssetDatabase.LoadAllAssetsAtPath(path).OfType<Sprite>().ToArray();
        DisplayedSpriteTextures.Clear();

        SpriteSize = Mathf.RoundToInt(DisplayedSprites[0].rect.width);
        int size = Mathf.FloorToInt(SpriteSize * Mathf.Max(SpriteScale, 1));

        for (int i = 0; i < DisplayedSprites.Length; i++) {
			Sprite s = DisplayedSprites[i];
			Color[] colors = s.texture.GetPixels((int)s.rect.x, (int)s.rect.y, (int)s.rect.width, (int)s.rect.height);
			Texture2D t = new Texture2D((int)s.rect.width, (int)s.rect.height, TextureFormat.ARGB32, false);
			t.SetPixels(colors);
			t.Apply();

			TextureScale.Bilinear(t, size, size);
			Color[] pix = t.GetPixels(0, 0, size, size);
			t = new Texture2D(size, size, TextureFormat.ARGB32, false);
			t.SetPixels(pix);
			t.Apply();

            DisplayedSpriteTextures.Add(t);
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

        if (collisionWindow != null)
            collisionWindow.Close();
    }

	public GameObject GetSelectedTile() {
        if (CurrentEditMode == EditMode.Tile)
            return CurrentTile != null ? CurrentTile.gameObject : null;
        else if (CurrentEditMode == EditMode.Pipe)
            return CurrentPipe != null ? CurrentPipe.gameObject : null;
        else if (CurrentEditMode == EditMode.Object)
            return CurrentObject != null ? CurrentObject.gameObject : null;
        return null;
    }

    public Texture2D GetSelectedTileTexture() {
        return DisplayedSpriteTextures[SelectedTextureIndex];
    }
}
