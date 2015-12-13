using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum EnemyType { Exploder, Shooter, Boss };
public enum SecondaryEnemyType { Normal, Little, Big };

public class EnemyManager : MonoBehaviour {

    UserInterfaceManager uim;
	GameManager gm;
    AnnouncementManager am;
	List<GameObject> enemies;
    List<GameObject> spawningEnemies;

	int enemyActionCounter;
    bool otherCharactersAreEnemies;
	
	void Start () {
		gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        am = gm.gameObject.GetComponent<AnnouncementManager>();
        uim = GameObject.Find("UserInterface").GetComponent<UserInterfaceManager>();
		OnLevelWasLoaded (GameState.GetLevel());
        if (gm.GetLevelObjective() == LevelObjective.KillYourCharacters) {
            otherCharactersAreEnemies = true;
        } else {
            otherCharactersAreEnemies = false;
        }
    }

	void Update () {
		
	}

	void OnLevelWasLoaded(int level) {
        GetEnemiesInScene();
        GiveEnemyCountToUI();
    }

	public void PlayersTurnActivated() {

	}

	public void TriggerEnemyActions() {
        if (otherCharactersAreEnemies) {
            AllEnemyActionsCompleted();
            return;
        }
		enemyActionCounter = 0;
		foreach (GameObject e in enemies) {
            if (e.activeSelf) {
                e.BroadcastMessage("TriggerActions");
            } else {
                Invoke("EnemyActionsCompleted", 0.5f);
            }
		}
		if (enemies.Count == 0) {
			Invoke("AllEnemyActionsCompleted", 0.2f);
		}
	}

	public void EnemyActionsCompleted() {
		enemyActionCounter += 1;
		if (enemyActionCounter == enemies.Count) {
			AllEnemyActionsCompleted();
		}
	}

    public void SpawnTriggered(int spawnNumber) {
        am.EnemySpawnTriggered();
        List<GameObject> toRemove = new List<GameObject>();
        foreach (GameObject obj in spawningEnemies) {
            if (obj.GetComponent<EnemyState>().spawnNumber == spawnNumber) {
                obj.SetActive(true);
                obj.BroadcastMessage("Spawned");
                toRemove.Add(obj);
            }
        }
        foreach (GameObject obj in toRemove) {
            spawningEnemies.Remove(obj);
        }
    }

	void EnemyBasicAssignments(GameObject obj) {

		EnemyState es = obj.GetComponent<EnemyState> ();
		es.Init (gameObject);
		obj.SendMessage ("InitActions", gameObject);
		enemies.Add (obj);
        CheckForSpawnerDetails(obj, es);
	}

    void CheckForSpawnerDetails(GameObject obj, EnemyState es) {
        if (es.willSpawnLater) {
            spawningEnemies.Add(obj);
            obj.SetActive(false);
        }
    }

	public void DeleteEnemyFromList(GameObject enemyobj) {
		enemies.Remove (enemyobj);
        if (enemies.Count == 0) {
            gm.AllEnemiesDestroyed();
        }
        GiveEnemyCountToUI();
    }

	void GetEnemiesInScene() {

        enemies = new List<GameObject>();
        spawningEnemies = new List<GameObject>();
        GameObject[] additionalEnemies = GameObject.FindGameObjectsWithTag("Enemy");
		foreach (GameObject enemy in additionalEnemies) {
			if (enemy.GetComponent<EnemyState>() != null) {
				EnemyBasicAssignments(enemy);
			}
		}

	}

	void AllEnemyActionsCompleted() {
		gm.EnemyTurnOver ();
	}

    void GiveEnemyCountToUI() {
        if (!otherCharactersAreEnemies) {
            uim.UpdateEnemyCount(enemies.Count);
        }
    }

}
