using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;

[ExecuteInEditMode]
public class Tile : MonoBehaviour {

	#if UNITY_EDITOR
    /// <summary>
    /// Whenever layer changes are done or tile modifications this will run
    /// making sure all changes are saved
    /// </summary>
    public void ForceUpdateTile(GameData data) {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        int layerIndex = data.GetLayerIndex(this);
        BaseLayerName = data.GetLayerName(layerIndex);

        sr.sortingOrder = SubLayer;
        sr.sortingLayerName = layerIndex != -1 ? layerIndex.ToString() : "Default";
        EditorUtility.SetDirty(this); //Force update
    }

    public void RemoveColliders() {
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
            DestroyImmediate(collider, true);
        CollisionType = CollisionTypes.None;
    }

    public void AddCollider<T>() where T : Component {
        if (GetComponent<Collider2D>())
            RemoveColliders();
        gameObject.AddComponent<T>();
    }

    public void SetCollider(int type) {
        switch (type) {
            case 0: RemoveColliders(); break;
            case 1: AddCollider<BoxCollider2D>(); break;
            case 2: AddCollider<CircleCollider2D>(); break;
            case 3: AddCollider<EdgeCollider2D>(); break;
            case 4: AddCollider<PolygonCollider2D>(); break;
        }
        CollisionType = (CollisionTypes)type;
    }
	#endif

    public void ChangeSprite(Sprite s) {
        if (s == null)
            return;
        GetComponent<SpriteRenderer>().sprite = s;
    }

	void OnDestroy() {
        
	}

	//TileData properties
	public bool destructable;
	public int MaxHealth;
    public int CurrHealth;
	public string BaseLayerName;
	public int SubLayer;

    //Tile properties
    float MaxHeight;
    float CurrHeight;
    public float HitHeight {
        get { return CurrHeight; }
        set { CurrHeight = MaxHeight = value; }
    }
	public WalkingNoise WalkingNoiseType = WalkingNoise.None;
	public TripThreshold TripThresholdType = TripThreshold.None;
    public CollisionTypes CollisionType = CollisionTypes.None;

    public enum CollisionTypes {
        None = 0,
        Box = 1,
        Circle = 2,
        Edge = 3,
        Polygon = 4
    }
    public static string[] CollisionTypeStrings = { "None", "Box", "Circle", "Edge", "Polygon" };

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

}
