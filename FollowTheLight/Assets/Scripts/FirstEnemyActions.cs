using UnityEngine;
using System.Collections;

public class FirstEnemyActions : MonoBehaviour {

	EnemyManager em;

    GameObject aoePrefab;
	NavMeshAgent nva;
	Animator animator;

	float actionTime;
	int actionDamage;
	float movementTime;

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
		MoveTowardsPlayer ();
        Invoke("StopMovingThenCast", movementTime);
	}

	private bool CheckIfPlayerInSight (Vector3 characterPos, GameObject character){
		Vector3 direction = characterPos - transform.position;

		Debug.DrawRay(transform.position, direction, Color.green, 4.0f);

		RaycastHit hit;
		Physics.Raycast(transform.position, direction, out hit, direction.magnitude);
		if (hit.collider.gameObject == character) {
			Debug.Log ("player found");
			return true;
		} else {
			Debug.Log ("player not found");
			return false;
		}


	}

	void MoveTowardsPlayer() {
		foreach (GameObject character in CharacterManager.Base.characters) {
			Debug.Log (CheckIfPlayerInSight(character.transform.position, character));
		}


		var distance = Vector3.Distance(transform.position, GameState.activeCharacter.transform.position);
		if (distance < 20) {
			nva.Resume ();
			nva.destination = GameState.activeCharacter.transform.position;
		}
	}

    void CastAreaDamage() {
		animator.SetTrigger ("Attack");
        GameObject spawnedAreaDamage = (GameObject)Instantiate(aoePrefab, transform.position, Quaternion.identity);
		spawnedAreaDamage.name = gameObject.name + "AOE";
		AreaDamageBehavior adb = spawnedAreaDamage.GetComponent<AreaDamageBehavior> ();
		// time, size, damage for the aoe effect and start casting it
		float animationDelay = 1.0f;
		adb.Init (actionTime, 20f, actionDamage, animationDelay);
		Invoke ("ActionsCompletedInformManager", actionTime + animationDelay);
    }

	void ActionsCompletedInformManager() {
		em.EnemyActionsCompleted();
	}

	void StopMovingThenCast() {
		nva.Stop ();
		CastAreaDamage ();
	}
}
