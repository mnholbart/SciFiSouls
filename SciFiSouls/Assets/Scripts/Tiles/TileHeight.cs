using UnityEngine;

public class TileHeight : MonoBehaviour, IHeightCollider {

    Tile MyTile;

    void Awake() {
        MyTile = transform.parent.GetComponent<Tile>();
    }

	void Start() {
        transform.localPosition = Vector3.zero;

        CopyCollider();
    }

    void CopyCollider() {
        switch ((int)MyTile.CollisionType) {
            case 0: return;
            case 1: gameObject.AddComponent<BoxCollider2D>().GetCopyOf<BoxCollider2D>(transform.parent.GetComponent<BoxCollider2D>()); break;
            case 2: gameObject.AddComponent<CircleCollider2D>().GetCopyOf<CircleCollider2D>(transform.parent.GetComponent<CircleCollider2D>()); break;
            case 3: gameObject.AddComponent<EdgeCollider2D>().GetCopyOf<EdgeCollider2D>(transform.parent.GetComponent<EdgeCollider2D>()); break;
            case 4: gameObject.AddComponent<PolygonCollider2D>().GetCopyOf<PolygonCollider2D>(transform.parent.GetComponent<PolygonCollider2D>()); break;
            default: return;
        }
        Collider2D c = GetComponent<Collider2D>();
        if (c == null)
            Debug.LogError("Failed to copy tile parent collider");
        c.isTrigger = true;
        gameObject.layer = LayerMask.NameToLayer("HeightBoxes");

        gameObject.AddComponent<Rigidbody2D>();
    }

    public bool PassedHeightCheck() {
        float f = Random.Range(0f, 1f);
        if (f > MyTile.HitHeight) {
            return true;
        }
        return false;
    }

}
