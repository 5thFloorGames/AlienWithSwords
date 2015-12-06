using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TutorialTextHandler : MonoBehaviour {

    UserInterfaceManager uim;
    Text text;
    string toWrite;
    string str;

	void Start () {
        uim = FindObjectOfType<UserInterfaceManager>();
        text = GameObject.Find("Tutorial").GetComponentInChildren<Text>();
	}

	void Update () {
    }

    public void ShowText(string givenString) {
        toWrite = givenString;
        str = "";
        StartCoroutine(GenerateText());
    }

    public void ClearText() {
        str = "";
        text.text = str;
    }

    IEnumerator GenerateText() {
        while (str.Length != toWrite.Length) {
            str += toWrite[str.Length];
            text.text = str;
            yield return new WaitForSeconds(0.05f);
        }
    }
}
