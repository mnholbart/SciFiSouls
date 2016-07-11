using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class Player : Entity, IDamageable, IHealable, IKillable {

    public int SpriteRotation = -90; //0 is if the sprite was facing left

	public float MoveSpeed = 10;

    public float CurrentStamina;
    public float MaxStamina = 40;
    public float StaminaRegenPerSecond = 7.5f;
    public float SprintStaminaUsePerSecond = 15f;
    public float StaminaDrainSprintDelayTime = 2f;
    bool sprinting = false;
    bool sprintDelayed = false;

    Vector3 LastPosition;

	Rigidbody2D body;

	void Start() {
		body = GetComponent<Rigidbody2D>();

        SpawnPlayer();
	}

	void Update() {
		CheckInput();
	}

	void CheckInput() {
		MovementInput();
		RotateTowardsMouse();
	}

    void SpawnPlayer() {
        FullHeal();
        FullStamina();
        LastPosition = transform.position;
        dead = false;
    }

	void MovementInput() {
        SprintInput();

		Vector3 moveVector = Vector3.zero;
		if (Input.GetKey(KeyCode.W)) {
			moveVector.y += 1;
		}
		if (Input.GetKey(KeyCode.S)) {
			moveVector.y -= 1;
		}
		if (Input.GetKey(KeyCode.A)) {
			moveVector.x -= 1;
		}
		if (Input.GetKey(KeyCode.D)) {
			moveVector.x += 1;
		}
		moveVector.Normalize();
		moveVector.x *= MoveSpeed*Time.deltaTime * (sprinting ? 2 : 1);
		moveVector.y *= MoveSpeed*Time.deltaTime * (sprinting ? 2 : 1);

//		Debug.Log(moveVector);
		body.velocity = moveVector;
        LastPosition = transform.position;
	}

    void SprintInput() {
        if (CanSprint())
            sprinting = true;
        else sprinting = false;

        if (sprinting) {
            CurrentStamina -= SprintStaminaUsePerSecond * Time.deltaTime;
            if (CurrentStamina < 0) //Penalty for running out of stamina
                StartCoroutine(DelaySprint(StaminaDrainSprintDelayTime));
        } else {
            CurrentStamina += StaminaRegenPerSecond * Time.deltaTime;
        }
        CurrentStamina = Mathf.Clamp(CurrentStamina, 0, MaxStamina);
        GUIManager.instance.UpdateStaminaBarImage(CurrentStamina, MaxStamina);
    }

    bool CanSprint() {
        if (!Input.GetKey(KeyCode.LeftShift))
            return false;
        if (LastPosition == transform.position)
            return false;
        if (sprintDelayed)
            return false;
        return true;
    }

	void RotateTowardsMouse() {
		Vector3 mouse = Input.mousePosition;
		Vector3 screenPoint = Camera.main.WorldToScreenPoint(transform.localPosition);
		Vector3 offset = new Vector2(mouse.x - screenPoint.x, mouse.y - screenPoint.y);
		float angle = Mathf.Atan2(offset.y, offset.x) * Mathf.Rad2Deg;
		transform.eulerAngles = new Vector3(0, 0, angle + SpriteRotation);
	}

    public IEnumerator DelaySprint(float duration) {
        sprintDelayed = true;
        yield return new WaitForSeconds(duration);
        sprintDelayed = false;
    }

	public void Damage(int amount) {
        if (dead)
            return;

        CurrentHealth -= amount;
        GUIManager.instance.PlayerReceiveDamage(amount, CurrentHealth, MaxHealth);

        if (CurrentHealth < 0)
            Kill();
	}
    
    public void FullHeal() {
        CurrentHealth = MaxHealth;
        GUIManager.instance.UpdatePlayerHealthbar(CurrentHealth, MaxHealth);
    }

    public void FullStamina() {
        CurrentStamina = MaxStamina;
        GUIManager.instance.UpdatePlayerStaminaBar(CurrentStamina, MaxStamina);
    }

    public void Heal(int amount) {
        CurrentHealth += amount;
        GUIManager.instance.PlayerReceiveHealing(amount, CurrentHealth, MaxHealth);
    }

	public void Kill() {
        dead = true;
	}
}
