using UnityEngine;
using System.Collections;

/// <summary>
/// Handles saving and loading data using MapStateData
/// </summary>
public class SaveLoadManager : MonoBehaviour {

	//This is potentially redundant or uneccesary and I'll need to look into alternatives or better methods.
	//Preventing any potential corruption or loss of data is important
	//The idea for save and load right now will use a few temporary save files
	//Such as the last 3 saves will be kept at any time
	//The structure could be as follows PER SLAVE SLOT Saves/Slot1/Save*.save
	//Save1.save is the currently used save file that is periodically updated (on death, objective update, setting changes, etc)
	//Save2.save is the currently written and loaded file on the disk, if you did LoadData(1); you would load Save2.save
	//Save3.save is a backup in case Save2.save is somehow corrupted so we at worst lose data since the last soft update
	//Any time a save is performed we can write to the Save1, change Save2 to Save3, change Save1 to Save2 and create a new Save1 on next save, save3 would be deleted

	void Start () {
	
	}
		
	void Update () {
	
	}

	/// <summary>
	/// Saves the data to save index i to allow multiple save slots.
	/// Hard save will do a full write to disk
	/// </summary>
	void FullSaveData(int i) {

	}

	/// <summary>
	/// Loads the data from save index i to allow multiple save slots.
	/// </summary>
	void LoadData(int i) {

	}
}
