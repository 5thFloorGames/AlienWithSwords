using UnityEngine;
using System.Collections;

public class DisableOnStartIfNotInEditor : MonoBehaviour {

	void Awake () {
		#if !UNITY_EDITOR
		gameObject.SetActive (false);
		#endif
	}
	
	void Update () {
		
	}
}
