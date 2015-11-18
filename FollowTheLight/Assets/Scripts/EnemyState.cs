using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnemyState : MonoBehaviour {

	public bool dead;
	public int currentHealth;
	Image healthMeter;
	GameObject em;

	Transform char1;
	Transform char2;
	Transform char3;

	int maximumHealth;

	public void Init(int amount, GameObject manager) {
		dead = false;
		maximumHealth = amount;
		currentHealth = amount;
		healthMeter = transform.FindChild("EnemyInfo").FindChild("HealthMeter").GetComponent<Image>();
		em = manager;
		UpdateHealthToHealthMeter ();
	}
	
	void Start () {
		char1 = GameObject.Find("Character1").transform;
		if (GameState.GetLevel() > 1) {
			char2 = GameObject.Find("Character2").transform;
		}
		
		if (GameState.GetLevel() > 2) {
			char3 = GameObject.Find("Character3").transform;
		}
	}

	void Update () {
		if (GameState.activeCharacter == "Character1") {
			transform.LookAt(char1);
		} else if (GameState.activeCharacter == "Character2") {
			transform.LookAt(char2);
		} else {
			transform.LookAt(char3);
		}
	}

	void TakeDamage(int amount) {
		if (!dead) {
			//Debug.Log(gameObject.name + " took " +  amount + " damage");
			currentHealth -= amount;
			if (currentHealth <= 0) {
				currentHealth = 0;
				//Debug.Log (gameObject.name + " is dead :(");
				dead = true;
				Death();
			}
			UpdateHealthToHealthMeter ();
		}
	}

	void UpdateHealthToHealthMeter() {
		healthMeter.fillAmount = ((float)currentHealth/maximumHealth);
	}

	void Death() {
		em.SendMessage ("DeleteEnemyFromList", gameObject);
		Destroy (gameObject);
	}
}
