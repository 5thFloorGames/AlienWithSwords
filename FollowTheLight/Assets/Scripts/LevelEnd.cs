using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelEnd : MonoBehaviour {

	Dictionary<string, bool> charactersEnding;
	GameManager gm;
	bool endingActive;

	void Start () {
        endingActive = false;
		charactersEnding = new Dictionary<string, bool>();
		GameObject[] characters = GameObject.FindGameObjectsWithTag ("Player");
		foreach (GameObject character in characters) {
			charactersEnding[character.name] = false;
		}
		
		gm = (GameManager)GameObject.Find ("GameManager").GetComponent<GameManager>();
		endingActive = true;
	}

	void OnTriggerEnter (Collider other) {
		if (endingActive) {
			charactersEnding [other.name] = true;
			CheckEndOfAllCharacters ();
		}
	}

	void OnTriggerExit (Collider other) {
		if (endingActive) {
			charactersEnding [other.name] = false;
		}
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
