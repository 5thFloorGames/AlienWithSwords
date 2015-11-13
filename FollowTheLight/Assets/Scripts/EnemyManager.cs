using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour {

	Transform enemyHolder;
	GameObject enemy;
	List<GameObject> enemies;
	
	void Start () {
		enemyHolder = (Transform)GameObject.Find ("Enemies").transform;
		enemy = (GameObject) Resources.Load("Enemy");
		enemies = new List<GameObject> ();

		InstantiateEnemies ();
	}

	void Update () {
		
	}

	public void PlayersTurnActivated() {

	}

	public void TriggerEnemyActions() {
		foreach (GameObject e in enemies) {
			e.GetComponent<EnemyActions>().TriggerActions();
		}
	}

	void InstantiateEnemies() {
		if (GameState.GetLevel() == 1) {
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
