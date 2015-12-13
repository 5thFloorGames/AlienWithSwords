using UnityEngine;
using System.Collections;

public class LevelLoader : MonoBehaviour {

    public LevelObjective levelObjective;

    GameObject gameManager;
	GameObject userInterface;
    GameObject eventSystem;
	
	void Awake () {

		userInterface = GameObject.Find("UserInterface");
		if (userInterface == null) {
			userInterface = Instantiate((GameObject)Resources.Load("UserInterface"));
			userInterface.name = "UserInterface";
		}

		gameManager = GameObject.Find("GameManager");
		if (gameManager == null) {
			gameManager = Instantiate((GameObject)Resources.Load("GameManager"));
			gameManager.name = "GameManager";
		}

        eventSystem = GameObject.Find("EventSystem");
        if (eventSystem == null) {
            eventSystem = Instantiate((GameObject)Resources.Load("EventSystem"));
            eventSystem.name = "EventSystem";
        }
	}

    void Start() {
    }

	void Update () {
	
	}
}
