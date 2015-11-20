using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterManager : MonoBehaviour {

	static CharacterManager _instance;
	
	public static CharacterManager Base {
		get {
			if (_instance == null) _instance = FindObjectOfType<CharacterManager>();
			if (_instance == null) Debug.Log("WARNING: No CharacterManager in the scene!!!");
			return _instance;
		} set {
			_instance = value; }
	}

	public List<GameObject> characters;
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
		if (spawns.Length == 0) {
			Debug.Log ("Create a spawn point in the scene to spawn a character (Resources). Name it Spawn1, Spawn2 or Spawn3.");
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
		AssignCharacterStats (character, 20, 10f);
	}

	void SpawnSecondCharacter(string name, Vector3 spawnSpot, Quaternion rotation) {
		GameObject character = LoadCharacterToScene (name, spawnSpot, rotation);
		secondActive = true;
		AssignCharacterStats (character, 10, 50f);
	}

	void SpawnThirdCharacter(string name, Vector3 spawnSpot, Quaternion rotation) {
		GameObject character = LoadCharacterToScene (name, spawnSpot, rotation);
		thirdActive = true;
		AssignCharacterStats (character, 30, 30f);
	}

	GameObject LoadCharacterToScene(string name, Vector3 position, Quaternion rotation) {
		GameObject prefab = (GameObject) Resources.Load(name);
		GameObject character = ((GameObject) Instantiate (prefab, position, rotation));
        character.name = name;
        characters.Add(character);
		uim.ShowCharacterInfos(name);
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
			}
		}
        character.BroadcastMessage("EnterCharacter");
        GameState.activeCharacter = character;
	}

}
