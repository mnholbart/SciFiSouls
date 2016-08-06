using UnityEngine;
using System.Collections;

public class Gun2 : AttackBase {

    public override void Aim(AttackData myWeapon) {

    }

    public override void Shoot(AttackData data) {
        GameObject g = Resources.Load("Prefabs/Weapons/StaticAttack", typeof(GameObject)) as GameObject;
        int i = Random.Range(0, data.myWeapon.Sprites.Count);
        g = Instantiate(g);
        StaticAttack p = g.GetComponent<StaticAttack>();
        g.SetActive(false);
        p.SetScale(data.myWeapon.SpriteScales[i]);
        p.SetSprite(data.myWeapon.Sprites[i]);
        p.SetStartPosition(data.entityPosition);
        p.duration = p.startDuration = data.myWeapon.Duration;
        p.SetRotation(data.targetDegreeRotation + data.myWeapon.SpriteRotations[i]);
        g.SetActive(true);

        StartCoroutine(FadeAttack(p));
    }

    IEnumerator FadeAttack(StaticAttack g) {
        SpriteRenderer r = g.GetComponent<SpriteRenderer>();

        while (g.duration > 0) {
            Color c = r.color;
            c.a = g.duration / g.startDuration;
            r.color = c;
            yield return null;
        }
    }

}
