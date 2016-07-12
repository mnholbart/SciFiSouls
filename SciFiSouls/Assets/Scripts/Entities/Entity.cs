using UnityEngine;
using System.Collections;

public class Entity : MonoBehaviour {

    [HideInInspector]
    public Sprint sprint;
    [HideInInspector]
    public Stamina stamina;
    [HideInInspector]
    public Movement movement;
    [HideInInspector]
    public Health health;
    [HideInInspector]
    public Die die;
    [HideInInspector]
    public Shoot shoot;
    [HideInInspector]
    public Dodge dodge;
    [HideInInspector]
    public Sneak sneak;

    public void Awake() {
        sprint = GetComponent<Sprint>();
        stamina = GetComponent<Stamina>();
        movement = GetComponent<Movement>();
        health = GetComponent<Health>();
        die = GetComponent<Die>();
        shoot = GetComponent<Shoot>();
        dodge = GetComponent<Dodge>();
        sneak = GetComponent<Sneak>();
    }

    public void Start() {

    }
}
