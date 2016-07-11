using UnityEngine;
using System.Collections;

public class CameraManager : MonoBehaviour {

    public static CameraManager instance;

    void Awake() {
        if (instance != null) { 
            Debug.LogError("Attempted to create more than one CameraManager instance");
            Destroy(instance);
        }
        instance = this;
    }

    void Update () {
		TrackPlayer();
	}

	void TrackPlayer() {
		Vector3 pos = PlayerManager.instance.Player.transform.position;
		pos.z = -4;
		Camera.main.transform.position = pos;
	}
}
