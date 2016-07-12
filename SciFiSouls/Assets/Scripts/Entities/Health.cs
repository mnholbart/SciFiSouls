using UnityEngine;
using System.Collections;

public class Health : ActivitySystem {

    [HideInInspector]
    public bool isDirty = false;

    public int MaxHealth = 100;
    private int _CurrentHealth;
    public int CurrentHealth { get { return _CurrentHealth; } private set { _CurrentHealth = value; isDirty = true; } }

    Entity entity;
    Die die;

    new void Start() {
        entity = GetComponent<Entity>();
        die = entity.die;

        FullHeal();

        base.Start();
    }

    public void Damage(int amount) {
        CurrentHealth -= amount;

        if (CurrentHealth <= 0 && die)
            TryStartActivity(Activity_Kill);
    }

    public void Heal(int amount) {
        CurrentHealth += amount;
    }

    public void FullHeal() {
        CurrentHealth = MaxHealth;
    }


    // --------------------------
    //           Tasks
    // --------------------------

    //--Other Task Restrictions--
    protected override void AddOtherTaskRestrictions() {

    }

    //----------My Tasks---------

    protected override void AddTaskRestrictions() {

    }

    protected override void AddTasks() {

    }

    // ---------------------------
    //          Activities
    // ---------------------------

    //-Other Activity Restrictions

    protected override void AddOtherActivityRestrictions() {

    }

    //-------My Activities--------

    void Activity_Kill() {
        die.TryStartActivity(die.Activity_Kill);
    }

    protected override void AddActivityRestrictions() {

    }

    protected override void AddActivities() {
        AddActivity(Activity_Kill);
    }
}
