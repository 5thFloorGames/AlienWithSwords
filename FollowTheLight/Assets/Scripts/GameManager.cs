using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	private CharacterManager cm;
	private EnemyManager em;
	private UserInterfaceManager uim;

	void Start () {
        Cursor.visible = false;
        cm = gameObject.GetComponent<CharacterManager> ();
		em = gameObject.GetComponent<EnemyManager> ();
		uim = GameObject.Find ("UserInterface").GetComponent<UserInterfaceManager>();

		StartPlayerTurn ();
	}

	void Update () {
		if (Input.GetButton ("Cancel")) {
			QuitGame();
		}
		if (GameState.playersTurn) {
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
		Debug.Log ("enemies' turn start");
		GameState.playersTurn = false;
		uim.ShowEnemyUI ();
		em.TriggerEnemyActions ();

		StartCoroutine(EnemyTurn());
	}

	void StartPlayerTurn() {
		uim.HideEnemyUI ();
		GameState.playersTurn = true;
		Debug.Log ("   player's turn again");
	}

	IEnumerator EnemyTurn() {
		yield return new WaitForSeconds(5.0f);
		StartPlayerTurn ();
	}
	
}
