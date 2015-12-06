using UnityEngine;
using System.Collections;

public class TutorialUIEvents : MonoBehaviour {
    GameObject uim;
    GameObject characterInfo;

	void Awake () {
        uim = GameObject.Find("UserInterface");
        characterInfo = uim.transform.FindChild("CharacterPanel").FindChild("Character1").gameObject;
        HideUnnecessaryThings();
	}

	void Update () {
	
	}

    void HideUnnecessaryThings() {
        HideHealth();
        HideActions();
    }
    
    void HideHealth() {
        GameObject meter = characterInfo.transform.FindChild("HealthMeter").gameObject;
        GameObject meterBg = characterInfo.transform.FindChild("HealthMeterBackground").gameObject;
        GameObject meterText = characterInfo.transform.FindChild("HealthText").gameObject;
        meter.SetActive(false);
        meterBg.SetActive(false);
        meterText.SetActive(false);
    }

    void HideActions() {
        GameObject points = characterInfo.transform.FindChild("ActionPoints").gameObject;
        points.SetActive(false);
    }

}
