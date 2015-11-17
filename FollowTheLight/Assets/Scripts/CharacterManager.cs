using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterManager : MonoBehaviour {
	FPSController fpsc;
	List<GameObject> characters;
	int theActiveCharacter;
	bool playersTurn;
	MovementMeasurements firstMm;

	void Start () {

		characters = new List<GameObject>();
		//create character from prefab
		GameObject firstCharPrefab = (GameObject) Resources.Load("Character1");
		GameObject firstChar = (GameObject) Instantiate (firstCharPrefab, new Vector3(0, 1, 0), Quaternion.identity);
		firstChar.name = "Character1";
		//GameObject firstChar = GameObject.Find ("Character1");
		characters.Add (firstChar);
		firstChar.GetComponent<FPSController> ().enterCharacter ();
		firstMm = firstChar.GetComponent<MovementMeasurements> ();
		firstMm.enterCharacter ();
		firstMm.maximumMovement = 100;
		CharacterState firstCs = firstChar.GetComponent<CharacterState> ();
		firstCs.Init(100);
	}

	void Update () {
	
	}

	public void PlayersTurnActivated() {
		firstMm.ResetMovement ();
	}

}
