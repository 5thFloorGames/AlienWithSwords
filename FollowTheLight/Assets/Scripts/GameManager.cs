using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	private CharacterManager cm;
	private EnemyManager em;
	private UserInterfaceManager uim;

	private bool playersTurn;

	
	void Start () {
		cm = gameObject.GetComponent<CharacterManager> ();
		uim = GameObject.Find ("UserInterface").GetComponent<UserInterfaceManager>();
		StartPlayerTurn ();
	}

	void Update () {
		if (Input.GetButton ("Cancel")) {
			QuitGame();
		}
		if (playersTurn) {
			if (Input.GetButton ("Submit")) {
				StartEnemyTurn();
			}
		}
	}

	public void QuitGame() {
		Application.LoadLevel(0);
	}

	void StartEnemyTurn() {
		playersTurn = false;
		uim.ShowEnemyUI ();
		cm.deactivatePlayer ();
		StartCoroutine(enemyTurn());
	}

	void StartPlayerTurn() {
		cm.activatePlayer ();
		uim.HideEnemyUI ();
		playersTurn = true;
	}

	IEnumerator enemyTurn() {
		Debug.Log ("enemies' turn start");
		yield return new WaitForSeconds(5.0f);
		Debug.Log ("   player's turn again");
		StartPlayerTurn ();
	}
	
}
