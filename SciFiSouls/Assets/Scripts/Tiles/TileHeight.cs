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
        switch ((int)MyTile.MyColliderData.data.CurrentColliderType) {
            case 0: return;
            case 1: gameObject.AddComponent<BoxCollider>().GetCopyOf<BoxCollider>(transform.parent.GetComponent<BoxCollider>()); break;
            case 2: gameObject.AddComponent<CapsuleCollider>().GetCopyOf<CapsuleCollider>(transform.parent.GetComponent<CapsuleCollider>()); break;
            case 3: gameObject.AddComponent<MeshCollider>().GetCopyOf<MeshCollider>(transform.parent.GetComponent<MeshCollider>()); break;
            default: return;
        }
        Collider c = GetComponent<Collider>();
        if (c == null)
            Debug.LogError("Failed to copy tile parent collider");
        if (c is MeshCollider) {
            MeshCollider mc = (MeshCollider)c;
            mc.convex = true;
            mc.isTrigger = true;
        } else 
            c.isTrigger = true;
        gameObject.layer = LayerMask.NameToLayer("HeightBoxes");

        gameObject.AddComponent<Rigidbody>();
    }

    public bool PassedHeightCheck() {
        float f = Random.Range(0f, 1f);
        if (f > MyTile.HitHeight) {
            return true;
        }
        return false;
    }

}
