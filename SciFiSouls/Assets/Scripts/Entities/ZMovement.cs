using UnityEngine;
using System.Collections;
using System;

public class ZMovement : ActivitySystem, IChangeZHeight {

    float CurrentZ = 0;
    int currentFloor = 1;

    float DesiredZ = 1;
    int desiredFloor = -1;
    SpriteRenderer sr { get { return entity.SpriteRenderer; } }
    Inventory inv { get { return entity.inventory; } }

    public void ForceSetFloor(int i, float zHeight) {
        currentFloor = i;
        CurrentZ = zHeight;

        string layerName = GameManager.data.GetLayerIndex("Floor" + i.ToString()).ToString();
        sr.sortingLayerName = layerName;
        if (inv)
            inv.GetCurrentlyEquippedWeapon().SetLayerName(layerName);
        transform.position = new Vector3(transform.position.x, transform.position.y, CurrentZ);
    }

    public void Activity_GoUpFloor() {
        CurrentZ = DesiredZ;
        string layerName = GameManager.data.GetLayerIndex("Floor" + desiredFloor.ToString()).ToString();
        sr.sortingLayerName = layerName;
        if (inv)
            inv.GetCurrentlyEquippedWeapon().SetLayerName(layerName);
        transform.position = new Vector3(transform.position.x, transform.position.y, CurrentZ);
    }

    public void Activity_GoDownFloor() {
        CurrentZ = DesiredZ;        
        string layerName = GameManager.data.GetLayerIndex("Floor" + desiredFloor.ToString()).ToString();
        sr.sortingLayerName = layerName;
        if (inv)
            inv.GetCurrentlyEquippedWeapon().SetLayerName(layerName);
        transform.position = new Vector3(transform.position.x, transform.position.y, CurrentZ);
    }

    void OnSuccess_GoUpFloor() {
        ResetZMovement();
    }

    void OnSuccess_GoDownFloor() {
        ResetZMovement();
    }

    void OnFail_GoUpFloor() {
        ResetZMovement();
    }

    void OnFail_GoDownFloor() {
        ResetZMovement();
    }

    void ResetZMovement() {
        desiredFloor = -1;
        DesiredZ = -1;
    }

    protected override void AddTaskRestrictions() {

    }

    public void Task_CheckNewFloor() {
        //Cant do because we dont know new floors Z height, so I think once rooms are made each room will have to have its
        //height set for each floor which should be more than enough, 5 floor heights per room, once thats done 
        //Can have falling or flying up set to the next up floor level

        if (entity.transform.position.z < CurrentZ) { //Go up floor

        }
        if (entity.transform.position.z > CurrentZ) { //Go down floor

        }
    }

    protected override void AddTasks() {
        AddTask(Task_CheckNewFloor);
    }

    protected override void AddOtherTaskRestrictions() {

    }

    protected override void AddActivityRestrictions() {

    }

    protected override void AddActivities() {
        AddActivity(Activity_GoUpFloor);
        AddActivity(Activity_GoDownFloor);

        Add_OnSuccessActivity_Function(Activity_GoUpFloor, OnSuccess_GoUpFloor);
        Add_OnFailActivity_Function(Activity_GoUpFloor, OnFail_GoUpFloor);
        Add_OnSuccessActivity_Function(Activity_GoDownFloor, OnSuccess_GoDownFloor);
        Add_OnFailActivity_Function(Activity_GoDownFloor, OnFail_GoDownFloor);
    }

    protected override void AddOtherActivityRestrictions() {
        Add_CanRunActivity_Function(Activity_GoUpFloor, CanRunActivity_GoUpFloor);
        Add_CanRunActivity_Function(Activity_GoDownFloor, CanRunActivity_GoDownFloor);
    }

    public bool CanRunActivity_GoUpFloor() {
        if (DesiredZ > CurrentZ)
            return false;

        if (desiredFloor > 5)
            return false;

        if (DesiredZ > 0)
            return false;

        return true;
    }

    public bool CanRunActivity_GoDownFloor() {
        if (DesiredZ < CurrentZ)
            return false;

        if (desiredFloor < 1)
            return false;

        if (DesiredZ > 0)
            return false;

        return true;
    }

    public void GoUpToHeight(float height, int floor) {
        DesiredZ = height;
        desiredFloor = floor;
        TryStartActivity(Activity_GoUpFloor);
    }

    public void GoDownToHeight(float height, int floor) {
        DesiredZ = height;
        desiredFloor = floor;
        TryStartActivity(Activity_GoDownFloor);
    }
}
