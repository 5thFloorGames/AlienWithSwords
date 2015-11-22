using UnityEngine;
using System.Collections;

public class CharacterState : MonoBehaviour {

    public int health;
	public bool dead;
    public CharacterType type;

	int maximumHealth;

    CharacterManager cm;
	UserInterfaceManager uim;

	AudioListener audioListener;
	Camera characterCamera;
	GameObject sprite;

	void Awake() {
		audioListener = GetComponentInChildren<AudioListener> ();
		characterCamera = GetComponentInChildren<Camera> ();
		sprite = transform.FindChild ("Sprite").gameObject;
	}

	public void Init(CharacterType setType, int amount, GameObject manager) {
        type = setType;
		dead = false;
        cm = manager.GetComponent<CharacterManager>();
		uim = GameObject.Find ("UserInterface").GetComponent<UserInterfaceManager>();
		maximumHealth = amount;
		health = amount;
		updateHealthToUI ();
	}

	public void EnterCharacter() {
		audioListener.enabled = true;
		characterCamera.enabled = true;
		sprite.SetActive(false);
	}

	public void LeaveCharacter() {
		audioListener.enabled = false;
		characterCamera.enabled = false;
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
			updateHealthToUI ();
		}
    }

    void AnnounceDeathToManager() {
        cm.CharacterDied(gameObject, type);
    }

	void updateHealthToUI() {
		uim.UpdateHealthMeter (gameObject.name, health, maximumHealth);
	}
}
