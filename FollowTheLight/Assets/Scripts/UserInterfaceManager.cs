using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UserInterfaceManager : MonoBehaviour {

	GameObject enemyTurnUI;
    GameObject levelCompletedUI;
    GameObject levelFailedUI;
	GameObject characterPanel;
    //GameObject crosshairs;

    Dictionary <string, Image> healthMeters;
    Dictionary <string, Text> healthTexts;
	Dictionary <string, Image> distanceMeters;
    Dictionary <string, Text> distanceTexts;
    Dictionary <string, Transform> actionPoints;
	
	void Awake () {

		DontDestroyOnLoad (gameObject);

        healthMeters = new Dictionary<string, Image>();
        healthTexts = new Dictionary<string, Text>();
        distanceMeters = new Dictionary<string, Image>();
        distanceTexts = new Dictionary<string, Text>();
        actionPoints = new Dictionary<string, Transform> ();

		enemyTurnUI = (GameObject)transform.Find ("EnemyTurn").gameObject;
		levelCompletedUI = (GameObject)transform.Find ("LevelCompleted").gameObject;
        levelFailedUI = (GameObject)transform.Find("LevelFailed").gameObject;

		characterPanel = (GameObject)transform.Find ("CharacterPanel").gameObject;

		foreach (Transform charinf in characterPanel.transform) {
			GameObject obj = charinf.FindChild("HealthMeter").gameObject;
            Image img = obj.GetComponent<Image>();
			healthMeters.Add(charinf.name, img);

            Text text = charinf.FindChild("HealthText").gameObject.GetComponent<Text>();
            healthTexts.Add(charinf.name, text);

            obj = charinf.FindChild("DistanceMeter").gameObject;
            img = obj.GetComponent<Image>();
            distanceMeters.Add(charinf.name, img);

            text = charinf.FindChild("DistanceText").gameObject.GetComponent<Text>();
            distanceTexts.Add(charinf.name, text);

            obj = charinf.FindChild("ActionPoints").gameObject;
            actionPoints.Add(charinf.name, obj.transform);
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
        if (GameState.GetLevel() == 1) {
            TutorialMovementRestored();
            TutorialActionsRestored();
        }
	}

	public void HideEnemyUI() {
		enemyTurnUI.SetActive (false);
	}

    public void ShowLevelFailedUI() {
        levelFailedUI.SetActive(true);
    }

    public void HideLevelFailedUI() {
        levelFailedUI.SetActive(false);
    }

	public void ShowLevelCompletedUI() {
		levelCompletedUI.SetActive (true);
	}

	public void HideLevelCompletedUI() {
		levelCompletedUI.SetActive (false);
	}

    public void UpdateHealthMeter(string characterName, float currentHealth, float maximum) {
        healthMeters[characterName].fillAmount = (currentHealth / maximum);
        healthTexts[characterName].text = currentHealth.ToString();
    }

    public void UpdateDistanceMeter(string characterName, float distance, float maximum) {
        if (GameState.GetLevel() == 1) {
            TutorialOom(distance, maximum);
        }

		distanceMeters[characterName].fillAmount =  (1 - distance / maximum);
        distanceTexts[characterName].text = (maximum - distance).ToString("F1");
	}

    public void UpdateActionPoints(string characterName, int actions, int maximum) {
        int counter = 0;
        foreach (Transform actionPoint in actionPoints[characterName]) {
            if (counter < actions) {
                actionPoint.gameObject.SetActive(true);
            } else {
                actionPoint.gameObject.SetActive(false);
            }
            counter += 1;
        }
        if (GameState.GetLevel() == 1) {
            TutorialOoa(actions);
        }
    }

	public void HideCharacterInfos() {
		foreach (Transform charinf in characterPanel.transform) {
			charinf.gameObject.SetActive(false);
		}
	}

    public void ShowCharacterInfos(string characterName) {
        foreach (Transform charinf in characterPanel.transform) {
            if (charinf.name == characterName) {
                charinf.gameObject.SetActive(true);
            }
        }
    }

    public void ActiveCharacterUI(string characterName) {
        foreach (Transform charinf in characterPanel.transform) {
            if (charinf.name == characterName) {
                charinf.localScale = new Vector3(1.1f, 1.1f, 1.1f);
            } else {
                charinf.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            }
        }
    }

    void TutorialMovementRestored() {
       TutorialTextHandler tth = FindObjectOfType<TutorialTextHandler>();
       tth.MovementRestored();
    }

    void TutorialOom(float distance, float maximum) {
        if (distance == maximum) {
            TutorialTextHandler tth = FindObjectOfType<TutorialTextHandler>();
            tth.OutOfMovementInform();
        }
    }

    void TutorialActionsRestored() {
        TutorialTextHandler tth = FindObjectOfType<TutorialTextHandler>();
        tth.ActionsRestored();
    }

    void TutorialOoa(int actions) {
        if (actions == 1) {
            TutorialTextHandler tth = FindObjectOfType<TutorialTextHandler>();
            tth.OutOfActionsInform();
            tth.ClearText();
        }
    }
}
