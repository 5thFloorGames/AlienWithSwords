using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour {

	Transform enemyHolder;
	GameObject enemy;
	List<GameObject> enemies;
	
	void Start () {
		InstantiateEnemies ();
	}

	void Update () {
		
	}

	void OnLevelWasLoaded(int level) {
		if (level != 0) {
			InstantiateEnemies ();
		}
	}

	public void PlayersTurnActivated() {

	}

	public void TriggerEnemyActions() {
		foreach (GameObject e in enemies) {
			e.BroadcastMessage("TriggerActions");
		}
	}

	void InstantiateEnemies() {
		enemyHolder = (Transform)GameObject.Find ("Enemies").transform;
		enemy = (GameObject) Resources.Load("Enemy");
		enemies = new List<GameObject> ();
		if (GameState.GetLevel() == 1) {
			InstantiateEnemiesForFirstLevel();
		}

		if (GameState.GetLevel() == 2) {
			InstantiateEnemiesForFirstLevel();
		}
	}

	void InstantiateEnemiesForFirstLevel() {
		GameObject first = (GameObject) Instantiate (enemy, new Vector3(33, 3, 2), Quaternion.identity);
		enemyBasicAssignments (first, "first", 50);

		GameObject second = (GameObject)Instantiate (enemy, new Vector3 (33, 3, -2), Quaternion.identity);
		enemyBasicAssignments (second, "second", 80);
	}

	void enemyBasicAssignments(GameObject obj, string name, int health) {
		obj.name = name;
		obj.tag = "Enemy";
		obj.transform.parent = enemyHolder;

		EnemyState es = obj.GetComponent<EnemyState> ();
		es.Init(100, gameObject);

		enemies.Add (obj);
	}

	void deleteEnemyFromList(GameObject enemyobj) {
		enemies.Remove (enemyobj);
	}

}
