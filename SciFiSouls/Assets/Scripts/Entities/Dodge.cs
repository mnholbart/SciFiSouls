using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(Movement))]
public class Dodge : ActivitySystem {
    
    public bool dodging = false;
    public float DodgeSpeed = 320;
    public float DodgeDuration = 0.5f;
    public float DodgeStaminaCost = 15f;

    Entity entity;
    Sprint sprint;
    Movement movement;
    Stamina stamina;

    new void Awake() {
        entity = GetComponent<Entity>();

        base.Awake();
    }

    new void Start() {
        sprint = entity.sprint;
        movement = entity.movement;
        stamina = entity.stamina;

        base.Start();
    }

    IEnumerator StartDodge() {
        dodging = true;
        Vector3 dodgeDirection = movement.MoveVector;

        dodgeDirection.Normalize();
        dodgeDirection.x *= DodgeSpeed * Time.deltaTime;
        dodgeDirection.y *= DodgeSpeed * Time.deltaTime;

        movement.body.velocity = dodgeDirection;

        float duration = DodgeDuration;
        while (duration > 0) {
            yield return null;
            duration -= Time.deltaTime;
            stamina.DrainStamina(DodgeStaminaCost / DodgeDuration * Time.deltaTime);
        }
        dodging = false;
    }

    public void Activity_Dodge() {
        StartCoroutine(StartDodge());
    }

    protected override void AddActivities() {
        AddActivity(Activity_Dodge);
    }

    protected override void AddActivityRestrictions() {
        Add_CanRunActivity_Function(Activity_Dodge, CanRunActivity_Dodge);
    }

    bool CanRunActivity_Dodge() {
        if (dodging)
            return false;

        return true;
    }

    bool CanRunTask_RegenerateStamina() {
        if (dodging)
            return false;

        return true;
    }

    bool CanRunTask_Sprint() {
        if (dodging)
            return false;

        return true;
    }

    bool CanRunTask_Move() {
        if (dodging)
            return false;

        return true;
    }

    protected override void AddOtherActivityRestrictions() {
        if (sprint)
            sprint.Add_CanRunTask_Function(sprint.Task_Sprint, CanRunTask_Sprint);
    }

    protected override void AddOtherTaskRestrictions() {
        stamina.Add_CanRunTask_Function(stamina.Task_RegenerateStamina, CanRunTask_RegenerateStamina);
        movement.Add_CanRunTask_Function(movement.Task_Move, CanRunTask_Move);
    }

    protected override void AddTaskRestrictions() {
    }

    protected override void AddTasks() {
    }
}
