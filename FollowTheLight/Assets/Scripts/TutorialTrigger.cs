using UnityEngine;
using System.Collections;

public class TutorialTrigger : MonoBehaviour {
    public string givenString;
    public string uiEventSendMessage;
    public string flashMessage;
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
                if (uiEventSendMessage != "") {
                    GameObject.Find("TutorialUIEvents").SendMessage(uiEventSendMessage);
                }
                if (flashMessage == "FlashHealth" ) {
                    GameObject.Find("UserInterface").GetComponent<UserInterfaceManager>().FlashHealth("Character1", 3);
                } else if (flashMessage == "FlashMovement") {
                    GameObject.Find("UserInterface").GetComponent<UserInterfaceManager>().FlashMovement("Character1", 3);
                } else if (flashMessage == "FlashActionPoints") {
                    GameObject.Find("UserInterface").GetComponent<UserInterfaceManager>().FlashActionPoints("Character1", 3);
                }
                triggered = true;
            }
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