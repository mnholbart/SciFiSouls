using UnityEngine;
using System.Collections;

public class StaticAttack : MonoBehaviour {
    
    public float degreeRot;
    public Vector3 start;
    public float duration;

    public void Init(Vector3 spawn, float degreeRot, float duration) {
        transform.position = spawn;
        //Vector3 direction = (Vector2)(Quaternion.Euler(0, 0, degreeRot) * Vector2.right);
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, degreeRot - 90));
        this.duration = duration;
    }

    void Update() {
        duration -= Time.deltaTime;
        if (duration < 0)
            Destroy(gameObject);
    }

}
