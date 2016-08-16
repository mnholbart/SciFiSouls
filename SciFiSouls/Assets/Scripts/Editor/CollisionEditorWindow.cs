using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class CollisionEditorWindow : EditorWindow {
    public static CollisionEditorWindow CreateWindow(TileWindow w) {
        CollisionEditorWindow window = (CollisionEditorWindow)EditorWindow.GetWindow(typeof(CollisionEditorWindow));
        window.titleContent.text = "Collision Editor";
        window.Show();

        window.tileWindow = w;
        window.minSize = new Vector2(1000, 300);
        return window;
    }

    TileWindow tileWindow;
    Texture2D gridTexture;
    int textureSize = 16;
    Color PreviousHandleColor;

    public enum Type {
        LightChecked,
        MediumChecked,
        DarkChecked,
        BlackChecked,
        LightSolid,
        MediumSolid,
        DarkSolid,
        BlackSolid
    }
    Type gridType = Type.DarkChecked;

    public enum EditMode {
        Collider,
        WalkCollider
    }

    public EditMode CurrentEditMode = EditMode.Collider;
    ColliderData.Data CurrentData;

    public Tile CurrentTile = null;

    void OnGUI() {
        EditorUtilities.CreateStyles();
        GameObject g = tileWindow.GetSelectedTile();
        if (tileWindow == null || g == null || tileWindow.selectedTexture == null) {
            CurrentTile = null;
            return;
        }
        CurrentTile = tileWindow.GetSelectedTile().GetComponent<Tile>();
        if (CurrentEditMode == EditMode.Collider) {
            CurrentData = CurrentTile.MyColliderData.data;
        } else if (CurrentEditMode == EditMode.WalkCollider) {
            CurrentData = CurrentTile.MyColliderData.moveData;
        }

        DrawToolbar();
        DrawLeftToolbar();
        DrawBackgroundTexture();
        DrawRightToolbar();
    }

    void DrawCollisionEditor(Rect r) {
        ColliderData data = CurrentTile.MyColliderData;
        Vector2[] points = CurrentData.points;
        float closestDistanceSq = Mathf.Infinity;

        Vector2 origin = new Vector2(10, 10);
        Vector2 origin3 = new Vector3(10, 10, 0);

        if (points == null || points.Length == 0 || !CurrentData.isValidPolygon()) {
            GenerateNewPoints(data);
        }

        bool createNewPoint = false;
        if (Event.current.clickCount == 2 && Event.current.type == EventType.MouseDown) {
            createNewPoint = true;
            Event.current.Use();
        }

		int closestPreviousPoint = 0;
        Vector2 closestPoint = Vector2.zero;
        Vector2 ov = (points.Length > 0) ? points[points.Length - 1] : Vector2.zero;
        for (int i = 0; i < points.Length; i++) {
            Vector2 v = points[i];

            if (createNewPoint) {
                Vector2 localMousePosition = (Event.current.mousePosition - origin);
                Vector2 closestPointToCursor = EditorUtilities.ClosestPointOnLine(localMousePosition, ov, v);
                float lengthSq = (closestPointToCursor - localMousePosition).sqrMagnitude;
                if (lengthSq < closestDistanceSq) {
                    closestDistanceSq = lengthSq;
                    closestPoint = closestPointToCursor;
                    closestPreviousPoint = i;
                }
            }

            PreviousHandleColor = Handles.color;
            Handles.color = Color.green;
            Handles.DrawLine(v + origin, ov + origin);
            Handles.color = PreviousHandleColor;
            ov = v;
        }

        if (createNewPoint && closestDistanceSq < 16.0f) {
            var tmpList = new List<Vector2>(CurrentData.points);
            tmpList.Insert(closestPreviousPoint, closestPoint);
            CurrentData.points = tmpList.ToArray();
            HandleUtility.Repaint();
        }

        Event ev = Event.current;
        int deletedIndex = -1;

        for (int i = 0; i < CurrentData.points.Length; i++) {
            Vector2 v = CurrentData.points[i];
			int id = "PolyEditor".GetHashCode() * 10000 + i;
            v = EditorUtilities.Handle(EditorUtilities.BoxHandleStyle, id, v + origin3, true) - origin;

            if (GUIUtility.keyboardControl == id && ev.type == EventType.KeyDown) {
                switch (ev.keyCode) {
                    case KeyCode.Backspace:
                    case KeyCode.Delete:
                        {
                            GUIUtility.keyboardControl = 0;
                            GUIUtility.hotControl = 0;
                            deletedIndex = i;
                            ev.Use();
                            break;
                        }
                    case KeyCode.Escape:
                        {
                            GUIUtility.hotControl = 0;
                            GUIUtility.keyboardControl = 0;
                            ev.Use();
                            break;
                    }
                }
            }

            v.x = Mathf.Round(v.x * 2) / 2.0f; 
            v.y = Mathf.Round(v.y * 2) / 2.0f;

            v.x = Mathf.Clamp(v.x, 0.0f, CurrentTile.SpriteWidth);
            v.y = Mathf.Clamp(v.y, 0.0f, CurrentTile.SpriteHeight);

            CurrentData.points[i] = v;
        }

        if (deletedIndex != -1 && CurrentData.points.Length > 3) {
            var tmpList = new List<Vector2>(CurrentData.points);
            tmpList.RemoveAt(deletedIndex);
            CurrentData.points = tmpList.ToArray();
        }
    }

    void GenerateNewPoints(ColliderData data) {
        Vector2[] points = new Vector2[4];
        int w = CurrentTile.SpriteWidth;
        int h = CurrentTile.SpriteHeight;

        points = new Vector2[4];
        points[0] = new Vector2(0, 0);
        points[1] = new Vector2(0, h);
        points[2] = new Vector2(w, h);
        points[3] = new Vector2(w, 0);
        CurrentData.points = points;        
    }

    void DrawBackgroundTexture() {
        DrawBackgroundTexture(Vector2.zero);
    }

    void DrawToolbar() {
        GUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.ExpandWidth(true));
        GUILayout.Label("Tile: " + tileWindow.GetSelectedTile().name);
        GUILayout.Label("Background");
        Type t = (Type)EditorGUILayout.EnumPopup(gridType);
        if (t != gridType) {
            gridTexture = null;
            gridType = t;
            InitTexture();
        }
        if (GUILayout.Button("Apply Changes")) {
            if (CurrentEditMode == EditMode.Collider)
                CurrentTile.UpdateCollision();
            if (CurrentEditMode == EditMode.WalkCollider)
                CurrentTile.UpdateWalkCollision();

        }
        GUILayout.EndHorizontal();
    }

    public void SetEditMode(EditMode mode) {
        if (mode == EditMode.WalkCollider) {
            CurrentEditMode = EditMode.WalkCollider;
        } else if (mode == EditMode.Collider) {
            CurrentEditMode = EditMode.Collider;
        }
    }

    void DrawLeftToolbar() {
        GUILayout.BeginArea(new Rect(0, EditorStyles.toolbar.fixedHeight, position.width / 4, position.height));
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Edit Mode", GUILayout.MaxWidth(80));
        EditMode mode = (EditMode)EditorGUILayout.EnumPopup(CurrentEditMode);
        SetEditMode(mode);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label("Collider Type", GUILayout.ExpandWidth(true));
        CurrentData.CurrentColliderType = (ColliderData.ColliderTypes)EditorGUILayout.EnumPopup(CurrentData.CurrentColliderType);
        GUILayout.EndHorizontal();

        if (CurrentEditMode == EditMode.Collider) {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Height", GUILayout.MaxWidth(60));
            CurrentTile.HitHeight = EditorGUILayout.Slider(CurrentTile.HitHeight, 0.0f, 1.0f);
            EditorGUILayout.EndHorizontal();
            Collider2D coll = CurrentTile.GetComponent<Collider2D>();
            if (coll != null) {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Is Trigger?", GUILayout.MaxWidth(80));
                coll.isTrigger = EditorGUILayout.Toggle(coll.isTrigger);
                EditorGUILayout.EndHorizontal();
            }
        }

        GUILayout.EndArea();
    }

    bool vertexFoldout = false;
    void DrawRightToolbar() {
        GUILayout.BeginArea(new Rect(position.width * 3 / 4, EditorStyles.toolbar.fixedHeight, position.width / 4, position.height));
        Vector2[] points = CurrentData.points;
        if (CurrentData.CurrentColliderType == ColliderData.ColliderTypes.Mesh) {
            vertexFoldout = EditorGUILayout.Foldout(vertexFoldout, "Vertices");
            if (vertexFoldout) {
                EditorGUI.indentLevel++;
                for (int i = 0; i < points.Length; i++) {
                    EditorGUILayout.LabelField(points[i].ToString());
                }
                EditorGUI.indentLevel--;
            }
        } else if (CurrentData.CurrentColliderType == ColliderData.ColliderTypes.Circle) {
            EditorGUILayout.LabelField("Center: " + CurrentData.cylinderCenter.ToString());
            EditorGUILayout.LabelField("Radius: " + CurrentData.radius);
        } else if (CurrentData.CurrentColliderType == ColliderData.ColliderTypes.Box) {
            ColliderData data = CurrentTile.MyColliderData;
            EditorGUILayout.LabelField("X1: " + CurrentData.boxBoundsX.x + " X2: " + CurrentData.boxBoundsX.y);
            EditorGUILayout.LabelField("Y1: " + CurrentData.boxBoundsY.x + " Y2: " + CurrentData.boxBoundsY.y);
        }
        GUILayout.EndArea();
    }

    void DrawBackgroundTexture(Vector2 offset) {
        InitTexture();
        //Rect rect = GUILayoutUtility.GetRect(128.0f, position.height);
        GUILayout.BeginArea(new Rect(position.width/4, EditorStyles.toolbar.fixedHeight, position.width/2, position.height));
        Rect rect = new Rect(0, 0, position.width/2, position.height);
        GUI.DrawTextureWithTexCoords(rect, gridTexture, new Rect(-offset.x / textureSize, (offset.y - rect.height) / textureSize, rect.width / textureSize, rect.height / textureSize), false);
        Rect origin = new Rect(10, 10, 0, 0);
        GUI.DrawTexture(new Rect(origin.x, origin.y, CurrentTile.SpriteWidth, CurrentTile.SpriteWidth), tileWindow.selectedTexture, ScaleMode.ScaleAndCrop, true);

        switch (CurrentData.CurrentColliderType) {
            case ColliderData.ColliderTypes.Box: DrawBoxCollider(origin); break;
            case ColliderData.ColliderTypes.Circle: DrawCylinderCollider(origin); break;
            case ColliderData.ColliderTypes.Mesh: DrawCollisionEditor(origin);  break;
            default: break;
        }
        GUILayout.EndArea();
    }

    void DrawCylinderCollider(Rect r) {
        Vector2 origin = new Vector2(r.x, r.y);

        if (!CurrentData.isValidCylinder()) {
            CurrentData.cylinderCenter = new Vector3(CurrentTile.SpriteWidth / 2, CurrentTile.SpriteHeight / 2, 0);
            CurrentData.radius = 1;
        }

        int id = 136635;
        Vector3 handlePos;

        //Draw center handle
        handlePos = new Vector3(CurrentData.cylinderCenter.x + origin.x, CurrentData.cylinderCenter.y + origin.y, 0);
        handlePos = (EditorUtilities.PositionHandle(id, handlePos) - origin);
        CurrentData.cylinderCenter = handlePos;

        //Draw radius handle
        handlePos = new Vector3(CurrentData.cylinderCenter.x + CurrentData.radius + origin.x, CurrentData.cylinderCenter.y + origin.y, 0);
        handlePos = (EditorUtilities.PositionHandle(id + 1, handlePos + new Vector3(0, 15, 0)) - origin - new Vector2(0, 15));
        CurrentData.radius = handlePos.x - CurrentData.cylinderCenter.x;
        PreviousHandleColor = Handles.color;
        Handles.color = Color.green;
        //Handles.SphereCap(id + 2, data.cylinderCenter + new Vector3(origin.x, origin.y, 0), Quaternion.identity, data.radius);
        Handles.DrawWireDisc(CurrentData.cylinderCenter + new Vector3(origin.x, origin.y, 0), Vector3.forward, CurrentData.radius);
        Handles.color = PreviousHandleColor;

        CurrentData.cylinderCenter.x = Mathf.Round(CurrentData.cylinderCenter.x);
        CurrentData.cylinderCenter.y = Mathf.Round(CurrentData.cylinderCenter.y);
        CurrentData.radius = Mathf.Round(CurrentData.radius);

        CurrentData.cylinderCenter.x = Mathf.Clamp(CurrentData.cylinderCenter.x, 0.0f, CurrentTile.SpriteWidth);
        CurrentData.cylinderCenter.y = Mathf.Clamp(CurrentData.cylinderCenter.y, 0.0f, CurrentTile.SpriteHeight);
        CurrentData.radius = Mathf.Clamp(CurrentData.radius, 1, CurrentData.SpriteWidth / 2);
        CurrentData.cylinderCenter.z = 0;

        EditorUtilities.SetPositionHandleValue(id, new Vector2(CurrentData.cylinderCenter.x, CurrentData.cylinderCenter.y));
        EditorUtilities.SetPositionHandleValue(id + 1, new Vector2(CurrentData.cylinderCenter.x + CurrentData.radius, CurrentData.cylinderCenter.y));
    }

    void DrawBoxCollider(Rect r) {
        Vector2 origin = new Vector2(r.x, r.y);
        ColliderData data = CurrentTile.MyColliderData;

        if (!CurrentData.isValidBox()) {
            CurrentData.boxBoundsX.y = CurrentTile.SpriteWidth;
            CurrentData.boxBoundsY.y = CurrentTile.SpriteHeight;
        }

        Vector3[] pt = new Vector3[] {
                new Vector3(CurrentData.boxBoundsX.x + origin.x, CurrentData.boxBoundsY.x + origin.y, 0),
                new Vector3(CurrentData.boxBoundsX.y + origin.x, CurrentData.boxBoundsY.x + origin.y, 0),
                new Vector3(CurrentData.boxBoundsX.y + origin.x, CurrentData.boxBoundsY.y + origin.y, 0),
                new Vector3(CurrentData.boxBoundsX.x + origin.x, CurrentData.boxBoundsY.y + origin.y, 0),
            };
        Color32 transparentColor = new Color32(127, 201, 122, 255);
        transparentColor.a = 10;
        Handles.DrawSolidRectangleWithOutline(pt, transparentColor, new Color32(127, 201, 122, 255));

        // Draw grab handles
        Vector3 handlePos;

        int id = 135635;

        // Draw top handle
        handlePos = (pt[0] + pt[1]) * 0.5f;
        handlePos = (EditorUtilities.PositionHandle(id + 0, handlePos) - origin);
        CurrentData.boxBoundsY.x = handlePos.y;
        if (CurrentData.boxBoundsY.x > CurrentData.boxBoundsY.y) CurrentData.boxBoundsY.x = CurrentData.boxBoundsY.y;

        // Draw bottom handle
        handlePos = (pt[2] + pt[3]) * 0.5f;
        handlePos = (EditorUtilities.PositionHandle(id + 1, handlePos) - origin);
        CurrentData.boxBoundsY.y = handlePos.y;
        if (CurrentData.boxBoundsY.y < CurrentData.boxBoundsY.x) CurrentData.boxBoundsY.y = CurrentData.boxBoundsY.x;

        // Draw left handle
        handlePos = (pt[0] + pt[3]) * 0.5f;
        handlePos = (EditorUtilities.PositionHandle(id + 2, handlePos) - origin);
        CurrentData.boxBoundsX.x = handlePos.x;
        if (CurrentData.boxBoundsX.x > CurrentData.boxBoundsX.y) CurrentData.boxBoundsX.x = CurrentData.boxBoundsX.y;

        // Draw right handle
        handlePos = (pt[1] + pt[2]) * 0.5f;
        handlePos = (EditorUtilities.PositionHandle(id + 3, handlePos) - origin);
        CurrentData.boxBoundsX.y = handlePos.x;
        if (CurrentData.boxBoundsX.y < CurrentData.boxBoundsX.x) CurrentData.boxBoundsX.y = CurrentData.boxBoundsX.x;

        CurrentData.boxBoundsX.x = Mathf.Round(CurrentData.boxBoundsX.x);
        CurrentData.boxBoundsX.y = Mathf.Round(CurrentData.boxBoundsX.y);
        CurrentData.boxBoundsY.x = Mathf.Round(CurrentData.boxBoundsY.x);
        CurrentData.boxBoundsY.y = Mathf.Round(CurrentData.boxBoundsY.y);

        // constrain
        CurrentData.boxBoundsX.x = Mathf.Clamp(CurrentData.boxBoundsX.x, 0.0f, CurrentTile.SpriteWidth);
        CurrentData.boxBoundsX.y = Mathf.Clamp(CurrentData.boxBoundsX.y, 0.0f, CurrentTile.SpriteHeight);
        CurrentData.boxBoundsY.x = Mathf.Clamp(CurrentData.boxBoundsY.x, 0.0f, CurrentTile.SpriteWidth);
        CurrentData.boxBoundsY.y = Mathf.Clamp(CurrentData.boxBoundsY.y, 0.0f, CurrentTile.SpriteHeight);

        EditorUtilities.SetPositionHandleValue(id + 0, new Vector2(0, CurrentData.boxBoundsY.x));
        EditorUtilities.SetPositionHandleValue(id + 1, new Vector2(0, CurrentData.boxBoundsY.y));
        EditorUtilities.SetPositionHandleValue(id + 2, new Vector2(CurrentData.boxBoundsX.x, 0));
        EditorUtilities.SetPositionHandleValue(id + 3, new Vector2(CurrentData.boxBoundsX.y, 0));
    }

    void InitTexture() {
        if (gridTexture != null)
            return;

        gridTexture = new Texture2D(textureSize, textureSize);
        Color c0 = Color.white;
        Color c1 = new Color(0.8f, 0.8f, 0.8f, 1.0f);

        switch (gridType) {
            case Type.LightChecked: c0 = new Color32(255, 255, 255, 255); c1 = new Color32(217, 217, 217, 255); break;
            case Type.MediumChecked: c0 = new Color32(178, 178, 178, 255); c1 = new Color32(151, 151, 151, 255); break;
            case Type.DarkChecked: c0 = new Color32(37, 37, 37, 255); c1 = new Color32(31, 31, 31, 255); break;
            case Type.BlackChecked: c0 = new Color32(14, 14, 14, 255); c1 = new Color32(0, 0, 0, 255); break;
            case Type.LightSolid: c0 = new Color32(255, 255, 255, 255); c1 = c0; break;
            case Type.MediumSolid: c0 = new Color32(178, 178, 178, 255); c1 = c0; break;
            case Type.DarkSolid: c0 = new Color32(37, 37, 37, 255); c1 = c0; break;
            case Type.BlackSolid: c0 = new Color32(0, 0, 0, 255); c1 = c0; break;
        }

        for (int y = 0; y < gridTexture.height; ++y) {
            for (int x = 0; x < gridTexture.width; ++x) {
                bool xx = (x < gridTexture.width / 2);
                bool yy = (y < gridTexture.height / 2);
                gridTexture.SetPixel(x, y, (xx == yy) ? c0 : c1);
            }
        }
        gridTexture.Apply();
        gridTexture.filterMode = FilterMode.Point;
        gridTexture.hideFlags = HideFlags.HideAndDontSave;
    }
}