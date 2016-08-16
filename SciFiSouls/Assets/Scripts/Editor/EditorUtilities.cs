using UnityEngine;
using UnityEditor;
using System.Collections;

public static class EditorUtilities {

    public static Vector2 positionHandleOffset;
    public static Vector2 activePositionHandlePosition = Vector2.zero;
    static int activePositionHandleId = 0;
    public static GUIStyle BoxHandleStyle;

    public static Vector2 ClosestPointOnLine(Vector2 p, Vector2 p1, Vector2 p2) {
        float magSq = (p2 - p1).sqrMagnitude;
        if (magSq < float.Epsilon)
            return p1;

        float u = ((p.x - p1.x) * (p2.x - p1.x) + (p.y - p1.y) * (p2.y - p1.y)) / magSq;
        if (u < 0.0f || u > 1.0f)
            return p1;

        return p1 + (p2 - p1) * u;
    }

    public static Vector2 PositionHandle(int id, Vector2 position) {
        return Handle(BoxHandleStyle, id, position, false);
    }

    public static void CreateStyles() {
        BoxHandleStyle = new GUIStyle(EditorStyles.toolbarButton);
        Texture2D t = (Texture2D)Resources.Load("Editor/Handle", typeof(Texture2D));
        Texture2D h = (Texture2D)Resources.Load("Editor/HandleHover", typeof(Texture2D));
        BoxHandleStyle.fixedWidth = BoxHandleStyle.fixedHeight = 12;
        BoxHandleStyle.stretchHeight = true;
        BoxHandleStyle.stretchWidth = true;
        BoxHandleStyle.normal.background = t;
        BoxHandleStyle.hover.background = h;
    }

    public static Vector2 Handle(GUIStyle style, int id, Vector2 position, bool allowKeyboardFocus) {
        if (style == null)
            CreateStyles();
        int handleSize = (int)style.fixedWidth;
        Rect rect = new Rect(position.x - handleSize / 2, position.y - handleSize / 2, handleSize, handleSize);
        int controlID = id;

        switch (Event.current.GetTypeForControl(controlID)) {
            case EventType.MouseDown:
                {
                    if (rect.Contains(Event.current.mousePosition)) {
                        activePositionHandleId = id;
                        if (allowKeyboardFocus) {
                            GUIUtility.keyboardControl = controlID;
                        }
                        positionHandleOffset = Event.current.mousePosition - position;
                        GUIUtility.hotControl = controlID;
                        Event.current.Use();
                    }
                    break;
                }

            case EventType.MouseDrag:
                {
                    if (GUIUtility.hotControl == controlID) {
                        position = Event.current.mousePosition - positionHandleOffset;
                        GUI.changed = true;
                        Event.current.Use();
                    }
                    break;
                }

            case EventType.MouseUp:
                {
                    if (GUIUtility.hotControl == controlID) {
                        activePositionHandleId = 0;
                        position = Event.current.mousePosition - positionHandleOffset;
                        GUIUtility.hotControl = 0;
                        GUI.changed = true;
                        Event.current.Use();
                    }
                    break;
                }

            case EventType.Repaint:
                {
                    bool selected = (GUIUtility.keyboardControl == controlID ||
                                     GUIUtility.hotControl == controlID);
                    style.Draw(rect, selected, false, false, false);
                    break;
                }
        }

        return position;
    }

    public static void SetPositionHandleValue(int id, Vector2 val) {
        if (id == activePositionHandleId)
            activePositionHandlePosition = val;
    }
}
