﻿using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;

[ExecuteInEditMode]
public class Tile : MonoBehaviour {

	#if UNITY_EDITOR
	public AddTileAction AddedBy;

    public void ForceUpdateTile(GameData data) {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        int layerIndex = data.GetLayerIndex(this);
        BaseLayerName = data.GetLayerName(layerIndex);

        sr.sortingOrder = SubLayer;
        sr.sortingLayerName = layerIndex != -1 ? layerIndex.ToString() : "Default";
        EditorUtility.SetDirty(this); //Force update
    }
	#endif

	void OnDestroy() {
		#if UNITY_EDITOR
		if (AddedBy != null) {
			AddedBy.ForceRemove();
			AddedBy = null;
		}
		#endif
	}

	//TileData properties
	public bool destructable;
	public int MaxHealth;
	public string BaseLayerName;
	public int SubLayer;

	//Tile properties
	protected int CurrentHealth = 0;
	public WalkingNoise WalkingNoiseType = WalkingNoise.None;
	public TripThreshold TripThresholdType = TripThreshold.None;

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

}