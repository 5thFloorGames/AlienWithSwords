using UnityEngine;
using System.Collections;

public class UserInterfaceManager : MonoBehaviour {

	private GameObject enemyTurnUI;
	private GameObject crosshairs;
	
	void Awake () {
		enemyTurnUI = (GameObject)transform.Find ("EnemyTurn").gameObject;
		crosshairs = (GameObject)transform.Find ("Crosshairs").gameObject;
	}

	void Update () {
		
	}

	public void ShowEnemyUI() {
		enemyTurnUI.SetActive (true);
	}

	public void HideEnemyUI() {
		enemyTurnUI.SetActive (false);
	}
}
