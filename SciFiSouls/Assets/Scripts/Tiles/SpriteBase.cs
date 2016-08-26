using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Collections;

public abstract class SpriteBase : MonoBehaviour, ISprite {

    //TileData properties
    public bool cover;
    public bool destructable;
    public bool isWall;
    public bool WalkingCollider;
    public int MaxHealth;
    public int CurrHealth;
    public string BaseLayerName;
    public int SubLayer;
    public float zHeight = 0;

    //Tile properties
    public ColliderData MyColliderData;
    public int SpriteWidth;
    public int SpriteHeight;
    public float MaxHeight;
    public float CurrHeight;
    public float HitHeight {
        get { return CurrHeight; }
        set { CurrHeight = MaxHeight = value; }
    }
    public WalkingNoise WalkingNoiseType = WalkingNoise.None;
    public TripThreshold TripThresholdType = TripThreshold.None;
    public enum CollisionTypes {
        None = 0,
        Box = 1,
        Circle = 2,
        Mesh = 3
    }
    public static string[] CollisionTypeStrings = { "None", "Box", "Circle", "Mesh" };


    //Tile enums
    /// <summary>
    /// Noise type when walked on
    /// </summary>
    public enum WalkingNoise {
        None,
        Metallic,
        Organic,
        Stone,
        Tile
    }

    /// <summary>
    /// Tripping movement threshold
    /// </summary>
    public enum TripThreshold {
        None,
        Stationary,
        Walking,
        Running
    }

    public void DamageHeight(float height) {
        CurrHeight = height;
    }

    Vector3 startPos;

#if UNITY_EDITOR
    /// <summary>
    /// Whenever layer changes are done or tile modifications this will run
    /// making sure all changes are saved
    /// </summary>
    public void ForceUpdateTile(GameData data) {
        /*
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        int layerIndex = data.GetLayerIndex(this);
        BaseLayerName = data.GetLayerName(layerIndex);

        sr.sortingOrder = SubLayer;
        sr.sortingLayerName = layerIndex != -1 ? layerIndex.ToString() : "Default";
        EditorUtility.SetDirty(this); //Force update
        */
        throw new System.Exception("NYI");
    }

    public void SetLayerIndex(int i, GameData d) {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        sr.sortingLayerName = d.layers[i];
        BaseLayerName = d.GetLayerName(i);
    }

    public void UpdateSubLayerIndex(GameData d) {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        sr.sortingOrder = SubLayer;
    }

    public void SetSubLayerIndex(int i, GameData d) {
        SubLayer = i;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        sr.sortingOrder = SubLayer;
    }

    public void RemoveColliders(GameObject g) {
        Collider collider = g.GetComponent<Collider>();
        while (collider != null) {
            if (collider != null)
                DestroyImmediate(collider, true);
            collider = g.GetComponent<Collider>();
        }
    }

    public void AddCollider<T>(GameObject g) where T : Component {
        if (g.GetComponent<Collider>())
            RemoveColliders(g);
        g.AddComponent<T>();
    }

    public void SetCollider(ColliderData.Data data, GameObject g) {
        switch ((int)data.CurrentColliderType) {
            case 0: RemoveColliders(g); break;
            case 1: AddCollider<BoxCollider>(g); break;
            case 2: AddCollider<CapsuleCollider>(g); break;
            case 3: AddCollider<MeshCollider>(g); break;
        }
    }

    public void ChangeSprite(Sprite s) {
        if (s == null)
            return;
        GetComponent<SpriteRenderer>().sprite = s;
        SpriteWidth = MyColliderData.data.SpriteWidth = MyColliderData.moveData.SpriteWidth = MyColliderData.triggerData.SpriteWidth = s.texture.width;
        SpriteHeight = MyColliderData.data.SpriteHeight = MyColliderData.moveData.SpriteHeight = MyColliderData.triggerData.SpriteHeight = s.texture.height;
    }

    public void UpdateCollision() {
        ColliderData.Data data = MyColliderData.data;
        SetCollider(data, gameObject);
        switch (MyColliderData.data.CurrentColliderType) {
            case ColliderData.ColliderTypes.None: break;
            case ColliderData.ColliderTypes.Box: UpdateBoxCollider(gameObject, data, MyColliderData.data.ColliderDepth); break;
            case ColliderData.ColliderTypes.Circle: UpdateCylinderCollider(gameObject, data, MyColliderData.data.ColliderDepth); break;
            case ColliderData.ColliderTypes.Mesh: UpdateMeshCollider(gameObject, data, 0f, -MyColliderData.data.ColliderDepth); break;
        }
    }

