using UnityEngine;
using System.Collections.Generic;
using System;

public class Inventory : ActivitySystem {

    WeaponController WeaponObject = null;
    int WeaponIndexToSwitchTo = -1;
    public int NumberQuickSwapSlots = 9;

    Entity entity;
    Shoot shoot;

    //Array of weapon data held 1-9 or whatever
    //Activity_ChangeWeapon will tell WeaponObj to switch the current weapon data to the weapon data in 1-9
    public WeaponData[] QuickSwapSlots;

    new void Awake() {
        entity = GetComponent<Entity>();
        QuickSwapSlots = new WeaponData[NumberQuickSwapSlots];

        base.Awake();
    }

    new void Start() {
        shoot = entity.shoot;

        InstantiateWeaponPrefab();

        QuickSwapSlots[1] = InventoryManager.instance.GetWeaponByName("Gun1");
        QuickSwapSlots[2] = InventoryManager.instance.GetWeaponByName("Gun2");

        base.Start();
    }

    void InstantiateWeaponPrefab() {
        GameObject prefab = Resources.Load("Prefabs/Weapons/WeaponController", typeof(GameObject)) as GameObject;
        GameObject g = Instantiate(prefab);

        g.transform.SetParent(transform, false);
        g.transform.position = new Vector3(0, 0, 0);

        WeaponObject = g.GetComponent<WeaponController>();

        GameObject WeaponAttackScripts = Instantiate(Resources.Load<GameObject>("Prefabs/Weapons/RangedWeaponAttackScripts"));
        WeaponAttackScripts.transform.SetParent(transform, false);
        WeaponAttackScripts.transform.localPosition = Vector3.zero;
        WeaponObject.WeaponAttackScripts = WeaponAttackScripts;
    }

    public WeaponController GetCurrentlyEquippedWeapon() {
        return WeaponObject;
    }

    bool CanStartActivity_Shoot() {
        if (WeaponObject.CurrentEquippedWeaponType != WeaponController.EquippedWeaponType.ranged)
            return false;

        return true;
    }

    bool CanStartActivity_Aim() {
        if (WeaponObject.CurrentEquippedWeaponType != WeaponController.EquippedWeaponType.ranged)
            return false;

        return true;
    }

    bool CanStartActivity_SwitchToWeaponIndex() {
        if (WeaponIndexToSwitchTo < 0)
            return false;

        if (QuickSwapSlots[WeaponIndexToSwitchTo] == null)
            return false;

        if (QuickSwapSlots[WeaponIndexToSwitchTo] == WeaponObject.EquippedWeapon)
            return false;

        return true;
    }

    void OnFailActivity_SwitchToWeaponIndex() {
        WeaponIndexToSwitchTo = -1;
    }

    public void Activity_SwitchToWeaponIndex() {
        int index = WeaponIndexToSwitchTo;
        WeaponIndexToSwitchTo = -1;

        WeaponObject.TrySwapWeapon(QuickSwapSlots[index]);
    }

    public void SwitchWeapon(int index) {
        WeaponIndexToSwitchTo = index;
        TryStartActivity(Activity_SwitchToWeaponIndex);
    }

    protected override void AddActivities() {
        AddActivity(Activity_SwitchToWeaponIndex);
        Add_OnFailActivity_Function(Activity_SwitchToWeaponIndex, OnFailActivity_SwitchToWeaponIndex);
    }

    protected override void AddActivityRestrictions() {
        Add_CanRunActivity_Function(Activity_SwitchToWeaponIndex, CanStartActivity_SwitchToWeaponIndex);
    }

    protected override void AddOtherActivityRestrictions() {
        shoot.Add_CanRunActivity_Function(shoot.Activity_Shoot, CanStartActivity_Shoot);
        shoot.Add_CanRunActivity_Function(shoot.Activity_Aim, CanStartActivity_Aim);
    }

    protected override void AddOtherTaskRestrictions() {

    }

    protected override void AddTaskRestrictions() {

    }

    protected override void AddTasks() {

    }
}
