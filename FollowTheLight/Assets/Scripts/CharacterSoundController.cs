using UnityEngine;
using System.Collections;

public class CharacterSoundController : MonoBehaviour {

	public CharacterType charType;
	public bool outOfActions;
	public bool outOfMovement;
	
	[SerializeField] AudioSource quoteSource;
	[SerializeField] AudioSource effectSource;
	
	AudioClip[] selectCharacterQuotes;
	AudioClip[] attackingQuotes;
	AudioClip[] healingQuotes;
	AudioClip[] killEnemyQuotes;
	AudioClip[] dyingQuotes;
	AudioClip[] killCharQuotes;
	AudioClip[] outOfActionsQuotes;
    AudioClip[] lastOneAliveQuotes;
	
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
		outOfActionsQuotes = Resources.LoadAll<AudioClip> ("Audio/" + charType.ToString() + "/Quote_OutOfActions");
        lastOneAliveQuotes = Resources.LoadAll<AudioClip>("Audio/" + charType.ToString() + "/Quote_LastOneAlive");

        attackSFX = Resources.LoadAll<AudioClip> ("Audio/" + charType.ToString() + "/SFX_Attack");
		attackHitSFX = Resources.LoadAll<AudioClip> ("Audio/" + charType.ToString() + "/SFX_AttackHit");
		healSFX = Resources.LoadAll<AudioClip> ("Audio/" + charType.ToString() + "/SFX_Healing");
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
		if (Random.Range (0, 2) == 0) {
			PlayRandomQuote (selectCharacterQuotes);
		}
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
		if (outOfActions && outOfMovement) {
			PlayRandomQuote (outOfActionsQuotes);
		}
	}

    public void PlayLastOneAliveQuote() {
        PlayRandomQuote(lastOneAliveQuotes);
    }



    // Playing the SFX

    public void PlayAttackSFX() {
		PlayRandomSFX (attackSFX);
	}

	public void PlayHealSFX() {
		PlayRandomSFX (healSFX);
	}

	public AudioClip[] GetAttackHitClips() {
		return attackHitSFX;
	}



	// The helper functions

	private void PlayRandomQuote(AudioClip[] clips){
		if (clips.Length == 0) {
			return;
		}
		if (clips.Length > 1) {
			int n = Random.Range (1, clips.Length);
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
			int n = Random.Range (1, clips.Length);
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