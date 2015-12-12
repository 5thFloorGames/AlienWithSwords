using UnityEngine;
using System.Collections;

public class HealerDamageCollisionDetector : MonoBehaviour {

    LaserController lc;

	void Start () {
        lc = transform.parent.parent.FindChild("Laser").GetComponent<LaserController>();
	}

    void OnParticleCollision(GameObject other) {
        lc.AttackParticlesCollided();
    }
}
