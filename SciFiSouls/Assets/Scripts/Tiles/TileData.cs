using UnityEngine;
using System.Collections;

[System.Serializable]
public class TileData {

	public string name;

	public Sprite TileSprite;

	//Tile properties
	public bool destructable = false;
	public int MaxHealth = 0;

	//Layer stuff
	public string BaseLayerName = "";
	public int SubLayer = 0;
}
