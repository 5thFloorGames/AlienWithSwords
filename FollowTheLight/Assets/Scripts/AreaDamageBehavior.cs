using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AreaDamageBehavior : MonoBehaviour {

    ParticleSystem explosion;

	int damage;
    float delayFromEnemy;
    float lifeTime;

	public void Init (int damageAmount, float delay, float timeFromEnemy) {
        explosion = GetComponent<ParticleSystem>();
        delayFromEnemy = delay;
        lifeTime = timeFromEnemy;
		damage = damageAmount;
        StartCoroutine(ExplosionOperations());
	}

	void OnTriggerEnter (Collider other) {
		if (other.tag == "Player" && (other.GetType() == typeof(CapsuleCollider))) {
            if (CheckIfCharacterInSight(other.gameObject)) {
                other.gameObject.SendMessageUpwards("TakeDamage", damage);
            }
		}
	}

	void Start () {

	}

	void Update () {
		
	}

    bool CheckIfCharacterInSight(GameObject character) {
        Vector3 direction = (character.transform.position + new Vector3(0, 1, 0)) - (transform.position + new Vector3(0, 1, 0));

        Debug.DrawRay((transform.position + new Vector3(0, 1, 0)), direction, Color.red, 4.0f);

        RaycastHit hit;
        Physics.Raycast(transform.position, direction, out hit, (direction.magnitude + 1f));
        if (hit.collider.gameObject == character) {
            //Debug.Log (gameObject.name + " sees " + character.name);
            return true;
        } else {
            //Debug.Log (gameObject.name + " does not see " + hit.collider.name);
            return false;
        }
    }

    IEnumerator ExplosionOperations() {
        yield return new WaitForSeconds(delayFromEnemy);
        explosion.Emit(700);
        gameObject.GetComponent<Collider>().enabled = true;
        yield return new WaitForSeconds(lifeTime);
        Destroy(gameObject);
    }
}
