using UnityEngine;
using System.Collections;

public class CharacterActions : MonoBehaviour {

	public int bulletDamage;

	GameObject bullet;
	GameObject cameraObj;
	float bulletCooldown;
	float previousFire;
	
	void Start () {
		bullet = (GameObject)Resources.Load ("Bullet");
		bulletCooldown = 0.5f;
	}

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
		firedBullet.GetComponent<BulletDamages> ().setDamage (bulletDamage);
		Rigidbody bulletrb = firedBullet.GetComponent<Rigidbody> ();
		bulletrb.AddForce(transform.rotation * bullet.transform.forward * 2000f);
	}
}
