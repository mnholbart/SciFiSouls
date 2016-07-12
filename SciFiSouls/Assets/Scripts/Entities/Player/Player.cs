using UnityEngine;
using UnityEngine.Events;
using System.Collections;

[RequireComponent(typeof(Sprint))]
[RequireComponent(typeof(Stamina))]
[RequireComponent(typeof(Movement))]
[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(Die))]
[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(Shoot))]
[RequireComponent(typeof(Dodge))]
public class Player : Entity {

    [HideInInspector]
    public PlayerController controller;
    [HideInInspector]
    public PlayerInput input;
    

    new void Awake() {
        base.Awake();
    }

    new void Start() {
        base.Start();
        controller = GetComponent<PlayerController>();
        input = GetComponent<PlayerInput>();
    }

    void Update() {

    }
}
