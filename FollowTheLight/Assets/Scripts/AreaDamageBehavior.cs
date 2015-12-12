using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AreaDamageBehavior : MonoBehaviour {

    ParticleSystem explosion;
    GameObject caster;

	int damage;
    float delayFromEnemy;
    float lifeTime;

	public void Init (int damageAmount, float delay, float timeFromEnemy, GameObject goInfo) {
        caster = goInfo;
        explosion = GetComponent<ParticleSystem>();
        delayFromEnemy = delay;
        lifeTime = timeFromEnemy;
		damage = damageAmount;
        StartCoroutine(ExplosionOperations());
	}

	void OnTriggerEnter (Collider other) {
		if (other.tag == "Player" && (other.GetType() == typeof(CapsuleCollider))) {
            if (CheckIfCharacterInSight(other.gameObject)) {
                List <object> info = new List<object>();
                object dmgObject = damage;
                info.Add(dmgObject);
                info.Add(caster);
                other.gameObject.SendMessageUpwards("TakeDamage", info);
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
        Physics.Raycast(transform.position, direction, out hit, (direction.magnitude + 1f), ~(1 << 9));

        if (hit.collider.gameObject == character) {
            return true;
        } else {
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
