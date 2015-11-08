using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class zSpellCaster : MonoBehaviour {

	public int orbLimit;
	List<GameObject> orbList;
    bool casting = false;

	GameObject orb;
	Rigidbody rbPlayer;

	GameObject spawnedOrb;
	Rigidbody rbOrb;

	void Start () {
		rbPlayer = gameObject.GetComponent<Rigidbody> ();
		orb = (GameObject)Resources.Load ("LightOrb");
		orbList = new List<GameObject> ();
	}

	void Update () {
		if (Input.GetKeyDown(KeyCode.Alpha1) && !casting) {
			startCastingOrb();
            casting = true;
		}
		if (Input.GetKeyUp(KeyCode.Alpha1) && casting) {
			spawnedOrb.GetComponent<OrbLife>().releaseOrb();
            casting = false;
		}

		checkOrbLimits ();
	}

	void startCastingOrb() {
		spawnedOrb = (GameObject)Instantiate (orb, gameObject.transform.position + gameObject.transform.rotation * new Vector3(0, 1.0f, 1.5f), Quaternion.identity);
		spawnedOrb.transform.parent = gameObject.transform;
		registerOrb (spawnedOrb);
		rbOrb = spawnedOrb.GetComponent<Rigidbody>();
	}

	void releaseOrb() {
		spawnedOrb.GetComponent<OrbLife> ().released = true;
		rbOrb.constraints = RigidbodyConstraints.None;
		rbOrb.velocity = rbPlayer.rotation * new Vector3 (0, 2.0f, 2.0f);
		spawnedOrb.transform.parent = null;
	}

	public void registerOrb(GameObject orb) {
		orbList.Add(orb);
	}

	public void removeOrbFromList(GameObject orb) {
		orbList.Add(orb);
	}

	void checkOrbLimits() {
		if (orbList.Count > orbLimit) {
			GameObject oldestOrb = orbList[0];
			orbList.RemoveAt(0);
			oldestOrb.GetComponent<OrbLife>().startSuicide();
			
		}
	}
}
