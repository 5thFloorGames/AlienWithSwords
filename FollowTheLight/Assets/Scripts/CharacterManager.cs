using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterManager : MonoBehaviour {
	FPSController fpsc;
	List<GameObject> characters;
	int theActiveCharacter;
	MovementMeasurements firstMm;

	void Start () {

		characters = new List<GameObject>();
		CreateFirstCharacter ();
		CreateSecondCharacter ();
	}

	void Update () {
		if (Input.GetKeyDown (KeyCode.Alpha1)) {
			EnterCharacter(GetCharacterObject("Character1"));
		}
		if (Input.GetKeyDown (KeyCode.Alpha2)) {
			EnterCharacter(GetCharacterObject("Character2"));
		}
	}

	void OnLevelWasLoaded(int level) {
		if (characters != null) {
			ResetCharacterMovement ();
		}
	}

	public void PlayersTurnActivated() {
		ResetCharacterMovement ();
	}

	void CreateFirstCharacter() {
		string name = "Character1";

		GameObject character = loadCharacterToScene (name, new Vector3(0, 1, 0), Quaternion.identity);
		character.name = name;

		characters.Add (character);

		AssignCharacterStats (character, 100, 100f);
		EnterCharacter (character);
	}

	void CreateSecondCharacter() {
		string name = "Character2";
		
		GameObject character = loadCharacterToScene (name, new Vector3(1, 1, 1), Quaternion.identity);
		character.name = name;

		characters.Add (character);
		
		AssignCharacterStats (character, 100, 100f);
	}

	GameObject loadCharacterToScene(string name, Vector3 position, Quaternion rotation) {
		GameObject prefab = (GameObject) Resources.Load(name);
		return ((GameObject) Instantiate (prefab, position, rotation));
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
				other.GetComponent<FPSController> ().leaveCharacter ();
				other.GetComponent<MovementMeasurements> ().leaveCharacter ();
				other.GetComponentInChildren<AudioListener> ().enabled = false;
				character.GetComponentInChildren<Camera> ().enabled = false;
			}
		}
		character.GetComponent<FPSController> ().enterCharacter ();
		character.GetComponent<MovementMeasurements> ().enterCharacter ();
		character.GetComponentInChildren<AudioListener> ().enabled = true;
		character.GetComponentInChildren<Camera> ().enabled = true;
	}

}
