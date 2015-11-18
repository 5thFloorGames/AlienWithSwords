using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AreaDamageBehavior : MonoBehaviour {

	int damage;
	float timeForSuiciding = 0.1f;

	public void Init (float time, float size, int damageAmount) {
		time = time - timeForSuiciding;
		damage = damageAmount;
		iTween.ScaleTo(gameObject, iTween.Hash(
			"scale", new Vector3 (size, size, size),
			"time", time,
			"oncomplete", "fadeAwayAndSuicide"));
	}

	void OnTriggerEnter (Collider other) {
		Debug.Log (gameObject.name + " hit " + other.name);
		if (other.tag == "Player") {
			 other.gameObject.SendMessageUpwards("takeDamage", damage);
		}
	}

	void Start () {

	}

	void Update () {
		
	}

	void fadeAwayAndSuicide() {
		// implement fading away for the final effects
		gameObject.GetComponent<Collider> ().enabled = true;
		StartCoroutine (suicide());
	}

	IEnumerator suicide() {
		yield return new WaitForSeconds(timeForSuiciding);
		Destroy (gameObject);
	}


}
