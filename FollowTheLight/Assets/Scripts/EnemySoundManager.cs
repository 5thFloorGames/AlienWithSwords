using UnityEngine;
using System.Collections;

public class EnemySoundManager : MonoBehaviour {

	AudioClip[] attackingQuotes;
	AudioClip[] aimedQuotes;
	
	void Start () {
		attackingQuotes = Resources.LoadAll<AudioClip>("Audio/Enemy/Quote_Attack");
		aimedQuotes = Resources.LoadAll<AudioClip>("Audio/Enemy/Quote_Aimed");
	}

	void Update () {
	
	}

	public AudioClip GetAttackQuote() {
		return GetAudioClipFromList (attackingQuotes);
	}

	public AudioClip GetAimedQuote() {
		return GetAudioClipFromList (aimedQuotes);
	}

	AudioClip GetAudioClipFromList(AudioClip[] clips){
		if (clips.Length == 0) {
			return null;
		}
		AudioClip clip;
		if (clips.Length > 1) {
			int n = Random.Range (1, clips.Length);
			clip = clips [n];
			clips [n] = clips [0];
			clips [0] = clip;
		} else {
			clip = clips[0];
		}
		return clip;
	}
}
