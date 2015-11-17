using UnityEngine;
using System.Collections;

public class LevelLoader : MonoBehaviour {
	GameObject gameManager;
	GameObject userInterface;
	
	void Awake () {

		userInterface = GameObject.Find("UserInterface");
		if (userInterface == null) {
			userInterface = Instantiate<GameObject>((GameObject)Resources.Load("UserInterface"));
			userInterface.name = "UserInterface";
		}

		gameManager = GameObject.Find("GameManager");
		if (gameManager == null) {
			gameManager = Instantiate<GameObject>((GameObject)Resources.Load("GameManager"));
			gameManager.name = "GameManager";
		}
	}

	void Update () {
	
	}
}
