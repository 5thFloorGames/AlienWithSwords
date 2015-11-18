using UnityEngine;
using System.Collections;

public class BulletDamages : MonoBehaviour {

	int damage;

	public void setDamage(int amount) {
		damage = amount;
	}

	void OnCollisionEnter (Collision coll) {
		GameObject other = coll.gameObject;
		if (other.tag == "Enemy") {
			other.gameObject.SendMessageUpwards("TakeDamage", damage);
		}
		Destroy (gameObject);
	}
}
