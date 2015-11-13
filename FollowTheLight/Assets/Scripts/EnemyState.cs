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
		updateHealthToHealthMeter ();
	}
	
	void Start () {
	
	}

	void Update () {
	
	}

	void takeDamage(int amount) {
		if (!dead) {
			//Debug.Log(gameObject.name + " took " +  amount + " damage");
			currentHealth -= amount;
			if (currentHealth <= 0) {
				currentHealth = 0;
				//Debug.Log (gameObject.name + " is dead :(");
				dead = true;
				death();
			}
			updateHealthToHealthMeter ();
		}
	}

	void updateHealthToHealthMeter() {
		healthMeter.fillAmount = ((float)currentHealth/maximumHealth);
	}

	void death() {
		em.SendMessage ("deleteEnemyFromList", gameObject);
		Destroy (gameObject);
	}
}
