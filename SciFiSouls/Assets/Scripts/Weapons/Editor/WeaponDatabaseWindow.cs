using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Enum = System.Enum;
using System.Linq;

public class WeaponDatabaseWindow : EditorWindow {

    [MenuItem("Tools/Weapon Database")]
    public static void ShowWindow() {
        WeaponDatabaseWindow window = (WeaponDatabaseWindow)EditorWindow.GetWindow(typeof(WeaponDatabaseWindow));
        window.titleContent.text = "Weapon Database";
        window.Show();

        window.database = Resources.Load("WeaponDatabase", typeof(WeaponDatabase)) as WeaponDatabase;
        window.Initialize();
        window.database.Refresh();

        window.minSize = new Vector2(250, 350);
    }

    WeaponDatabase database;
    int ViewMask = 0;
    int ViewMaskSelectionIndex = 0;
    List<string> ViewMaskOptions = new List<string>();

    List<bool> weaponFoldouts = new List<bool>();
    List<bool> allSpritesFoldouts = new List<bool>();
    List<List<bool>> spritesFoldouts = new List<List<bool>>();

    void Initialize() {
        string[] names = Enum.GetNames(typeof(WeaponData.WeaponType));
        ViewMaskOptions.Add("View ALL Weapon Types");
        for (int i = 1; i < names.Length; i++) {
            ViewMaskOptions.Add("View " + names[i] + " Weapon Types");
        }

        if (ViewMaskSelectionIndex == 0) {
            for (int i = 0; i < names.Length; i++) {
                ViewMask = ViewMask | (int)Mathf.Pow(2, i);
                ResetFoldouts();
            }
        }
    }

    void OnGUI() {
        if (GUILayout.Button("Add Weapon \"NewWeapon\"")) {
            if (Resources.Load("WeaponData/NewWeapon") == null) {
                WeaponData data = ScriptableObjectUtility.CreateWeaponDataAsset();
                RenameWeapon("NewWeapon", data);
                database.AddWeapon(data);
            }
        }
        if (GUILayout.Button("Manual Refresh")) {
            database.Refresh();
        }

        GUILayout.Space(25);

        DrawWeaponSelection();

        DrawWeapons();

        SaveDatabase();

    }

    void SaveDatabase() {
        EditorUtility.SetDirty(database);
        for (int i = 0; i < database.weapons.Count; i++) {
            EditorUtility.SetDirty(database.weapons[i]);
        }
    }

    void DrawWeaponSelection() {
        int temp = GUILayout.SelectionGrid(ViewMaskSelectionIndex, ViewMaskOptions.ToArray(), 1);
        if (temp == ViewMaskSelectionIndex)
            return;
        else ViewMaskSelectionIndex = temp;

        if (ViewMaskSelectionIndex == 0) { 
            string[] names = Enum.GetNames(typeof(WeaponData.WeaponType));
            for (int i = 0; i < names.Length; i++) {
                ViewMask = ViewMask | (int)Mathf.Pow(2, i);
                ResetFoldouts();
            }
        }
        else {
            ViewMask = (int)Mathf.Pow(2, ViewMaskSelectionIndex);
            ResetFoldouts();
        }
        
    }

    void ResetFoldouts() {
        weaponFoldouts.Clear();
        allSpritesFoldouts.Clear(); ;
        spritesFoldouts.Clear();
    }

    void CheckAddFoldout(int i) {
        while (i >= weaponFoldouts.Count)
            weaponFoldouts.Add(false);
        while (i >= allSpritesFoldouts.Count)
            allSpritesFoldouts.Add(false);
    }

    void CheckAddAllSpriteFoldout(int index) {
        while (index >= spritesFoldouts.Count) {
            spritesFoldouts.Add(new List<bool>());
        }
    }

    void CheckAddSpriteFoldout(int index, int i) {
        //Debug.Log(spritesFoldouts[index].Count);
        while (i >= spritesFoldouts[index].Count)
            spritesFoldouts[index].Add(false);
        //Debug.Log(spritesFoldouts[index].Count);
    }

    void DrawWeapons() {
        if (ViewMask == 0)
            return;

        List<WeaponData> data = database.weapons;

        if (data.Count == 0) {
            EditorGUILayout.LabelField("No Weapons Found");
            return;
        }

        for (int i = 0; i < data.Count; i++) {
            WeaponData curr = data[i];
            if (!InMask(curr) || curr == null)
                continue;
            CheckAddFoldout(i);
            EditorGUILayout.BeginHorizontal();
            weaponFoldouts[i] = EditorGUILayout.Foldout(weaponFoldouts[i], curr.name);
            if (GUILayout.Button("Delete")) {
                database.weapons.Remove(data[i]);
                DeleteWeapon(curr);
                i--;
                continue;
            }
            EditorGUILayout.EndHorizontal();
            if (weaponFoldouts[i])
                DrawWeapon(curr, i);
        }
    }

