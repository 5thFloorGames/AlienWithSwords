using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnemyState : MonoBehaviour {

	public bool dead;
    public int maximumHealth;
    int currentHealth;

	Image healthMeter;
	GameObject em;
	Animator animator;
    EnemyMovement move;

	public void Init(GameObject manager) {
		dead = false;
		healthMeter = transform.FindChild("EnemyInfo").FindChild("HealthMeter").GetComponent<Image>();
		em = manager;
		UpdateHealthToHealthMeter ();
	}
	
	void Start () {
        if (maximumHealth == 0) {
            maximumHealth = 30;
        }
        currentHealth = maximumHealth;
		animator = gameObject.GetComponentInChildren<Animator>();
        move = gameObject.GetComponent<EnemyMovement>();
        move.DisableNva();
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
				//animator.SetTrigger("GetHit");
			}
			UpdateHealthToHealthMeter ();
		}
	}

	void AimedAt(GameObject character) {
		animator.SetBool ("AimedAt", true);
        move.CharacterAimedYou(character);
	}

	void NotAimedAt() {
		animator.SetBool ("AimedAt", false);
	}

	void UpdateHealthToHealthMeter() {
		healthMeter.fillAmount = ((float)currentHealth/maximumHealth);
	}

	void StartDying() {
		gameObject.GetComponentInChildren<Collider> ().enabled = false;
		animator.SetBool ("Dying", true);
		em.SendMessage ("DeleteEnemyFromList", gameObject);
		Invoke ("AdjustSpriteForDying", 0.1f);
		Invoke ("Death", 0.5f);
	}

	void AdjustSpriteForDying() {
		transform.FindChild ("Sprite").transform.position += new Vector3 (0, 0.2f, 0);
	}

	void Death() {
        Invoke("ColliderDeactivate", 1.0f);
        move.EnableNva();
		GameObject prefab = (GameObject) Resources.Load("BloodPool");
		transform.FindChild ("EnemyInfo").gameObject.SetActive (false);
		Instantiate (prefab, transform.position, Quaternion.identity);
	}

    void ColliderDeactivate() {
        GetComponentInChildren<Collider>().enabled = false;
    }

}
