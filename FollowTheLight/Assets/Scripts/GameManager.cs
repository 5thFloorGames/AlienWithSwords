using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	private CharacterManager cm;
	private EnemyManager em;
	private UserInterfaceManager uim;

	private bool playerTurn;
	
	void Start () {
		cm = gameObject.GetComponent<CharacterManager> ();
		uim = GameObject.Find ("UserInterface").GetComponent<UserInterfaceManager>();
		StartPlayerTurn ();
	}

	void Update () {
		if (Input.GetButton ("Cancel")) {
			QuitGame();
		}
		if (playerTurn) {
			if (Input.GetButton ("Submit")) {
				StartEnemyTurn();
			}
		}
	}

	public void QuitGame() {
		GameState.Reset ();
		Application.LoadLevel(0);
	}

	void StartEnemyTurn() {
		playerTurn = false;
		uim.ShowEnemyUI ();
		cm.deactivatePlayer ();
		StartCoroutine(enemyTurn());
	}

	void StartPlayerTurn() {
		cm.activatePlayer ();
		uim.HideEnemyUI ();
		playerTurn = true;
	}

	IEnumerator enemyTurn() {
		Debug.Log ("enemies' turn start");
		yield return new WaitForSeconds(5.0f);
		Debug.Log ("   player's turn again");
		StartPlayerTurn ();
	}
	
}
