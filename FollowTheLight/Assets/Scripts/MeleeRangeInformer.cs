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

	void Start () {
        cas = transform.root.GetComponent<CharacterActionsSecond>();
        cldr = GetComponent<MeshCollider>();
		wouldHitEnemy = false;
		hitList = new List<GameObject> ();
	}

	void OnTriggerEnter (Collider other) {
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
        if (cldr == null) {
            cldr = GetComponent<MeshCollider>();
        }
        cldr.enabled = true;
        if (hitList != null) {
            foreach (GameObject enemyObj in hitList) {
                enemyObj.SendMessage("AimedAt", transform.root.gameObject);
            }
        }
	}
}
