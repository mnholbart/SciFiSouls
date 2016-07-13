using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class WeaponDatabase : ScriptableObject {

    public List<WeaponData> weapons = new List<WeaponData>();


    public void AddWeapon(WeaponData data) {
        weapons.Add(data);
        Refresh();
    }

    public void Refresh() {
        weapons = Resources.LoadAll<WeaponData>("WeaponData").ToList<WeaponData>();
        weapons = weapons.Where(item => item != null).ToList(); //Remove missing data
    }
}
