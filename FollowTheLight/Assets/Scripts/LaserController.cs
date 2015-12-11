using UnityEngine;
using System.Collections;

public class LaserController : MonoBehaviour {

	ParticleSystem ps;
	LineRenderer lr;
	bool laserOn;
	
	void Start () {
		lr = GetComponent <LineRenderer>();
		laserOn = false;
		ps = transform.parent.GetComponentInChildren<ParticleSystem>();
		ps.Stop();
	}

	void Update () {
		if (laserOn) {
			lr.SetPosition(0, transform.position);
		}
	}

	public void ShootLaser(Vector3 targetPosition) {
		laserOn = true;
		SetLaserTarget (targetPosition);
		StartCoroutine (ShowLaserFor(1.0f));
	}

	public void HealLaser(Vector3 targetPosition) {
		ps.Play ();
		StartCoroutine (ShowParticlesFor(0.5f));
		//laserOn = true;
		//SetLaserTarget (targetPosition);
	}

	void SetLaserTarget(Vector3 targetPosition) {
		lr.SetPosition(1, targetPosition);
	}

	IEnumerator ShowLaserFor(float seconds) {
		lr.enabled = true;
		yield return new WaitForSeconds (seconds);
		ResetLaser ();
	}

	IEnumerator ShowParticlesFor(float seconds) {
		yield return new WaitForSeconds (seconds);
		ps.Stop ();
	}

	void ResetLaser() {

		lr.enabled = false;
		laserOn = false;
	}
}