using UnityEngine;
using System.Collections;

public class PlayerManager : MonoBehaviour {

	public static PlayerManager instance;

	//Editor stuff
	public GameObject PlayerPrefab;

	//Runtime stuff
	public Player Player;

	void Awake() {
		if (instance != null) {
			Debug.LogError("Attempted to create more than one PlayerManager instance");
			Destroy(instance);
		}
		instance = this;
	}

	public void InitializePlayer() {
		GameObject obj = Instantiate(PlayerPrefab) as GameObject;
		Player = obj.GetComponent<Player>();
        Player.Spawn(new Vector3(0, 0, 0), 1, -.01f);
	}
}
