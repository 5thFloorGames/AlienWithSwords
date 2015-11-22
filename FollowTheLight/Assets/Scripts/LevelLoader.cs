using UnityEngine;
using System.Collections;

public enum LevelObjective { DestroyEnemies, GetToLevelEnd };

public class LevelLoader : MonoBehaviour {

    public LevelObjective levelObjective;

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

    void Start() {
        gameManager.GetComponent<GameManager>().SetLevelObjective(levelObjective);
    }

	void Update () {
	
	}
}
