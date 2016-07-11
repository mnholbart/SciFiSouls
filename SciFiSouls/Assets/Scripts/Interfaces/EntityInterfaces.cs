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

