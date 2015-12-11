using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AnnouncementManager : MonoBehaviour {

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
		combatLog = transform.FindChild ("Announcements").FindChild("CombatLog").GetComponent<Text>();
		announcement = transform.FindChild ("Announcements").FindChild("Announcement").GetComponent<Text>();
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

	public void CharacterTookDamage(CharacterType type, int damageAmount) {
		string name = GetCharacterName (type);
		combatTextInc += "\n" + name + " took " + damageAmount + " damage.";
			
		StartCoroutine(GenerateCombatLog());
	}

	public void CharacterDied(CharacterType type) {
		string name = GetCharacterName (type);
		combatTextInc += "\n" + name + " DIED...";
		
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
