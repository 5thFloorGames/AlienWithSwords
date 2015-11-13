using UnityEngine;
using System.Collections;

public class EnemyState : MonoBehaviour {

	public bool dead;
	public int health;

	int maximumHealth;

	public void Init(int amount) {
		dead = false;
		maximumHealth = amount;
		health = amount;
		//updateHealthToUI ();
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
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
			//updateHealthToUI ();
		}
	}
}
