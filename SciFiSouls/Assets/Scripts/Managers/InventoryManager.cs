using UnityEngine;
using System.Collections;

public class InventoryManager : MonoBehaviour {

    public static InventoryManager instance;

    public WeaponDatabase database;

    void Awake() {
        if (instance != null) {
            Debug.LogError("Attempted to create more than one CameraManager instance");
            Destroy(instance);
        }
        instance = this;

        database = Resources.Load("WeaponDatabase", typeof(WeaponDatabase)) as WeaponDatabase;
        database.Refresh();
    }

    void Start() {

    }

    public WeaponData GetWeaponByName(string test) {
        WeaponData data = database.weapons.FindByName<WeaponData>(test);
        return data;
    }
}
