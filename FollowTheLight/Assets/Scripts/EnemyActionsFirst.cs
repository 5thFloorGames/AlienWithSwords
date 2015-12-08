using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyActionsFirst : MonoBehaviour {

	EnemyManager em;
	EnemyMovement move;
	Animator animator;

    GameObject aoePrefab;
	int actionDamage;
    float aoeLifetime;

	void Start () {
		move = GetComponent<EnemyMovement> ();
        aoePrefab = (GameObject)Resources.Load("AreaDamage");
		animator = gameObject.GetComponentInChildren<Animator>();

		actionDamage = 5;
        aoeLifetime = 1.7f;
        
    }

	void Update () {

	}

	public void InitActions(GameObject manager) {
		em = manager.GetComponent<EnemyManager>();
	}

	public void TriggerActions () {
		move.Go();
	}

	public void MovingCompleteStartAttack(GameObject target) {
		CastAreaDamage ();
	}

    void CastAreaDamage() {
		float animationDelay = 1.0f;

		animator.SetTrigger ("Attack");
        GameObject spawnedAreaDamage = (GameObject)Instantiate(aoePrefab, transform.position, Quaternion.identity);
		spawnedAreaDamage.name = gameObject.name + "Aoe";
		AreaDamageBehavior adb = spawnedAreaDamage.GetComponent<AreaDamageBehavior> ();

		adb.Init (actionDamage, animationDelay, aoeLifetime);
		Invoke ("ActionsCompletedInformManager", animationDelay + aoeLifetime);
    }

	void ActionsCompletedInformManager() {
		em.EnemyActionsCompleted();
	}

}
