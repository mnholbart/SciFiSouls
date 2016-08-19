using UnityEngine;
using System.Collections.Generic;

public class WallDepth : MonoBehaviour {

    GameObject box;
    
    public List<float> FloorZHeights = new List<float>();
    public static Material wallMaterial;
    BoxCollider bc;

    void Start() {
        int boxHeight = 10;

        box = GameObject.CreatePrimitive(PrimitiveType.Cube);
        box.transform.localScale = new Vector3(1, 1, boxHeight);
        box.transform.SetParent(transform, true);
        box.transform.localPosition = new Vector3(0, 0, boxHeight/2);
        bc = box.AddComponent<BoxCollider>();        
        if (wallMaterial == null)
            wallMaterial = Resources.Load("Materials/Wall", typeof(Material)) as Material;
        box.GetComponent<MeshRenderer>().sharedMaterial = wallMaterial;
        box.layer = LayerMask.NameToLayer("Wall");
        box.name = "DepthBox";

        CameraManager.instance.RegisterWall(gameObject);
    }

    public void SetFloor(int i) {
        int index = GameManager.data.GetFloorIndex(i);
        index = Mathf.FloorToInt(index / 2);
        if (index < FloorZHeights.Count && index > 0)
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, -FloorZHeights[index]);
        int boxHeight = 10;
        box.transform.localScale = new Vector3(1, 1, boxHeight);
        box.transform.localPosition = new Vector3(0, 0, (boxHeight / 2) * Mathf.Sign(i));
    }
}
