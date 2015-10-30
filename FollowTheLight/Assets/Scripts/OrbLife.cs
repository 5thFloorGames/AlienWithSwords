using UnityEngine;
using System.Collections;

public class OrbLife : MonoBehaviour {

	public bool released;

	public float suicideTimer;
	public bool maxLifeTime;
	public float maxSize;

	GameObject playerObject;
	Rigidbody rb;
	SpellCaster scscript;
	Light lighting;

	void Start () {
		rb = gameObject.GetComponent<Rigidbody> ();
		lighting = gameObject.GetComponent<Light>();
		playerObject = GameObject.Find ("Player");
		scscript = playerObject.GetComponent<SpellCaster>();
		released = false;
	}

	void Update () {
		growIfNotReleased ();
	}

	public void releaseOrb() {
		released = true;
		rb.constraints = RigidbodyConstraints.None;
		rb.velocity = playerObject.transform.rotation * new Vector3 (0, 2.0f, 8.0f);
		gameObject.transform.parent = null;
	}

	public void startSuicide() {
		StartCoroutine (suicideTiming());
	}

	IEnumerator suicideTiming() {
		while (suicideTimer > 0.0f) {
			yield return new WaitForSeconds(1);
			suicideTimer -= 1.0f;
		}
		suicide ();
	}

	void suicide() {
		Destroy (gameObject);
	}

	void lifeTimeChecker() {

	}

	void growIfNotReleased() {
		if (!released) {
			if (gameObject.transform.localScale.x >= maxSize) {
				gameObject.transform.localScale = new Vector3 (maxSize, maxSize, maxSize);
				lighting.range = maxSize/2;
			} else {
				gameObject.transform.localScale += new Vector3 (1.0f, 1.0f, 1.0f) * Time.deltaTime;
				lighting.range += ((0.5f * Time.deltaTime));
			}
		}
	}
}
