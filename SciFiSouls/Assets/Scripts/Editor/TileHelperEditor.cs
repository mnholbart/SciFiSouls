using UnityEngine;
using UnityEditor;

[CustomEditor (typeof(TileHelper))]
public class TileHelperEditor : Editor {

	TileHelper GridTarget;

	bool ShowKeybinds = true;
	bool ShowActions = false;
	bool ShowRedoActions = false;


	public void OnEnable() {
		GridTarget = (TileHelper)target;
	}

	public override void OnInspectorGUI() {

		GUILayout.BeginHorizontal();
		GUILayout.Label("Grid Enabled", GUILayout.MaxWidth(200));
		GridTarget.GridEnabled = EditorGUILayout.Toggle(GridTarget.GridEnabled);
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Label("Snap To Grid Enabled", GUILayout.MaxWidth(200));
		GridTarget.SnapEnabled = EditorGUILayout.Toggle(GridTarget.SnapEnabled);
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Label("Grid Color");
		GridTarget.GridColor = EditorGUILayout.ColorField(GridTarget.GridColor);
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Label(" Grid Draw Distance ");
		GridTarget.GridDistance = (float)EditorGUILayout.IntSlider((int)GridTarget.GridDistance, 1, 40);
		GridTarget.UpdateGridDistance();
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Label(" Grid Size ");
		int size = EditorGUILayout.IntSlider(TileHelper.Width, 1, 10);
		TileHelper.Width = TileHelper.Height = size;
		GUILayout.EndHorizontal();

		ShowActions = EditorGUILayout.Foldout(ShowActions, "Actions");
		if (ShowActions) {
			EditorGUI.indentLevel+=1;
			ActionsGUI();
			EditorGUI.indentLevel-=1;
		}

		ShowRedoActions = EditorGUILayout.Foldout(ShowRedoActions, "Redo-Actions");
		if (ShowRedoActions) {
			EditorGUI.indentLevel+=1;
			RedoActionsGUI();
			EditorGUI.indentLevel-=1;
		}

		ShowKeybinds = EditorGUILayout.Foldout(ShowKeybinds, "Keybinds");
		if (ShowKeybinds) {
			EditorGUI.indentLevel+=1;
			KeybindsGUI();
			EditorGUI.indentLevel-=1;
		}

		SceneView.RepaintAll(); //Force repaint or it wont update until interacting with scene
		this.Repaint();
	}

	void RedoActionsGUI() {
		for (int i = 0; i < GridTarget.RedoActions.Count; i++) {
			GridAction action = GridTarget.RedoActions[i];
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(action.ActionToString());
			EditorGUILayout.EndHorizontal();
		}
	}

	void ActionsGUI() {
		for (int i = 0; i < GridTarget.Actions.Count; i++) {
			GridAction action = GridTarget.Actions[i];
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(action.ActionToString());
			if (GUILayout.Button("Undo")) {
				TileHelper._instance.UndoAt(i);
			}
			if (action is AddTileAction && GUILayout.Button("Ref")) {
				((AddTileAction)action).FocusObject();
			}
			EditorGUILayout.EndHorizontal();
		}
	}

	void KeybindsGUI() {
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Add Tile");
		GridTarget.AddTileModifier = (EventModifiers)EditorGUILayout.EnumPopup(GridTarget.AddTileModifier);
		GridTarget.AddTileKeybind = (KeyCode)EditorGUILayout.EnumPopup(GridTarget.AddTileKeybind);
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Increase Grid Size");
		GridTarget.IncreaseGridSizeModifier = (EventModifiers)EditorGUILayout.EnumPopup(GridTarget.IncreaseGridSizeModifier);
		GridTarget.IncreaseGridSizeKeybind = (KeyCode)EditorGUILayout.EnumPopup(GridTarget.IncreaseGridSizeKeybind);
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Decrease Grid Size");
		GridTarget.DecreaseGridSizeModifier = (EventModifiers)EditorGUILayout.EnumPopup(GridTarget.DecreaseGridSizeModifier);
		GridTarget.DecreaseGridSizeKeybind = (KeyCode)EditorGUILayout.EnumPopup(GridTarget.DecreaseGridSizeKeybind);
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Toggle Snap To Grid");
		GridTarget.SnapToGridModifier = (EventModifiers)EditorGUILayout.EnumPopup(GridTarget.SnapToGridModifier);
		GridTarget.SnapToGridKeybind = (KeyCode)EditorGUILayout.EnumPopup(GridTarget.SnapToGridKeybind);
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Undo");
		GridTarget.UndoModifier = (EventModifiers)EditorGUILayout.EnumPopup(GridTarget.UndoModifier);
		GridTarget.UndoKeybind = (KeyCode)EditorGUILayout.EnumPopup(GridTarget.UndoKeybind);
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Show Grid Coordinates");
		GridTarget.ShowGridCoordinatesModifier = (EventModifiers)EditorGUILayout.EnumPopup(GridTarget.ShowGridCoordinatesModifier);
		EditorGUILayout.EndHorizontal();
	}
}
