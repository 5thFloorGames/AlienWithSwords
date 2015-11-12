using UnityEngine;
using System.Collections;

public class CharacterState : MonoBehaviour {

    public int health;
	public bool dead;

	void Start () {
		dead = false;
	}

	void Update () {
	
	}

    void takeDamage(int amount) {
        Debug.Log(gameObject.name + " took " +  amount + " damage");
		health -= amount;
		if (health <= 0) {
			health = 0;
			Debug.Log (gameObject.name + " is dead :(");
			dead = true;
		}
    }
}
