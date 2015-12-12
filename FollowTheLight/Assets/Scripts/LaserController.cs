using UnityEngine;
using System.Collections;

public class LaserController : MonoBehaviour {

    CharacterActionsThird cat;
	ParticleSystem ps;
    ParticleSystem psattack;
	LineRenderer lr;
    bool attackFlying;
	bool laserOn;
	
	void Start () {
        cat = transform.parent.parent.GetComponent<CharacterActionsThird>();
		lr = GetComponent <LineRenderer>();
		laserOn = false;
		ps = transform.parent.FindChild("Overlay").FindChild("Particles").GetComponentInChildren<ParticleSystem>();
		ps.Stop();
        psattack = transform.parent.FindChild("Overlay").FindChild("attackParticles").GetComponentInChildren<ParticleSystem>();
        psattack.Stop();
    }

	void Update () {
		if (laserOn) {
			lr.SetPosition(0, transform.position);
		}
	}

    public void AttackParticlesCollided() {
        if (attackFlying) {
            cat.AttackParticlesCollided();
            attackFlying = false;
        }
    }

	public void ShootLaser(Vector3 targetPosition) {
        attackFlying = true;
        psattack.Play();
        StartCoroutine(ShowAttackParticlesFor(0.5f));
        //laserOn = true;
        //SetLaserTarget (targetPosition);
        //StartCoroutine (ShowLaserFor(1.0f));
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

    IEnumerator AttackAutomaticallyOff() {
        yield return new WaitForSeconds(5.0f);
        AttackParticlesCollided();
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

    IEnumerator ShowAttackParticlesFor(float seconds) {
        yield return new WaitForSeconds(seconds);
        psattack.Stop();
    }

    void ResetLaser() {
		lr.enabled = false;
		laserOn = false;
	}
}