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
		PlayASound (esm.GetAttackQuote());
	}

	public void PlayAimedQuote() {
		PlayASound (esm.GetAttackQuote());
	}

	void PlayASound(AudioClip clip) {
		source.clip = clip;
		source.PlayOneShot(source.clip);
	}
}
