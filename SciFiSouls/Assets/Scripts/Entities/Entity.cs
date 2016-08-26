using UnityEngine;
using System.Collections;

public class Entity : MonoBehaviour {

    [HideInInspector] public Sprint sprint;
    [HideInInspector] public Stamina stamina;
    [HideInInspector] public Movement movement;
    [HideInInspector] public Health health;
    [HideInInspector] public Die die;
    [HideInInspector] public Shoot shoot;
    [HideInInspector] public Dodge dodge;
    [HideInInspector] public Sneak sneak;
    [HideInInspector] public Inventory inventory;
    [HideInInspector] public ZMovement zMovement;
    [HideInInspector] public SpriteRenderer SpriteRenderer;
    [HideInInspector] public Cover cover;

    public void Awake() {
        sprint = GetComponent<Sprint>();
        stamina = GetComponent<Stamina>();
        movement = GetComponent<Movement>();
        health = GetComponent<Health>();
        die = GetComponent<Die>();
        shoot = GetComponent<Shoot>();
        dodge = GetComponent<Dodge>();
        sneak = GetComponent<Sneak>();
        inventory = GetComponent<Inventory>();
        zMovement = GetComponent<ZMovement>();
        cover = GetComponent<Cover>();
        SpriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Start() {

    }

    public void Spawn(Vector3 position, int floor, float floorHeight) {
        StartCoroutine(TrySpawn(position, floor, floorHeight));
    }

    IEnumerator TrySpawn(Vector3 position, int floor, float floorHeight) {
        yield return new WaitForEndOfFrame();
        movement.SetPosition(position);
        zMovement.ForceSetFloor(floor, floorHeight);
    }
}
