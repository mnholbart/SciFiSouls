using UnityEngine;
using System.Collections;

public class StaticAttack : MonoBehaviour {
    
    public float degreeRot;
    public Vector3 startVector;
    public float duration;
    public float startDuration;

    public void SetSprite(Sprite s) {
        GetComponent<SpriteRenderer>().sprite = s;
    }

    public void SetRotation(float rot) {
        Quaternion rotation = Quaternion.Euler(0, 0, rot);
        transform.rotation = rotation;
    }

    void Update() {
        duration -= Time.deltaTime;
        if (duration < 0)
            Destroy(gameObject);
    }

    public void SetStartPosition(Vector3 start) {
        startVector = start;
        transform.position = start;
    }

    public void SetScale(Vector3 v) {
        transform.localScale = v;
    }
}
