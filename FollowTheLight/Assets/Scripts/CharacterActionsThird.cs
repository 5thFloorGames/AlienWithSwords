using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
    GameObject aimedCharacter;

	GameObject target;

	float actionCooldown;
	float previousActionTime;

	Transform cameraTf;
	GameObject overlay;
	GameObject crosshairs;
	LaserController lc;

	UserInterfaceManager uim;
	CharacterSoundController csc;
	
	void Awake() {
		csc = GetComponentInChildren<CharacterSoundController>();
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
				if (Input.GetButtonDown ("Fire1") && (enemyInAim || characterInAim)) {
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
		if (actions == 0) {
			csc.outOfActions = true;
			csc.PlayOutOfActionsQuote();
		} else {
			csc.outOfActions = false;
		}
		uim.UpdateActionPoints(gameObject.name, actions, maxActions);
	}

	void CheckIfSomethingWithinAim () {
		Vector3 start = cameraTf.position;
		Vector3 direction = (cameraTf.rotation * new Vector3 (0, 0, 500f));
		RaycastHit hit;
        //Debug.DrawRay(start, direction, Color.green, 0.01f);
		if (Physics.Raycast (start, direction, out hit, (direction.magnitude + 1.0f))) {
			if (hit.collider.tag == "Player") {
				CharacterAimedAt ();
                CheckIfDifferentCharacter(hit);
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
			csc.PlayAttackingQuote();
			csc.PlayAttackSFX();
			if (hit.collider.tag == "Enemy" || hit.collider.tag == "Player") {
				target = hit.collider.transform.root.gameObject;
				lc.ShootLaser(hit.point);
			}
		}
	}

    public void AttackParticlesCollided() {
        SendDamageMessage();
    }

	void HealAimedCharacter() {
		previousActionTime = Time.time;
		Vector3 start = cameraTf.position;
		Vector3 direction = (cameraTf.rotation * new Vector3 (0, 0, 100f));
		RaycastHit hit;
		if (Physics.Raycast (start, direction, out hit, (direction.magnitude + 1.0f))) {
			actions -= 1;
			csc.PlayHealingQuote();
			csc.PlayHealSFX();
			if (hit.collider.tag == "Player") {
				target = hit.collider.gameObject;
				Invoke ("SendHealMessage", 0.5f);
				lc.HealLaser (hit.point);
			}
		}
	}

	void SendHealMessage() {
        List<object> info = new List<object>();
        object healObject = healing;
        info.Add(healObject);
        info.Add(gameObject);
        target.SendMessageUpwards ("Heal", info);
	}

	void SendDamageMessage() {
        List<object> info = new List<object>();
        object dmgObject = damage;
        info.Add(dmgObject);
        info.Add(gameObject);
        target.SendMessageUpwards ("TakeDamage", info);
	}

	void CheckIfDifferentEnemy(RaycastHit hit) {
		if (aimedEnemy != hit.transform.root.gameObject) {
			if (aimedEnemy) {
				aimedEnemy.SendMessage("NotAimedAt");
			}
			aimedEnemy = hit.transform.root.gameObject;
			aimedEnemy.SendMessage("AimedAt", gameObject);
		}
	}

    void CheckIfDifferentCharacter(RaycastHit hit) {
        if (aimedCharacter != hit.transform.root.gameObject) {
            if (aimedCharacter) {
                aimedCharacter.SendMessage("NotAimedAt");
            }
            aimedCharacter = hit.transform.root.gameObject;
            aimedCharacter.SendMessage("AimedAt", gameObject);
        }
    }
	
	void EnemyAimedAt() {
		if (!enemyInAim) {
			crosshairs.SetActive(false);
			enemyInAim = true;
		}
	}
	
	void EnemyNotAimedAt() {
		if (enemyInAim) {
            if (!characterInAim) {
				crosshairs.SetActive(true);
            }
			aimedEnemy.SendMessage("NotAimedAt");
			aimedEnemy = null;
			enemyInAim = false;
		}
	}

	void CharacterAimedAt() {
		if (!characterInAim) {
			crosshairs.SetActive(false);
            characterInAim = true;
		}
	}
	
	void CharacterNotAimedAt() {
		if (characterInAim) {
            if (!enemyInAim) {
				crosshairs.SetActive(true);
            }
            aimedCharacter.SendMessage("NotAimedAt");
            characterInAim = false;
			aimedCharacter = null;
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
		if (!dead) {
			actions = maxActions;
			UpdateActionsToUI();
		}
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
