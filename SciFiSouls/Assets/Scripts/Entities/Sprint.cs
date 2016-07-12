using UnityEngine;
using System.Collections.Generic;
using System;

public class Sprint : ActivitySystem {

    public bool sprinting = false;
    public float StaminaDrain = 20f;

    Entity entity;
    Stamina stamina;
    Sneak sneak;

    delegate void UseStamina(float amount);

    new void Awake() {
        entity = GetComponent<Entity>();

        base.Awake();
    }

    new void Start() {
        stamina = entity.stamina;
        sneak = entity.sneak;

        base.Start();
    }

    new void Update() {
        if (sprinting && stamina) {
            if (stamina.CurrentStamina - StaminaDrain * Time.deltaTime <= 0)
                stamina.OverExertion();

            stamina.DrainStamina(StaminaDrain * Time.deltaTime);
        }

        base.Update();
    }

    // --------------------------
    //           Tasks
    // --------------------------
    protected override void AddOtherTaskRestrictions() {
        if (stamina)
            stamina.Add_CanRunTask_Function(stamina.Task_RegenerateStamina, CanRunTask_RegenerateStamina);
        if (sneak) 
            sneak.Add_CanRunTask_Function(sneak.Task_Sneak, CanRunTask_Sneak);
    }

    bool CanRunTask_RegenerateStamina() {
        if (sprinting)
            return false;

        return true;
    }
    public void Task_Sprint() {
        sprinting = true;
    }

    void OnFail_Task_Sprint() {
        sprinting = false;
    }

    public bool CanRunTask_Sprint() {
        return true;
    }

    bool CanRunTask_Sneak() {
        if (sprinting)
            return false;

        return true;
    }

    protected override void AddTaskRestrictions() {
        Add_CanRunTask_Function(Task_Sprint, CanRunTask_Sprint);
    }

    protected override void AddTasks() {
        AddTask(Task_Sprint);
        Add_OnFailTask_Function(Task_Sprint, OnFail_Task_Sprint);
    }

    protected override void AddActivityRestrictions() {

    }

    protected override void AddActivities() {

    }

    protected override void AddOtherActivityRestrictions() {

    }
}
