using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AnnouncementManager : MonoBehaviour {

    GameObject guide;
    GameObject agreementForm;

    Image levelFade;
    Image dyingFade;
    Image damageFade;

	public float announcementDisplayTime;
    public float combatLogDisplayTime;

	Text combatLog;
	string combatTextNow;
	string combatTextInc;
    float mostRecentCombatEvent;

	Text announcement;
	string announcementNow;
	string announcementInc;
    float mostRecentAnnouncement;

    bool preventAdditionalAnnouncements;
    bool charactersAreEnemies;

    int startTutorialPageNumber;
    bool agreementTriggered;
    bool startTutorialTriggered;

	void Start () {
        guide = transform.FindChild("Guide").gameObject;
        agreementForm = transform.FindChild("UserAgreementForm").gameObject;
        Transform tf = transform.FindChild("Announcements");
		combatLog = tf.FindChild("MaskForCombatLog").GetComponentInChildren<Text>();
		announcement = tf.FindChild("MaskForAnnouncement").GetComponentInChildren<Text>();
        levelFade = tf.FindChild("LevelFade").GetComponent<Image>();
        dyingFade = tf.FindChild("DyingFade").GetComponent<Image>();
        damageFade = tf.FindChild("DamageFade").GetComponent<Image>();

        ActivateDyingAndDamage();
		ResetCombatLog ();
		ResetAnnouncements ();

        startTutorialPageNumber = 0;
	}

	void Update () {
		if (announcementNow.Length > 0) {
			CheckAnnouncementTimers();
		}
        if (combatTextNow.Length > 0) {
            CheckCombatLogTimers();
        }
        if (Input.GetButtonDown("Submit") && GameState.playersTurn) {
            ToggleGuide();
        }
	}

	void OnLevelWasLoaded() {
		preventAdditionalAnnouncements = false;
	}

    public void InformLevelObjective(LevelObjective obj) {
        if (obj == LevelObjective.KillYourCharacters) {
            charactersAreEnemies = true;
        } else {
            charactersAreEnemies = false;
        }
        if (GameState.GetLevel() > 1) {
            GetComponent<AudioSource>().enabled = true;
        }
    }

	public void ResetCombatLog() {
		combatTextInc = "";
		combatTextNow = "";
		combatLog.text = "";
	}

	public void ResetAnnouncements() {
		announcementInc = "";
		announcementNow = "";
		announcement.text = "";
	}



    // Story and how to play game start functions

    public void GameStartTriggered() {
        Cursor.visible = true;
        if (!startTutorialTriggered) {
            startTutorialTriggered = true;
            transform.FindChild("Story").gameObject.SetActive(true);
        }
    }

    public void GameStartNextPage() {
        if (startTutorialPageNumber == 0) {
            transform.FindChild("Story").gameObject.SetActive(false);
            transform.FindChild("HowToPlay").gameObject.SetActive(true);
            transform.FindChild("HowToPlay").FindChild("text_page1").gameObject.SetActive(true);
        } else if (startTutorialPageNumber == 1){
            transform.FindChild("HowToPlay").FindChild("text_page1").gameObject.SetActive(false);
            transform.FindChild("HowToPlay").FindChild("text_page2").gameObject.SetActive(true);
        } else if (startTutorialPageNumber == 2) {
            transform.FindChild("HowToPlay").FindChild("text_page2").gameObject.SetActive(false);
            transform.FindChild("HowToPlay").FindChild("text_page3").gameObject.SetActive(true);
        } else if (startTutorialPageNumber == 3) {
            transform.FindChild("HowToPlay").FindChild("text_page3").gameObject.SetActive(false);
            LeaveStartingTutorial();
        }
        startTutorialPageNumber += 1;
    }

    void LeaveStartingTutorial() {
#if !UNITY_EDITOR
            Cursor.visible = false;
#endif
        GetComponent<AudioSource>().enabled = true;
        transform.FindChild("Story").gameObject.SetActive(false);
        transform.FindChild("HowToPlay").gameObject.SetActive(false);
        Invoke("ActivatePlayerTurnAgain", 0.5f);
    }



    // Agreement form functions

    public void UserAgreementFormTriggered() {
        if (agreementTriggered) {
            return;
        }

        agreementTriggered = true;
#if !UNITY_EDITOR
            Cursor.visible = true;
#endif
        GameState.playersTurn = false;

        agreementForm.SetActive(true);
        agreementForm.transform.FindChild("AgreementText1").gameObject.SetActive(true);
        agreementForm.transform.FindChild("AgreementText2").gameObject.SetActive(false);
        agreementForm.transform.FindChild("NextPage").gameObject.SetActive(true);
        agreementForm.transform.FindChild("IAgree").gameObject.SetActive(false);
    }

    public void UserAgreementFormNextPage() {
        agreementForm.transform.FindChild("AgreementText1").gameObject.SetActive(false);
        agreementForm.transform.FindChild("AgreementText2").gameObject.SetActive(true);
        agreementForm.transform.FindChild("NextPage").gameObject.SetActive(false);
        agreementForm.transform.FindChild("IAgree").gameObject.SetActive(true);
    }

    public void UserAgreementFormAgreed() {
        #if !UNITY_EDITOR
            Cursor.visible = false;
        #endif
        agreementForm.SetActive(false);
        Invoke("ActivatePlayerTurnAgain", 0.5f);
    }

    void ActivatePlayerTurnAgain() {
        GameState.playersTurn = true;
    }



    // Guide

    void ToggleGuide() {
        if (guide.activeSelf) {
            guide.SetActive(false);
        } else {
            guide.SetActive(true);
        }
    }



    // Fading effects

    public void LevelLoadedFader() {
        if (levelFade == null) {
            levelFade = transform.FindChild("Announcements").FindChild("LevelFade").GetComponent<Image>();
        }
        levelFade.gameObject.SetActive(true);
        levelFade.canvasRenderer.SetAlpha(1.0f);
        levelFade.CrossFadeAlpha(0.0f, 1.0f, false);
    }

    public void DyingFader() {
        StartCoroutine(DyingFading());
    }

    public void RemoveDyingFader() {
        if (dyingFade != null) {
            dyingFade.canvasRenderer.SetAlpha(0.0f);
            dyingFade.CrossFadeAlpha(0.0f, 0.0f, false);
        }
    }

    public void ActiveCharacterDamagedFader() {
        StartCoroutine(DamageFading());
    }

    void ActivateDyingAndDamage() {
        damageFade.gameObject.SetActive(true);
        damageFade.canvasRenderer.SetAlpha(0.0f);
        dyingFade.gameObject.SetActive(true);
        dyingFade.canvasRenderer.SetAlpha(0.0f);
    }

    public void EndGameFading(float timer) {
        dyingFade.CrossFadeAlpha(1.0f, timer, false);
    }

    IEnumerator DyingFading() {
        dyingFade.CrossFadeAlpha(0.8f, 2.0f, false);
        yield return new WaitForSeconds(2.0f);
    }

    IEnumerator DamageFading() {
        damageFade.CrossFadeAlpha(0.6f, 0.2f, false);
        yield return new WaitForSeconds(0.2f);
        damageFade.CrossFadeAlpha(0.0f, 0.2f, false);
    }





    // Combat log information

    public void CharacterHealedACharacter(CharacterType type, int amount, CharacterType sourceType) {
        combatTextInc += "\n" + GetCharacterName(sourceType) + " restored " + amount + " health to " + GetCharacterName(type) + ".";
        StartCoroutine(GenerateCombatLog());
    }

    public void CharacterTriedToHealFullHealth(CharacterType type, CharacterType sourceType) {
        combatTextInc += "\n" + GetCharacterName(sourceType) + " tried to heal " + GetCharacterName(type) + " but he was already at full health.";
        StartCoroutine(GenerateCombatLog());
    }

    public void CharacterTookDamageFromEnemy(CharacterType type, int damageAmount, EnemyType sourceType) {

        combatTextInc += "\n" + FirstLetterToUpper(GetEnemyArticle(sourceType)) + " " + sourceType + " dealt " + damageAmount + " damage to " + GetCharacterName(type) + ".";
        StartCoroutine(GenerateCombatLog());
    }

    public void CharacterTookDamageFromCharacter(CharacterType type, int damageAmount, CharacterType sourceType) {
        combatTextInc += "\n" + GetCharacterName(sourceType) + " dealt " + damageAmount + " damage to " + GetCharacterName(type) + ".";
        StartCoroutine(GenerateCombatLog());
    }

    public void EnemyTookDamageFromCharacter(EnemyType type, int damageAmount, CharacterType sourceType) {
        combatTextInc += "\n" + GetCharacterName(sourceType) + " dealt " + damageAmount + " damage to " + GetEnemyArticle(type) + " " + type + ".";
        StartCoroutine(GenerateCombatLog());
    }

	public void CharacterDiedFromEnemy(CharacterType type, EnemyType sourceType) {
        combatTextInc += "\n" + FirstLetterToUpper(GetEnemyArticle(sourceType)) + " " + sourceType + " KILLED " + GetCharacterName(type) + "!";
        StartCoroutine(GenerateCombatLog());
    }

    public void CharacterDiedFromCharacter(CharacterType type, CharacterType sourceType) {
        combatTextInc += "\n" + GetCharacterName(sourceType) + " KILLED " + GetCharacterName(type) + "!";
        StartCoroutine(GenerateCombatLog());
    }

    public void EnemyDiedFromCharacter(EnemyType type, CharacterType sourceType) {
        combatTextInc += "\n" + GetCharacterName(sourceType) + " DESTROYED " + GetEnemyArticle(type) + " " + type + "!";
        StartCoroutine(GenerateCombatLog());
    }



    // Announcements

    public void EnemySpawnTriggered() {
        announcementInc += "\nYou made enemies spawn.";
        StartCoroutine(GenerateAnnouncement());
    }

    public void EnemyTurnStarted() {
        if (!charactersAreEnemies) {
            announcementInc += "\nEnemies will attack now.";
            StartCoroutine(GenerateAnnouncement());
        }
	}

	public void PlayerTurnStart() {
		if (preventAdditionalAnnouncements) {
			return;
		}
		announcementInc += "\nYou go.";
		StartCoroutine (GenerateAnnouncement());
	}

	public void LevelCompleted() {
		announcementInc += "\nLevel completed.";
		StartCoroutine (GenerateAnnouncement());
	}

	public void LevelFailed() {
		announcementInc += "\nYou failed, try again.";
		StartCoroutine (GenerateAnnouncement());
		preventAdditionalAnnouncements = true;
	}



    // Functionalities

	IEnumerator GenerateCombatLog() {
        mostRecentCombatEvent = Time.time;
		while (combatTextNow.Length != combatTextInc.Length) {
			combatTextNow += combatTextInc[combatTextNow.Length];
			combatLog.text = combatTextNow;
			yield return new WaitForSeconds(0.01f);
		}
	}

	IEnumerator GenerateAnnouncement() {
		mostRecentAnnouncement = Time.time;
		while (announcementNow.Length != announcementInc.Length) {
			announcementNow += announcementInc[announcementNow.Length];
			announcement.text = announcementNow;
			yield return new WaitForSeconds(0.01f);
		}
	}

	void CheckAnnouncementTimers() {
		if (Time.time - mostRecentAnnouncement > announcementDisplayTime) {
			ResetAnnouncements();
		}
	}

    void CheckCombatLogTimers() {
        if (Time.time - mostRecentCombatEvent > combatLogDisplayTime) {
            ResetCombatLog();
        }
    }

	string GetCharacterName(CharacterType type) {
		string name;
		if (type == CharacterType.Character1) {
			name = "Pol";
		} else if (type == CharacterType.Character2) {
			name = "Bosco";
		} else {
			name = "Joseph";
		}
		return name;
	}

    string GetEnemyArticle(EnemyType type) {
        if (type == EnemyType.Exploder) {
            return "an";
        } else if (type == EnemyType.Boss) {
            return "the";
        } else {
            return "a";
        }
    }

    public string FirstLetterToUpper(string str) {
        if (str == null)
            return null;

        if (str.Length > 1)
            return char.ToUpper(str[0]) + str.Substring(1);

        return str.ToUpper();
    }
}
