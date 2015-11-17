using UnityEngine;
using System.Collections;

public class EnemyActions : MonoBehaviour {
    GameObject areaDamage;
	Transform char1;
    Transform char2;

	void Start () {
        areaDamage = (GameObject)Resources.Load("AreaDamage");
		char1 = GameObject.Find ("Character1").transform;
        char2 = GameObject.Find("Character2").transform;
    }

	void Update () {
        if (GameState.activeCharacter == "Character1") {
            transform.LookAt(char1);
        } else {
            transform.LookAt(char2);
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
