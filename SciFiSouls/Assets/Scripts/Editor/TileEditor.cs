using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

[CustomEditor(typeof(Tile))]
[CanEditMultipleObjects]
public class TileEditor : Editor {

    Tile tile;
    GameData data;

    bool defaultInspector = false;

    SerializedProperty tripThresholdProperty;
    SerializedProperty walkingNoiseProperty;
    SerializedProperty destructableProperty;
    SerializedProperty healthProperty;

    void OnEnable() {
        tripThresholdProperty = serializedObject.FindProperty("TripThresholdType");
        walkingNoiseProperty = serializedObject.FindProperty("WalkingNoiseType");
        destructableProperty = serializedObject.FindProperty("destructable");
        healthProperty = serializedObject.FindProperty("MaxHealth");
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();
        tile = (Tile)target;
        data = (GameData)Resources.Load("GameData", typeof(GameData));

        if (defaultInspector) {
            base.DrawDefaultInspector();
            return;
        }

        if (GUILayout.Button("Show Default Inspector")) {
            defaultInspector = true;
        }
        if (GUILayout.Button("Revert to Prefab")) {
            PrefabUtility.RevertPrefabInstance(tile.gameObject);
        }

        EditorGUILayout.BeginHorizontal();
        int layer = EditorGUILayout.Popup("Layer", data.GetLayerIndex(tile), data.layers.ToArray());
        if (layer != data.GetLayerIndex(tile)) {
            Undo.RegisterCompleteObjectUndo(tile, "Change Layer");
            tile.SetLayerIndex(layer, data);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        int subLayer = EditorGUILayout.DelayedIntField("Sub Layer", tile.SubLayer);
        if (subLayer != tile.SubLayer) {
            Undo.RegisterCompleteObjectUndo(tile, "Change Sub Layer");
            tile.SetSubLayerIndex(subLayer, data);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(tripThresholdProperty, new GUIContent("Trip Threshold"));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(walkingNoiseProperty, new GUIContent("Walking Noise"));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(destructableProperty, new GUIContent("Destructable"));
        EditorGUILayout.EndHorizontal();

        if (tile.destructable) {
            EditorGUI.indentLevel++;
            EditorGUILayout.IntSlider(healthProperty, 0, 1000, new GUIContent("Max Health"));
            TileDestructable destructable = tile.GetComponentInChildren<TileDestructable>();
            PropertyModification[] mods = PrefabUtility.GetPropertyModifications(destructable);
            PropertyModification d = null;
            if (mods != null && mods.Length > 0)
                d = (from item in mods where item != null && item.target.name == "Destructable" select item).FirstOrDefault();

            EditorGUILayout.BeginHorizontal();
            destructionFoldout = EditorGUILayout.Foldout(destructionFoldout, "");
            EditorGUILayout.LabelField(new GUIContent("Destruction Phases"), d == null ? EditorStyles.label : EditorStyles.boldLabel);
            if (GUILayout.Button("Reset Phases")) {
                PrefabUtility.ResetToPrefabState(destructable);
            }
            EditorGUILayout.EndHorizontal();
            if (destructionFoldout) {
                EditorGUI.indentLevel++;
                DrawDestructionPhases(destructable);
                EditorGUI.indentLevel--;
            }

            EditorGUI.indentLevel--;
        }

        serializedObject.ApplyModifiedProperties();
        EditorUtility.SetDirty(tile);
    }

    bool destructionFoldout = false;
    void DrawDestructionPhases(TileDestructable d) {

        for (int i = 0; i < d.phases.Count; i++) {
            if (d.phases[i] == null)
                return;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Phase " + i, GUILayout.MaxWidth(90));
            if (GUILayout.Button("Delete", GUILayout.MaxWidth(160))) {
                d.phases.RemoveAt(i);
                i--;
                continue;
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Percent", GUILayout.MaxWidth(80));
            d.phases[i].percentActivate = EditorGUILayout.Slider(d.phases[i].percentActivate, 0f, 1f);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Sprite", GUILayout.MaxWidth(80));
            d.phases[i].sprite = EditorGUILayout.ObjectField(d.phases[i].sprite, typeof(Sprite), false) as Sprite;
            EditorGUILayout.EndHorizontal();
            if (tile.MyColliderData.data.CurrentColliderType != ColliderData.ColliderTypes.None) {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Height", GUILayout.MaxWidth(80));
                d.phases[i].height = EditorGUILayout.Slider(d.phases[i].height, 0f, 1f);
                EditorGUILayout.EndHorizontal();
            }
        }
        if (GUILayout.Button("Add Phase", GUILayout.MaxWidth(200))) {
            d.phases.Add(new DestructionPhase());
        }
        EditorUtility.SetDirty(d);
    }
}
