using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnemyState : MonoBehaviour {

	public bool dead;
	public int currentHealth;
	Image healthMeter;
	GameObject em;

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

	}

	void Update () {

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
