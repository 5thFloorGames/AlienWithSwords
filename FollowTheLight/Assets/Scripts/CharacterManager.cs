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
            SwitchCharacter(GetCharacterObject("Character1"));
		}
		if (Input.GetKeyDown (KeyCode.Alpha2) && secondActive) {
            SwitchCharacter(GetCharacterObject("Character2"));
		}
		if (Input.GetKeyDown(KeyCode.Alpha3) && thirdActive) {
            SwitchCharacter(GetCharacterObject("Character3"));
        }

        #if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.I) && GetCharacterObject("Character1") != null) {
            ResurrectCharacter(GetCharacterObject("Character1"), CharacterType.Character1);
        }
        if (Input.GetKeyDown(KeyCode.O) && GetCharacterObject("Character2") != null) {
            ResurrectCharacter(GetCharacterObject("Character2"), CharacterType.Character2);
        }
        if (Input.GetKeyDown(KeyCode.P) && GetCharacterObject("Character3") != null) {
            ResurrectCharacter(GetCharacterObject("Character3"), CharacterType.Character3);
        }
        #endif
    }



    void SwitchCharacter(GameObject character) {
        foreach (GameObject other in characters) {
            if (other != character) {
                other.BroadcastMessage("LeaveCharacter");
            }
        }
        character.BroadcastMessage("EnterCharacter");
        GameState.activeCharacter = character;
        uim.ActiveCharacterUI(character.name);
    }

    public void PlayersTurnActivated() {
        ResetCharacterMovement();
        ResetCharacterActions();
    }

    public void PlayerTurnEnded() {
        foreach (GameObject character in characters) {
            CharacterActionsSecond cas = character.GetComponentInChildren<CharacterActionsSecond>();
            if (cas != null) {
                cas.PlayerTurnEnded();
            }
        }
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

    public void ResurrectCharacter(GameObject character, CharacterType type) {
        if (type == CharacterType.Character1) {
            firstActive = true;
        } else if (type == CharacterType.Character2) {
            secondActive = true;
        } else if (type == CharacterType.Character3) {
            thirdActive = true;
        }
        character.BroadcastMessage("CharacterResurrected");
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
        GameObject[] spawns = GameObject.FindGameObjectsWithTag("Spawn");
        if (spawns.Length == 0) {
            Debug.Log("Create a spawn point in the scene to spawn a character (Resources). Name it Spawn1, Spawn2 or Spawn3.");
            return;
        }
        int spawnCount = 0;
        foreach (GameObject spawn in spawns) {

            spawnCount += 1;

            if (spawn.name == "Spawn1" && !firstActive) {
                firstActive = true;
                LoadCharacterToScene("Character1", (spawn.transform.position + new Vector3(0, 1, 0)), spawn.transform.rotation);
            } else if (spawn.name == "Spawn2" && !secondActive) {
                secondActive = true;
                LoadCharacterToScene("Character2", (spawn.transform.position + new Vector3(0, 1, 0)), spawn.transform.rotation);
            } else if (spawn.name == "Spawn3" && !thirdActive) {
                thirdActive = true;
                LoadCharacterToScene("Character3", (spawn.transform.position + new Vector3(0, 1, 0)), spawn.transform.rotation);
            } else {
                spawnCount -= 1;
                Debug.Log("A spawn point must always be named Spawn1, Spawn2 or Spawn3 based on the character you want it to spawn.");
                Debug.Log("You cannot spawn more than one of each character type.");
            }
        }

        GameState.amountOfCharacters = spawnCount;

        if (GetCharacterObject("Character1") != null) {
            SwitchCharacter(GetCharacterObject("Character1"));
            return;
        } else if (GetCharacterObject("Character2") != null) {
            SwitchCharacter(GetCharacterObject("Character2"));
            return;
        } else if (GetCharacterObject("Character3") != null) {
            SwitchCharacter(GetCharacterObject("Character3"));
            return;
        }

    }

	void LoadCharacterToScene(string name, Vector3 position, Quaternion rotation) {
		GameObject prefab = (GameObject) Resources.Load(name);
		GameObject character = ((GameObject) Instantiate (prefab, position, rotation));
        character.name = name;
        characters.Add(character);
        if (uim != null) {
            uim.ShowCharacterInfos(name);
        }
        character.GetComponent<CharacterState>().Init(gameObject);
    }



	GameObject GetCharacterObject(string name) {
		foreach (GameObject character in characters) {
			if (character.name == name) {
				return character;
			}
		}
		return null;
	}

    void SetInitializedToFalse() {
        initialized = false;
    }

}
