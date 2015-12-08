using UnityEngine;
using System.Collections;

public class MeleeWeaponDamages : MonoBehaviour {
	public int damageAmount;

	void Start () {
	
	}

	void OnTriggerEnter (Collider other) {
		if (other.tag == "Enemy" && (other.GetType() == typeof(CapsuleCollider))) {
			other.gameObject.SendMessageUpwards("TakeDamage", damageAmount);
		}
	}
}