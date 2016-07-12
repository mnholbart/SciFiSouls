using UnityEngine;
using System.Collections;
using System;

public class Die : ActivitySystem {

    public bool dead = false;

    Entity entity;
    Sprint sprint;
    Stamina stamina;
    Movement movement;
    Dodge dodge;

    new void Awake() {
        entity = GetComponent<Entity>();

        base.Awake();
    }

    new void Start() {
        stamina = entity.stamina;
        sprint = entity.sprint;
        movement = entity.movement;
        dodge = entity.dodge;

        base.Start();
    }

    // --------------------------
    //           Tasks
    // --------------------------

    //--Other Task Restrictions--
    protected override void AddOtherTaskRestrictions() {
        stamina.Add_CanRunTask_Function(stamina.Task_RegenerateStamina, CanRunTask_RegenerateStamina);
        sprint.Add_CanRunTask_Function(sprint.Task_Sprint, CanRunTask_Sprint);
        movement.Add_CanRunTask_Function(movement.Task_Move, CanRunTask_Move);
    }

    bool CanRunTask_RegenerateStamina() {
        if (dead)
            return false;

        return true;
    }

    bool CanRunTask_Sprint() {
        if (dead)
            return false;

        return true;
    }

    bool CanRunTask_Move() {
        if (dead)
            return false;

        return true;
    }

    bool CanRunActivity_Dodge() {
        if (dead)
            return false;

        return true;
    }

    //----------My Tasks---------

    protected override void AddTaskRestrictions() {

    }

    protected override void AddTasks() {

    }

    protected override void AddActivityRestrictions() {

    }

    protected override void AddActivities() {
        AddActivity(Activity_Kill);
    }

    public void Activity_Kill() {
        dead = true;
    }

    protected override void AddOtherActivityRestrictions() {
        dodge.Add_CanRunActivity_Function(dodge.Activity_Dodge, CanRunActivity_Dodge);
    }
}
