using UnityEngine;
using System.Collections;

public class EnemyActions : MonoBehaviour {
    GameObject areaDamage;
	Transform char1;
    Transform char2;
    Transform char3;

	void Start () {
        areaDamage = (GameObject)Resources.Load("AreaDamage");
        char1 = GameObject.Find("Character1").transform;
        if (Application.loadedLevel > 1) {
            char2 = GameObject.Find("Character2").transform;
        }

        if (Application.loadedLevel > 2) {
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

	public void TriggerActions () {
		// Debug.Log (gameObject.name + " enemy used an ability");
        CastAreaDamage();
	}

    void CastAreaDamage() {
        GameObject spawnedAreaDamage = (GameObject)Instantiate(areaDamage, transform.position, Quaternion.identity);
		spawnedAreaDamage.name = gameObject.name + "AOE";
		AreaDamageBehavior aob = spawnedAreaDamage.GetComponent<AreaDamageBehavior> ();
		aob.time = 2.0f;
		aob.size = 20f;
		aob.damage = 5;
    }
}
