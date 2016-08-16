using UnityEngine;
using System.Collections;
using System;

public class Die : ActivitySystem {

    public bool dead = false;
    
    Sprint sprint {  get { return entity.sprint; } }
    Stamina stamina {  get { return entity.stamina; } }
    Movement movement { get { return entity.movement; } }
    Dodge dodge { get { return entity.dodge; } }
    Shoot shoot { get { return entity.shoot; } }
    Inventory inventory { get { return entity.inventory; } }
    PlayerController controller;


    new void Start() {
        if (entity is Player)
            controller = ((Player)entity).controller;

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

    bool CanRunActivity_Shoot() {
        if (dead)
            return false;

        return true;
    }

    bool CanRunActivity_Aim() {
        if (dead)
            return false;

        return true;
    }

    bool CanRunTask_RotateTowardsMouse() {
        if (dead)
            return false;

        return true;
    }

    bool CanRunActivity_SwapToWeaponIndex() {
        if (dead)
            return false;

        return true;
    }

    //----------My Tasks---------

    protected override void AddTaskRestrictions() {

    }

    protected override void AddTasks() {
        if (controller) {
            controller.Add_CanRunTask_Function(controller.Task_RotateTowardsMouse, CanRunTask_RotateTowardsMouse);
        }
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
        if (dodge)
            dodge.Add_CanRunActivity_Function(dodge.Activity_Dodge, CanRunActivity_Dodge);
        if (shoot) {
            shoot.Add_CanRunActivity_Function(shoot.Activity_Shoot, CanRunActivity_Shoot);
            shoot.Add_CanRunActivity_Function(shoot.Activity_Aim, CanRunActivity_Aim);
        }
        if (inventory)
            inventory.Add_CanRunActivity_Function(inventory.Activity_SwitchToWeaponIndex, CanRunActivity_SwapToWeaponIndex);
                            
    }
}
