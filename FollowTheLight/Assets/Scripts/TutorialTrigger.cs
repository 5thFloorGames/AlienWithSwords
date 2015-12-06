using UnityEngine;
using System.Collections;

public class TutorialTrigger : MonoBehaviour {
    public string givenString;
    public string uiEventSendMessage;
    public float delay;
    TutorialTextHandler tth;

    bool triggered;

	void Start () {
        triggered = false;
        tth = GameObject.Find("Guide").GetComponent<TutorialTextHandler>();
        givenString = givenString.Replace("__", "\n");
    }

	void OnTriggerEnter (Collider other) {
        if (!triggered) {
            if (other.tag == "Player" && other.GetType() == typeof(CapsuleCollider)) {
                StartCoroutine(GiveTextToHandler());
                if (uiEventSendMessage != null) {
                    Debug.Log(uiEventSendMessage + " trying to send this to ui event");
                }
            }
            triggered = true;
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.tag == "Player" && other.GetType() == typeof(CapsuleCollider)) {
            tth.ClearText();
        }
    }

    IEnumerator GiveTextToHandler() {
        yield return new WaitForSeconds(delay);
        tth.ShowText(givenString);
    }
}