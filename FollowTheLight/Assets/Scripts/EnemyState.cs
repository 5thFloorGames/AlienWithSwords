using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnemyState : MonoBehaviour {

	public bool dead;
	public int currentHealth;
	Image healthMeter;

	int maximumHealth;

	public void Init(int amount) {
		dead = false;
		maximumHealth = amount;
		currentHealth = amount;
		healthMeter = transform.FindChild("EnemyInfo").FindChild("HealthMeter").GetComponent<Image>();
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
				Destroy (gameObject);
			}
			updateHealthToHealthMeter ();
		}
	}

	void updateHealthToHealthMeter() {
		healthMeter.fillAmount = ((float)currentHealth/maximumHealth);
	}
}
