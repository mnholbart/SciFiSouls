using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Cover : ActivitySystem {

    public List<GameObject> coverObjects = new List<GameObject>();
    GameObject coverObject;
    CoverCollision coll;

    public float Radius = 0.65f;

    new void Start() {
        CreateCoverObject();
        coll.UpdateSphereCollider(Radius);
        base.Start();
    }

    void CreateCoverObject() {
        GameObject g = new GameObject("Cover");
        g.transform.SetParent(transform, false);
        g.transform.localPosition = new Vector3(0, 0, 0);
        g.AddComponent<SphereCollider>();
        g.GetComponent<SphereCollider>().isTrigger = true;
        coll = g.AddComponent<CoverCollision>();
        coll.TriggerEnterEvent += OnCoverEnter;
        coll.TriggerExitEvent += OnCoverExit;
        coverObject = g;
    }

    void OnCoverEnter(Collider other) {
        SpriteBase s = other.GetComponent<SpriteBase>();
        if (s != null && s.cover) {
            coverObjects.Add(other.gameObject);
        }
    }

    void OnCoverExit(Collider other) {
        if (other.GetComponent<SpriteBase>() && coverObjects.Contains(other.gameObject)) {
            coverObjects.Remove(other.gameObject);
        }
    }

    protected override void AddActivities() {
    }

    protected override void AddActivityRestrictions() {
    }

    protected override void AddOtherActivityRestrictions() {
    }

    protected override void AddOtherTaskRestrictions() {
    }

    protected override void AddTaskRestrictions() {
    }

    protected override void AddTasks() {
    }
}
