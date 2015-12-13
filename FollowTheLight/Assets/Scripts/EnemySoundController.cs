using UnityEngine;
using System.Collections;

public class EnemySoundController : MonoBehaviour {

	AudioSource source;
	EnemySoundManager esm;


	void Start () {
		source = GetComponent<AudioSource>();
		esm = GameObject.FindGameObjectWithTag("GameController").GetComponent<EnemySoundManager>();
	}

	void Update () {
		
	}


    // Quotes

	public void PlayShootingQuote() {
        if (Random.Range(0, 3) == 0) {
            return;
        }

        StartCoroutine (PlayWithRandomDelay(esm.GetShootingQuote()));
	}

    public void PlayExplodingQuote() {
        if (Random.Range(0, 3) == 0) {
            return;
        }
        StartCoroutine(PlayWithRandomDelay(esm.GetExplodingQuote()));
    }

	public void PlayAimedQuote() {
		PlayASound (esm.GetAimedQuote());
	}

    public void PlayDyingQuote() {
        PlayASound(esm.GetDyingQuote());
    }

    public void PlaySpawningQuote() {
        StartCoroutine(PlayWithRandomDelay(esm.GetSpawningQuote()));
    }



    // Effects

    public void PlayExplosionSFX() {
		StartCoroutine (PlayWithRandomDelay(esm.GetExplosionSFX()));
	}

	public void PlayShotSFX() {
		StartCoroutine (PlayWithRandomDelay(esm.GetShotSFX()));
	}

	public AudioClip GetShotHitSFX() {
		return esm.GetShotHitSFX();
	}

	IEnumerator PlayWithRandomDelay(AudioClip clip) {
		yield return new WaitForSeconds (Random.Range(0.001f, 0.050f));
		PlayASound (clip);
	}

	void PlayASound(AudioClip clip) {
		source.clip = clip;
		source.PlayOneShot(source.clip);
	}
}
