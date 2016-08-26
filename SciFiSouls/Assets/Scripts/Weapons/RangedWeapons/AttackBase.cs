using UnityEngine;
using System.Collections.Generic;

public abstract class AttackBase : MonoBehaviour {

    public class AttackData {
        public float targetDegreeRotation;
        public Vector3 entityPosition;
        public Vector3 mousePosition;
        public Vector3 entityVelocity;
        public WeaponData myWeapon;

        public int LayerIndex;
        public int SubLayerIndex;

        public List<GameObject> CoverObjects;
    }

    public abstract void Shoot(AttackData data);
    public abstract void Aim(AttackData data);

    public abstract void SetLayers(GameObject g, AttackData d);
}
