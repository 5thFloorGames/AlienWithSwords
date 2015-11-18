using UnityEngine;
using System.Collections;

public class DisableOnStartIfNotInEditor : MonoBehaviour {

	void Start () {
		#if !UNITY_EDITOR
		gameObject.SetActive (false);
		#endif
	}
	
	void Update () {
		
	}
}
