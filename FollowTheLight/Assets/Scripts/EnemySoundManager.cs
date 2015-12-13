using UnityEngine;
using System.Collections;

public class EnemySoundManager : MonoBehaviour {

	AudioClip[] shootingQuotes;
    AudioClip[] explodingQuotes;
    AudioClip[] spawnQuotes;
    AudioClip[] aimedQuotes;
    AudioClip[] dyingQuotes;

	AudioClip[] explosionSFX;
	AudioClip[] shotSFX;
	AudioClip[] shotHitSFX;
	
	void Start () {
		shootingQuotes = Resources.LoadAll<AudioClip>("Audio/Enemy/Quote_Shoot");
        explodingQuotes = Resources.LoadAll<AudioClip>("Audio/Enemy/Quote_Explode");
        spawnQuotes = Resources.LoadAll<AudioClip>("Audio/Enemy/Quote_Spawn");
        aimedQuotes = Resources.LoadAll<AudioClip>("Audio/Enemy/Quote_Aimed");
        dyingQuotes = Resources.LoadAll<AudioClip>("Audio/Enemy/Quote_Die");

        explosionSFX = Resources.LoadAll<AudioClip>("Audio/Enemy/SFX_Explosion");
		shotSFX = Resources.LoadAll<AudioClip>("Audio/Enemy/SFX_Shot");
		shotHitSFX = Resources.LoadAll<AudioClip>("Audio/Enemy/SFX_ShotHit");
	}

	void Update () {
	
	}



    // Quotes

	public AudioClip GetShootingQuote() {
		return GetAudioClipFromList (shootingQuotes);
	}

    public AudioClip GetExplodingQuote() {
        return GetAudioClipFromList(explodingQuotes);
    }

    public AudioClip GetSpawningQuote() {
        return GetAudioClipFromList(spawnQuotes);
    }

	public AudioClip GetAimedQuote() {
		return GetAudioClipFromList (aimedQuotes);
	}

    public AudioClip GetDyingQuote() {
        return GetAudioClipFromList(dyingQuotes);
    }



    // Effects

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
