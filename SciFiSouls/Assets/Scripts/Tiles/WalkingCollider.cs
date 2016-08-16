using UnityEngine;
using System.Collections;

public class WalkingCollider : MonoBehaviour {

    Tile MyTile;

    void Awake() {
        MyTile = transform.parent.GetComponent<Tile>();
    }

    void Start() {
        transform.localPosition = Vector3.zero;
        enabled = MyTile.WalkingCollider;
    }
}
