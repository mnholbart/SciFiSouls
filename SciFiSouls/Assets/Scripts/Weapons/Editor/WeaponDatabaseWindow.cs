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

    void Initialize() {
        string[] names = Enum.GetNames(typeof(WeaponData.WeaponType));
        ViewMaskOptions.Add("View ALL Weapon Typees");
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
        //weaponFoldouts.Add(false);
    }

    void CheckAddFoldout(int i) {
        while (i >= weaponFoldouts.Count)
            weaponFoldouts.Add(false);
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
                DrawWeapon(curr);
        }
    }

    void DrawWeapon(WeaponData data) {
        EditorGUI.indentLevel++;
        GUILayout.BeginHorizontal(); //Name

        string temp = EditorGUILayout.DelayedTextField("Weapon Sprite", data.name);
        if (temp != data.name) {
            data.name = temp;
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
        GUILayout.BeginHorizontal(); //Projectile sprite
        if (data.MyWeaponType == WeaponData.WeaponType.Ranged) { 
            EditorGUILayout.LabelField("Projectile Sprite");
            data.ProjectileSprite = EditorGUILayout.ObjectField(data.ProjectileSprite, typeof(Sprite), false) as Sprite;
        }
        GUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal(); //Weapon sprite offset
        data.WeaponRenderOffsetFromPlayer = EditorGUILayout.Vector2Field("Render Offset From Entity", data.WeaponRenderOffsetFromPlayer);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal(); //Projectile range
        if (data.MyWeaponType == WeaponData.WeaponType.Ranged) {
            data.ProjectileRange = EditorGUILayout.FloatField("Projectile Range/Duration", data.ProjectileRange);
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal(); //Attack logic script
        EditorGUILayout.LabelField("Attack Script");
        data.AttackScript = EditorGUILayout.ObjectField(data.AttackScript, typeof(AttackBase), false) as AttackBase;
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal(); //Attack Cooldown
        EditorGUILayout.LabelField("Attack Cooldown");
        data.AttackCooldown = EditorGUILayout.FloatField(data.AttackCooldown);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal(); //Projectile speed
        if (data.MyWeaponType == WeaponData.WeaponType.Ranged) {
            EditorGUILayout.LabelField("Projectile Speed");
            data.ProjectileSpeed = EditorGUILayout.FloatField(data.ProjectileSpeed);
        }
        EditorGUILayout.EndHorizontal();
        EditorGUI.indentLevel--;
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