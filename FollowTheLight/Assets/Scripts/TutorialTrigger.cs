using UnityEngine;
using System.Collections;

public class TutorialTrigger : MonoBehaviour {
    public string givenString;
    TutorialTextHandler tth;

	void Start () {
        tth = GameObject.Find("Tutorial").GetComponent<TutorialTextHandler>();
        givenString = givenString.Replace("__", "\n");
    }

	void OnTriggerEnter (Collider other) {
        if (other.tag == "Player" && other.GetType() == typeof(CapsuleCollider)) {
            tth.ShowText(givenString);
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.tag == "Player" && other.GetType() == typeof(CapsuleCollider)) {
            tth.ClearText();
        }
    }
}