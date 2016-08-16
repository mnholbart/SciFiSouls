using UnityEngine;
using System.Collections;
using System;

public class Sneak : ActivitySystem {

    public bool sneaking = false;
    
    Sprint sprint;

    new void Awake() {

        base.Awake();
    }

    new void Start () {
        sprint = entity.sprint;

        base.Start();
	}
	
	new void Update () {

        base.Update();
	}

    public void Task_Sneak() {
        sneaking = true;
    }

    void OnFail_Task_Sneak() {
        sneaking = false;
    }

    bool CanRunTask_Sprint() {
        if (sneaking)
            return false;

        return true;
    }

    protected override void AddActivities() {

    }

    protected override void AddActivityRestrictions() {

    }

    protected override void AddOtherActivityRestrictions() {

    }

    protected override void AddOtherTaskRestrictions() {
        sprint.Add_CanRunTask_Function(sprint.Task_Sprint, CanRunTask_Sprint);
    }

    protected override void AddTaskRestrictions() {
    }

    protected override void AddTasks() {
        AddTask(Task_Sneak);
        Add_OnFailTask_Function(Task_Sneak, OnFail_Task_Sneak);
    }
}
