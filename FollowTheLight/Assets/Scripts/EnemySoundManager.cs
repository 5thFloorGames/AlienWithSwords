using UnityEngine;
using System.Collections;

public class EnemySoundManager : MonoBehaviour {

	AudioClip[] attackingQuotes;
	AudioClip[] aimedQuotes;

	AudioClip[] explosionSFX;
	AudioClip[] shotSFX;
	AudioClip[] shotHitSFX;
	
	void Start () {
		attackingQuotes = Resources.LoadAll<AudioClip>("Audio/Enemy/Quote_Attack");
		aimedQuotes = Resources.LoadAll<AudioClip>("Audio/Enemy/Quote_Aimed");

		explosionSFX = Resources.LoadAll<AudioClip>("Audio/Enemy/SFX_Explosion");
		shotSFX = Resources.LoadAll<AudioClip>("Audio/Enemy/SFX_Shot");
		shotHitSFX = Resources.LoadAll<AudioClip>("Audio/Enemy/SFX_ShotHit");
	}

	void Update () {
	
	}

	public AudioClip GetAttackQuote() {
		return GetAudioClipFromList (attackingQuotes);
	}

	public AudioClip GetAimedQuote() {
		return GetAudioClipFromList (aimedQuotes);
	}



	public AudioClip GetExplosionSFX() {
		return GetAudioClipFromList (explosionSFX);
	}

	public AudioClip GetShotSFX() {
		return GetAudioClipFromList (shotSFX);
	}

	public AudioClip GetShotHitSFX() {
		return GetAudioClipFromList (shotHitSFX);
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
