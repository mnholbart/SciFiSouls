using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {

    public Vector3 direction;
    public float degreeRot;
    Rigidbody2D body;
    public Vector3 start;
    public float maxRange;
    public float speed;

    public void Init(Sprite sprite, Vector3 startPosition, Vector3 endPosition) {
        transform.position = startPosition;
        start = startPosition;
        direction = endPosition - startPosition;
        direction.Normalize();
        GetComponent<SpriteRenderer>().sprite = sprite;
    }

    public void Init(Sprite sprite, Vector3 startPosition, float directionDegrees, float range, float projectileSpeed, Vector3 entitySpeed) {
        transform.position = startPosition;
        start = startPosition;
        degreeRot = directionDegrees;
        maxRange = range;
        direction = (Vector2)(Quaternion.Euler(0, 0, degreeRot) * Vector2.right);
        direction.Normalize();
        GetComponent<SpriteRenderer>().sprite = sprite;
        direction *= projectileSpeed;
        direction += entitySpeed;
        //transform.rotation.eulerAngles = new Vector3(0, 0, 0);
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, directionDegrees - 90));
    }

    void Start() {
        body = GetComponent<Rigidbody2D>();
    }

    void Update() {
        body.velocity = direction;
        if (Vector3.Distance(transform.position, start) > maxRange)
            Destroy(gameObject);
    }
}
