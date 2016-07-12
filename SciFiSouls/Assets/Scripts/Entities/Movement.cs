using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(Rigidbody2D))]
public class Movement : ActivitySystem {

    public float MoveSpeed = 40;
    public Vector3 MoveVector;
    public Vector3 MoveVectorLastFrame;

    Entity entity;
    public Rigidbody2D body;
    Sprint sprint;
    Sneak sneak;

    public enum MovementType {
        Sneak,
        Walk,
        Run
    }
    public MovementType CurrMovementType;

    new void Awake() {
        entity = GetComponent<Entity>();

        base.Awake();
    }

    new void Start() {
        sprint = entity.sprint;
        sneak = entity.sneak;
        body = GetComponent<Rigidbody2D>();

        base.Start();
    }
    

    public void Task_Move() {
        CurrMovementType = MovementType.Walk;
        MoveVector.Normalize();
        MoveVector.x *= MoveSpeed * Time.deltaTime;
        MoveVector.y *= MoveSpeed * Time.deltaTime;
        
        if (sprint && sprint.sprinting) {
            CurrMovementType = MovementType.Run;
            MoveVector.x *= 1.5f;
            MoveVector.y *= 1.5f;
        } else if (sneak && sneak.sneaking) {
            CurrMovementType = MovementType.Sneak;
            MoveVector.x *= .5f;
            MoveVector.y *= .5f;
        }

        body.velocity = MoveVector;
        MoveVectorLastFrame = MoveVector;
    }

    public bool MovedLastFrame() {
        return MoveVectorLastFrame != Vector3.zero;
    }

    // --------------------------
    //           Tasks
    // --------------------------

    //--Other Task Restrictions--
    protected override void AddOtherTaskRestrictions() {
        sprint.Add_CanRunTask_Function(sprint.Task_Sprint, CanRunTask_Sprint);
    }

    bool CanRunTask_Sprint() {
        if (!MovedLastFrame())
            return false;

        return true;
    }

    bool CanRunTask_Dodge() {
        if (!MovedLastFrame())
            return false;

        return true;
    }

    protected override void AddTaskRestrictions() {
        Add_CanRunTask_Function(Task_Move, CanRunTask_Move);
    }

    bool CanRunTask_Move() {
        return true;
    }

    protected override void AddTasks() {
        AddTask(Task_Move);
    }

    protected override void AddActivityRestrictions() {
    }

    protected override void AddActivities() {
    }

    protected override void AddOtherActivityRestrictions() {
    }
}
