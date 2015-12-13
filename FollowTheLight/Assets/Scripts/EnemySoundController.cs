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

	public void PlayShootingQuote() {
		StartCoroutine (PlayWithRandomDelay(esm.GetShootingQuote()));
	}

	public void PlayAimedQuote() {
		PlayASound (esm.GetShootingQuote());
	}

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
		yield return new WaitForSeconds (Random.Range(0.01f, 0.60f));
		PlayASound (clip);
	}

	void PlayASound(AudioClip clip) {
		source.clip = clip;
		source.PlayOneShot(source.clip);
	}
}
