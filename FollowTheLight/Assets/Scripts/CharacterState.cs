using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterState : MonoBehaviour {
	
	public int maximumHealth;
	public CharacterType type;
    public CharacterName name;
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

    public void YouKilledACharacter() {

    }

    public void YouKilledAnEnemy() {

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
	
	void TakeDamage(List<object> info) {

        if (info[0] == null || info[1] == null) {
            Debug.Log("WARNING: correct info not given for dealing damage!");
        }

		if (!dead) {

            // Handling the info
            int amount = int.Parse(info[0].ToString());
            GameObject source = (GameObject)info[1];
            bool sourceIsCharacter = false;
            CharacterState sourceCs = source.GetComponent<CharacterState>();
            EnemyState sourceEs = source.GetComponent<EnemyState>();

            if (sourceCs != null) {
                sourceIsCharacter = true;
            }

            health -= amount;

            if (sourceIsCharacter) {
                am.CharacterTookDamageFromCharacter(type, amount, sourceCs.type);
            } else {
                am.CharacterTookDamageFromEnemy(type, amount, sourceEs.type);
            }

            if (health <= 0) {
                Death();

                if (sourceIsCharacter) {
                    sourceCs.YouKilledACharacter();
                    am.CharacterDiedFromCharacter(type, sourceCs.type);
                } else {
                    am.CharacterDiedFromEnemy(type, sourceEs.type);
                }

			}
			uim.DamageTakenUIUpdate(type.ToString());
			UpdateHealthToUI ();
		}
	}

	void Heal(List<object> info) {
        if (info[0] == null || info[1] == null) {
            Debug.Log("WARNING: correct info not given for healing!");
        }

        if (!dead) {

            // Handling the info
            int amount = int.Parse(info[0].ToString());
            GameObject source = (GameObject)info[1];
            bool sourceIsCharacter = false;
            CharacterState sourceCs = source.GetComponent<CharacterState>();
            EnemyState sourceEs = source.GetComponent<EnemyState>();

            if (sourceCs != null) {
                sourceIsCharacter = true;
            }

            health += amount;


			if (health >= maximumHealth) {
				health = maximumHealth;
                if (sourceIsCharacter) {
                    am.CharacterTriedToHealFullHealth(type, sourceCs.type);
                }
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
        health = 0;
        cas.PlayDyingQuote();
        NotAimedAt ();
		sprite.SetActive (false);
		AnnounceDeathToManager();
		GameObject prefab = (GameObject) Resources.Load("playerExplodeParticles");
		Instantiate (prefab, transform.position + deathParticleAdjustment, Quaternion.identity);
        DisableColliders();
	}
	
	void AnnounceDeathToManager() {
		uim.CharacterDiedUIUpdate (type.ToString());
		cm.CharacterDied(gameObject, type);
	}
	
	void CharacterResurrected() {
        EnableColliders();
		uim.CharacterAliveUIUpdate (type.ToString());
		if (!inCharacter) {
			sprite.SetActive(true);
		}
		dead = false;
		health = maximumHealth;
		UpdateHealthToUI();
	}

    void EnableColliders() {
        GetComponent<CharacterController>().enabled = true;
        GetComponent<CapsuleCollider>().enabled = true;
    }

    void DisableColliders() {
        GetComponent<CharacterController>().enabled = false;
        GetComponent<CapsuleCollider>().enabled = false;
    }
	
	void UpdateHealthToUI() {
		uim.UpdateHealthMeter (gameObject.name, health, maximumHealth);
	}
}