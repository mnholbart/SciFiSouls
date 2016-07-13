using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class WeaponData : ScriptableObject {

    public Sprite WeaponSprite;
    public Vector3 WeaponRenderOffsetFromPlayer = new Vector3();
    public Sprite ProjectileSprite;
    public float ProjectileRange;

    public int WeaponDamage;
    public float ProjectileSpeed;
    public float AttackCooldown;
    public float AttackCooldownRemaining;

    public enum WeaponType {
        NULL = 1,
        Ranged = 2,
        Melee = 4,
    }
    public WeaponType MyWeaponType = WeaponType.NULL;

    public AttackBase AttackScript;
}

