using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyActionsFirst : MonoBehaviour {

	public int actionDamage;

	EnemyManager em;
	EnemyMovement move;
	Animator animator;

    GameObject aoePrefab;
    float aoeLifetime;

	EnemySoundController esc;

	void Start () {
		move = GetComponent<EnemyMovement> ();
        aoePrefab = (GameObject)Resources.Load("AreaDamage");
		animator = gameObject.GetComponentInChildren<Animator>();

		if (actionDamage == 0) {
			actionDamage = 5;
		}
		//actionDamage = 5;
        aoeLifetime = 1.7f;
        
    }

	void Update () {

	}

	public void InitActions(GameObject manager) {
		esc = GetComponent<EnemySoundController> ();
		em = manager.GetComponent<EnemyManager>();
	}

	public void TriggerActions () {
		esc.PlayAttackQuote ();
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

		adb.Init (actionDamage, animationDelay, aoeLifetime, gameObject);
		Invoke ("PlayExplosionSound", animationDelay);
		Invoke ("ActionsCompletedInformManager", animationDelay + aoeLifetime);
    }

	void PlayExplosionSound() {
		esc.PlayExplosionSFX ();
	}

	void ActionsCompletedInformManager() {
		em.EnemyActionsCompleted();
	}

}
