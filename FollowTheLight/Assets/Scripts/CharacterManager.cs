using UnityEngine;
using System.Collections;

public class CharacterManager : MonoBehaviour {
	private PlayerController pc;

	public void deactivatePlayer() {
		pc.deactivate ();
	}

	public void activatePlayer() {
		pc.activate ();
	}
	
	void Start () {
		pc = GameObject.Find("Player").GetComponent<PlayerController>();
		pc.enterCharacter ();
	}

	void Update () {
	
	}
	
}