    public void UpdateWalkCollision() {
        GameObject g = transform.FindChild("WalkingCollider").gameObject;
        ColliderData.Data data = MyColliderData.moveData;
        SetCollider(data, g);
        switch (MyColliderData.moveData.CurrentColliderType) {
            case ColliderData.ColliderTypes.None: break;
            case ColliderData.ColliderTypes.Box: UpdateBoxCollider(g, data, -.1f); break;
            case ColliderData.ColliderTypes.Circle: UpdateCylinderCollider(g, data, -.1f); break;
            case ColliderData.ColliderTypes.Mesh: UpdateMeshCollider(g, data, 0f, .1f); break;
        }
    }

    float GetTileScaleX() {
        return SpriteWidth / 128;
    }

    float GetTileScaleY() {
        return SpriteWidth / 128;
    }

    public void UpdateTriggerCollision() {
        GameObject g = transform.FindChild("TriggerCollider").gameObject;
        ColliderData.Data data = MyColliderData.triggerData;
        SetCollider(data, g);
        switch (MyColliderData.triggerData.CurrentColliderType) {
            case ColliderData.ColliderTypes.None: break;
            case ColliderData.ColliderTypes.Box: UpdateBoxCollider(g, data, MyColliderData.triggerData.ColliderDepth); break;
            case ColliderData.ColliderTypes.Circle: UpdateCylinderCollider(g, data, MyColliderData.triggerData.ColliderDepth); break;
            case ColliderData.ColliderTypes.Mesh: UpdateMeshCollider(g, data, 0f, -MyColliderData.triggerData.ColliderDepth); break;
        }
        if (g.GetComponent<Collider>())
            g.GetComponent<Collider>().isTrigger = true;
    }

    void UpdateMeshCollider(GameObject g, ColliderData.Data data, float zStart, float zEnd) {
        MeshCollider mc = g.GetComponent<MeshCollider>();
        if (data.MeshColliderMesh == null)
            data.MeshColliderMesh = new Mesh();
        data.MeshColliderMesh.Clear();

        List<Vector2> verts = new List<Vector2>();
        for (int i = 0; i < data.points.Length; i++) {
            verts.Add(new Vector2((data.points[i].x / SpriteWidth - .5f), (SpriteHeight - data.points[i].y) / SpriteHeight - .5f) * GetTileScaleY());
        }
        data.MeshColliderMesh = TileUtility.CreateMesh(verts.ToArray(), zStart, zEnd);
        mc.sharedMesh = data.MeshColliderMesh;
    }

    void UpdateBoxCollider(GameObject g, ColliderData.Data data, float depth) {
        BoxCollider bc = g.GetComponent<BoxCollider>();
        Vector3 size = new Vector3();
        size.x = (data.boxBoundsX.y - data.boxBoundsX.x) / SpriteWidth * GetTileScaleX();
        size.y = (data.boxBoundsY.y - data.boxBoundsY.x) / SpriteHeight * GetTileScaleY();
        size.z = depth;
        bc.size = size;
        float centerX = ((data.boxBoundsX.x + data.boxBoundsX.y) / 2) / SpriteWidth - .5f;
        float centerY = (1 - (data.boxBoundsY.x + data.boxBoundsY.y) / 2) / SpriteHeight - .5f + 1;
        float centerZ = transform.position.z - depth / 2f;
        bc.center = new Vector3(centerX, centerY, centerZ);
    }

    void UpdateCylinderCollider(GameObject g, ColliderData.Data data, float depth) {
        CapsuleCollider cc = g.GetComponent<CapsuleCollider>();
        float centerX = (data.cylinderCenter.x / SpriteWidth - .5f) * GetTileScaleX();
        float centerY = (1 - data.cylinderCenter.y / SpriteHeight - .5f) * GetTileScaleY();
        float centerZ = transform.position.z - depth / 2f;
        cc.center = new Vector3(centerX, centerY, centerZ);
        cc.radius = data.radius / SpriteWidth * GetTileScaleX();
        cc.height = depth;
        cc.direction = 2;
    }

    public void SetIsWall(bool b) {
        isWall = b;
        if (b) {
            if (!GetComponent<WallDepth>())
                gameObject.AddComponent<WallDepth>();
        } else {
            WallDepth wd = GetComponent<WallDepth>();
            DestroyImmediate(wd, true);
        }
    }
#endif
}
