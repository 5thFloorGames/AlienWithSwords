using UnityEngine;
using System.Collections;

public class CharacterSoundController : MonoBehaviour {

	[SerializeField] AudioSource quoteSource;
	[SerializeField] AudioSource effectSource;
	 
	[SerializeField] AudioClip[] selectCharacterQuotes;
	[SerializeField] AudioClip[] attackingQuotes;
	[SerializeField] AudioClip[] killsAnEnemyQuotes;

	[SerializeField] AudioClip dyingQuote;
	[SerializeField] AudioClip killsACharacterQuote;

	[SerializeField] AudioClip[] attackSoundEffects;

	
	void Start () {
		//source = GetComponent<AudioSource>();
	}

	void Update () {
	
	}


	// Playing the Quotes

	public void PlaySelectionQuote() {
		if (selectCharacterQuotes.Length == 0) {
			return;
		}
		int n = Random.Range (1, selectCharacterQuotes.Length);
		quoteSource.clip = selectCharacterQuotes[n];
		quoteSource.PlayOneShot(quoteSource.clip);
		if (selectCharacterQuotes.Length > 1) {
			selectCharacterQuotes [n] = selectCharacterQuotes [0];
			selectCharacterQuotes [0] = quoteSource.clip;
		}
	}
}