    void DrawWeapon(WeaponData data, int index) {
        EditorGUI.indentLevel++;
        GUILayout.BeginHorizontal(); //Name

        string temp = EditorGUILayout.DelayedTextField("Weapon Name", data.name);
        temp = new string(temp.ToCharArray().Where(c => !char.IsWhiteSpace(c)).ToArray());
        if (temp != data.name || temp != data.Name) {
            data.name = temp;
            data.Name = temp;
            RenameWeapon(temp, data);
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal(); //Weapon Type
        EditorGUILayout.PrefixLabel("Weapon Type");
        WeaponData.WeaponType newType = (WeaponData.WeaponType)EditorGUILayout.EnumPopup(data.MyWeaponType);
        if (newType != data.MyWeaponType) {
            data.MyWeaponType = newType;
        }

        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal(); //Weapon sprite
        EditorGUILayout.PrefixLabel("Weapon Sprite");
        data.WeaponSprite = EditorGUILayout.ObjectField(data.WeaponSprite, typeof(Sprite), false) as Sprite;
        GUILayout.EndHorizontal();

        CheckAddAllSpriteFoldout(index);
        allSpritesFoldouts[index] = EditorGUILayout.Foldout(allSpritesFoldouts[index], "Sprites");
        if (allSpritesFoldouts[index])
            DrawSprites(data, index);

        EditorGUILayout.BeginHorizontal(); //Weapon sprite offset
        data.WeaponRenderOffsetFromPlayer = EditorGUILayout.Vector2Field("Render Offset From Entity", data.WeaponRenderOffsetFromPlayer);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal(); //Projectile range
        if (data.MyWeaponType == WeaponData.WeaponType.Ranged) {
            data.Range = EditorGUILayout.FloatField("Range", data.Range);
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal(); //Another offset
        data.Offset = EditorGUILayout.Vector2Field("Another Offset", data.Offset);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal(); //Projectile duration
        if (data.MyWeaponType == WeaponData.WeaponType.Ranged) {
            data.Duration = EditorGUILayout.FloatField("Duration", data.Duration);
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal(); //Attack Cooldown
        EditorGUILayout.LabelField("Attack Cooldown");
        data.AttackCooldown = EditorGUILayout.FloatField(data.AttackCooldown);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal(); //Projectile speed
        if (data.MyWeaponType == WeaponData.WeaponType.Ranged) {
            EditorGUILayout.LabelField("Speed");
            data.Speed = EditorGUILayout.FloatField(data.Speed);
        }
        EditorGUILayout.EndHorizontal();
        EditorGUI.indentLevel--;
    }

    void DrawSprites(WeaponData data, int index) {
        for (int i = 0; i < data.Sprites.Count; i++) {
            EditorGUILayout.BeginHorizontal();
            CheckAddSpriteFoldout(index, i);
            EditorGUI.indentLevel++;
            spritesFoldouts[index][i] = EditorGUILayout.Foldout(spritesFoldouts[index][i], "Sprite: " + i);
            if (GUILayout.Button("Delete")) {
                data.Sprites.RemoveAt(i);
                data.SpriteRotations.RemoveAt(i);
                data.SpriteScales.RemoveAt(i);
                spritesFoldouts[index].RemoveAt(i);
                i--;
                continue;
            }
            EditorGUILayout.EndHorizontal();
            if (!spritesFoldouts[index][i]) {
                EditorGUI.indentLevel--;
                continue;
            }

            EditorGUI.indentLevel++;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Sprite");
            data.Sprites[i] = EditorGUILayout.ObjectField(data.Sprites[i], typeof(Sprite), false) as Sprite;
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Rotation");
            data.SpriteRotations[i] = EditorGUILayout.FloatField(data.SpriteRotations[i]);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            //EditorGUILayout.LabelField("Scale");
            data.SpriteScales[i] = EditorGUILayout.Vector3Field("Scale", data.SpriteScales[i]);
            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel--;
            EditorGUI.indentLevel--;
        }
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(" ", GUILayout.Width((position.width - 150)/2));
        if (GUILayout.Button("Add Sprite", GUILayout.Width(150))) { //Width same as number above
            data.Sprites.Add(null);
            data.SpriteRotations.Add(0f);
            data.SpriteScales.Add(new Vector3(1,1,1));
            spritesFoldouts[index].Add(false);
        }
        EditorGUILayout.EndHorizontal();
    }

    bool InMask(WeaponData data) {
        return (ViewMask & (int)data.MyWeaponType) == (int)data.MyWeaponType;
    }

    void RenameWeapon(string newName, WeaponData data) {
        string assetPath = AssetDatabase.GetAssetPath(data.GetInstanceID());
        AssetDatabase.RenameAsset(assetPath, newName);
        EditorUtility.SetDirty(data);
    }

    void DeleteWeapon(WeaponData data) {
        string assetPath = AssetDatabase.GetAssetPath(data.GetInstanceID());
        AssetDatabase.DeleteAsset(assetPath);
        AssetDatabase.Refresh();
    }
}