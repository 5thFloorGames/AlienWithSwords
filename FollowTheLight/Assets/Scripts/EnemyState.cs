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
		if (!dead) {;
			currentHealth -= amount;
			if (currentHealth <= 0) {
				currentHealth = 0;
				dead = true;
				StartDying();
			}
			if (!dead) {
				animator.SetTrigger("GetHit");
			}
			UpdateHealthToHealthMeter ();
		}
	}

	void UpdateHealthToHealthMeter() {
		healthMeter.fillAmount = ((float)currentHealth/maximumHealth);
	}

	void StartDying() {
		gameObject.GetComponentInChildren<Collider> ().enabled = false;
		animator.SetBool ("Dying", true);
		em.SendMessage ("DeleteEnemyFromList", gameObject);
		Invoke ("Death", 3.0f);
	}

	void Death() {
		Destroy (gameObject);
	}
}
