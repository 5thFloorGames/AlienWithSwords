using UnityEngine;
using System.Collections;

public class LookAtActiveCharacter : MonoBehaviour {
	
	void Start () {

	}

	void Update () {
		if (GameState.activeCharacter != null) {
			transform.LookAt (GameState.activeCharacter.transform, Vector3.up);
		}
	}

}
