﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterActionsFirst : MonoBehaviour {

	public int damage;
	public int coneDamage;
    public int maxActions;

	bool inCharacter;
    bool dead;
    int actions;

	CharacterSoundController cas;
	GameObject bullet;
	Transform bulletSpawnPoint;
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
		cas = GetComponentInChildren<CharacterSoundController>();
        bulletSpawnPoint = transform.FindChild("Camera");
        overlay = bulletSpawnPoint.FindChild("Overlay").gameObject;
        bulletSpawnPoint = bulletSpawnPoint.FindChild("BulletSpawnPoint");
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
        if (inCharacter && !dead) {
            CheckIfEnemyWithinAim();
		    if (GameState.playersTurn) {
			    if (Time.time - previousFiringTime >= bulletCooldown && actions > 0 && enemyInAim) {
				    if (Input.GetButtonUp ("Fire1")){
					    previousFiringTime = Time.time;
					    Shoot();
                        actions -= 1;
                        updateActionsToUI();
				    }
			    }
		    }
        }
    }

	void Shoot() {
		cas.PlayAttackingQuote ();
		cas.PlayAttackSFX ();
		GameObject firedBullet = (GameObject)Instantiate (bullet, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
		BulletDamages bs = firedBullet.GetComponent<BulletDamages> ();
		bs.SetHitSFX (cas.GetAttackHitClips());
		
		Rigidbody bulletrb = firedBullet.GetComponent<Rigidbody> ();
		bulletrb.AddForce(bulletSpawnPoint.rotation * bullet.transform.forward * 2000f);
        StartCoroutine(DealDamageWithRayCast());
	}

	void CheckIfEnemyWithinAim () {
		Vector3 start = bulletSpawnPoint.position;
		Vector3 direction = (bulletSpawnPoint.rotation * new Vector3 (0, 0, 500f));
		RaycastHit hit;
        //Debug.DrawRay(start, direction, Color.red, 0.1f);
		if (Physics.Raycast (start, direction, out hit, (direction.magnitude + 1.0f))) {
			if (hit.collider.tag == "Enemy" || hit.collider.tag == "Player" && hit.collider.gameObject != gameObject) {
				EnemyAimedAt();
				CheckIfDifferentEnemy(hit);
			} else {
				EnemyNotAimedAt();
			}
		} else {
			EnemyNotAimedAt();
		}
	}

    IEnumerator DealDamageWithRayCast() {
        yield return new WaitForSeconds(0.2f);
        Vector3 start = bulletSpawnPoint.position;
        Vector3 direction = (bulletSpawnPoint.rotation * new Vector3(0, 0, 500f));
        RaycastHit hit;
        //Debug.DrawRay(start, direction, Color.red, 0.1f);
        if (Physics.Raycast(start, direction, out hit, (direction.magnitude + 1.0f))) {
            if (hit.collider.tag == "Enemy" || hit.collider.tag == "Player") {
                List<object> info = new List<object>();
                object dmgObject = damage;
                info.Add(dmgObject);
                info.Add(gameObject);
                hit.collider.gameObject.SendMessageUpwards("TakeDamage", info);
            } 
        } 
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

	void EnemyAimedAt() {
		if (!enemyInAim) {
			crossHairs.SetActive(false);
			enemyInAim = true;
		}
	}
	
	void EnemyNotAimedAt() {
		if (enemyInAim) {
			crossHairs.SetActive(true);
			aimedEnemy.SendMessage("NotAimedAt");
			aimedEnemy = null;
			enemyInAim = false;
		}
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
		var angle = bulletSpawnPoint.rotation * startingAngle;
		var direction = angle * Vector3.forward;
		var pos = bulletSpawnPoint.position;
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
		if (actions == 0) {
			cas.outOfActions = true;
			cas.PlayOutOfActionsQuote();
		} else {
			cas.outOfActions = false;
		}
        uim.UpdateActionPoints(transform.name, actions, maxActions);
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
	        updateActionsToUI();
		}
    }

    void EnterCharacter() {
        inCharacter = true;
		overlay.SetActive (true);
    }

    void LeaveCharacter() {
		EnemyNotAimedAt ();
        inCharacter = false;
		overlay.SetActive (false);
    }
}
