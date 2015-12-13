using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyActionsSecond : MonoBehaviour {

    public int actionDamage;

    EnemyManager em;
    EnemyMovement move;
    Animator animator;
	LayerMask layerMask;

    float shotLifetime;
    GameObject shotPrefab;

	GameObject bulletShotTarget;
	GameObject bulletShot;
    bool bulletFlying;

	EnemyActionsSecond thisScript;
    EnemySoundController esc;

    void Start() {
		layerMask = (1 << 9);
		layerMask |= Physics.IgnoreRaycastLayer;
		layerMask = ~layerMask;

		thisScript = GetComponent<EnemyActionsSecond>();
        move = GetComponent<EnemyMovement>();
        animator = gameObject.GetComponentInChildren<Animator>();

        shotLifetime = 4.0f;
        shotPrefab = (GameObject)Resources.Load("EnemyShot");

        if (actionDamage == 0) {
            actionDamage = 5;
        }

        esc = GetComponent<EnemySoundController>();
    }

    void Update() {

    }

    public void InitActions(GameObject manager) {
        bulletFlying = false;
        em = manager.GetComponent<EnemyManager>();
    }

    public void TriggerActions() {
        move.Go();
    }

	void ShotAtTarget(GameObject target, GameObject bullet) {

	}

	public void ShotCollided(GameObject go) {
        if (bulletFlying) {
            bulletFlying = false;
            List<object> info = new List<object>();
            object dmgObject = actionDamage;
            info.Add(dmgObject);
            info.Add(gameObject);
            move.targetedCharacter.SendMessageUpwards("TakeDamage", info);
            ActionsCompletedInformManager();
            
        }
	}

	public void ShotMissed(GameObject bullet) {
		ActionsCompletedInformManager ();
	}

    public void MovingCompleteStartAttack(GameObject target) {
        StartCoroutine(ShootAtCharacter(target));
    }


    IEnumerator ShootAtCharacter(GameObject target) {
		if (move.CheckIfCharacterInSight (target)) {
			Vector3 start = transform.position + new Vector3 (0, 0.8f, 0);
			Vector3 end = target.transform.position + target.transform.rotation * new Vector3 (0, 0.7f, 0);
			Vector3 direction = (end - start).normalized;

			animator.SetTrigger ("Attack");
            esc.PlayShootingQuote();
			//Invoke("ActionsCompletedInformManager", shotLifetime);
			yield return new WaitForSeconds (0.5f);
			GameObject shot = (GameObject)Instantiate (shotPrefab, (start + direction), Quaternion.Inverse (transform.rotation));
			shot.name = gameObject.name + "Shot";
			shot.GetComponent<EnemyShotDamages> ().Init (actionDamage, 0.0f, shotLifetime, thisScript);
			Rigidbody shotrb = shot.GetComponent<Rigidbody> ();
			shotrb.AddForce (direction * 500.0f);

            bulletFlying = true;
		} else {
			ActionsCompletedInformManager();
		}
    }

    void ActionsCompletedInformManager() {
        em.EnemyActionsCompleted();
    }
}