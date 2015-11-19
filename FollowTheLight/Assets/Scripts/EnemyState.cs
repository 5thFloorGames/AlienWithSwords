using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnemyState : MonoBehaviour {

	public bool dead;
	public int currentHealth;
	Image healthMeter;
	GameObject em;
	Animator animator;

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
		animator = gameObject.GetComponentInChildren<Animator>();
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
				StartDying();
			}
			UpdateHealthToHealthMeter ();
		}
	}

	void UpdateHealthToHealthMeter() {
		healthMeter.fillAmount = ((float)currentHealth/maximumHealth);
	}

	void StartDying() {
		animator.SetBool ("Dying", true);
		em.SendMessage ("DeleteEnemyFromList", gameObject);
		Invoke ("Death", 2.0f);
	}

	void Death() {
		Destroy (gameObject);
	}
}
