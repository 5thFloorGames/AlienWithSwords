using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TutorialTextHandler : MonoBehaviour {

    Text text;
    Text sideText;

    string toWrite;
    string str;

    string toWriteSide;
    string strSide;


    bool outOfMovement;
    bool apShown;
    bool outOfActionPoints;

    int oomTimes;
    int ooaTimes;


	void Start () {
        text = transform.FindChild("Text").GetComponent<Text>();
        sideText = transform.FindChild("SideText").GetComponent<Text>();
        outOfMovement = false;
        apShown = false;
        outOfActionPoints = false;
        oomTimes = 0;
        ooaTimes = 0;
    }

	void Update () {

    }



    public void ShowTextInstant(string givenString) {
        toWrite = givenString;
        str = givenString;
        text.text = givenString;
    }

    public void ShowText(string givenString) {
        toWrite = givenString.Replace("__", "\n");
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
            yield return new WaitForSeconds(0.01f);
        }
    }



    public void ShowSideTextInstant(string givenString) {
        toWriteSide = givenString;
        strSide = givenString;
        sideText.text = givenString;
    }

    public void ShowSideText(string givenString) {
        toWriteSide = givenString.Replace("__", "\n");
        strSide = "";
        StartCoroutine(GenerateSideText());
    }

    public void ClearSideText() {
        strSide = "";
        sideText.text = strSide;
    }

    IEnumerator GenerateSideText() {
        while (strSide.Length != toWriteSide.Length) {
            strSide += toWriteSide[strSide.Length];
            sideText.text = strSide;
            yield return new WaitForSeconds(0.01f);
        }
    }

    public void OutOfMovementInform() {
        outOfMovement = true;
        oomTimes += 1;
        CheckStatus();
    }

    public void MovementRestored() {
        outOfMovement = false;
        ClearSideText();
    }

    public void OutOfActionsInform() {
        outOfActionPoints = true;
        ooaTimes += 1;
    }

    public void ActionsRestored() {
        outOfActionPoints = false;
        ClearSideText();
    }

    void CheckStatus() {
        if (outOfMovement && oomTimes < 3) {
            EndTurnText();
        } else if (outOfActionPoints && ooaTimes < 3) {
            EndTurnText();
        } else if (outOfActionPoints && outOfMovement) {
            EndTurnText();
        }
    }


    void EndTurnText() {
        ShowSideText("To end your turn__press enter");
    }
}
