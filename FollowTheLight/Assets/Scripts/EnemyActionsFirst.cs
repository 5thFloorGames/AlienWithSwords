using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyActionsFirst : MonoBehaviour {

	EnemyManager em;

    GameObject aoePrefab;
	NavMeshAgent nva;
	Animator animator;

	int actionDamage;
	float movementTime;
    float aoeLifetime;

    bool playerSeen;

	void Start () {
        aoePrefab = (GameObject)Resources.Load("AreaDamage");
		nva = GetComponent<NavMeshAgent> ();
		animator = gameObject.GetComponentInChildren<Animator>();

		actionDamage = 5;
        aoeLifetime = 1.7f;

        movementTime = 1.0f;
    }

	void Update () {

	}

	public void InitActions(GameObject manager) {
		em = manager.GetComponent<EnemyManager>();
	}

	public void TriggerActions () {
        playerSeen = false;
		CheckVisibleCharacters ();
        Invoke("StopMovingAndCastIfSeenPlayer", movementTime);
	}

	bool CheckIfCharacterInSight (GameObject character) {
        Vector3 enemyView = transform.position + new Vector3(0, 2, 0);
		Vector3 direction = (character.transform.position + new Vector3 (0, 1, 0)) - enemyView;

		Debug.DrawRay(enemyView, direction, Color.green, 2.0f);
		RaycastHit hit;
		Physics.Raycast(enemyView, direction, out hit, (direction.magnitude + 1.0f));
		if (hit.collider.gameObject == character) {
			return true;
		} else {
			return false;
		}
	}

	void CheckVisibleCharacters() {

        List<GameObject> knownCharacters = new List<GameObject>();

		foreach (GameObject character in GameState.characters) {
            if (CheckIfCharacterInSight(character) && !character.GetComponent<CharacterState>().dead) {
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
		animator.SetBool ("Walking", true);
        nva.Resume();
        nva.destination = position;
    }

    void CastAreaDamage() {
		animator.SetTrigger ("Attack");
        GameObject spawnedAreaDamage = (GameObject)Instantiate(aoePrefab, transform.position, Quaternion.identity);
		spawnedAreaDamage.name = gameObject.name + "Aoe";
		AreaDamageBehavior adb = spawnedAreaDamage.GetComponent<AreaDamageBehavior> ();
		float animationDelay = 1.0f;
		adb.Init (actionDamage, animationDelay, aoeLifetime);
		Invoke ("ActionsCompletedInformManager", animationDelay + aoeLifetime);
    }

	void ActionsCompletedInformManager() {
		em.EnemyActionsCompleted();
	}

	void StopMovingAndCastIfSeenPlayer() {
        if (playerSeen) {
			animator.SetBool ("Walking", false);
            nva.Stop();
            CastAreaDamage();
        } else {
            ActionsCompletedInformManager();
        }
	}
}
