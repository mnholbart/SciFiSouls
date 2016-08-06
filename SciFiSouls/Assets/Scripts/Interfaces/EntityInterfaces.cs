using UnityEngine;
using System.Collections;

public interface IDamageable {
	void Damage(int amount);
}

public interface IHealable {
    void Heal(int amount);
}

public interface IKillable {
	void Kill();
}

public interface IHeightCollider {
    bool PassedHeightCheck();
}

public interface IHeightCallbacks {
    void OnPassHeightCheck(GameObject g);
    void OnFailHeightCheck(GameObject g);
}

public interface IDestructable {
    void Damage(int amount);
    void DamageStructuralIntegrity(DestructionPhase phase);
}
