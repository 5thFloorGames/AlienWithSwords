using UnityEngine;
using System.Collections;

public class LookAtActiveCharacter : MonoBehaviour {
	
	void Start () {

	}

	void Update () {
		transform.LookAt (GameState.activeCharacter.transform, Vector3.up);
	}

}
