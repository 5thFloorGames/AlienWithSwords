using UnityEngine;
using System.Collections;

public class CharacterState : MonoBehaviour {

    public int health;

	void Start () {
	
	}

	void Update () {
	
	}

    void Damaged() {
        Debug.Log("character dmged");
    }

    public void ReduceHealthBy(int amount) {
        Debug.Log("character " + gameObject.name + " took " + amount + " damage");
    }
}
