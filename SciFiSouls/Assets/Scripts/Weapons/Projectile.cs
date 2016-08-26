using UnityEngine;
using System.Collections.Generic;

public class Projectile : MonoBehaviour {

    AttackBase source;
    Rigidbody body;

    Vector3 startVector;
    public Vector3 moveVector;

    public float speed;
    public float maxRange;

    List<GameObject> coverObjects = new List<GameObject>();

    void Start() {
        body = GetComponent<Rigidbody>();
        gameObject.layer = LayerMask.NameToLayer("Projectiles");
        ProjectileManager.instance.RegisterProjectile(this.transform);
    }

    void Update() {
        body.velocity = moveVector * speed;
        if (Vector3.Distance(transform.position, startVector) > maxRange)
            Destroy(gameObject);
    }

    public void SetSprite(Sprite s) {
        GetComponent<SpriteRenderer>().sprite = s;
    }

    public void SetStartPosition(Vector3 start) {
        startVector = start;
        transform.position = start;
    }

    public void SetSource(AttackBase source) {
        this.source = source;
    }

    public void SetRotation(float rot) {
        Quaternion rotation = Quaternion.Euler(0, 0, rot);
        transform.rotation = rotation;
    }

    public void AddCoverObjects(List<GameObject> objs) {
        for (int i = 0; i < objs.Count; i++) {
            coverObjects.Add(objs[i]);
        }
    }

    void OnTriggerEnter(Collider collide) {
        if (coverObjects.Contains(collide.GetComponent<IHeightCollider>().GetParentObject()))
            return;

        if (collide.GetComponentInParent<WallDepth>()) {
            WallCollision();
            return;
        }

        bool passed = collide.GetComponent<IHeightCollider>().PassedHeightCheck();
        if (passed) {
            if (source.GetComponent<IHeightCallbacks>() != null)
                source.GetComponent<IHeightCallbacks>().OnPassHeightCheck(this.gameObject);
            else Destroy(this);
        } else {
            source.GetComponent<IHeightCallbacks>().OnFailHeightCheck(this.gameObject);
        }
    }

    void WallCollision() {
        Destroy(gameObject);
    }
}
