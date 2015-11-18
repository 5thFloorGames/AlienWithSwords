using UnityEngine;
using System.Collections;

public class FirstCharacterActions : MonoBehaviour {

	public int damage;
    public int maxActions;

	bool inCharacter;
    int actions;

	GameObject bullet;
	GameObject cameraObj;
	GameObject hands;

	float bulletCooldown;
	float previousFiringTime;

    UserInterfaceManager uim;

    void Awake() {
        uim = GameObject.Find("UserInterface").GetComponent<UserInterfaceManager>();
		hands = transform.FindChild ("Hands").gameObject;
    }

    void Start () {
		bullet = (GameObject)Resources.Load ("Bullet");
		bulletCooldown = 0.5f;
        updateActionsToUI();
	}

	void Update () {
		if (GameState.playersTurn && inCharacter && actions > 0) {
			if (Time.time - previousFiringTime >= bulletCooldown) {
				if (Input.GetButton ("Fire1")){
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

    void updateActionsToUI() {
        uim.UpdateActionMeter(gameObject.transform.parent.name, actions, maxActions);
    }


    // Character Manager calls these with a broadcast message

    void ResetActions() {
        actions = maxActions;
        updateActionsToUI();
    }

    void EnterCharacter() {
        inCharacter = true;
		hands.SetActive (true);
    }

    void LeaveCharacter() {
        inCharacter = false;
		hands.SetActive (false);
    }
}
