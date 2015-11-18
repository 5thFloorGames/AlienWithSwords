using UnityEngine;
using System.Collections;

public class LookAtActiveCharacter : MonoBehaviour {

	int knownCharacters;
	Transform char1;
	Transform char2;
	Transform char3;
	
	void Start () {
		knownCharacters = 0;
		CheckPlayerPositions ();
	}

	void Update () {
		if (GameState.activeCharacter != gameObject.transform.parent.gameObject.name) {
			if (GameState.activeCharacter == "Character1" && knownCharacters > 0) {
				transform.LookAt(char1);
			} else if (GameState.activeCharacter == "Character2" && knownCharacters > 1) {
				transform.LookAt(char2);
			} else if (GameState.activeCharacter == "Character3" && knownCharacters > 2) {
				transform.LookAt(char3);
			}
		}
	}

	void OnLevelWasLoaded(int level) {
		CheckPlayerPositions ();
	}

	void CheckPlayerPositions () {
		if (GameState.amountOfCharacters > knownCharacters) {
			char1 = GameObject.Find("Character1").transform;
			knownCharacters += 1;
			
			if (GameState.amountOfCharacters > 1) {
				char2 = GameObject.Find("Character2").transform;
				knownCharacters += 1;
			}
			
			if (GameState.amountOfCharacters > 2) {
				char3 = GameObject.Find("Character3").transform;
				knownCharacters += 1;
			}
			
			if (GameState.amountOfCharacters > 3) {
				Debug.Log (gameObject.name + "trying to look for more characters");
			}
		}
	}
}
