using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UserInterfaceManager : MonoBehaviour {

	GameObject enemyTurnUI;
    GameObject levelCompletedUI;
    GameObject levelFailedUI;
	GameObject characterPanel;

    Dictionary <string, Image> healthMeters;
    Dictionary <string, Text> healthTexts;
	Dictionary <string, Image> distanceMeters;
    Dictionary <string, Text> distanceTexts;
    Dictionary <string, Transform> actionPoints;
	Dictionary <string, GameObject> deadMarks;
	
	void Awake () {

		DontDestroyOnLoad (gameObject);

        healthMeters = new Dictionary<string, Image>();
        healthTexts = new Dictionary<string, Text>();
        distanceMeters = new Dictionary<string, Image>();
        distanceTexts = new Dictionary<string, Text>();
        actionPoints = new Dictionary<string, Transform> ();
		deadMarks = new Dictionary<string, GameObject> ();

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

			obj = charinf.FindChild("CharacterDeadMarker").gameObject;
			deadMarks.Add(charinf.name, obj);
			DisableAllDeadMarks();
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

	public void DamageTakenUIUpdate(string charName) {
		//FlashMovementColor (charName, 2 );
		//FlashActionPointsColor (charName, 2);
		FlashHealthColor (charName, 2);
	}

	public void DisableAllDeadMarks() {
		foreach (KeyValuePair <string, GameObject> mark in deadMarks) {
			mark.Value.SetActive(false);
		}
	}

	public void CharacterDiedUIUpdate(string name) {
		deadMarks [name].SetActive (true);
	}

	public void CharacterAliveUIUpdate(string name) {
		deadMarks [name].SetActive (false);
	}

    public void UpdateHealthMeter(string characterName, float currentHealth, float maximum) {
        if (currentHealth == maximum) {
            FlashHealth(characterName, 1);
        }

        healthMeters[characterName].fillAmount = (currentHealth / maximum);
        healthTexts[characterName].text = currentHealth.ToString();
    }

    public void UpdateDistanceMeter(string characterName, float distance, float maximum) {
        if (GameState.GetLevel() == 1) {
            TutorialOom(distance, maximum);
        }
        if (distance == 0) {
            FlashMovement(characterName, 1);
        }
		distanceMeters[characterName].fillAmount =  (1 - distance / maximum);
        distanceTexts[characterName].text = (maximum - distance).ToString("F1");
	}

    public void UpdateActionPoints(string characterName, int actions, int maximum) {

        if (GameState.GetLevel() == 1) {
            TutorialOoa(actions, maximum);
        }
        if (actions == maximum) {
            FlashActionPoints(characterName, 1);
        }

        foreach (Transform actionPoint in actionPoints[characterName]) {
            if (actions == 0) {
                actionPoint.gameObject.SetActive(false);
            } else if (actions > 0 && actionPoint.name == "Point1") {
                actionPoint.gameObject.SetActive(true);
            } else if (actions > 1 && actionPoint.name == "Point2") {
                actionPoint.gameObject.SetActive(true);
            } else if (actions > 3) {
                actionPoint.gameObject.SetActive(true);
            }
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
            string name = charinf.name;
            float alphaValue;
            if (name != characterName) {
                alphaValue = 0.3f;
            } else {
                alphaValue = 1.0f;
            }
            charinf.FindChild("CharacterImage").GetComponent<Image>().CrossFadeAlpha(alphaValue, 0.1f, true);
            charinf.FindChild("CharacterName").GetComponent<Text>().CrossFadeAlpha(alphaValue, 0.1f, true);
            distanceMeters[name].CrossFadeAlpha(alphaValue, 0.1f, true);
            healthMeters[name].CrossFadeAlpha(alphaValue, 0.1f, true);
            Transform apParent = charinf.FindChild("ActionPoints");
            foreach (Transform ap in apParent) {
                Image img = ap.GetComponent<Image>();
                Color clr = img.color;
                clr.a = alphaValue;
                img.color = clr;
            }
        }
    }



	public void FlashMovementColor (string characterName, int times) {
		Image meter = distanceMeters[characterName];
		Text txt = distanceTexts[characterName];
		
		StartCoroutine(FlashImageColor(meter, new Color(1, 1, 1), times));
		StartCoroutine(FlashTextColor(txt, times));
	}
	
	public void FlashActionPointsColor (string characterName, int times) {
		foreach (Transform actionPoint in actionPoints[characterName]) {
			StartCoroutine(FlashImageColor(actionPoint.GetComponent<Image>(), new Color(1, 1, 1), times));
		}
	}
	
	public void FlashHealthColor (string characterName, int times) {
		Image meter = healthMeters[characterName];
		Text txt = healthTexts[characterName];
		
		StartCoroutine(FlashImageColor(meter,new Color(0, 0, 0), times));
		StartCoroutine(FlashTextColor(txt, times));
	}
	
	IEnumerator FlashImageColor (Image img, Color originalColor, int times) {
		int i = 0;
		while (i < times) {
			yield return new WaitForSeconds(0.1f);
			img.color = new Color (1.0f, 0.0f, 0.0f);
			yield return new WaitForSeconds(0.2f);
			img.color = originalColor;
			yield return new WaitForSeconds(0.1f);
			i += 1;
		}
	}
	
	IEnumerator FlashTextColor (Text txt, int times) {
		int i = 0;
		while (i < times) {
			txt.CrossFadeColor(new Color(1.0f, 0.0f, 0.0f), 0.1f, false, false);
			yield return new WaitForSeconds(0.1f);
			txt.CrossFadeColor(new Color(1.0f, 1.0f, 1.0f), 0.1f, false, false);
			yield return new WaitForSeconds(0.05f);
			i += 1;
		}
	}



    public void FlashMovement (string characterName, int times) {
        if (GameState.activeCharacter == null || characterName != GameState.activeCharacter.name) {
            return;
        }
        Image meter = distanceMeters[characterName];
        Text txt = distanceTexts[characterName];
        
        StartCoroutine(FlashImage(meter, times));
        StartCoroutine(FlashText(txt, times));
    }

    public void FlashActionPoints (string characterName, int times) {
        if (GameState.activeCharacter == null || characterName != GameState.activeCharacter.name) {
            return;
        }
        foreach (Transform actionPoint in actionPoints[characterName]) {
            StartCoroutine(FlashImage(actionPoint.GetComponent<Image>(), times));
        }
    }

    public void FlashHealth (string characterName, int times) {
        if (GameState.activeCharacter == null || characterName != GameState.activeCharacter.name) {
            return;
        }
        Image meter = healthMeters[characterName];
        Text txt = healthTexts[characterName];

        StartCoroutine(FlashImage(meter, times));
        StartCoroutine(FlashText(txt, times));
    }

    IEnumerator FlashImage (Image img, int times) {
        yield return new WaitForSeconds(0.25f);
        int i = 0;
        while (i < times) {
            img.CrossFadeAlpha(0.0f, 0.2f, false);
            yield return new WaitForSeconds(0.2f);
            img.CrossFadeAlpha(1.0f, 0.2f, false);
            yield return new WaitForSeconds(0.2f);
            i += 1;
        }
    }

    IEnumerator FlashText(Text txt, int times) {
        yield return new WaitForSeconds(0.25f);
        int i = 0;
        while (i < times) {
            txt.CrossFadeAlpha(0.0f, 0.2f, false);
            yield return new WaitForSeconds(0.2f);
            txt.CrossFadeAlpha(1.0f, 0.2f, false);
            yield return new WaitForSeconds(0.2f);
            i += 1;
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

    void TutorialOoa(int actions, int maximum) {
        TutorialTextHandler tth = FindObjectOfType<TutorialTextHandler>();
        tth.ClearText();
        if (actions == 0) {
            tth.OutOfActionsInform();
        }
    }
}
