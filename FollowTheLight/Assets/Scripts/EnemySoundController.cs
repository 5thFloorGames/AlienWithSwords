using UnityEngine;
using System.Collections;

public class EnemySoundController : MonoBehaviour {

	AudioSource source;
	EnemySoundManager esm;


	void Awake () {
		source = GetComponent<AudioSource>();
		esm = GameObject.FindGameObjectWithTag("GameController").GetComponent<EnemySoundManager>();
	}

	void Update () {
		
	}

	public void PlayAttackQuote() {
		StartCoroutine (PlayWithRandomDelay(esm.GetAttackQuote()));
	}

	IEnumerator PlayWithRandomDelay(AudioClip clip) {
		yield return new WaitForSeconds (Random.Range(0.01f, 0.60f));
		PlayASound (clip);
	}

	public void PlayAimedQuote() {
		PlayASound (esm.GetAttackQuote());
	}

	void PlayASound(AudioClip clip) {
		Debug.Log (clip);
		source.clip = clip;
		source.PlayOneShot(source.clip);
	}
}
