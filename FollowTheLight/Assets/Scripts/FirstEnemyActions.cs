using UnityEngine;
using System.Collections;

public class FirstEnemyActions : MonoBehaviour {
    GameObject aoePrefab;
	EnemyManager em;
	float actionTime;
	int actionDamage;

	void Start () {
        aoePrefab = (GameObject)Resources.Load("AreaDamage"); 
		actionTime = 1.0f;
		actionDamage = 5;
    }

	void Update () {
        
	}

	public void InitActions(GameObject manager) {
		em = manager.GetComponent<EnemyManager>();
	}

	public void TriggerActions () {
		// Debug.Log (gameObject.name + " enemy used an ability");
        CastAreaDamage();
	}

    void CastAreaDamage() {
        GameObject spawnedAreaDamage = (GameObject)Instantiate(aoePrefab, transform.position, Quaternion.identity);
		spawnedAreaDamage.name = gameObject.name + "AOE";
		AreaDamageBehavior adb = spawnedAreaDamage.GetComponent<AreaDamageBehavior> ();
		// time, size, damage for the aoe effect and start casting it
		adb.Init (actionTime, 20f, actionDamage);
		Invoke ("ActionsCompletedInformManager", actionTime);
    }

	void ActionsCompletedInformManager() {
		em.EnemyActionsCompleted();
	}
}
