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
        wall = (WallDepth)target;
        heightValues = serializedObject.FindProperty("FloorZHeights");
        data = (GameData)Resources.Load("GameData", typeof(GameData));
        wall.UpdateFloorDepthsList();
    }

    public override void OnInspectorGUI() {
        if (heightValues == null || data == null)
            OnEnable();

        serializedObject.Update();
        wall = (WallDepth)target;

        if (heightValues == null)
            return;

        int j = 0;
        for (int i = 0; i < (heightValues.arraySize); i += 2) {
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
