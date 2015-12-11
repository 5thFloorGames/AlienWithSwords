using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AnnouncementManager : MonoBehaviour {

	Text text;
	string str;
	string toWrite;

	void Start () {
		text = transform.FindChild ("Announcements").FindChild("Text").GetComponent<Text>();
		str = "";
		toWrite = "";
		ResetAnnouncements ();
	}

	void Update () {
		
	}

	public void ResetAnnouncements() {
		toWrite = "";
		str = "";
		text.text = "";
	}

	public void CharacterTookDamage(CharacterType type, int damageAmount) {
		string name = GetCharacterName (type);
		toWrite += name + " took " + damageAmount + " damage.     \n";
			
		StartCoroutine(GenerateText());
	}

	public void CharacterDied(CharacterType type) {
		string name = GetCharacterName (type);
		toWrite += name + " DIED...\n";
		
		StartCoroutine(GenerateText());
	}

	IEnumerator GenerateText() {
		while (str.Length != toWrite.Length) {
			str += toWrite[str.Length];
			text.text = str;
			yield return new WaitForSeconds(0.01f);
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
