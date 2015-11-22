using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum CharacterType { Character1, Character2, Character3 };

public class CharacterManager : MonoBehaviour {

	List<GameObject> characters;
    UserInterfaceManager uim;
	bool firstActive;
	bool secondActive;
	bool thirdActive;

    bool initialized = false;

	void Awake() {

	}

	void Start () {
        if (!initialized) {
            initialized = true;
            HandleSpawning();
            Invoke("SetInitializedToFalse", 0.1f);
        }
	}

    void OnLevelWasLoaded(int level) {
        if (!initialized && level != 0) {
            initialized = true;
            HandleSpawning();
            Invoke("SetInitializedToFalse", 0.1f);
        }
    }

    void Update () {
		if (Input.GetKeyDown (KeyCode.Alpha1) && firstActive) {
			EnterCharacter(GetCharacterObject("Character1"));
		}
		if (Input.GetKeyDown (KeyCode.Alpha2) && secondActive) {
			EnterCharacter(GetCharacterObject("Character2"));
		}
		if (Input.GetKeyDown(KeyCode.Alpha3) && thirdActive) {
            EnterCharacter(GetCharacterObject("Character3"));
        }
    }

    public void PlayersTurnActivated() {
        ResetCharacterMovement();
        ResetCharacterActions();
    }

    public void CharacterDied(GameObject character, CharacterType type) {
        if (type == CharacterType.Character1) {
            firstActive = false;
        } else if (type == CharacterType.Character2) {
            secondActive = false;
        } else if (type == CharacterType.Character3) {
            thirdActive = false;
        }
        character.BroadcastMessage("CharacterDied");
        CheckIfAllCharactersDead();
    }

    void CheckIfAllCharactersDead() {
        if ((!firstActive && !secondActive && !thirdActive)) {
            gameObject.GetComponent<GameManager>().AllCharactersDead();
        }
    }

    void HandleSpawning() {
        uim = GameObject.Find("UserInterface").GetComponent<UserInterfaceManager>();

        firstActive = false;
        secondActive = false;
        thirdActive = false;

        if (uim != null) {
            uim.HideCharacterInfos();
        }

        characters = new List<GameObject>();
        GameState.characters = characters;
        CheckSpawns();
    }

	void CheckSpawns() {
		GameObject[] spawns = GameObject.FindGameObjectsWithTag ("Spawn");
		if (spawns.Length == 0) {
			Debug.Log ("Create a spawn point in the scene to spawn a character (Resources). Name it Spawn1, Spawn2 or Spawn3.");
            return;
		}
		int spawnCount = 0;
		foreach (GameObject spawn in spawns) {

			spawnCount += 1;
			string charName;

			if (spawn.name == "Spawn1" && !firstActive) {
				charName = "Character1";
				SpawnFirstCharacter(charName, (spawn.transform.position + new Vector3(0, 1, 0)), spawn.transform.rotation);
			} else if (spawn.name == "Spawn2" && !secondActive) {
				charName = "Character2";
				SpawnSecondCharacter(charName, (spawn.transform.position + new Vector3(0, 1, 0)), spawn.transform.rotation);
			} else if (spawn.name == "Spawn3" && !thirdActive) {
				charName = "Character3";
				SpawnThirdCharacter(charName, (spawn.transform.position + new Vector3(0, 1, 0)), spawn.transform.rotation);
			} else {
				spawnCount -= 1;
				Debug.Log ("A spawn point must always be named Spawn1, Spawn2 or Spawn3 based on the character you want it to spawn.");
				Debug.Log ("You cannot spawn more than one of each character type.");
			}
		}
		EnterCharacter (characters[0]);
		GameState.amountOfCharacters = spawnCount;
	}

	void SpawnFirstCharacter(string name, Vector3 spawnSpot, Quaternion rotation) {
		GameObject character = LoadCharacterToScene (name, spawnSpot, rotation);
		firstActive = true;
		AssignCharacterStats (character, CharacterType.Character1, 20, 10f);
	}

	void SpawnSecondCharacter(string name, Vector3 spawnSpot, Quaternion rotation) {
		GameObject character = LoadCharacterToScene (name, spawnSpot, rotation);
		secondActive = true;
		AssignCharacterStats (character, CharacterType.Character2, 10, 50f);
	}

	void SpawnThirdCharacter(string name, Vector3 spawnSpot, Quaternion rotation) {
		GameObject character = LoadCharacterToScene (name, spawnSpot, rotation);
		thirdActive = true;
		AssignCharacterStats (character, CharacterType.Character3, 30, 30f);
	}

	GameObject LoadCharacterToScene(string name, Vector3 position, Quaternion rotation) {
		GameObject prefab = (GameObject) Resources.Load(name);
		GameObject character = ((GameObject) Instantiate (prefab, position, rotation));
        character.name = name;
        characters.Add(character);
        if (uim != null) {
            uim.ShowCharacterInfos(name);
        }
        return character;
	}

	void AssignCharacterStats(GameObject character, CharacterType type, int health, float maximumMovement) {
		character.GetComponent<CharacterMovement> ().maximumMovement = maximumMovement;
		character.GetComponent<CharacterState> ().Init (type, health, gameObject);
	}

	void ResetCharacterMovement() {
		foreach (GameObject character in characters) {
			character.SendMessage("ResetMovement");
		}
	}

    void ResetCharacterActions() {
        foreach (GameObject character in characters) {
            character.BroadcastMessage("ResetActions");
        }
    }

	GameObject GetCharacterObject(string name) {
		foreach (GameObject character in characters) {
			if (character.name == name) {
				return character;
			}
		}
		return null;
	}

	void EnterCharacter(GameObject character) {
		foreach (GameObject other in characters) {
			if (other != character) {
                other.BroadcastMessage("LeaveCharacter");
			}
		}
        character.BroadcastMessage("EnterCharacter");
        GameState.activeCharacter = character;
	}
    

    void SetInitializedToFalse() {
        initialized = false;
    }

}
