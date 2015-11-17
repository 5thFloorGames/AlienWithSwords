using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UserInterfaceManager : MonoBehaviour {

	GameObject enemyTurnUI;
    GameObject levelCompletedUI;
	GameObject characterPanel;
    //GameObject crosshairs;

    Dictionary <string, Image> healthMeters;
	Dictionary <string, Image> distanceMeters;
    Dictionary <string, Image> actionMeters;
	
	void Awake () {

		DontDestroyOnLoad (gameObject);

		distanceMeters = new Dictionary<string, Image>();
		healthMeters = new Dictionary<string, Image> ();
        actionMeters = new Dictionary<string, Image> ();

		enemyTurnUI = (GameObject)transform.Find ("EnemyTurn").gameObject;
		levelCompletedUI = (GameObject)transform.Find ("LevelCompleted").gameObject;
		//crosshairs = (GameObject)transform.Find ("Crosshairs").gameObject;

		characterPanel = (GameObject)transform.Find ("CharacterPanel").gameObject;

		foreach (Transform charinf in characterPanel.transform) {
			GameObject obj = charinf.FindChild("DistanceMeter").gameObject;
			Image img = obj.GetComponent<Image>();
			distanceMeters.Add(charinf.name, img);

			obj = charinf.FindChild("HealthMeter").gameObject;
			img = obj.GetComponent<Image>();
			healthMeters.Add(charinf.name, img);

            obj = charinf.FindChild("ActionMeter").gameObject;
            img = obj.GetComponent<Image>();
            actionMeters.Add(charinf.name, img);
        }

	}

	void Update () {
		
	}

	void OnLevelWasLoaded(int level) {
		if (level == 0) {
			Destroy (gameObject);
		}
	}

	public void ShowEnemyUI() {
		enemyTurnUI.SetActive (true);
	}

	public void HideEnemyUI() {
		enemyTurnUI.SetActive (false);
	}

	public void ShowLevelCompletedUI() {
		levelCompletedUI.SetActive (true);
	}

	public void HideLevelCompletedUI() {
		levelCompletedUI.SetActive (false);
	}

	public void UpdateDistanceMeter(string characterName, float distance, float maximum) {
		// Debug.Log (characterName + " has moved: " + distance);
		distanceMeters [characterName].fillAmount =  (1 - distance / maximum);
	}

	public void UpdateHealthMeter(string characterName, float currentHealth, float maximum) {
		healthMeters [characterName].fillAmount = (currentHealth / maximum);
	}

    public void UpdateActionMeter(string characterName, float actions, float maximum) {
        actionMeters[characterName].fillAmount = (actions / maximum);
    }
}
