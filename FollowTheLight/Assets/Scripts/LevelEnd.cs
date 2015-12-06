using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelEnd : MonoBehaviour {

	Dictionary<string, bool> charactersEnding;

	void Start () {
        Invoke("LateStart", 0.5f);
		
	}

    void LateStart() {
        CreateCharacterDictionary();
    }

	void OnTriggerEnter (Collider other) {
        if (other.GetType() == typeof(CapsuleCollider)) {
            charactersEnding[other.name] = true;
            CheckEndOfAllCharacters();
        }
	}

	void OnTriggerExit (Collider other) {
		charactersEnding [other.name] = false;
	}

	void CheckEndOfAllCharacters() {
        if (GameState.characters.Count != charactersEnding.Count) {
            CreateCharacterDictionary();
        }
		foreach(KeyValuePair<string, bool> entry in charactersEnding) {
			if (entry.Value == false) {
				return;
			}
		}
        GameObject.Find("GameManager").GetComponent<GameManager>().AllCharactersInLevelEnd();
	}

    void CreateCharacterDictionary() {
        charactersEnding = new Dictionary<string, bool>();
        foreach (GameObject character in GameState.characters) {
            charactersEnding[character.name] = false;
        }
    }

}
