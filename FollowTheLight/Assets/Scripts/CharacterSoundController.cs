using UnityEngine;
using System.Collections;

public class CharacterSoundController : MonoBehaviour {

	public CharacterType charType;
	
	[SerializeField] AudioSource quoteSource;
	[SerializeField] AudioSource effectSource;
	
	AudioClip[] selectCharacterQuotes;
	AudioClip[] attackingQuotes;
	AudioClip[] healingQuotes;
	AudioClip[] killEnemyQuotes;
	AudioClip[] dyingQuotes;
	AudioClip[] killCharQuotes;
	AudioClip[] outOfActions;
	
	AudioClip[] attackSFX;
	AudioClip[] attackHitSFX;
	AudioClip[] healSFX;

	void Awake() {
		selectCharacterQuotes = Resources.LoadAll<AudioClip>("Audio/" + charType.ToString() + "/Quote_Selected");
		attackingQuotes = Resources.LoadAll<AudioClip>("Audio/" + charType.ToString() + "/Quote_Attack");
		healingQuotes = Resources.LoadAll<AudioClip>("Audio/" + charType.ToString() + "/Quote_Heal");
		killEnemyQuotes = Resources.LoadAll<AudioClip>("Audio/" + charType.ToString() + "/Quote_KillEnemy");
		killCharQuotes = Resources.LoadAll<AudioClip>("Audio/" + charType.ToString() + "/Quote_KillChar");
		dyingQuotes = Resources.LoadAll<AudioClip>("Audio/" + charType.ToString() + "/Quote_Die");
		outOfActions = Resources.LoadAll<AudioClip> ("Audio/" + charType.ToString() + "/Quote_OutOfActions");

		attackSFX = Resources.LoadAll<AudioClip> ("Audio/" + charType.ToString() + "/SFX_Attack");
		attackHitSFX = Resources.LoadAll<AudioClip> ("Audio/" + charType.ToString() + "/SFX_AttackHit");
		healSFX = Resources.LoadAll<AudioClip> ("Audio/" + charType.ToString() + "/SFX_Heal");
	}
	
	void Start () {
		if (quoteSource == effectSource) {
			Debug.Log ("WARNING: You should have different audio sources for quotes and effects (" + transform.parent.name + ")!");
			Debug.Log ("The first component should be for quotes, the second for SFX.");
		}
	}
	
	void Update () {
		
	}
	
	
	// Playing the Quotes
	
	public void PlaySelectionQuote() {
		PlayRandomQuote (selectCharacterQuotes);
	}

	public void PlayAttackingQuote() {
		if (Random.Range (0,3) > 0) {
			PlayRandomQuote (attackingQuotes);
		}
	}

	public void PlayHealingQuote() {
		if (Random.Range (0,3) > 0) {
			PlayRandomQuote (healingQuotes);
		}
	}

	public void PlayKillingAnEnemyQuote() {
		PlayRandomQuote (killEnemyQuotes);
	}

	public void PlayDyingQuote() {
		PlayRandomQuote (dyingQuotes);
	}

	public void PlayKillingCharacterQuote() {
		PlayRandomQuote (killCharQuotes);
	}

	public void PlayOutOfActionsQuote() {
		PlayRandomQuote (outOfActions);
	}



	public void PlayAttackSFX() {
		PlayRandomSFX (attackSFX);
	}

	public void PlayHealSFX() {
		PlayRandomSFX (healSFX);
	}

	public AudioClip[] GetAttackHitClips() {
		return attackHitSFX;
	}



	private void PlayRandomQuote(AudioClip[] clips){
		if (clips.Length == 0) {
			return;
		}
		if (clips.Length > 1) {
			int n = Random.Range (0, clips.Length);
			quoteSource.clip = clips [n];
			quoteSource.PlayOneShot (quoteSource.clip);
			clips [n] = clips [0];
			clips [0] = quoteSource.clip;
		} else {
			quoteSource.clip = clips[0];
			quoteSource.PlayOneShot(quoteSource.clip);
		}
	}

	private void PlayRandomSFX(AudioClip[] clips){
		if (clips.Length == 0) {
			return;
		}
		if (clips.Length > 1) {
			int n = Random.Range (0, clips.Length);
			effectSource.clip = clips [n];
			effectSource.PlayOneShot (effectSource.clip);
			clips [n] = clips [0];
			clips [0] = effectSource.clip;
		} else {
			effectSource.clip = clips[0];
			effectSource.PlayOneShot(effectSource.clip);
		}
	}

}