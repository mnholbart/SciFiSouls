using UnityEngine;
using System;
using System.Collections.ObjectModel;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class GameData : ScriptableObject {

	public List<string> layers = new List<string>();

	/// <summary>
	/// Gets the index of the layer.
	/// </summary>
	public int GetLayerIndex(Tile tile) {
		if (layers.Count == 0) {
            return -1;
        } else if (layers.Contains(tile.BaseLayerName)) {
			return layers.IndexOf(tile.BaseLayerName);
		} else {
			if (tile.BaseLayerName != "") //No warning if its just empty/just initialized
				Debug.LogWarningFormat("No layer found with name \"{0}\" on Tile: {1} setting layer to 0 named {2}", tile.BaseLayerName, tile.name, layers[0]);
			return 0;
		}
	}

    public int GetLayerIndex(string layerName) {
        if (layers.Count == 0)
            return -1;
        else if (layers.Contains(layerName)) {
            return layers.IndexOf(layerName);
        } else {
            if (layerName != "") //No warning if its just empty/just initialized
                Debug.LogWarningFormat("No layer found with name \"{0}\": setting layer to 0 named {2}", layerName, layers[0]);
            return 0;
        }
    }

    public void SetToFloorLayer(SpriteRenderer sr, int floorNumber) {
        int baseID = layers.IndexOf("Floor0");
        baseID += floorNumber * 2;
        sr.sortingLayerName = layers[baseID];
    }

    public void SetToFloorFloatLayer(SpriteRenderer sr, int floorNumber) {
        int baseID = layers.IndexOf("Floor0");
        baseID += floorNumber * 2;
        baseID++;
        sr.sortingLayerName = layers[baseID];
    }

    public int GetFloorIndex(int i) {
        int baseIndex = layers.IndexOf("Floor0");
        baseIndex += i;
        return baseIndex;
    }

    public int GetFloorCount() {
        return layers.Count / 2;
    }

	public string GetLayerName(int layerIndex) {
        if (layers.Count == 0) {
            Debug.LogWarning("No layer exists, add one with the layer editor");
            return "";
        } else if (layers.Count > layerIndex && layerIndex >= 0) {
			return layers[layerIndex];
		} else {
			Debug.LogWarningFormat("No layer exists at index {0}, add one with the layer editor", layerIndex);
			return "";
		}
    }
}
