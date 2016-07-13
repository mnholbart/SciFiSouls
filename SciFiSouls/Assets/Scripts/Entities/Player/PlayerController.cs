using UnityEngine;
using System.Collections;
using System;

public class PlayerController : ActivitySystem {

    public int SpriteRotation = -90; //0 is if the sprite was facing left

    Player player;
    Die die;

    new void Start() {
        player = GetComponent<Player>();
        die = player.die;

        base.Start();
    }

    new void Update () {

        base.Update();
	}

    public void Task_RotateTowardsMouse() {
        float angle = GetMouseAngleDegrees();
        transform.eulerAngles = new Vector3(0, 0, angle + SpriteRotation);
    }

    public float GetMouseAngleDegrees() {
        Vector3 mouse = Input.mousePosition;
        Vector3 screenPoint = Camera.main.WorldToScreenPoint(transform.localPosition);
        Vector3 offset = new Vector2(mouse.x - screenPoint.x, mouse.y - screenPoint.y);
        float angle = Mathf.Atan2(offset.y, offset.x) * Mathf.Rad2Deg;

        return angle;
    }

    public Vector3 GetMouseWorldPosition() {
        Vector3 screenPoint = Camera.main.WorldToScreenPoint(transform.localPosition);
        return screenPoint;
    }

    bool CanRunTask_Rotate() {
        if (die && die.dead)
            return false;

        return true;
    }

    protected override void AddTaskRestrictions() {
        Add_CanRunTask_Function(Task_RotateTowardsMouse, CanRunTask_Rotate);
    }

    protected override void AddTasks() {
        AddTask(Task_RotateTowardsMouse);
    }

    protected override void AddOtherTaskRestrictions() {

    }

    protected override void AddActivityRestrictions() {

    }

    protected override void AddActivities() {

    }

    protected override void AddOtherActivityRestrictions() {

    }
}
