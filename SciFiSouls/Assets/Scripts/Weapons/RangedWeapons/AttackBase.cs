using UnityEngine;
using System.Collections;

public abstract class AttackBase : MonoBehaviour {

    public class AttackData {
        public AttackData(WeaponData MyWeapon, Vector3 EntityPos, Vector3 MousePos, Vector3 EntityVel, float TargetDegreeRotation) {
            myWeapon = MyWeapon;
            entityPosition = EntityPos;
            mousePosition = MousePos;
            entityVelocity = EntityVel;
            targetDegreeRotation = TargetDegreeRotation;
        }

        public float targetDegreeRotation;

        public Vector3 entityPosition;
        public Vector3 mousePosition;
        public Vector3 entityVelocity;

        public WeaponData myWeapon;
    }

    public abstract void Shoot(AttackData data);
    public abstract void Aim(AttackData data);

}
