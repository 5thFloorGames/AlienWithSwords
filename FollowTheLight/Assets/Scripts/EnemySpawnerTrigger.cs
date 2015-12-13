using UnityEngine;
using System.Collections;

public class EnemySpawnerTrigger : MonoBehaviour {

    public int spawnNumber;
    EnemyManager em;
    bool triggered;

	void Start () {
        triggered = false;
        em = GameObject.Find("GameManager").GetComponent<EnemyManager>();
	}
	
	void Update () {
	
	}

    void OnTriggerEnter (Collider other) {
        if (!triggered && other.tag == "Player") {
            triggered = true;
            em.SpawnTriggered(spawnNumber);
        }
    }
}
