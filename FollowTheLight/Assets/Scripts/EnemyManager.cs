using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour {

	GameManager gm;
	Transform enemyHolder;
	GameObject enemy;
	List<GameObject> enemies;

	int enemyActionCounter;
	
	void Start () {
		gm = GameObject.Find("GameManager").GetComponent<GameManager>();
		OnLevelWasLoaded (GameState.GetLevel());
	}

	void Update () {
		
	}

	void OnLevelWasLoaded(int level) {
		Invoke("InstantiateEnemies", 0.1f);
	}

	public void PlayersTurnActivated() {

	}

	public void TriggerEnemyActions() {
		enemyActionCounter = 0;
		foreach (GameObject e in enemies) {
			e.BroadcastMessage("TriggerActions");
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

	void InstantiateEnemies() {

		enemyHolder = (Transform)GameObject.Find ("Enemies").transform;
		enemy = (GameObject) Resources.Load("Enemy1");
		enemies = new List<GameObject> ();

		HandleEnemiesInScene ();

		if (GameState.GetLevel() == 1) {
			//InstantiateEnemiesForFirstLevel();
		}

		if (GameState.GetLevel() == 2) {
			//InstantiateEnemiesForFirstLevel();
		}
	}

	void InstantiateEnemiesForFirstLevel() {
		GameObject first = (GameObject) Instantiate (enemy, new Vector3(33, 3, 2), Quaternion.identity);
		EnemyBasicAssignments (first, "first", 50);

		GameObject second = (GameObject)Instantiate (enemy, new Vector3 (33, 3, -2), Quaternion.identity);
		EnemyBasicAssignments (second, "second", 80);
	}

	void EnemyBasicAssignments(GameObject obj, string name, int health) {
		obj.name = name;
		obj.tag = "Enemy";
		obj.transform.SetParent(enemyHolder);

		EnemyState es = obj.GetComponent<EnemyState> ();
		es.Init (health, gameObject);

		obj.SendMessage ("InitActions", gameObject);

		enemies.Add (obj);
	}

	void DeleteEnemyFromList(GameObject enemyobj) {
		enemies.Remove (enemyobj);
	}

	void HandleEnemiesInScene() {
		GameObject[] additionalEnemies = GameObject.FindGameObjectsWithTag("Enemy");
		foreach (GameObject enemy in additionalEnemies) {
			if (enemy.GetComponent<EnemyState>() != null) {
				EnemyBasicAssignments(enemy, "sceneEnemy", 75);
			}
		}
	}

	void AllEnemyActionsCompleted() {
		gm.EnemyTurnOver ();
	}

}
