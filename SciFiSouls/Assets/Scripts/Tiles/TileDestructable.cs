using UnityEngine;
using System.Collections.Generic;
using System;

public class TileDestructable : MonoBehaviour, IDestructable {

    Tile MyTile;

    public List<DestructionPhase> phases = new List<DestructionPhase>();

    void Awake() {
        MyTile = transform.parent.GetComponent<Tile>();
        enabled = MyTile.destructable;
    }

    void Start() {

    }

    public void Damage(int amount) {
        MyTile.CurrHealth -= amount;
        float percent = (float)MyTile.CurrHealth / (float)MyTile.MaxHealth;
        DestructionPhase newPhase = null;
        for (int i = 0; i < phases.Count; i++) {
            DestructionPhase temp = phases[i];
            if (percent < temp.percentActivate) {
                if (newPhase == null)
                    newPhase = temp;
                else if (temp.percentActivate < newPhase.percentActivate)
                    newPhase = temp;
            }
        }
        DamageStructuralIntegrity(newPhase);
    }

    public void DamageStructuralIntegrity(DestructionPhase phase) {
        if (phase == null)
            return;

        MyTile.ChangeSprite(phase.sprite);
        MyTile.DamageHeight(phase.height);
    }
}
