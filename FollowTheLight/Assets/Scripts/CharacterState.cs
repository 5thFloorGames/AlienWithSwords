using UnityEngine;
using System.Collections;

public class CharacterState : MonoBehaviour {

    public int health;
	public bool dead;

	int maximumHealth;

	UserInterfaceManager uim;

	void Awake() {
	}

	public void Init(int amount) {
		dead = false;
		uim = GameObject.Find ("UserInterface").GetComponent<UserInterfaceManager>();
		maximumHealth = amount;
		health = amount;
		updateHealthToUI ();
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

    void takeDamage(int amount) {
		if (!dead) {
	        Debug.Log(gameObject.name + " took " +  amount + " damage");
			health -= amount;
			if (health <= 0) {
				health = 0;
				Debug.Log (gameObject.name + " is dead :(");
				dead = true;
			}
			updateHealthToUI ();
		}
    }

	void updateHealthToUI() {
		uim.UpdateHealthMeter (gameObject.name, health, maximumHealth);
	}
}
