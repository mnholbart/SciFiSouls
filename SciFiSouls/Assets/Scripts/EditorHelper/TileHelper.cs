#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[ExecuteInEditMode]
public class TileHelper : MonoBehaviour {

	public static TileHelper _instance;

	//Grid stuff
	public Color GridColor = Color.gray;
	public static bool GridSnapEnabled = true;
	public static bool HeightSnapEnabled = true;
    public bool GridEnabled = true;
	public float GridDistance = 20f;
	public static int Width = 1;
	public static int Height = 1;
	public static SpriteBase SelectedTile;

    //Tile specific paste settings
    public static float PasteHeight = 0;
    public static int LayerIndex = 0;
    public static int SublayerIndex = 0;
    public static GameData data;

    //Editor stuff
    public KeyCode AddTileKeybind = KeyCode.P;
	public KeyCode IncreaseGridSizeKeybind = KeyCode.Equals;
	public KeyCode DecreaseGridSizeKeybind = KeyCode.Minus;
	public KeyCode SnapToGridKeybind = KeyCode.L;
	public EventModifiers AddTileModifier = EventModifiers.None;
	public EventModifiers IncreaseGridSizeModifier = EventModifiers.None;
	public EventModifiers DecreaseGridSizeModifier = EventModifiers.None;
	public EventModifiers SnapToGridModifier = EventModifiers.None;
	public EventModifiers ShowGridCoordinatesModifier = EventModifiers.Alt;
	GUIStyle GridCoordsStyle = new GUIStyle();

	//Need constructor because Start/OnEnable/Awake aren't called on recompile, and this runs in editor
	//Other option is to use the asset refresh callback
	public TileHelper() {
		if (_instance == null)
			_instance = this;
		else {
			Debug.LogError("Two TileHelper.cs being created");
		}
	}

	void OnEnable() {
		SceneView.onSceneGUIDelegate += this.GridUpdate;
	}

	void OnDestroy() {
		SceneView.onSceneGUIDelegate -= this.GridUpdate;
	}

	public void UpdateSelectedTile(SpriteBase g) {
		SelectedTile = g;
	}

	void GridUpdate(SceneView sceneView) {
		Event e = Event.current;

		if (e.isKey && e.rawType == EventType.keyUp) {
			if (e.modifiers == AddTileModifier && e.keyCode == AddTileKeybind)
				AddTile(e);
			if (e.modifiers == IncreaseGridSizeModifier && e.keyCode == IncreaseGridSizeKeybind)
				IncreaseGridSize();
			if (e.modifiers == DecreaseGridSizeModifier && e.keyCode == DecreaseGridSizeKeybind)
				DecreaseGridSize();
			if (e.modifiers == SnapToGridModifier && e.keyCode == SnapToGridKeybind)
				ToggleSnapToGrid();
		}

		if (Selection.activeTransform != null && Selection.activeTransform.GetComponent<Tile>() && e.button == 0 && e.type == EventType.MouseUp) {
			if (GridSnapEnabled) {
				SnapSelectedToGrid();
			}
		}
	}
    
	void ToggleSnapToGrid() {
		GridSnapEnabled = !GridSnapEnabled;
		SnapSelectedToGrid();
	}

	void SnapSelectedToGrid() {
		Object[] objects = Selection.objects;
		for (int i = 0; i < objects.Length; i++) {
			GameObject o = (GameObject)objects[i];
			if (o.GetComponent<Tile>()) {
				Transform t = o.transform;
				Vector2 gridPos = GetGridPos(t.position);
				t.position = new Vector3(gridPos.x + Width/2f, gridPos.y + Height/2f, HeightSnapEnabled ? PasteHeight : 0);
                t.rotation = Quaternion.identity;
			}
		}
	}

    /// <summary>
    /// Get a grid coordinate from a world position
    /// </summary>
	public Vector2 GetGridPos(Vector3 position) {
		position.x -= Width/2f;
		position.y -= Height/2f;
		Vector3 rounded = new Vector3(Mathf.Round(position.x/Width)*Width, Mathf.Round(position.y/Height)*Height, 0);
		return new Vector2(rounded.x, rounded.y);
	}

	void IncreaseGridSize() {
		Width = Mathf.Min(10, Width + 1);
		Height = Width;
		UpdateGridDistance();
	}

