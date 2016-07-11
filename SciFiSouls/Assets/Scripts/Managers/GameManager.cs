using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	public static GameManager instance;

	void Awake() {
		if (instance != null) {
			Debug.LogError("Attempted to create more than one GameManager instance");
			Destroy(instance);
		}
		instance = this;
		DontDestroyOnLoad(transform.root.gameObject);
	}

	void Start () {
		PlayerManager.instance.InitializePlayer();
	}


	void Update () {
	
	}
}
