using UnityEngine;
using System.Collections;

public class TriggerGameStart : MonoBehaviour {

    AnnouncementManager am;
    bool triggered;

	void Start () {
        triggered = false;
        am = GameObject.Find("GameManager").GetComponent<AnnouncementManager>();
	}

	void OnTriggerEnter (Collider other) {
        if (!triggered && other.tag == "Player") {
            triggered = true;
            am.UserAgreementFormTriggered();
        }
    }

}
