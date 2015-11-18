using UnityEngine;
using System.Collections;

public class FirstEnemyActions : MonoBehaviour {

	EnemyManager em;
    GameObject aoePrefab;
	NavMeshAgent nva;

	int knownCharacters;
	Transform char1;
	Transform char2;
	Transform char3;

	float actionTime;
	int actionDamage;
	float movementTime;

	void Start () {
        aoePrefab = (GameObject)Resources.Load("AreaDamage");
		nva = GetComponent<NavMeshAgent> ();

		actionTime = 1.0f;
		actionDamage = 5;

		movementTime = 1.0f;

		knownCharacters = 0;
		CheckPlayerPositions ();
    }

	void Update () {

	}

	public void InitActions(GameObject manager) {
		em = manager.GetComponent<EnemyManager>();
	}

	public void TriggerActions () {
		// Debug.Log (gameObject.name + " enemy used an ability");
		MoveTowardsPlayer ();
        Invoke("StopMovingThenCast", movementTime);
	}

	void CheckPlayerPositions () {
		if (GameState.amountOfCharacters > knownCharacters) {
			char1 = GameObject.Find("Character1").transform;
			knownCharacters += 1;
			
			if (GameState.amountOfCharacters > 1) {
				char2 = GameObject.Find("Character2").transform;
				knownCharacters += 1;
			}
			
			if (GameState.amountOfCharacters > 2) {
				char3 = GameObject.Find("Character3").transform;
				knownCharacters += 1;
			}
			
			if (GameState.amountOfCharacters > 3) {
				Debug.Log ("enemy trying to look for more characters");
			}
		}
	}

	void MoveTowardsPlayer() {
		nva.Resume ();
		nva.destination = char1.position;
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

	void StopMovingThenCast() {
		nva.Stop ();
		CastAreaDamage ();
	}
}
