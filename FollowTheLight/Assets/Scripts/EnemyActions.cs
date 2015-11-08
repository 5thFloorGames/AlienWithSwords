using UnityEngine;
using System.Collections;

public class EnemyActions : MonoBehaviour {

	void Start () {

	}

	public void TriggerActions () {
		Debug.Log (gameObject.name + " enemy used an ability");
	}

}
