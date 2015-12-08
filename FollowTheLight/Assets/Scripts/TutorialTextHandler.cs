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

   public bool actionsActivated;

    bool outOfMovement;
    bool outOfActionPoints;

    int oomTimes;
    int ooaTimes;

    bool endOfTurnShown;


	void Start () {
        text = transform.FindChild("Text").GetComponent<Text>();
        sideText = transform.FindChild("SideText").GetComponent<Text>();
        actionsActivated = false;
        outOfMovement = false;
        outOfActionPoints = false;
        endOfTurnShown = false;
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
        if (!actionsActivated) {
            if (givenString.Contains("Left click")) {
                actionsActivated = true;
            }
        }
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
            if (!toWriteSide[strSide.Length-1].Equals("\n")) {
                yield return new WaitForSeconds(0.01f);
            } else {
                yield return new WaitForSeconds(2.0f);
            }
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
        StopCoroutine("EndTurnTextWithDelay");
    }

    public void OutOfActionsInform() {
        outOfActionPoints = true;
        ooaTimes += 1;
        CheckStatus();
    }

    public void ActionsRestored() {
        outOfActionPoints = false;
        ClearSideText();
        StopCoroutine("EndTurnTextWithDelay");
    }

    void CheckStatus() {
        if (outOfMovement && !actionsActivated) {
            EndTurnText();
        } else if (outOfActionPoints && ooaTimes < 3 && oomTimes > 1) {
            StartCoroutine("EndTurnTextWithDelay");
        } else if (outOfActionPoints && outOfMovement) {
            StartCoroutine("EndTurnTextWithDelay");
        }
    }

    IEnumerator EndTurnTextWithDelay() {
        yield return new WaitForSeconds(3.0f);
        EndTurnText();
    }


    void EndTurnText() {
        if (endOfTurnShown) {
            ShowSideTextInstant("Tab");
        } else {
            ShowSideText("To end your turn__press tab");
            endOfTurnShown = true;
        }
    }
}
