﻿using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(Inventory))]
public class Shoot : ActivitySystem {
    
    Inventory inventory;

    new void Awake() {

        base.Awake();
    }

    new void Start() {
        inventory = entity.inventory;
        base.Start();
    }

    new void Update () {

        base.Update();
	}

    public void Activity_Shoot() {
        inventory.GetCurrentlyEquippedWeapon().Shoot();
    }

    public void Activity_Aim() {

    }

    protected override void AddActivities() {
        AddActivity(Activity_Shoot);
        AddActivity(Activity_Aim);
    }

    protected override void AddActivityRestrictions() {

    }

    protected override void AddOtherActivityRestrictions() {

    }

    protected override void AddOtherTaskRestrictions() {

    }

    protected override void AddTaskRestrictions() {

    }

    protected override void AddTasks() {

    }
}
