using UnityEngine;
using System.Collections;
using System;

public class Gun1 : AttackBase, IHeightCallbacks {

    public override void Aim(AttackData myWeapon) {

    }

    public override void Shoot(AttackData data) {
        GameObject g = Resources.Load("Prefabs/Weapons/Projectile", typeof(GameObject)) as GameObject;
        g = Instantiate(g);
        g.SetActive(false);
        Projectile p = g.GetComponent<Projectile>();
        p.SetSource(this);
        p.SetSprite(data.myWeapon.Sprites[0]);
        p.SetStartPosition(data.entityPosition);
        p.moveVector = WeaponUtils.GetBodyVelocity(data.entityPosition, data.targetDegreeRotation);
        p.speed = data.myWeapon.Speed;
        p.maxRange = data.myWeapon.Range;
        p.SetRotation(data.targetDegreeRotation + data.myWeapon.SpriteRotations[0]);
        g.transform.localScale = new Vector3(2, 2, 1);
        BoxCollider2D box = g.GetComponent<BoxCollider2D>();
        box.size = new Vector3(.02f, .04f);
        box.offset = new Vector3(.01f, .02f);
        g.SetActive(true);
    }

    public void OnPassHeightCheck(GameObject p) {

    }

    public void OnFailHeightCheck(GameObject p) {
        Destroy(p);
    }
}

