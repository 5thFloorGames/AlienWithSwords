using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterManager : MonoBehaviour {
	List<GameObject> characters;
    UserInterfaceManager uim;
    bool firstLoaded;
    bool secondLoaded;
    bool thirdLoaded;

	void Awake() {
		firstLoaded = false;
		secondLoaded = false;
		thirdLoaded = false;
	}

	void Start () {
        uim = GameObject.Find("UserInterface").GetComponent<UserInterfaceManager>();

        characters = new List<GameObject>();

        OnLevelWasLoaded(GameState.GetLevel());
	}

	void Update () {
		if (Input.GetKeyDown (KeyCode.Alpha1) && firstLoaded) {
			EnterCharacter(GetCharacterObject("Character1"));
		}
		if (Input.GetKeyDown (KeyCode.Alpha2) && secondLoaded) {
			EnterCharacter(GetCharacterObject("Character2"));
		}
        if (Input.GetKeyDown(KeyCode.Alpha3) && thirdLoaded) {
            EnterCharacter(GetCharacterObject("Character3"));
        }
    }

	void OnLevelWasLoaded(int level) {
        if (characters != null) {
            CheckCharacters(level);
            AssignCharacterSpawningPoints();
		}
	}

	public void PlayersTurnActivated() {
		ResetCharacterMovement ();
        ResetCharacterActions();
	}

    void CheckCharacters(int level) {
        if (!firstLoaded) {
            CreateFirstCharacter();
            firstLoaded = true;
			GameState.amountOfCharacters = 1;
			uim.ShowCharacterInfos(GameState.amountOfCharacters);
        }
        if (!secondLoaded && level > 1) {
            CreateSecondCharacter();
            secondLoaded = true;
			GameState.amountOfCharacters = 2;
			uim.ShowCharacterInfos(GameState.amountOfCharacters);
        }
        if (!thirdLoaded && level > 1) {
            CreateThirdCharacter();
            thirdLoaded = true;
			GameState.amountOfCharacters = 3;
			uim.ShowCharacterInfos(GameState.amountOfCharacters);
        }
    }

	void CreateFirstCharacter() {
		string name = "Character1";
		GameObject character = LoadCharacterToScene (name, new Vector3(0, 1, 0), Quaternion.identity);
		
		AssignCharacterStats (character, 10, 10f);
		EnterCharacter (character);
	}

	void CreateSecondCharacter() {
		string name = "Character2";	
		GameObject character = LoadCharacterToScene (name, new Vector3(0, 1, 0), Quaternion.identity);
		AssignCharacterStats (character, 10, 10f);
	}

    void CreateThirdCharacter() {
        string name = "Character3";
        GameObject character = LoadCharacterToScene(name, new Vector3(0, 1, 0), Quaternion.identity);
        AssignCharacterStats(character, 10, 10f);
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

    void AssignCharacterSpawningPoints() {
        if (firstLoaded) {
            if (GameState.GetLevel() > 0) {
                GetCharacterObject("Character1").transform.position = new Vector3(0, 1, 0);
            }
        }

       if (secondLoaded) {
			GetCharacterObject("Character2").transform.position = new Vector3(1, 1, 0);
       }

       if (thirdLoaded) {
			GetCharacterObject("Character3").transform.position = new Vector3(2, 1, 0);
       }
    }

}
