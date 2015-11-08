using UnityEngine;
using System.Collections;

public class UserInterfaceManager : MonoBehaviour {

	private GameObject enemyTurn;
	private GameObject crosshairs;
	
	void Start () {
		enemyTurn = (GameObject)transform.Find ("EnemyTurn").gameObject;
		crosshairs = (GameObject)transform.Find ("Crosshairs").gameObject;
	}

	void Update () {
		
	}

	public void ShowEnemyUI() {
		enemyTurn.SetActive (true);
	}

	public void HideEnemyUI() {
		enemyTurn.SetActive (false);
	}
}
