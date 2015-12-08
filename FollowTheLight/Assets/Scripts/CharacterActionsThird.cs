using UnityEngine;
using System.Collections;

public class CharacterActionsThird : MonoBehaviour {
	
	public int healing;
	public int damage;
	public int maxActions;

	int actions;
	bool inCharacter;
	bool dead;
	bool characterInAim;
	bool enemyInAim;
	GameObject aimedEnemy;

	float actionCooldown;
	float previousActionTime;

	Transform cameraTf;
	GameObject overlay;
	GameObject crosshairs;
	LaserController lc;
	
	UserInterfaceManager uim;
	
	void Awake() {
		uim = GameObject.Find("UserInterface").GetComponent<UserInterfaceManager>();
        lc = GetComponentInChildren<LaserController>();
        cameraTf = transform.FindChild("Camera");
        overlay = cameraTf.FindChild ("Overlay").gameObject;
		crosshairs = overlay.transform.FindChild ("Crosshairs").gameObject;
		dead = false;

		actionCooldown = 0.6f;
		previousActionTime = Time.time;
	}
	
	void Start () {
		UpdateActionsToUI();
	}
	
	void Update () {
		if (GameState.playersTurn && inCharacter && !dead) {
			CheckIfSomethingWithinAim ();
			if (Time.time - previousActionTime >= actionCooldown && actions > 0) {
				if (Input.GetButtonDown ("Fire1") && enemyInAim) {
					DamageAimedCharacter ();
					UpdateActionsToUI ();
				}
				if (Input.GetButtonDown ("Fire2") && characterInAim) {
					HealAimedCharacter ();
					UpdateActionsToUI ();
				}
			}
		}
	}
	
	void UpdateActionsToUI() {
		uim.UpdateActionPoints(gameObject.name, actions, maxActions);
	}

	void CheckIfSomethingWithinAim () {
		Vector3 start = cameraTf.position;
		Vector3 direction = (cameraTf.rotation * new Vector3 (0, 0, 30f));
		RaycastHit hit;
		if (Physics.Raycast (start, direction, out hit, (direction.magnitude + 1.0f))) {
			if (hit.collider.tag == "Player") {
				CharacterAimedAt ();
			} else if (hit.collider.tag == "Enemy") {
				EnemyAimedAt();
				CheckIfDifferentEnemy(hit);
			} else {
				CharacterNotAimedAt();
				EnemyNotAimedAt();
			}
		} else {
			CharacterNotAimedAt();
			EnemyNotAimedAt();
		}
	}

	void DamageAimedCharacter() {
		previousActionTime = Time.time;
		Vector3 start = cameraTf.position;
		Vector3 direction = (cameraTf.rotation * new Vector3 (0, 0, 100f));
		RaycastHit hit;
		if (Physics.Raycast (start, direction, out hit)) {
			actions -= 1;
			if (hit.collider.tag == "Enemy") {
				hit.collider.gameObject.SendMessageUpwards ("TakeDamage", damage);
				lc.ShootLaser(hit.point);
			}
		}
	}

	void HealAimedCharacter() {
		previousActionTime = Time.time;
		Vector3 start = cameraTf.position;
		Vector3 direction = (cameraTf.rotation * new Vector3 (0, 0, 100f));
		RaycastHit hit;
		if (Physics.Raycast (start, direction, out hit, (direction.magnitude + 1.0f))) {
			actions -= 1;
			if (hit.collider.tag == "Player") {
				hit.collider.gameObject.SendMessageUpwards ("Heal", healing);
				lc.HealLaser (hit.point);
			}
		}
	}

	void CheckIfDifferentEnemy(RaycastHit hit) {
		if (aimedEnemy != hit.transform.parent.gameObject) {
			if (aimedEnemy) {
				aimedEnemy.SendMessage("NotAimedAt");
			}
			aimedEnemy = hit.transform.parent.gameObject;
			aimedEnemy.SendMessage("AimedAt");
		}
	}
	
	void EnemyAimedAt() {
		if (!enemyInAim) {
			crosshairs.transform.localScale = (new Vector3 (1.2f, 1.2f, 1.2f));
			enemyInAim = true;
		}
	}
	
	void EnemyNotAimedAt() {
		if (enemyInAim) {
			crosshairs.transform.localScale = (new Vector3(1f, 1f, 1f));
			aimedEnemy.SendMessage("NotAimedAt");
			aimedEnemy = null;
			enemyInAim = false;
		}
	}

	void CharacterAimedAt() {
		if (!characterInAim) {
			characterInAim = true;
		}
	}
	
	void CharacterNotAimedAt() {
		if (characterInAim) {
			characterInAim = false;
		}
	}
	
	
	// Character Manager calls these with a broadcast message
	
	void CharacterDied() {
		dead = true;
	}
	
	void CharacterResurrected() {
		dead = false;
		ResetActions();
	}
	
	void ResetActions() {
		actions = maxActions;
		UpdateActionsToUI();
	}
	
	void EnterCharacter() {
		inCharacter = true;
		overlay.SetActive (true);
	}
	
	void LeaveCharacter() {
		EnemyNotAimedAt ();
		CharacterNotAimedAt ();
		inCharacter = false;
		overlay.SetActive (false);
	}
}
