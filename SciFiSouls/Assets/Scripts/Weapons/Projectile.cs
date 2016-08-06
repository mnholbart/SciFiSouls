using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {

    AttackBase source;
    Rigidbody2D body;

    Vector3 startVector;
    public Vector3 moveVector;

    public float speed;
    public float maxRange;
    

    void Start() {
        body = GetComponent<Rigidbody2D>();
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

    void OnTriggerEnter2D(Collider2D collide) {
        if (!collide.GetComponent<IHeightCollider>().PassedHeightCheck()) {
            if (source.GetComponent<IHeightCallbacks>() != null)
                source.GetComponent<IHeightCallbacks>().OnPassHeightCheck(this.gameObject);
            else Destroy(this);
        } else {
            source.GetComponent<IHeightCallbacks>().OnFailHeightCheck(this.gameObject);
        }
    }
}
