using UnityEngine;
using System.Collections;

public enum LevelObjective { DestroyEnemies, GetAllCharactersToLevelEnd, GetOneCharacterToLevelEnd, KillYourCharacters };

public class GameManager : MonoBehaviour {

    public int howManyLevels;

	AnnouncementManager am;
	CharacterManager cm;
	EnemyManager em;
	UserInterfaceManager uim;
	bool levelCompleted;

    LevelObjective objective;

    bool initialized = false;

    void Awake() {
		DontDestroyOnLoad (gameObject);
        int lvl = Application.loadedLevel;
        if (lvl == 0) {
            lvl = 3;
        }
        GameState.SetLevel(lvl);
    }

	void Start () {
        if (!initialized) {
			OnLevelWasLoaded(GameState.GetLevel());
        }
	}

    void OnLevelWasLoaded(int level) {

        if (level == 0) {
            Cursor.visible = true;
            Destroy(gameObject);
        }

        GameState.SetLastLevel(howManyLevels);

        #if !UNITY_EDITOR
                    Cursor.visible = false;
        #endif

        if (!initialized && level != 0) {
            am = gameObject.GetComponent<AnnouncementManager>();
            cm = gameObject.GetComponent<CharacterManager>();
            em = gameObject.GetComponent<EnemyManager>();
            uim = GameObject.Find("UserInterface").GetComponent<UserInterfaceManager>();

            objective = GameObject.Find("LevelLoader").GetComponent<LevelLoader>().levelObjective;
            am.InformLevelObjective(objective);

            initialized = true;
            levelCompleted = false;
            am.LevelLoadedFader();
            Invoke("LateStart", 0.1f);
        }
    }

    void LateStart () {
		StartPlayerTurn ();
        initialized = false;
	}

	void Update () {

        if (Input.GetKeyDown(KeyCode.N)) {
            LevelComplete();
        }

        if (Input.GetButton ("Cancel")) {
			QuitGame();
		}

        if (Input.GetKeyDown(KeyCode.X)) {
            LoadSceneOfGameStateNumber();
        }
        if (GameState.playersTurn) {
			if (Input.GetKeyDown (KeyCode.Tab)) {
				StartEnemyTurn();
			}
		}
	}


    public void LevelComplete() {
		if (!levelCompleted) {
			levelCompleted = true;
			uim.ShowLevelCompletedUI();
			GameState.playersTurn = false;
            StartCoroutine(LevelCompletedLoadNextIn(3.0f));
		}
	}

    IEnumerator LevelCompletedLoadNextIn(float seconds) {
        yield return new WaitForSeconds(seconds);
        uim.HideLevelCompletedUI();
        if (GameState.GetLevel() == GameState.GetLastLevel()) {
            QuitGame();
        } else {
            GameState.LevelComplete();
            LoadSceneOfGameStateNumber();
        }
    }

    public void LevelFailed() {
        if (!levelCompleted) {
            levelCompleted = true;

            uim.PlayerTurnStartInfo();
            uim.LevelFailedInfo();
            GameState.playersTurn = false;
            StartCoroutine(LevelFailedLoadSameIn(3.0f));
        }
    }

    IEnumerator LevelFailedLoadSameIn(float seconds) {
        yield return new WaitForSeconds(seconds);
        uim.HideLevelFailedUI();
        LoadSceneOfGameStateNumber();
    }



    public void AllCharactersDead() {
        LevelFailed();
    }

    public void AllCharactersInLevelEnd() {
        if (objective == LevelObjective.GetAllCharactersToLevelEnd) {
            LevelComplete();
        }
    }

    public void AllEnemiesDestroyed() {
        if (objective == LevelObjective.DestroyEnemies) {
            LevelComplete();
        }
    }

    public void OneCharacterRemaining() {
        if (objective == LevelObjective.KillYourCharacters) {
            float endingTimer = 10.0f;
            StartCoroutine(FadeInHalf(endingTimer));
            am.GenerateWinnerMessage();
            uim.gameObject.SetActive(false);
            GameState.playersTurn = false;
            Invoke("GameEnding", endingTimer);
        }
    }

    IEnumerator FadeInHalf(float timer) {
        yield return new WaitForSeconds(timer/2);
        am.EndGameFading(timer/2);
    }

    void GameEnding() {
        LevelComplete();
    }



	void StartEnemyTurn() {
		GameState.playersTurn = false;
		uim.EnemyTurnStartInfo ();
        cm.PlayerTurnEnded();
		em.TriggerEnemyActions ();
	}

    public void EnemyTurnOver() {
        StartCoroutine(StartPlayerTurnIn(0.1f));
    }

    IEnumerator StartPlayerTurnIn(float seconds) {
        yield return new WaitForSeconds(seconds);
        StartPlayerTurn();
    }

    void StartPlayerTurn() {
		uim.PlayerTurnStartInfo ();
        cm.PlayersTurnActivated ();
		em.PlayersTurnActivated ();
		GameState.playersTurn = true;
	}



    public void SetLevelObjective(LevelObjective obj) {
        objective = obj;
    }

    public LevelObjective GetLevelObjective() {
        return objective;
    }

    void LoadSceneOfGameStateNumber() {
        Application.LoadLevel(GameState.GetLevel());
    }

    void QuitGame() {
        GameState.Reset();
        Application.LoadLevel(0);
    }

}
