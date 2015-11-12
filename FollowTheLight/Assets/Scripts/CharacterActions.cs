using UnityEngine;
using System.Collections;

public class CharacterActions : MonoBehaviour {

	GameObject bullet;
	GameObject cameraObj;
	float bulletCooldown;
	float previousFire;

	// Use this for initialization
	void Start () {
		bullet = (GameObject)Resources.Load ("Bullet");
		bulletCooldown = 0.5f;
	}
	
	// Update is called once per frame
	void Update () {
		if (GameState.playersTurn) {
			if (Time.time - previousFire >= bulletCooldown) {
				if (Input.GetButton ("Fire1")){
					previousFire = Time.time;
					Shoot();
				}
			}

		}
	}

	void Shoot() {
		GameObject firedBullet = (GameObject)Instantiate (bullet, transform.position + transform.rotation *
		                                                  new Vector3(0, 0, 1), transform.rotation);
		Rigidbody bulletrb = firedBullet.GetComponent<Rigidbody> ();
		bulletrb.AddForce(transform.rotation * bullet.transform.forward * 2000f);
	}
}
