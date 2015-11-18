using UnityEngine;
using System.Collections;

public class DisableOnStart : MonoBehaviour {
	
	void Awake () {
		gameObject.SetActive (false);
	}

	void Update () {
	
	}
}
