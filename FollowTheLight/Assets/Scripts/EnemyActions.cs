using UnityEngine;
using System.Collections;

public class EnemyActions : MonoBehaviour {
    GameObject areaDamage;

	void Start () {
        areaDamage = (GameObject)Resources.Load("AreaDamage");
	}

	public void TriggerActions () {
		Debug.Log (gameObject.name + " enemy used an ability");
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
