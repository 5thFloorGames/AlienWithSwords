using UnityEngine;
using System.Collections;

public class SpellCaster : MonoBehaviour {
	GameObject orb;

	void Start () {
		orb = (GameObject)Resources.Load ("LightOrb");
	}

	void Update () {
		if (Input.GetKeyDown(KeyCode.Alpha1)) {
			GameObject spawnedOrb = (GameObject)Instantiate (orb, gameObject.transform.position + gameObject.transform.rotation * new Vector3(0, 1, 1), Quaternion.identity);
			spawnedOrb.transform.parent = gameObject.transform;
		}
	}
}
