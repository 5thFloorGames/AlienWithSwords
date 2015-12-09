using UnityEngine;
using System.Collections;

public class CharacterActionsSecond : MonoBehaviour {

	public int slashDamage;
    public int maxActions;
    int strikeDamage;

    bool enemyInAim;
	
	bool inCharacter;
	bool dead;
	int actions;

	float actionCooldown;
	float previousActionTime;

    Transform cameraTf;
    GameObject overlay;

	GameObject weaponPivot;
	MeleeWeaponDamages mwd;
	MeleeRangeInformer slashRange;

	UserInterfaceManager uim;
	
	void Awake() {
		dead = false;
		uim = GameObject.Find("UserInterface").GetComponent<UserInterfaceManager>();
        cameraTf = transform.FindChild("Camera");
		overlay = cameraTf.FindChild ("Overlay").gameObject;
		weaponPivot = cameraTf.FindChild ("WeaponPivot").gameObject;
		mwd = weaponPivot.GetComponentInChildren<MeleeWeaponDamages> ();
		slashRange = GetComponentInChildren<MeleeRangeInformer> ();
		weaponPivot.SetActive (false);
		actionCooldown = 0.7f;
		previousActionTime = Time.time;
        enemyInAim = false;
	}
	
	void Start () {
		updateActionsToUI();
	}
	
	void Update () {
		if (GameState.playersTurn && inCharacter && actions > 0 && !dead && enemyInAim) {
			if (Time.time - previousActionTime >= actionCooldown) {
				if (Input.GetButtonDown ("Fire1")){
					Slash();
					updateActionsToUI();
				}
			}
		}
	}

    public void EnemiesEnteredAimRange() {
        enemyInAim = true;
    }

    public void NoEnemiesInAimRange() {
        enemyInAim = false;
    }

	void Slash() {
		actions -= 1;
		previousActionTime = Time.time;
		weaponPivot.SetActive (true);
		mwd.damageAmount = slashDamage;
		
		weaponPivot.transform.localRotation = Quaternion.Euler (0f, -89.99f, 90f);
		iTween.RotateTo (weaponPivot, iTween.Hash (
			"y", 89.99f,
			"time", 0.5f,
			"islocal", true,
			"oncomplete", "PutWeaponAway",
			"oncompletetarget", gameObject));
	}
	
	void Strike() {
		actions -= 1;
		previousActionTime = Time.time;
		weaponPivot.SetActive (true);
		mwd.damageAmount = strikeDamage;

		weaponPivot.transform.localRotation = Quaternion.Euler (-90f, 0f, 0f);
		iTween.RotateTo (weaponPivot, iTween.Hash (
			"x", 30f,
			"time", 0.6f,
			"islocal", true,
			"oncomplete", "PutWeaponAway",
			"oncompletetarget", gameObject));
	}

	void PutWeaponAway() {
		weaponPivot.SetActive (false);
	}
	
	void updateActionsToUI() {
		uim.UpdateActionPoints(gameObject.name, actions, maxActions);
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
		slashRange.ActivateTheHitList ();
		inCharacter = true;
		overlay.SetActive (true);
	}
	
	void LeaveCharacter() {
		slashRange.DeactivateTheHitList ();
		inCharacter = false;
		overlay.SetActive (false);
	}
}
