using UnityEngine;
using System.Collections;

public class Gun2 : AttackBase {

    public override void Aim(AttackData myWeapon) {

    }

    public override void Shoot(AttackData myWeapon) {
        GameObject g = Resources.Load("Prefabs/Weapons/StaticAttack", typeof(GameObject)) as GameObject;
        g = Instantiate(g);
        StaticAttack p = g.GetComponent<StaticAttack>();
        //p.Init(myWeapon.myWeapon.ProjectileSprite, myWeapon.entityPosition, myWeapon.targetDegreeRotation, myWeapon.myWeapon.ProjectileRange, myWeapon.myWeapon.ProjectileSpeed, myWeapon.entityVelocity);
        p.GetComponent<SpriteRenderer>().sprite = myWeapon.myWeapon.ProjectileSprite;
        p.Init(myWeapon.entityPosition, myWeapon.targetDegreeRotation, myWeapon.myWeapon.ProjectileRange);
        p.transform.localScale = new Vector3(5, 5, 0);
    }

}
