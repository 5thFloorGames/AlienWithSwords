using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterManager : MonoBehaviour {
	FPSController fpsc;
	List<GameObject> characters;
	int theActiveCharacter;
	bool playersTurn;

	void Start () {

		characters = new List<GameObject>();
		//create character from prefab
		//GameObject firstCharPrefab = (GameObject) Resources.Load("Character1");
		//GameObject firstChar = (GameObject) Instantiate (firstCharPrefab, new Vector3(0, 1, 0), Quaternion.identity);
		//firstChar.name = "Character1";
		GameObject firstChar = GameObject.Find ("Character1");
		characters.Add (firstChar);
		firstChar.GetComponent<FPSController> ().enterCharacter ();
		CharacterState firstCs = firstChar.GetComponent<CharacterState> ();
		firstCs.health = 100;
	}

	void Update () {
	
	}

}
