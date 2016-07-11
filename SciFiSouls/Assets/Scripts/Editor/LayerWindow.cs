using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class LayerWindow : EditorWindow {

	[MenuItem ("Tile/Layer Editor")]
	public static void  ShowWindow () {
		LayerWindow window = (LayerWindow)EditorWindow.GetWindow(typeof(LayerWindow));
		window.titleContent.text = "Layer Editor";
		window.Show();

		window.minSize = new Vector2(250, 350);

		GameData data = (GameData)Resources.Load("GameData", typeof(GameData));
		window.data = data;
	}

	int InsertIndex;
	string InsertName;
	GameData data;

	void OnGUI() {
		DrawLayers();
		DrawAddLayerButton();
		GUILayout.Space(20);
		DrawInsertLayerButton();
        DrawForceUpdateButton();
	}

    void DrawForceUpdateButton() {
        GUILayout.Space(20);
        if (GUILayout.Button("Force Update ALL Tiles")) { 
            SpriteManager.ForceUpdateTiles();
        }
    }
		
	void DrawInsertLayerButton() {
		EditorGUILayout.LabelField("Insert Layer_________________________________________________________________________");
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Insert Index:", GUILayout.MaxWidth(100));
		InsertIndex = EditorGUILayout.IntSlider(InsertIndex, 0, data.layers.Count);
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Layer Name:", GUILayout.MaxWidth(100));
		InsertName = EditorGUILayout.TextField(InsertName);
		if (GUILayout.Button("Insert Layer", GUILayout.MaxWidth(100))) {
			if (data.layers.Contains(InsertName)) {
				Debug.LogError("Cannot add duplicate layer name");
				EditorGUILayout.EndHorizontal();
				return;
			}
			if (string.IsNullOrEmpty(InsertName)) {
				Debug.LogError("Cannot add layer with empty name");
				return;
			}
			data.layers.Insert(InsertIndex, InsertName);
		}
		EditorGUILayout.EndHorizontal();
	}

	void DrawAddLayerButton() {
		if (GUILayout.Button("Add Layer")) {
			int i = data.layers.Count+1;
			string layerName = "New Layer" + i;
			if (!data.layers.Contains(layerName))
				data.layers.Add(layerName);
			else {
				while (data.layers.Contains(layerName)) {
					i++;
					layerName = "New Layer" + i;
				}
				if (!data.layers.Contains(layerName)) {
					data.layers.Add(layerName);
				}
			}
            SpriteManager.ForceUpdateTiles();
        }
	}

	void DrawLayers() {
		EditorGUILayout.LabelField("Rendered First");
		EditorGUI.indentLevel++;
		for (int i = 0; i < data.layers.Count; i++) {
			string s = data.layers[i];
			EditorGUILayout.BeginHorizontal();
			string t = EditorGUILayout.TextField(s);
			if (!data.layers.Contains(t)) {
				data.layers[i] = t;
			}
			if (i > 0 && data.layers[i-1] != null && GUILayout.Button("Up")) {
				string temp = data.layers[i];
				data.layers[i] = data.layers[i-1];
				data.layers[i-1] = temp;
                SpriteManager.ForceUpdateTiles();
            }
			if (i < data.layers.Count-1 && data.layers[i+1] != null && GUILayout.Button("Down")) {
				string temp = data.layers[i];
				data.layers[i] = data.layers[i+1];
				data.layers[i+1] = temp;
                SpriteManager.ForceUpdateTiles();
            }
			if (GUILayout.Button("Remove")) {
				data.layers.Remove(s);
                i--;
				EditorGUILayout.EndHorizontal();
                SpriteManager.ForceUpdateTiles();
                continue;
			}
			EditorGUILayout.EndHorizontal();
		}
		EditorGUI.indentLevel--;
		EditorGUILayout.LabelField("Rendered Last");
	}
}
