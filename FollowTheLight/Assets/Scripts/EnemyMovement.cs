using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyMovement : MonoBehaviour {

	public bool forceFollowCharacter;
	public CharacterType targetCharacter;
	public bool forceFollowFirstSeen;

	bool firstLockChecked;
	bool lockedToTarget;
	GameObject targetedCharacter;

	LayerMask layerMask;

	EnemyManager em;
	NavMeshAgent nva;
	Animator animator;
	float movementTime;
	
	bool playerSeen;
	
	void Start () {
		movementTime = 1.0f;
		firstLockChecked = false;
		lockedToTarget = false;
		em = GameObject.Find ("GameManager").GetComponent<EnemyManager>();
		nva = GetComponent<NavMeshAgent> ();
		animator = gameObject.GetComponentInChildren<Animator>();
		layerMask = (1 << 9);
		layerMask |= Physics.IgnoreRaycastLayer;
		layerMask = ~layerMask;
	}

	void Update () {
	
	}

	public void Go() {
		DetermineTargeting ();
	}

	void DetermineTargeting() {
		CheckFirstLock ();
		CheckLockedStatus ();
		if (lockedToTarget) {
			MoveTowardsPosition(targetedCharacter.transform.position);
			Invoke ("StopMovingAndAttack", movementTime);
		} else {
			CheckVisibleCharacters();
			Invoke("StopMovingAndAttackIfSeenPlayer", movementTime);
		}
	}

	void CheckFirstLock() {
		if (!firstLockChecked) {
			if (forceFollowCharacter) {
				foreach (GameObject character in GameState.characters) {
					if (character.GetComponent<CharacterState>().type == targetCharacter) {
						targetedCharacter = character.gameObject;
						lockedToTarget = true;
						break;
					}
				}
				if (targetedCharacter == null) {
					Debug.Log (targetCharacter.ToString()
					           + " not found, add a spawner or change enemy options ("
					           + gameObject.name + ")");
					lockedToTarget = false;
				}
			}
			firstLockChecked = true;
		}
	}

	void CheckLockedStatus() {
		if (lockedToTarget) {
			if (targetedCharacter.GetComponent<CharacterState>().dead) {
				forceFollowCharacter = false;
				lockedToTarget = false;
				targetedCharacter = null;
			}
		}
	}

	void CheckVisibleCharacters() {
		playerSeen = false;
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
				targetedCharacter = GameState.activeCharacter;
				if (forceFollowFirstSeen) {
					lockedToTarget = true;
				}
			} else {
				GameObject randomPick = knownCharacters[Random.Range(0, (knownCharacters.Count-1))];
				MoveTowardsPosition(randomPick.transform.position);
				targetedCharacter = randomPick;
				if (forceFollowFirstSeen) {
					lockedToTarget = true;
				}
			}
		}
	}

	bool CheckIfCharacterInSight (GameObject character) {

		Vector3 enemyView = transform.position + new Vector3(0, 2, 0);
		Vector3 direction = (character.transform.position + new Vector3 (0, 1, 0)) - enemyView;
		
		Debug.DrawRay(enemyView, direction, Color.green, 2.0f);
		RaycastHit hit;
		Physics.Raycast(enemyView, direction, out hit, (direction.magnitude + 1.0f), layerMask);
		if (hit.collider.gameObject == character) {
			return true;
		} else {
			return false;
		}
	}
	
	void MoveTowardsPosition(Vector3 position) {
		animator.SetBool ("Walking", true);
		nva.Resume();
		nva.destination = position;
	}

	void StopMovingAndAttackIfSeenPlayer() {
		if (playerSeen) {
			StopMovingAndAttack();
		} else {
			ActionsCompletedInformManager();
		}
	}
	
	void StopMovingAndAttack() {
		animator.SetBool ("Walking", false);
		nva.Stop();
		gameObject.SendMessage("MovingCompleteStartAttack", targetedCharacter);
	}

	void ActionsCompletedInformManager() {
		em.EnemyActionsCompleted();
	}
}
