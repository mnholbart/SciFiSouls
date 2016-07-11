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
	public bool SnapEnabled = true;
	public bool GridEnabled = true;
	public float GridDistance = 20f;
	public static int Width = 1;
	public static int Height = 1;
	public static GameObject SelectedTile;

	//Editor stuff
	public KeyCode AddTileKeybind = KeyCode.P;
	public KeyCode IncreaseGridSizeKeybind = KeyCode.Equals;
	public KeyCode DecreaseGridSizeKeybind = KeyCode.Minus;
	public KeyCode SnapToGridKeybind = KeyCode.L;
	public KeyCode UndoKeybind = KeyCode.Z;
	public KeyCode RedoKeybind = KeyCode.X;
	public EventModifiers AddTileModifier = EventModifiers.None;
	public EventModifiers IncreaseGridSizeModifier = EventModifiers.None;
	public EventModifiers DecreaseGridSizeModifier = EventModifiers.None;
	public EventModifiers SnapToGridModifier = EventModifiers.None;
	public EventModifiers UndoModifier = EventModifiers.None;
	public EventModifiers RedoModifier = EventModifiers.None;
	public EventModifiers ShowGridCoordinatesModifier = EventModifiers.Alt;
	GUIStyle GridCoordsStyle = new GUIStyle();

	//Undo stuff
	public List<GridAction> Actions = new List<GridAction>();
	public List<GridAction> RedoActions = new List<GridAction>();
	public int MaxSavedActions = 10;
	public int MaxSavedRedoActions = 10;

	//Need constructor because Start/OnEnable/Awake aren't called on recompile, and this runs in editor
	//Other option is to use the asset refresh callback
	public TileHelper() {
		if (_instance == null)
			_instance = this;
		else {
			Debug.LogError("Two Grid.cs being created");
		}
	}

	void OnEnable() {
		SceneView.onSceneGUIDelegate += this.GridUpdate;
	}

	void OnDestroy() {
		SceneView.onSceneGUIDelegate -= this.GridUpdate;
	}

	public void UpdateSelectedTile(GameObject g) {
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
			if (e.modifiers == UndoModifier && e.keyCode == UndoKeybind)
				UndoAction();
			if (e.modifiers == RedoModifier && e.keyCode == RedoKeybind)
				RedoAction();
		}

		if (Selection.activeTransform != null && Selection.activeTransform.GetComponent<Tile>() && e.button == 0 && e.type == EventType.MouseUp) {
			if (SnapEnabled) {
				SnapSelectedToGrid();
			}
		}
	}

	public void UndoAt(int i) {
		GridAction undo = Actions[i];
		undo.UndoAction();
		Actions.Remove(undo);
	}

	public void AddAction(GridAction action, bool redo) {
		if (Actions.Count > MaxSavedActions) {
			GridAction a = Actions[0];
			a.ForceRemove();
		}
		Actions.Add(action);
		if (!redo)
			RedoActions.Clear();
	}

	public void AddAction(GridAction action) {
		AddAction(action, true);
	}

	public void ForceRemove(GridAction action) {
		Actions.Remove(action);
	}

	void RedoAction() {
		if (RedoActions.Count <= 0)
			return;
		
		GridAction redo = RedoActions.Pop();
		if (redo is AddTileAction) {
			AddTile((AddTileAction)redo);
		}
	}

	void UndoAction() {
		if (Actions.Count <= 0)
			return;
		
		GridAction undo = Actions[Actions.Count-1];
		undo.UndoAction();
		Actions.Remove(undo);
		RedoActions.Push(undo);
	}

	void ToggleSnapToGrid() {
		SnapEnabled = !SnapEnabled;
		SnapSelectedToGrid();
	}

	void SnapSelectedToGrid() {
		Object[] objects = Selection.objects;
		for (int i = 0; i < objects.Length; i++) {
			GameObject o = (GameObject)objects[i];
			if (o.GetComponent<Tile>()) {
				Transform t = o.transform;
				Vector2 gridPos = GetGridPos(t.position);
				t.position = new Vector3(gridPos.x + Width/2f, gridPos.y + Height/2f, 0);
			}
		}
	}

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

	void AddTile(Event e) {
		AddTileAction action = new AddTileAction();
		action.InitAction(e);
	}

	void AddTile(AddTileAction action) {
		action.ReinitAction();
	}

	void OnDrawGizmos() {
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
