using UnityEngine;
using System.Collections;

public class CoverCollision : MonoBehaviour {

    public delegate void TriggerAction(Collider other);
    public event TriggerAction TriggerEnterEvent;
    public event TriggerAction TriggerExitEvent;

    void OnTriggerEnter(Collider other) {
        if (TriggerEnterEvent != null)
            TriggerEnterEvent(other);
    }

    void OnTriggerExit(Collider other) {
        if (TriggerExitEvent != null)
            TriggerExitEvent(other);
    }

    public void UpdateSphereCollider(float r) {
        GetComponent<SphereCollider>().radius = r;
    }
}
