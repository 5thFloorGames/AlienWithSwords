using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyMovement : MonoBehaviour {

    public bool fixateAfterSeeing;
    public bool focusCharAtStart;
	public CharacterType targetCharacter;
    public float movementTime;

	bool firstLockChecked;
	bool lockedToTarget;
	[HideInInspector] public GameObject targetedCharacter;

	LayerMask layerMask;

	EnemyManager em;
	NavMeshAgent nva;
	Animator animator;
	
	bool playerSeen;
	
	void Start () {
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

    public void DisableNva() {
        if (nva == null) {
            nva = GetComponent<NavMeshAgent>();
        }
        nva.enabled = false;
    }

    public void EnableNva() {
        if (nva == null) {
            nva = GetComponent<NavMeshAgent>();
        }
        nva.enabled = true;
    }

    public void CharacterAimedYou(GameObject character) {
        CheckFirstLock();
        CheckLockedStatus();
        if (!lockedToTarget && fixateAfterSeeing) {
            targetedCharacter = character;
            lockedToTarget = true;
        }
    }

	void DetermineTargeting() {
		CheckFirstLock ();
		CheckLockedStatus ();
        RefocusIfNecessary();
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
			if (focusCharAtStart) {
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
				focusCharAtStart = false;
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
				if (fixateAfterSeeing) {
					lockedToTarget = true;
				}
			} else {
				GameObject randomPick = knownCharacters[Random.Range(0, (knownCharacters.Count-1))];
				MoveTowardsPosition(randomPick.transform.position);
				targetedCharacter = randomPick;
				if (fixateAfterSeeing) {
					lockedToTarget = true;
				}
			}
		}
	}

	public bool CheckIfCharacterInSight (GameObject character) {

		Vector3 enemyView = transform.position + new Vector3(0, 0.8f, 0);
		Vector3 direction = (character.transform.position + new Vector3 (0, 0.7f, 0)) - enemyView;
		RaycastHit hit;
		Physics.Raycast(enemyView, direction, out hit, (direction.magnitude + 1.0f), layerMask);
        if (hit.collider == null) {
            return false;
        }
		if (hit.collider.gameObject == character) {
			return true;
		} else {
			return false;
		}
	}

    void RefocusIfNecessary() {
        GameObject target = targetedCharacter;
        if (lockedToTarget && fixateAfterSeeing) {
            if (!CheckIfCharacterInSight(target)) {
                lockedToTarget = false;
                targetedCharacter = null;
                CheckVisibleCharacters();
                if (!lockedToTarget) {
                    lockedToTarget = true;
                    targetedCharacter = target;
                }
            }
        }
    }
	
	void MoveTowardsPosition(Vector3 position) {
		animator.SetBool ("Walking", true);
        nva.enabled = true;
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
        nva.enabled = false;
		gameObject.SendMessage("MovingCompleteStartAttack", targetedCharacter);
	}

	void ActionsCompletedInformManager() {
		em.EnemyActionsCompleted();
	}
}
