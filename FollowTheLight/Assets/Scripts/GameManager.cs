using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	CharacterManager cm;
	EnemyManager em;
	UserInterfaceManager uim;
	bool levelCompleted;

	void Awake() {
		DontDestroyOnLoad (gameObject);
        GameState.SetLevel(Application.loadedLevel);
    }

	void Start () {
        #if !UNITY_EDITOR
            Cursor.visible = false;
        #endif
        cm = gameObject.GetComponent<CharacterManager> ();
		em = gameObject.GetComponent<EnemyManager> ();
		uim = GameObject.Find ("UserInterface").GetComponent<UserInterfaceManager>();
		levelCompleted = false;
		Invoke("LateStart", 0.1f);
	}

	void LateStart () {
		StartPlayerTurn ();
	}

	void Update () {

        #if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.N)) {
            LevelComplete();
        }
        #endif

        if (Input.GetButton ("Cancel")) {
			QuitGame();
		}
		if (GameState.playersTurn) {
			if (Input.GetButton ("Submit")) {
				StartEnemyTurn();
			}
		}
	}

	void OnLevelWasLoaded(int level) {

		if (level == 0) {
			Destroy(gameObject);
		}
		levelCompleted = false;
        if (uim != null) {
            uim.HideLevelCompletedUI();
        }
        Invoke("StartPlayerTurn", 0.1f);
	}
	
	public void LevelComplete() {
		if (!levelCompleted) {
			levelCompleted = true;

			uim.ShowLevelCompletedUI();
			GameState.playersTurn = false;
			if (GameState.GetLevel() == GameState.GetLastLevel()) {
				QuitGame ();
			} else {
				GameState.LevelComplete();
				LoadNextLevel();
			}
		}
	}

	public void QuitGame() {
		GameState.Reset ();
		Application.LoadLevel(0);
	}

	public void EnemyTurnOver() {
		StartCoroutine(StartPlayerTurnIn (0.1f));
	}

	void StartEnemyTurn() {
		GameState.playersTurn = false;
		uim.ShowEnemyUI ();
		em.TriggerEnemyActions ();
	}

	void StartPlayerTurn() {
		uim.HideEnemyUI ();
        cm.PlayersTurnActivated ();
		em.PlayersTurnActivated ();
		GameState.playersTurn = true;
	}

	void LoadNextLevel() {
		Application.LoadLevel (GameState.GetLevel());
	}

	IEnumerator StartPlayerTurnIn(float seconds) {
		yield return new WaitForSeconds(seconds);
		StartPlayerTurn ();
	}
	
}
