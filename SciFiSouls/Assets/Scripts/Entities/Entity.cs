using UnityEngine;
using System.Collections;

public abstract class Entity : MonoBehaviour {

    public bool dead = false;
    public int MaxHealth = 0;
    public int CurrentHealth = 0;

}
