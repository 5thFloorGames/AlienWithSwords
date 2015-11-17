using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelEnd : MonoBehaviour {

	Dictionary<string, bool> charactersEnding;
	GameManager gm;
	bool enabled;

	void Start () {
		enabled = false;
		Invoke("LateStart", 0.8f);
	}

	void OnTriggerEnter (Collider other) {
		if (enabled) {
			charactersEnding [other.name] = true;
			CheckEndOfAllCharacters ();
		}
	}

	void OnTriggerExit (Collider other) {
		if (enabled) {
			charactersEnding [other.name] = false;
		}
	}

	void LateStart() {
		charactersEnding = new Dictionary<string, bool>();
		GameObject[] characters = GameObject.FindGameObjectsWithTag ("Player");
		foreach (GameObject character in characters) {
			charactersEnding[character.name] = false;
		}

		gm = (GameManager)GameObject.Find ("GameManager").GetComponent<GameManager>();
		enabled = true;
	}

	void CheckEndOfAllCharacters() {
		foreach(KeyValuePair<string, bool> entry in charactersEnding) {
			if (entry.Value == false) {
				return;
			}
		}
		gm.LevelComplete ();
	}
}
