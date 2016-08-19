using UnityEngine;
using UnityEditor;
using System.Collections;

[CanEditMultipleObjects]
[CustomEditor(typeof(WallDepth))]
public class WallDepthEditor : Editor {

    WallDepth wall;
    GameData data;

    SerializedProperty heightValues;

    void OnEnable() {
        heightValues = serializedObject.FindProperty("FloorZHeights");
        data = (GameData)Resources.Load("GameData", typeof(GameData));

        for (int i = 0; i < targets.Length; i++) {
            WallDepth wd = (WallDepth)targets[i];
            while (wd.FloorZHeights.Count < data.GetFloorCount()) {
                wd.FloorZHeights.Add(0f);
            }
        }
        serializedObject.ApplyModifiedProperties();
        serializedObject.Update();
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();
        wall = (WallDepth)target;

        int j = 0;
        for (int i = 0; i < (data.layers.Count); i += 2) {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(data.layers[i], GUILayout.MaxWidth(100));
            EditorGUILayout.LabelField("Height", GUILayout.MaxWidth(80));
            SerializedProperty val = heightValues.GetArrayElementAtIndex(j);
            EditorGUILayout.DelayedFloatField(val, new GUIContent(""));
            //EditorGUILayout.PropertyField(val);
            EditorGUILayout.EndHorizontal();
            j++;
        }
        EditorUtility.SetDirty(wall);
        serializedObject.ApplyModifiedProperties();
    }
}
