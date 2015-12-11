using UnityEngine;
using System.Collections;

public class CharacterState : MonoBehaviour {
	
	public int maximumHealth;
	public CharacterType type;
	public bool dead;

	[SerializeField] Vector3 deathParticleAdjustment;
	
	int health;
	bool inCharacter;
	bool delayedDone;
	
	CharacterManager cm;
	UserInterfaceManager uim;
	AnnouncementManager am;
	
	CharacterSoundController cas;
	AudioListener audioListener;
	AudioSource controllerSoundSource;
	GameObject characterCamera;
	GameObject sprite;
	SpriteRenderer spriteRenderer;
	
	void Awake() {
		audioListener = GetComponentInChildren<AudioListener> ();
		controllerSoundSource = GetComponent<AudioSource> ();
		characterCamera = transform.FindChild("Camera").gameObject;
		sprite = transform.FindChild ("Sprite").gameObject;
		spriteRenderer = sprite.GetComponent<SpriteRenderer> ();
		dead = false;
	}
	
	public void Init(GameObject manager) {
		cm = manager.GetComponent<CharacterManager>();
		am = manager.GetComponent<AnnouncementManager>();
		uim = GameObject.Find ("UserInterface").GetComponent<UserInterfaceManager>();
		cas = GetComponentInChildren<CharacterSoundController>();
		health = maximumHealth;
		UpdateHealthToUI ();
	}
	
	public void EnterCharacter() {
		inCharacter = true;
		audioListener.enabled = true;
		characterCamera.SetActive(true);
		sprite.SetActive(false);
		controllerSoundSource.mute = true;
		cas.PlaySelectionQuote ();
		if (delayedDone) {
			controllerSoundSource.mute = false;
		} else {
			StartCoroutine (DelayedUnmute ());
			delayedDone = true;
		}
	}
	
	public void LeaveCharacter() {
		inCharacter = false;
		audioListener.enabled = false;
		characterCamera.SetActive(false);
		if (!dead) {
			sprite.SetActive(true);
		}
	}

	public void OutOfActions() {
		cas.PlayOutOfActionsQuote ();
	}
	
	IEnumerator DelayedUnmute() {
		yield return new WaitForSeconds (0.5f);
		if (inCharacter) {
			controllerSoundSource.mute = false;
		}
	}
	
	void Start () {
		
	}
	
	void Update () {
		
	}
	
	void OnLevelWasLoaded(int level) {
		if (level == 0) {
			Destroy (gameObject);
		}
	}
	
	void TakeDamage(int amount) {
		if (!dead) {
			am.CharacterTookDamage(type, amount);
			if (health - amount <= 0) {
				health = 0;
				cas.PlayDyingQuote ();
				Death ();
			} else {
				health -= amount;
			}
			uim.DamageTakenUIUpdate(type.ToString());
			UpdateHealthToUI ();
		}
	}

	void Heal(int amount) {
		if (!dead) {
			Debug.Log(gameObject.name + " got " + amount + " healing");
			health += amount;
			if (health >= maximumHealth) {
				health = maximumHealth;
			}
			UpdateHealthToUI();
		}
	}
	
	void AimedAt() {
		spriteRenderer.color = new Vector4 (0.5f, 0.5f, 0.5f, 1);
	}
	
	void NotAimedAt() {
		spriteRenderer.color = new Vector4 (1, 1, 1, 1);
	}
	
	void Death() {
		dead = true;
		NotAimedAt ();
		sprite.SetActive (false);
		AnnounceDeathToManager();
		GameObject prefab = (GameObject) Resources.Load("playerExplodeParticles");
		Instantiate (prefab, transform.position + deathParticleAdjustment, Quaternion.identity);
	}
	
	void AnnounceDeathToManager() {
		am.CharacterDied (type);
		uim.CharacterDiedUIUpdate (type.ToString());
		cm.CharacterDied(gameObject, type);
	}
	
	void CharacterResurrected() {
		uim.CharacterAliveUIUpdate (type.ToString());
		if (!inCharacter) {
			sprite.SetActive(true);
		}
		dead = false;
		health = maximumHealth;
		UpdateHealthToUI();
	}
	
	void UpdateHealthToUI() {
		uim.UpdateHealthMeter (gameObject.name, health, maximumHealth);
	}
}