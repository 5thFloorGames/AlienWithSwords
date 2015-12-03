using UnityEngine;
using System.Collections;

public class CharacterActionsFirst : MonoBehaviour {

	public int damage;
	public int coneDamage;
    public int maxActions;

	bool inCharacter;
    bool dead;
    int actions;

	GameObject bullet;
	GameObject cameraObj;
	GameObject overlay;
	GameObject crossHairs;

	float bulletCooldown;
	float previousFiringTime;
	bool enemyInAim;
	GameObject aimedEnemy;

	Quaternion startingAngle = Quaternion.AngleAxis(-45f, Vector3.up);
	Quaternion stepAngle = Quaternion.AngleAxis(3.75f, Vector3.up);
	bool aimingCone;

    UserInterfaceManager uim;

    void Awake() {
        uim = GameObject.Find("UserInterface").GetComponent<UserInterfaceManager>();
		overlay = transform.FindChild ("Overlay").gameObject;
		crossHairs = overlay.transform.FindChild ("Crosshairs").gameObject;
        dead = false;
    }

    void Start () {
		aimingCone = false;
		bullet = (GameObject)Resources.Load ("Bullet");
		bulletCooldown = 0.5f;
        updateActionsToUI();
	}

	void Update () {
		if (GameState.playersTurn && inCharacter && !dead) {
			CheckIfEnemyWithinAim ();
			if (Time.time - previousFiringTime >= bulletCooldown && actions > 0) {
				if (Input.GetButtonUp ("Fire1")){
					previousFiringTime = Time.time;
					Shoot();
                    actions -= 1;
                    updateActionsToUI();
				}
			}
		}
	}

	void Shoot() {
		GameObject firedBullet = (GameObject)Instantiate (bullet, transform.position + transform.rotation *
		                                                  new Vector3(0, 0, 1), transform.rotation);
		firedBullet.GetComponent<BulletDamages> ().setDamage (damage);
		Rigidbody bulletrb = firedBullet.GetComponent<Rigidbody> ();
		bulletrb.AddForce(transform.rotation * bullet.transform.forward * 2000f);
	}

	void CheckIfEnemyWithinAim () {
		Vector3 start = transform.position;
		Vector3 direction = (transform.rotation * new Vector3 (0, 0, 30f));
		RaycastHit hit;
		if (Physics.Raycast (start, direction, out hit, (direction.magnitude + 1.0f))) {
			if (hit.collider.tag == "Enemy") {
				if (!enemyInAim) {
					EnemyAimedAt();
				}
				if (aimedEnemy != hit.transform.parent.gameObject) {
					if (aimedEnemy) {
						aimedEnemy.SendMessage("NotAimedAt");
					}
					aimedEnemy = hit.transform.parent.gameObject;
					aimedEnemy.SendMessage("AimedAt");
				}
			} else {
				if (enemyInAim) {
					EnemyNotAimedAt();
					aimedEnemy.SendMessage("NotAimedAt");
					aimedEnemy = null;
				}
			}
		} else {
			if (enemyInAim) {
				EnemyNotAimedAt();
			}
		}
	}

	void EnemyAimedAt() {
		crossHairs.transform.localScale = (new Vector3(1.2f, 1.2f, 1.2f));
		enemyInAim = true;
	}
	
	void EnemyNotAimedAt() {
		crossHairs.transform.localScale = (new Vector3(1f, 1f, 1f));
		enemyInAim = false;
	}

	void AimCone() {
		if (aimingCone) {
			return;
		}
		DetectThings ();
		aimingCone = true;
		Debug.Log ("aiming");
	}

	void ShootCone() {
		actions -= 1;
		previousFiringTime = Time.time;
		if (!aimingCone) {
			return;
		}
		aimingCone = false;
		Debug.Log ("shooting");
	}

	void DetectThings() {
		RaycastHit hit;
		var angle = transform.rotation * startingAngle;
		var direction = angle * Vector3.forward;
		var pos = transform.position;
		for (var i = 0; i < 24; i++) {
			Debug.DrawRay(pos, (direction * 30f), Color.blue, 3.0f);
			if(Physics.Raycast(pos, direction, out hit, 30f)) {
				var enemy = hit.collider.GetComponentInParent<EnemyState>();
				if(enemy) {
					Debug.Log ("it's an enemy");
				}
			}
			direction = stepAngle * direction;
		}
	}

    void updateActionsToUI() {
        uim.UpdateActionPoints(gameObject.transform.parent.name, actions, maxActions);
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
        updateActionsToUI();
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
