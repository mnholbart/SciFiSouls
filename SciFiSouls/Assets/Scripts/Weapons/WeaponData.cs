using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class WeaponData : ScriptableObject {

    public string Name;

    public Sprite WeaponSprite;
    public Vector3 WeaponRenderOffsetFromPlayer = new Vector3();
    public Vector3 Offset = new Vector3();
    public List<Sprite> Sprites = new List<Sprite>();
    public List<float> SpriteRotations = new List<float>();
    public List<Vector3> SpriteScales = new List<Vector3>();

    public float Range;
    public float Duration;
    public float Speed;
    public int WeaponDamage;
    public float AttackCooldown;
    public float AttackCooldownRemaining;

    public enum WeaponType {
        NULL = 1,
        Ranged = 2,
        Melee = 4,
    }
    public WeaponType MyWeaponType = WeaponType.NULL;
}

