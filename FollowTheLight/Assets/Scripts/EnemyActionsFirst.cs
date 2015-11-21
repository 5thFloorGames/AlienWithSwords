using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyActionsFirst : MonoBehaviour {

	EnemyManager em;

    GameObject aoePrefab;
	NavMeshAgent nva;
	Animator animator;

	float actionTime;
	int actionDamage;
	float movementTime;

    bool playerSeen;

	void Start () {
        aoePrefab = (GameObject)Resources.Load("AreaDamage");
		nva = GetComponent<NavMeshAgent> ();
		animator = gameObject.GetComponentInChildren<Animator>();

		actionTime = 1.0f;
		actionDamage = 5;

		movementTime = 1.0f;
    }

	void Update () {

	}

	public void InitActions(GameObject manager) {
		em = manager.GetComponent<EnemyManager>();
	}

	public void TriggerActions () {
        // Debug.Log (gameObject.name + " enemy used an ability");
        playerSeen = false;
		CheckVisibleCharacters ();
        Invoke("StopMovingAndCastIfSeenPlayer", movementTime);
	}

	private bool CheckIfCharacterInSight (GameObject character){
		Vector3 direction = character.transform.position - transform.position;

		Debug.DrawRay(transform.position, direction, Color.green, 4.0f);

		RaycastHit hit;
		Physics.Raycast(transform.position, direction, out hit, direction.magnitude);
		if (hit.collider.gameObject == character) {
			//Debug.Log (gameObject.name + " sees " + character.name);
			return true;
		} else {
			//Debug.Log (gameObject.name + " does not see " + hit.collider.name);
			return false;
		}


	}

	void CheckVisibleCharacters() {

        List<GameObject> knownCharacters = new List<GameObject>();

		foreach (GameObject character in GameState.characters) {
            if (CheckIfCharacterInSight(character)) {
                knownCharacters.Add(character);
            }
		}

        if (knownCharacters.Count != 0) {
            playerSeen = true;
            if (knownCharacters.Contains(GameState.activeCharacter)) {
                MoveTowardsPosition(GameState.activeCharacter.transform.position);
            } else {
                GameObject randomPick = knownCharacters[Random.Range(0, (knownCharacters.Count-1))];
                MoveTowardsPosition(randomPick.transform.position);
            }
        }
	}

    void MoveTowardsPosition(Vector3 position) {
        nva.Resume();
        nva.destination = position;
    }

    void CastAreaDamage() {
		animator.SetTrigger ("Attack");
        GameObject spawnedAreaDamage = (GameObject)Instantiate(aoePrefab, transform.position, Quaternion.identity);
		spawnedAreaDamage.name = gameObject.name + "Aoe";
		AreaDamageBehavior adb = spawnedAreaDamage.GetComponent<AreaDamageBehavior> ();
		// time, size, damage for the aoe effect and start casting it
		float animationDelay = 1.0f;
		adb.Init (actionTime, 20f, actionDamage, animationDelay);
		Invoke ("ActionsCompletedInformManager", actionTime + animationDelay);
    }

	void ActionsCompletedInformManager() {
		em.EnemyActionsCompleted();
	}

	void StopMovingAndCastIfSeenPlayer() {
        if (playerSeen) {
            nva.Stop();
            CastAreaDamage();
        } else {
            ActionsCompletedInformManager();
        }
	}
}
