using UnityEngine;
using System.Collections;

public class EnemyShotDamages : MonoBehaviour {
	
	EnemyActionsSecond eas;
	float lifeTime;
	float startTime;
	//int damage;
    //float delayFromEnemy;

    public void Init(int damageAmount, float delay, float lifetimeFromEnemy, EnemyActionsSecond caller) {
		startTime = Time.time;
        lifeTime = lifetimeFromEnemy;
		eas = caller;
        //damage = damageAmount;
		//delayFromEnemy = delay;
    }

    void Start () {
	
	}
	
	void Update () {
		if (lifeTime > 0.1f) {
			if (lifeTime < Time.time - startTime) {
				CreateHitEffect();
				eas.ShotMissed(gameObject);
				Destroy(gameObject);
			}
		}
	}

    void OnTriggerEnter(Collider other) {
		if (other.tag == "Player") {
			eas.ShotCollided (gameObject);
	        CreateHitEffect();
	        Destroy(gameObject);
		}
    }

    void CreateHitEffect() {
        GameObject prefab = (GameObject)Resources.Load("EnemyShotHitEffect");
        Instantiate(prefab, transform.position + (transform.rotation * new Vector3(0, 0, -0.5f)), Quaternion.identity);
    }
}
