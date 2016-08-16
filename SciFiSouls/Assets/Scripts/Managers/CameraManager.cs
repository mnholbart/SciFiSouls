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

        Camera.main.transparencySortMode = TransparencySortMode.Orthographic;
    }

    void Update () {
		TrackPlayer();
	}

    void start() {

    }

	void TrackPlayer() {
		Vector3 pos = PlayerManager.instance.Player.transform.position;
		pos.z = -4 + PlayerManager.instance.Player.transform.position.z;
        Camera.main.transform.position = pos;
    }
}
