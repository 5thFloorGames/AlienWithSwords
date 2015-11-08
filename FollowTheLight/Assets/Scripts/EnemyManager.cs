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
		GameObject first = (GameObject) Instantiate (enemy, new Vector3(33, 2, 2), Quaternion.identity);
		enemyBasicAssignments (first, "first");

		GameObject second = (GameObject)Instantiate (enemy, new Vector3 (33, 2, -2), Quaternion.identity);
		enemyBasicAssignments (second, "second");
	}

	void enemyBasicAssignments(GameObject obj, string name) {
		obj.name = name;
		obj.transform.parent = enemyHolder;
		enemies.Add (obj);
	}
}
