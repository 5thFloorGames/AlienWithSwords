using UnityEngine;
using System.Collections;

public class TriggerUserAgreementForm : MonoBehaviour {

    AnnouncementManager am;
    bool triggered;

	void Start () {
        triggered = false;
        am = GameObject.Find("GameManager").GetComponent<AnnouncementManager>();
	}

	void OnTriggerEnter (Collider other) {
        if (!triggered) {
            
            triggered = true;
        }
    }
}
