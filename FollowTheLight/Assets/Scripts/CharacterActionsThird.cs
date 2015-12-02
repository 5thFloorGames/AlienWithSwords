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

	float actionCooldown;
	float previousActionTime;

	GameObject cameraObj;
	GameObject overlay;
	LaserController lc;
	
	UserInterfaceManager uim;
	
	void Awake() {
		lc = GetComponentInChildren<LaserController> ();
		CheckIfSomethingWithinAim ();
		uim = GameObject.Find("UserInterface").GetComponent<UserInterfaceManager>();
		overlay = transform.FindChild ("Overlay").gameObject;
		dead = false;

		actionCooldown = 0.6f;
		previousActionTime = Time.time;
	}
	
	void Start () {
		UpdateActionsToUI();
	}
	
	void FixedUpdate () {
		if (GameState.playersTurn && inCharacter && actions > 0 && !dead) {
			if (Time.time - previousActionTime >= actionCooldown) {
				CheckIfSomethingWithinAim ();
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
		uim.UpdateActionPoints(gameObject.transform.parent.name, actions, maxActions);
	}

	void CheckIfSomethingWithinAim () {
		Vector3 start = transform.position;
		Vector3 direction = (transform.rotation * new Vector3 (0, 0, 30f));
		RaycastHit hit;
		if (Physics.Raycast (start, direction, out hit, (direction.magnitude + 1.0f))) {
			if (hit.collider.tag == "Player") {
				if (!characterInAim) {
					CharacterAimedAt ();
				}
			} else if (hit.collider.tag == "Enemy") {
				if (!enemyInAim) {
					EnemyAimedAt();
				}
			} else {
				if (characterInAim) {
					CharacterNotAimedAt ();
				}
				if (enemyInAim) {
					EnemyNotAimedAt();
				}
			}
		} else {
			if (characterInAim) {
				CharacterNotAimedAt();
			}
		}
	}

	void DamageAimedCharacter() {
		previousActionTime = Time.time;
		Vector3 start = transform.position;
		Vector3 direction = (transform.rotation * new Vector3 (0, 0, 30f));
		RaycastHit hit;
		if (Physics.Raycast (start, direction, out hit)) {
			actions -= 1;
			if (hit.collider.tag == "Enemy") {
				hit.collider.gameObject.SendMessageUpwards ("TakeDamage", damage);
				lc.ShootLaser(hit.collider.transform.position);
			}
		}
	}

	void HealAimedCharacter() {
		previousActionTime = Time.time;
		Vector3 start = gameObject.transform.position;
		Vector3 direction = (gameObject.transform.rotation * new Vector3 (0, 0, 30f));
		RaycastHit hit;
		if (Physics.Raycast (start, direction, out hit, (direction.magnitude + 1.0f))) {
			actions -= 1;
			if (hit.collider.tag == "Player") {
				hit.collider.gameObject.SendMessageUpwards ("Heal", healing);
				lc.HealLaser (hit.collider.transform.position);
			}
		}
	}

	void EnemyAimedAt() {
		enemyInAim = true;
	}

	void EnemyNotAimedAt() {
		enemyInAim = false;
	}

	void CharacterAimedAt() {
		characterInAim = true;
	}
	
	void CharacterNotAimedAt() {
		characterInAim = false;
	}
	
	
	// CharacterType Manager calls these with a broadcast message
	
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
		inCharacter = false;
		overlay.SetActive (false);
	}
}
