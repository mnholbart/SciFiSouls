using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class RoomWindow : EditorWindow {

    [MenuItem("Editor/Room Editor")]
    public static void ShowWindow() {
        RoomWindow window = (RoomWindow)EditorWindow.GetWindow(typeof(RoomWindow));
        window.titleContent.text = "Room Editor";
        window.Show();

        window.minSize = new Vector2(250, 350);
    }

    void OnGUI() {

    }
}