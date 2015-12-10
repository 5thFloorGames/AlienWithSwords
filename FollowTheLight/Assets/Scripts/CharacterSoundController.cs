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
		if (quoteSource == effectSource) {
			Debug.Log ("WARNING: You should have different audio sources for quotes and effects (" + transform.parent.name + ")");
		}
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

	public void PlayAttackingQuote() {
		if (attackingQuotes.Length == 0) {
			return;
		}
		int n = Random.Range (1, attackingQuotes.Length);
		quoteSource.clip = attackingQuotes[n];
		quoteSource.PlayOneShot(quoteSource.clip);
		if (attackingQuotes.Length > 1) {
			attackingQuotes [n] = attackingQuotes [0];
			attackingQuotes [0] = quoteSource.clip;
		}
	}

	public void PlayKillingAnEnemyQuote() {
		if (killsAnEnemyQuotes.Length == 0) {
			return;
		}
		int n = Random.Range (1, killsAnEnemyQuotes.Length);
		quoteSource.clip = killsAnEnemyQuotes[n];
		quoteSource.PlayOneShot(quoteSource.clip);
		if (killsAnEnemyQuotes.Length > 1) {
			killsAnEnemyQuotes [n] = killsAnEnemyQuotes [0];
			killsAnEnemyQuotes [0] = quoteSource.clip;
		}
	}

	public void PlayDyingQuote() {
		quoteSource.clip = dyingQuote;
		quoteSource.PlayOneShot(quoteSource.clip);
	}
}