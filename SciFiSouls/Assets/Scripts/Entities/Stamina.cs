using UnityEngine;
using System.Collections;
using System;

public class Stamina : ActivitySystem {

    [HideInInspector]
    public bool isDirty = false;

    private float _CurrentStamina;
    public float CurrentStamina { get { return _CurrentStamina; } private set { _CurrentStamina = value;  isDirty = true; } }
    public float MaximumStamina = 50;
    public float StaminaRegenPerSecond = 7.5f;

    public float StaminaRegenDelayFromOverExertion = 2f;
    public float PercentOfDelayBeforeRegen = .5f; //What percent of the regen delay is spent at 0 stamina before it starts regenerating (still cant use it until time is up)

    public enum RegenStatus {
        OverExerted, //No regen, cant use stamina
        Cooling, //Regen, cant use stamina
        Regenerating //Regen, can use stamina
    }
    public RegenStatus CurrentRegenStatus;

    Entity entity;
    Sprint sprint;
    Dodge dodge;

    new void Awake() {
        entity = GetComponent<Entity>();

        base.Awake();
    }

    new void Start() {
        sprint = entity.sprint;
        dodge = entity.dodge;

        CurrentRegenStatus = RegenStatus.Regenerating;
        MaxStamina();

        base.Start();
    }    

    public void DrainStamina(float amount) {
        CurrentStamina -= amount;
        CurrentStamina = Mathf.Max(CurrentStamina, 0);
    }

    public void MaxStamina() {
        CurrentStamina = MaximumStamina;
    }

    public void OverExertion() {
        StartCoroutine(OverExert());
    }

    IEnumerator OverExert() {
        float duration = StaminaRegenDelayFromOverExertion;
        CurrentRegenStatus = RegenStatus.OverExerted;
        while (duration > StaminaRegenDelayFromOverExertion*PercentOfDelayBeforeRegen) {
            yield return null;
            duration -= Time.deltaTime;
        }
        CurrentRegenStatus = RegenStatus.Cooling;
        while (duration > 0) {
            yield return null;
            duration -= Time.deltaTime;
        }
        CurrentRegenStatus = RegenStatus.Regenerating;
    }

    // ----------------------
    //         Tasks
    // ----------------------
    protected override void AddOtherTaskRestrictions() {
        if (sprint)
            sprint.Add_CanRunTask_Function(sprint.Task_Sprint, CanRunTask_Sprint);
    }

    bool CanRunActivity_Dodge() {

        if (CurrentRegenStatus == RegenStatus.OverExerted)
            return false;

        if (dodge && CurrentStamina < dodge.DodgeStaminaCost)
            return false;

        return true;
    }

    bool CanRunTask_Sprint() {
        if (CurrentRegenStatus == RegenStatus.OverExerted || CurrentRegenStatus == RegenStatus.Cooling)
            return false;

        if (CurrentStamina <= 0)
            return false;

        return true;
    }

    /// <summary>
    /// Tries to regenerate stamina
    /// </summary>
    public void Task_RegenerateStamina() {
        CurrentStamina += StaminaRegenPerSecond * Time.deltaTime;
        CurrentStamina = Mathf.Min(CurrentStamina, MaximumStamina);
    }

    public bool CanRunTask_RegenerateStamina() {
        if (CurrentRegenStatus == RegenStatus.OverExerted)
            return false;

        return true;
    }

    protected override void AddTaskRestrictions() {
        Add_CanRunTask_Function(Task_RegenerateStamina, CanRunTask_RegenerateStamina);
    }

    protected override void AddTasks() {
        AddTask(Task_RegenerateStamina);
    }

    protected override void AddActivityRestrictions() {
    }

    protected override void AddActivities() {
    }

    protected override void AddOtherActivityRestrictions() {
        if (dodge)
            dodge.Add_CanRunActivity_Function(dodge.Activity_Dodge, CanRunActivity_Dodge);
    }
}
