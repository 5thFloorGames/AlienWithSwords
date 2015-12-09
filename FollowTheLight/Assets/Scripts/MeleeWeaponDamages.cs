using UnityEngine;
using System.Collections;

public class MeleeWeaponDamages : MonoBehaviour {
	public int damageAmount;

	void Start () {
	
	}

	void OnTriggerEnter (Collider other) {
		if (((other.GetType() == typeof(CapsuleCollider)) && other.tag == "Enemy") || (other.tag == "Player" && (other.GetType() == typeof(CapsuleCollider)))) {
			other.gameObject.SendMessageUpwards("TakeDamage", damageAmount);
		}
	}
}