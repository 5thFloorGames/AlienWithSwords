using UnityEngine;
using System.Collections;

public class EnemyActions : MonoBehaviour {
    GameObject explosion;

	void Start () {
        explosion = (GameObject)Resources.Load("AreaDamage");
	}

	public void TriggerActions () {
		Debug.Log (gameObject.name + " enemy used an ability");
        CastExplosion();
	}

    void CastExplosion() {
        Instantiate(explosion, transform.position, Quaternion.identity);
    }
}
