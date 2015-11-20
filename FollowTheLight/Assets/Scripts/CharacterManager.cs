using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterManager : MonoBehaviour {
	List<GameObject> characters;
    UserInterfaceManager uim;
	bool firstActive;
	bool secondActive;
	bool thirdActive;

	void Awake() {

	}

	void Start () {
        uim = GameObject.Find("UserInterface").GetComponent<UserInterfaceManager>();
        OnLevelWasLoaded(GameState.GetLevel());
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

	void OnLevelWasLoaded(int level) {
		firstActive = false;
		secondActive = false;
		thirdActive = false;

		uim.HideCharacterInfos ();

		characters = new List<GameObject>();
		CheckSpawns();
	}

	public void PlayersTurnActivated() {
		ResetCharacterMovement ();
        ResetCharacterActions();
	}

	void CheckSpawns() {
		GameObject[] spawns = GameObject.FindGameObjectsWithTag ("Spawn");
		int spawnCount = 0;
		foreach (GameObject spawn in spawns) {

			spawnCount += 1;
			string charName;

			if (spawn.name == "Spawn1") {
				charName = "Character1";
				SpawnFirstCharacter(charName, (spawn.transform.position + new Vector3(0, 1, 0)), spawn.transform.rotation);
				firstActive = true;
				uim.ShowCharacterInfos(charName);
			} else if (spawn.name == "Spawn2") {
				charName = "Character2";
				SpawnSecondCharacter(charName, (spawn.transform.position + new Vector3(0, 1, 0)), spawn.transform.rotation);
				secondActive = true;
				uim.ShowCharacterInfos(charName);
			} else if (spawn.name == "Spawn3") {
				charName = "Character3";
				SpawnThirdCharacter(charName, (spawn.transform.position + new Vector3(0, 1, 0)), spawn.transform.rotation);
				thirdActive = true;
				uim.ShowCharacterInfos(charName);
			} else {
				spawnCount -= 1;
			}
		}
		EnterCharacter (characters[0]);
		GameState.amountOfCharacters = spawnCount;
	}

	void SpawnFirstCharacter(string name, Vector3 spawnSpot, Quaternion rotation) {
		GameObject character = LoadCharacterToScene (name, spawnSpot, rotation);
		
		AssignCharacterStats (character, 20, 10f);
	}

	void SpawnSecondCharacter(string name, Vector3 spawnSpot, Quaternion rotation) {
		GameObject character = LoadCharacterToScene (name, spawnSpot, rotation);
		
		AssignCharacterStats (character, 10, 50f);
	}

	void SpawnThirdCharacter(string name, Vector3 spawnSpot, Quaternion rotation) {
		GameObject character = LoadCharacterToScene (name, spawnSpot, rotation);
		
		AssignCharacterStats (character, 30, 30f);
	}

	GameObject LoadCharacterToScene(string name, Vector3 position, Quaternion rotation) {
		GameObject prefab = (GameObject) Resources.Load(name);
		GameObject character = ((GameObject) Instantiate (prefab, position, rotation));
        character.name = name;
        characters.Add(character);
        return character;
	}

	void AssignCharacterStats(GameObject character, int health, float maximumMovement) {
		character.GetComponent<MovementMeasurements> ().maximumMovement = maximumMovement;
		character.GetComponent<CharacterState> ().Init (health);
	}

	void ResetCharacterMovement() {
		foreach (GameObject character in characters) {
			character.GetComponent<MovementMeasurements>().ResetMovement();
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
				other.GetComponentInChildren<AudioListener> ().enabled = false;
				other.GetComponentInChildren<Camera> ().enabled = false;
				other.transform.FindChild("Sprite").gameObject.SetActive(true);
			}
		}
        character.BroadcastMessage("EnterCharacter");
		character.GetComponentInChildren<AudioListener> ().enabled = true;
		character.GetComponentInChildren<Camera> ().enabled = true;
		character.transform.FindChild("Sprite").gameObject.SetActive(false);
        GameState.activeCharacter = character;
	}

}
