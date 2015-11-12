using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AreaDamageBehavior : MonoBehaviour {

	public float time;
	public float size;
	public int damage;

	void OnTriggerEnter (Collider other) {
		Debug.Log (gameObject.name + " hit " + other.name);
		if (other.tag == "Player") {
			 other.gameObject.SendMessageUpwards("takeDamage", damage);
		}
	}

	void Start () {
		iTween.ScaleTo(gameObject, iTween.Hash(
			"scale", new Vector3 (size, size, size),
			"delay", 0.5f,
			"time", time,
			"oncomplete", "fadeAwayAndSuicide"));
	}

	void Update () {
		
	}

	void fadeAwayAndSuicide() {
		// implement fading away for the final effects
		gameObject.GetComponent<Collider> ().enabled = true;
		StartCoroutine (suicide());
	}

	IEnumerator suicide() {
		yield return new WaitForSeconds(0.1f);
		Destroy (gameObject);
	}


}
