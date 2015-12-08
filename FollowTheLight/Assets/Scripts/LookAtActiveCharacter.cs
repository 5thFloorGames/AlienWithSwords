using UnityEngine;
using System.Collections;

public class LookAtActiveCharacter : MonoBehaviour {
	
	void Start () {

	}

	void Update () {
		if (GameState.activeCharacter != null) {
			transform.LookAt (2*transform.position - GameState.activeCharacter.transform.position, Vector3.up);
		}
	}
}
