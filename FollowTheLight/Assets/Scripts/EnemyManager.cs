using UnityEngine;
using System.Collections;

public class EnemyManager : MonoBehaviour {

	GameObject enemy;
	GameObject[] enemies;
	
	void Start () {
		enemy = (GameObject) Resources.Load("Enemy");
		InstantiateEnemies ();
	}

	void Update () {
		
	}

	void InstantiateEnemies() {
		if (GameState.GetLevel() == 1) {
			InstantiateEnemiesForFirstLevel();
		}
	}

	void InstantiateEnemiesForFirstLevel() {
		GameObject first = (GameObject) Instantiate (enemy, new Vector3(33, 2, 0), Quaternion.identity);
	}
}
