using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UserInterfaceManager : MonoBehaviour {

	GameObject enemyTurnUI;
	GameObject crosshairs;
	GameObject characterPanel;

	Dictionary <string, Image> distanceMeters;
	
	void Awake () {
		distanceMeters = new Dictionary<string, Image>();

		enemyTurnUI = (GameObject)transform.Find ("EnemyTurn").gameObject;
		crosshairs = (GameObject)transform.Find ("Crosshairs").gameObject;

		characterPanel = (GameObject)transform.Find ("CharacterPanel").gameObject;

		foreach (Transform charinf in characterPanel.transform) {
			GameObject obj = charinf.FindChild("DistanceMeter").gameObject;
			Image meterimg = obj.GetComponent<Image>();
			distanceMeters.Add(charinf.name, meterimg);
			Debug.Log (meterimg);
		}

		//distanceMeter = (GameObject)transform.Find ("DistanceMeter").gameObject;
	}

	void Update () {
		
	}

	public void ShowEnemyUI() {
		enemyTurnUI.SetActive (true);
	}

	public void HideEnemyUI() {
		enemyTurnUI.SetActive (false);
	}

	public void UpdateDistanceMeter(string characterName, float distance, float maximum) {
		Debug.Log (characterName + " has moved: " + distance);
		distanceMeters [characterName].fillAmount = ( 1- (distance/maximum));
	}
}
