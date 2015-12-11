using UnityEngine;
using System.Collections;

public class EnemySoundManager : MonoBehaviour {

	AudioClip[] attackingQuotes;
	
	void Start () {
		attackingQuotes = Resources.LoadAll<AudioClip>("Audio/Enemy/Quote_Attack");
	}

	void Update () {
	
	}

	public void GetAttackQuote() {

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
