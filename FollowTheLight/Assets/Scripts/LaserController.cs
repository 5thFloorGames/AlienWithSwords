using UnityEngine;
using System.Collections;

public class LaserController : MonoBehaviour {

	LineRenderer lr;
	bool laserOn;

	// Use this for initialization
	void Start () {
		lr = GetComponent <LineRenderer>();
		laserOn = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (laserOn) {
			lr.SetPosition(0, transform.position);
		}
	}

	public void ShootLaser(Vector3 targetPosition) {
		laserOn = true;
		SetLaserTarget (targetPosition);
		StartCoroutine (ShowLaserFor(0.5f));
	}

	public void HealLaser(Vector3 targetPosition) {
		laserOn = true;
		SetLaserTarget (targetPosition);
		StartCoroutine (ShowLaserFor(0.5f));
	}

	void SetLaserTarget(Vector3 targetPosition) {
		lr.SetPosition(1, targetPosition);
	}

	IEnumerator ShowLaserFor(float seconds) {
		lr.enabled = true;
		yield return new WaitForSeconds (seconds);
		ResetLaser ();
	}

	void ResetLaser() {
		lr.enabled = false;
		laserOn = false;
	}
}