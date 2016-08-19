using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraManager : MonoBehaviour {

    public static CameraManager instance;
    public List<WallDepth> walls = new List<WallDepth>();

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

    public void RegisterWall(GameObject wall) {
        WallDepth d = wall.GetComponent<WallDepth>();
        if (!walls.Contains(d))
            walls.Add(d);
    }

    public void ChangeZHeight(int floor, float zHeight) {
        for (int i = 0; i < walls.Count; i++) {
            walls[i].SetFloor(floor);
        }
    }
}
