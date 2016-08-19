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
        CapsuleCollider box = g.GetComponent<CapsuleCollider>();
        box.radius = .02f;
        box.height = .1f;
        box.center = new Vector3(0, .02f, 0);
        SetLayers(g, data);
        g.SetActive(true);
    }

    public override void SetLayers(GameObject g, AttackData d) {
        SpriteRenderer sr = g.GetComponent<SpriteRenderer>();
        sr.sortingLayerID = d.LayerIndex;
        sr.sortingOrder = d.SubLayerIndex;
    }

    public void OnPassHeightCheck(GameObject p) {

    }

    public void OnFailHeightCheck(GameObject p) {
        Destroy(p);
    }
}

