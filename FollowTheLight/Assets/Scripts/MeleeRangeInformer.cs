using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum AttackType { Strike, Slash };

public class MeleeRangeInformer : MonoBehaviour {
	public AttackType attackType;
	bool wouldHitEnemy;
	List<GameObject> hitList;
    CharacterActionsSecond cas;
    Collider cldr;
    GameObject caster;

	void Start () {
        cas = transform.root.GetComponent<CharacterActionsSecond>();
        cldr = GetComponent<MeshCollider>();
		wouldHitEnemy = false;
		hitList = new List<GameObject> ();
        caster = transform.root.gameObject;
	}

	void OnTriggerEnter (Collider other) {
        //if (hitList.Contains(other.gameObject)) {
        //    return;
        //}
        if (((other.GetType() == typeof(CapsuleCollider)) && other.tag == "Enemy") || (other.tag == "Player" && (other.GetType() == typeof(CapsuleCollider)))) {
            GameObject otherObj = other.transform.root.gameObject;
            otherObj.SendMessage("AimedAt", transform.root.gameObject);
            hitList.Add(otherObj);
            CheckHitListSize();
        }
    }

	void OnTriggerExit (Collider other) {
		if (((other.GetType() == typeof(CapsuleCollider)) && other.tag == "Enemy") || (other.tag == "Player" && (other.GetType() == typeof(CapsuleCollider)))) {
            GameObject otherObj = other.transform.root.gameObject;
            otherObj.SendMessage("NotAimedAt");
            hitList.Remove(otherObj);
			CheckHitListSize();
		}
	}

	void CheckHitListSize() {
		if (hitList.Count > 0) {
			if (!wouldHitEnemy) {
				wouldHitEnemy = true;
                cas.EnemiesEnteredAimRange();
            }
		} else {
			if (wouldHitEnemy) {
				wouldHitEnemy = false;
                cas.NoEnemiesInAimRange();
			}
		}
	}

    public void DealDamageToHitList(int damage) {
        List<GameObject> damageDealtTo = new List<GameObject>();
        if (hitList != null) {
            foreach (GameObject enemyObj in hitList) {
                if (damageDealtTo.Contains(enemyObj)) {
                    continue;
                }
                List<object> info = new List<object>();
                object dmgObject = damage;
                info.Add(dmgObject);
                info.Add(caster);
                enemyObj.SendMessage("TakeDamage", info);
                damageDealtTo.Add(enemyObj);
            }
        }
    }

	public void DeactivateTheHitList() {
		if (hitList != null) {
			foreach (GameObject enemyObj in hitList) {
				enemyObj.SendMessage ("NotAimedAt");
			}
		}
        if (cldr == null) {
            cldr = GetComponent<MeshCollider>();
        }
        cldr.enabled = false;
    }

	public void ActivateTheHitList() {
        List<GameObject> toRemove = new List<GameObject>();
        if (hitList != null) {
            foreach (GameObject enemyObj in hitList) {
                Bounds bounds = cldr.bounds;
                if (bounds.Contains(enemyObj.transform.position)) {
                    if (enemyObj.GetComponent<CharacterState>() != null) {
                        enemyObj.SendMessage("AimedAt", transform.root.gameObject);
                    } else if (enemyObj.GetComponent<EnemyState>().dead != true) {
                        enemyObj.SendMessage("AimedAt", transform.root.gameObject);
                    }
                } else {
                    toRemove.Add(enemyObj);
                }
            }

            foreach (GameObject obj in toRemove) {
                hitList.Remove(obj);
            }

            Debug.Log(hitList.Count);
        }

        if (cldr == null) {
            cldr = GetComponent<MeshCollider>();
        }
        cldr.enabled = true;
    }

    public void PlayerTurnEnded() {
        DeactivateTheHitList();
    }

    public void PlayerTurnStarted() {
        ActivateTheHitList();
    }

    void ResetActions() {
        //DeactivateTheHitList();
        //ActivateTheHitList();
    }
}
