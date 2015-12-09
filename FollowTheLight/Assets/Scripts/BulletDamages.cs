using UnityEngine;
using System.Collections;

public class BulletDamages : MonoBehaviour {

	int damage;

	public void setDamage(int amount) {
		damage = amount;
	}

	void OnCollisionEnter (Collision coll) {
		GameObject other = coll.gameObject;
		if (other.tag == "Enemy" || (other.tag == "Player" && (other.GetType() == typeof(CapsuleCollider)))) {
			other.SendMessageUpwards("TakeDamage", damage);
		}
		CreateHitEffect ();
		Destroy (gameObject);
	}

	void CreateHitEffect() {
		GameObject prefab = (GameObject) Resources.Load("BulletHitEffect");
		Instantiate (prefab, transform.position + (transform.rotation * new Vector3(0, 0, -0.5f)), Quaternion.identity);
	}
}
