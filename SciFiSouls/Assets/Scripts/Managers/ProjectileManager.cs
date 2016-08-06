using UnityEngine;
using System.Collections;

public class ProjectileManager : MonoBehaviour {

    public static ProjectileManager instance;

    public Transform ProjectileParent = null;

    void Awake() {
        if (instance != null) {
            Destroy(instance);
        }
        instance = this;
    }

    void Start() {
        GameObject parent = Instantiate(new GameObject());
        parent.name = "Projectiles";
        ProjectileParent = parent.transform;
    }

    public void RegisterProjectile(Transform projectile) {
        projectile.SetParent(ProjectileParent, true);
    }

}