	void DecreaseGridSize() {
		Width = Mathf.Max(1, Width - 1);
		Height = Width;
		UpdateGridDistance();
	}

	public void UpdateGridDistance() {
		GridDistance = Mathf.Round(GridDistance/Width)*Width;
		if (GridDistance < 1) {
			GridDistance = Width;
		}
	}

    /// <summary>
    /// Create a tile at the target mouse position using the Tile Editor selected tile
    /// </summary>
	void AddTile(Event e) {
        if (SelectedTile == null) {
            Debug.LogWarning("[TileHelper] Sprite to be created is null");
            return;
        }

        GameObject prefab = SelectedTile.gameObject;
        int size = Mathf.RoundToInt(prefab.GetComponent<SpriteRenderer>().sprite.rect.width);
        size = size / 16; //16 is base unit size so convert to unit size

        if (prefab == null || !prefab.GetComponent<SpriteBase>() || Camera.current == null)
            return;

        Vector3 newPos = new Vector3(0, 0, 0);
        Ray r = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        Plane p = new Plane(Vector3.forward, Vector3.zero);
        float d = 0;
        if (p.Raycast(r, out d)) {
            newPos = r.GetPoint(d);
        }

        float width = TileHelper.Width;
        float height = TileHelper.Height;
        if (GridSnapEnabled) {
            newPos.x = Mathf.CeilToInt(newPos.x / width) - (size%2 == 1 ? 1 : 0) * width + (size%2 == 1 ? (width / 2f) : 0);
            newPos.y = Mathf.CeilToInt(newPos.y / height) - (size%2 == 1 ? 1 : 0) * height + (size%2 == 1 ? (height / 2f) : 0);
        }
        
        GameObject obj = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
        Undo.RegisterCreatedObjectUndo(obj, "Create Tile" + SelectedTile.name);
        obj.transform.position = new Vector3(newPos.x, newPos.y, HeightSnapEnabled ? PasteHeight : 0);
        obj.transform.rotation = Quaternion.identity;
        SpriteBase t = obj.GetComponent<SpriteBase>();
        t.SetLayerIndex(LayerIndex, data);
        t.SetSubLayerIndex(SublayerIndex, data);
    }

	void OnDrawGizmos() {
        //Draw resizable grid lines in front of the camera only
		if (GridEnabled) {
			Vector3 pos = SceneView.currentDrawingSceneView.camera.transform.position;
			pos.x = Mathf.Round(pos.x/Width)*Width;
			pos.y = Mathf.Round(pos.y/Height)*Height;
			int numLines = Mathf.RoundToInt(GridDistance);
		
			Gizmos.color = GridColor;
			for (float y = pos.y + -numLines; y < pos.y + numLines + 1; y += 1) {
				Gizmos.DrawLine(new Vector3(Mathf.Floor(-GridDistance + pos.x), Mathf.Floor(y/Height) * Height, 0.0f),
					new Vector3(Mathf.Floor(GridDistance + pos.x), Mathf.Floor(y/Height) * Height, 0.0f));
			}
			for (float x = pos.x + -numLines; x < pos.x + numLines + 1; x += 1) {
				Gizmos.DrawLine(new Vector3(Mathf.Floor(x/Width) * Width, Mathf.Floor(-GridDistance + pos.y), 0.0f),
					new Vector3(Mathf.Floor(x/Width) * Width, Mathf.Floor(GridDistance + pos.y), 0.0f));
			}

            //Draw grid coordinates
			if (GridDistance <= 20 && Event.current.modifiers == ShowGridCoordinatesModifier) {
				float zoom = (Mathf.Abs(pos.z)/5f);
				GridCoordsStyle.fontSize = Mathf.Max(Mathf.RoundToInt(16 - zoom), 2);
				GridCoordsStyle.normal.textColor = GridColor;

				for (int i = -numLines/Width; i < numLines/Width; i++) {
					for (int j = -numLines/Height; j < numLines/Height; j++) {
						Handles.Label(new Vector3((i*Width) + pos.x, (j*Height) + pos.y + Height/2f, 0), "(" + (i+pos.x) + "," + (j+pos.y) + ")", GridCoordsStyle);
					}
				}
			}
		}
	}
}
#endif
