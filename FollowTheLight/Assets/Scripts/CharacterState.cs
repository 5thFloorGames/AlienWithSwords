using UnityEngine;
using System.Collections;

public class CharacterState : MonoBehaviour {

    public int maximumHealth;
    public CharacterType type;
    public bool dead;

    int health;

    CharacterManager cm;
	UserInterfaceManager uim;

	AudioListener audioListener;
	GameObject characterCamera;
	GameObject sprite;

	void Awake() {
		audioListener = GetComponentInChildren<AudioListener> ();
		characterCamera = transform.FindChild("Camera").gameObject;
		sprite = transform.FindChild ("Sprite").gameObject;
        dead = false;
    }

	public void Init(GameObject manager) {
        cm = manager.GetComponent<CharacterManager>();
		uim = GameObject.Find ("UserInterface").GetComponent<UserInterfaceManager>();
		health = maximumHealth;
		UpdateHealthToUI ();
	}

	public void EnterCharacter() {
		audioListener.enabled = true;
        characterCamera.SetActive(true);
		sprite.SetActive(false);
	}

	public void LeaveCharacter() {
		audioListener.enabled = false;
        characterCamera.SetActive(false);
        sprite.SetActive(true);
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
	        Debug.Log(gameObject.name + " took " +  amount + " damage");
			health -= amount;
			if (health <= 0) {
				health = 0;
				dead = true;
                AnnounceDeathToManager();
			}
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

    }

    void NotAimedAt() {

    }

    void AnnounceDeathToManager() {
        cm.CharacterDied(gameObject, type);
    }

    void CharacterResurrected() {
        dead = false;
        health = maximumHealth;
        UpdateHealthToUI();
    }

	void UpdateHealthToUI() {
		uim.UpdateHealthMeter (gameObject.name, health, maximumHealth);
	}
}
