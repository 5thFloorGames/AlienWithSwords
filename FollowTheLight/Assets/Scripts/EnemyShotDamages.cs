using UnityEngine;
using System.Collections;

public class EnemyShotDamages : MonoBehaviour {

    int damage;
    float delayFromEnemy;
    float lifeTime;

    public void Init(int damageAmount, float delay, float lifetimeFromEnemy) {

        delayFromEnemy = delay;
        lifeTime = lifetimeFromEnemy;
        damage = damageAmount;
    }

    void Start () {
	
	}
	
	void Update () {
	
	}

    void OnTriggerEnter(Collider other) {
        Debug.Log(other.name);
        if (other.tag == "Player" && (other.GetType() == typeof(CapsuleCollider))) {
            other.gameObject.SendMessageUpwards("TakeDamage", damage);
        }
        CreateHitEffect();
        Destroy(gameObject);
    }

    void CreateHitEffect() {
        GameObject prefab = (GameObject)Resources.Load("BulletHitEffect");
        Instantiate(prefab, transform.position + (transform.rotation * new Vector3(0, 0, -0.5f)), Quaternion.identity);
    }
}
