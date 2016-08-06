using UnityEngine;
using System.Collections;

public static class WeaponUtils {

    public static Vector3 GetBodyVelocity(Vector3 startPos, float degreeRot) {
        Vector3 v = new Vector3();

        v = (Vector2)(Quaternion.Euler(0, 0, degreeRot) * Vector2.right);
        v.Normalize();

        return v;
    }

}
