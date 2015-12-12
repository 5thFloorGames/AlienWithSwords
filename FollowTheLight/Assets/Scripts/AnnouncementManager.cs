using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AnnouncementManager : MonoBehaviour {

    Image levelFade;
    Image dyingFade;
    Image damageFade;

	public float announcementDisplayTime;

	Text combatLog;
	string combatTextNow;
	string combatTextInc;

	float mostRecentAnnouncement;
	Text announcement;
	string announcementNow;
	string announcementInc;
	bool preventAdditionalAnnouncements;

	void Start () {
        Transform tf = transform.FindChild("Announcements");
		combatLog = tf.FindChild("CombatLog").GetComponent<Text>();
		announcement = tf.FindChild("Announcement").GetComponent<Text>();
        levelFade = tf.FindChild("LevelFade").GetComponent<Image>();
        dyingFade = tf.FindChild("DyingFade").GetComponent<Image>();
        damageFade = tf.FindChild("DamageFade").GetComponent<Image>();
        combatTextNow = "";
		combatTextInc = "";
		ResetCombatLog ();
		ResetAnnouncements ();
	}

	void Update () {
		if (announcementNow.Length > 0) {
			CheckAnnouncementTimers();
		}
	}

	void OnLevelWasLoaded() {
		preventAdditionalAnnouncements = false;
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



    public void CharacterHealedACharacter(CharacterType type, int amount, CharacterType sourceType) {
        combatTextInc += "\n" + GetCharacterName(sourceType) + " restored " + amount + " health to " + GetCharacterName(type) + ".";
        StartCoroutine(GenerateCombatLog());
    }

    public void CharacterTriedToHealFullHealth(CharacterType type, CharacterType sourceType) {
        combatTextInc += "\n" + GetCharacterName(sourceType) + " tried to heal " + GetCharacterName(type) + " but he was already at full health.";
        StartCoroutine(GenerateCombatLog());
    }

    public void CharacterTookDamageFromEnemy(CharacterType type, int damageAmount, EnemyType sourceType) {

        combatTextInc += "\n" + sourceType + " dealt " + damageAmount + " damage to " + GetCharacterName(type) + ".";
        StartCoroutine(GenerateCombatLog());
    }

    public void CharacterTookDamageFromCharacter(CharacterType type, int damageAmount, CharacterType sourceType) {
        combatTextInc += "\n" + GetCharacterName(sourceType) + " dealt " + damageAmount + " damage to " + GetCharacterName(type) + ".";
        StartCoroutine(GenerateCombatLog());
    }

    public void EnemyTookDamageFromCharacter(EnemyType type, int damageAmount, CharacterType sourceType) {
        combatTextInc += "\n" + GetCharacterName(sourceType) + " dealt " + damageAmount + " damage to " + type + ".";
        StartCoroutine(GenerateCombatLog());
    }

	public void CharacterDiedFromEnemy(CharacterType type, EnemyType sourceType) {
        combatTextInc += "\n" + sourceType + " KILLED " + GetCharacterName(type) + "!";
        StartCoroutine(GenerateCombatLog());
    }

    public void CharacterDiedFromCharacter(CharacterType type, CharacterType sourceType) {
        combatTextInc += "\n" + GetCharacterName(sourceType) + " KILLED " + GetCharacterName(type) + "!";
        StartCoroutine(GenerateCombatLog());
    }

    public void EnemyDiedFromCharacter(EnemyType type, CharacterType sourceType) {
        combatTextInc += "\n" + GetCharacterName(sourceType) + " destroyed an " + type + "!";
        StartCoroutine(GenerateCombatLog());
    }



    public void EnemyTurnStarted() {
		announcementInc += "\nEnemies will attack now.";
		StartCoroutine (GenerateAnnouncement());
	}

	public void PlayerTurnStart() {
		if (preventAdditionalAnnouncements) {
			return;
		}
		announcementInc += "\nGo.";
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

	IEnumerator GenerateCombatLog() {
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
}
