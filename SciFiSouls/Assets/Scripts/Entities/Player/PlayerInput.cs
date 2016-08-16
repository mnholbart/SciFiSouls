using UnityEngine;
using System.Collections;
using System;

public class PlayerInput : ActivitySystem {

    Player player;
    Movement movement;
    Sprint sprint;
    Dodge dodge;
    Sneak sneak;
    Shoot shoot;
    Inventory inventory;
    ZMovement zMovement;

    new void Start() {
        player = GetComponent<Player>();
        movement = player.movement;
        sprint = player.sprint;
        dodge = player.dodge;
        sneak = player.sneak;
        shoot = player.shoot;
        inventory = player.inventory;
        zMovement = player.zMovement;

        base.Start();
    }

	new void Update () {
        if (movement)
            MovementInput();

        if (dodge)
            DodgeInput();

        if (shoot)
            ShootInput();

        if (inventory)
            InventoryInput();

        if (zMovement)
            zMovementInput();

        base.Update();
	}

    void zMovementInput() {
        if (Input.GetKeyDown(KeyCode.T))
            zMovement.GoUpToHeight(-1f, 2);
        if (Input.GetKeyDown(KeyCode.Y))
            zMovement.GoDownToHeight(0f, 1);
    }

    void InventoryInput() {
        int switchEquipped = -1;
        if (Input.GetKeyDown(KeyCode.Alpha1))
            switchEquipped = 1;
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            switchEquipped = 2;
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            switchEquipped = 3;
        else if (Input.GetKeyDown(KeyCode.Alpha4))
            switchEquipped = 4;
        else if (Input.GetKeyDown(KeyCode.Alpha5))
            switchEquipped = 5;

        if (switchEquipped > 0)
            inventory.SwitchWeapon(switchEquipped);
    }

    void ShootInput() {
        if (Input.GetKey(KeyCode.Mouse0)) {
            shoot.TryStartActivity(shoot.Activity_Shoot);
        }
        if (Input.GetKey(KeyCode.Mouse1)) {
            shoot.TryStartActivity(shoot.Activity_Aim);
        }
    }

    void DodgeInput() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            dodge.TryStartActivity(dodge.Activity_Dodge);
        }
    }

    void MovementInput() {
        Vector3 moveVector = Vector3.zero;
        if (Input.GetKey(KeyCode.W)) {
            moveVector.y += 1;
        }
        if (Input.GetKey(KeyCode.S)) {
            moveVector.y -= 1;
        }
        if (Input.GetKey(KeyCode.A)) {
            moveVector.x -= 1;
        }
        if (Input.GetKey(KeyCode.D)) {
            moveVector.x += 1;
        }
        movement.MoveVector = moveVector;
    }

    protected override void AddTaskRestrictions() {

    }

    protected override void AddTasks() {

    }


    protected override void AddOtherTaskRestrictions() {
        if (sprint)
            sprint.Add_CanRunTask_Function(sprint.Task_Sprint, CanRunTask_Sprint);

        if (sneak)
            sneak.Add_CanRunTask_Function(sneak.Task_Sneak, CanRunTask_Sneak);
    }

    bool CanRunTask_Sprint() {
        if (!Input.GetKey(KeyCode.LeftShift))
            return false;

        return true;
    }

    bool CanRunTask_Sneak() {
        if (!Input.GetKey(KeyCode.Z))
            return false;

        return true;
    }

    protected override void AddActivityRestrictions() {
    }

    protected override void AddActivities() {
    }

    protected override void AddOtherActivityRestrictions() {
    }
}
