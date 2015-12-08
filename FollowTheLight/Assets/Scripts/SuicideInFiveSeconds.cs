using UnityEngine;
using System.Collections;

public class SuicideInFiveSeconds : MonoBehaviour {
	
	void Start () {
		Invoke ("Suicide", 5f);
	}

	void Update () {
	
	}

	void Suicide() {
		Destroy (gameObject);
	}
}
