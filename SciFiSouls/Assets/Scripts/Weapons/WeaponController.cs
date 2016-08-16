using UnityEngine;
using System.Collections;
using System;

public class WeaponController : ActivitySystem {

    public WeaponData EquippedWeapon;
    public AttackBase AttackScript;

    public float UnequipTime = .5f;
    public float EquipTime = .25f;

    public int SpriteRotation = -90;

    public bool SwappingWeapon = false;
    public WeaponData NewWeapon = null;

    public GameObject WeaponAttackScripts;

    SpriteRenderer sr;
    Shoot shoot { get { return entity.shoot; } }
    Movement movement { get { return entity.movement; } }
    PlayerController pc;

    public enum EquippedWeaponType {
        none,
        ranged,
        melee
    }
    public EquippedWeaponType CurrentEquippedWeaponType = EquippedWeaponType.none;

    new void Start() {
        entity = transform.parent.GetComponent<Entity>();
        sr = GetComponent<SpriteRenderer>();
        if (entity is Player)
            pc = ((Player)entity).controller;

        base.Start();
    }

    new void Update() {
        RotateWithEntity();
        UpdateWeapon();

        base.Update();
    }

    public void SetLayerName(string layerName) {
        sr.sortingLayerName = layerName;
    }

    void UpdateWeapon() {
        if (EquippedWeapon != null)
            EquippedWeapon.AttackCooldownRemaining -= Time.deltaTime;
    }

    public void Shoot() {
        AttackBase.AttackData data = new AttackBase.AttackData();
        data.entityPosition = transform.position + (Quaternion.Euler(transform.rotation.eulerAngles) * EquippedWeapon.Offset);
        data.myWeapon = EquippedWeapon;
        data.LayerIndex = sr.sortingLayerID;
        data.SubLayerIndex = sr.sortingOrder;

        if (movement)
            data.entityVelocity = movement.body.velocity;

        if (pc) {
            data.mousePosition = pc.GetMouseWorldPosition();
            data.targetDegreeRotation = pc.GetMouseAngleDegrees();
        }

        EquippedWeapon.AttackCooldownRemaining = EquippedWeapon.AttackCooldown;

        AttackScript.Shoot(data);
    }

    public void RotateWithEntity() {
        if (entity) {
            transform.rotation = transform.parent.rotation;
            if (EquippedWeapon)
                transform.localPosition = EquippedWeapon.WeaponRenderOffsetFromPlayer;
        }
    }

    void UnequipWeapon() {
        EquippedWeapon = null;
        AttackScript = null;
    }

    void EquipWeapon(WeaponData data) {
        sr.sprite = data.WeaponSprite;
        EquippedWeapon = data;
        EquippedWeapon.AttackCooldownRemaining = 0;
        UpdateWeaponType(data);
        UpdateAttackScript();
        RotateWithEntity();
    }

    IEnumerator SwapWeapon(WeaponData data) {
        SwappingWeapon = true;

        float timer;
        if (EquippedWeapon != null) {
            UnequipWeapon();

            timer = UnequipTime;
            while (timer > 0) {
                yield return null;
                timer -= Time.deltaTime;
            }
            sr.sprite = null;
        }

        timer = EquipTime;
        while (timer > 0) {
            yield return null;
            timer -= Time.deltaTime;
        }

        EquipWeapon(data);
        SwappingWeapon = false;
    }

    void UpdateAttackScript() {
        AttackScript = (AttackBase)WeaponAttackScripts.GetComponent(EquippedWeapon.Name);
    }

    void UpdateWeaponType(WeaponData data) {
        if (data.MyWeaponType == WeaponData.WeaponType.Melee)
            CurrentEquippedWeaponType = EquippedWeaponType.melee;
        else if (data.MyWeaponType == WeaponData.WeaponType.Ranged)
            CurrentEquippedWeaponType = EquippedWeaponType.ranged;
        else CurrentEquippedWeaponType = EquippedWeaponType.none;
    }

    public void TrySwapWeapon(WeaponData data) {
        NewWeapon = data;
        TryStartActivity(Activity_EquipNewWeapon);
    }

    void OnFailStart_Activity_EquipNewWeapon() {
        NewWeapon = null;
    }

    void Activity_EquipNewWeapon() {
        StartCoroutine(SwapWeapon(NewWeapon));
    }

    bool CanStartActivity_EquipNewWeapon() {
        if (SwappingWeapon)
            return false;

        if (NewWeapon == null)
            return false;

        return true;
    }

    bool CanRunActivity_Shoot() {
        if (SwappingWeapon)
            return false;

        if (EquippedWeapon == null)
            return false;

        if (AttackScript == null)
            return false;

        if (EquippedWeapon.AttackCooldownRemaining > 0)
            return false;

        return true;
    }

    protected override void AddTaskRestrictions() {
    }

    protected override void AddTasks() {
    }

    protected override void AddOtherTaskRestrictions() {
    }

    protected override void AddActivityRestrictions() {
        Add_CanRunActivity_Function(Activity_EquipNewWeapon, CanStartActivity_EquipNewWeapon);
    }

    protected override void AddActivities() {
        AddActivity(Activity_EquipNewWeapon);
        Add_OnFailActivity_Function(Activity_EquipNewWeapon, OnFailStart_Activity_EquipNewWeapon);
    }

    protected override void AddOtherActivityRestrictions() {
        shoot.Add_CanRunActivity_Function(shoot.Activity_Shoot, CanRunActivity_Shoot);
    }
}
