using UnityEngine;
using System.Collections;
using System;

public class Gun1 : AttackBase {

    public override void Aim(AttackData myWeapon) {

    }

    public override void Shoot(AttackData myWeapon) {
        GameObject g = Resources.Load("Prefabs/Weapons/Projectile", typeof(GameObject)) as GameObject;
        g = Instantiate(g);
        Projectile p = g.GetComponent<Projectile>();
        p.Init(myWeapon.myWeapon.ProjectileSprite, myWeapon.entityPosition, myWeapon.targetDegreeRotation, myWeapon.myWeapon.ProjectileRange, myWeapon.myWeapon.ProjectileSpeed, myWeapon.entityVelocity);
    }
}

