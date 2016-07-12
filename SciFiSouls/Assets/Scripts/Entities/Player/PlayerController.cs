using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

    public int SpriteRotation = -90; //0 is if the sprite was facing left

    Player player;
    Die die;

    void Start() {
        player = GetComponent<Player>();
        die = player.die;
    }

    void Update () {
        if (CanRotate())
            RotateTowardsMouse();
	}
    void RotateTowardsMouse() {
        Vector3 mouse = Input.mousePosition;
        Vector3 screenPoint = Camera.main.WorldToScreenPoint(transform.localPosition);
        Vector3 offset = new Vector2(mouse.x - screenPoint.x, mouse.y - screenPoint.y);
        float angle = Mathf.Atan2(offset.y, offset.x) * Mathf.Rad2Deg;
        transform.eulerAngles = new Vector3(0, 0, angle + SpriteRotation);
    }

    bool CanRotate() {
        if (die && die.dead)
            return false;

        return true;
    }
}
