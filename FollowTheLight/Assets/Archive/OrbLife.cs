using UnityEngine;
using System.Collections;

public class OrbLife : MonoBehaviour {

	public bool released;

	public float suicideTimer;
	public bool maxLifeTime;
	public float maxSize;
    public float size;

	GameObject playerObject;
	Rigidbody rb;
	Light lighting;

	void Start () {
        size = gameObject.transform.localScale.x;
		rb = gameObject.GetComponent<Rigidbody> ();
		lighting = gameObject.GetComponent<Light>();
		playerObject = GameObject.Find ("Player");
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
			if (size >= maxSize) {
				gameObject.transform.localScale = new Vector3 (maxSize, maxSize, maxSize);
				lighting.range = maxSize*10;
			} else {
                size += 1.0f * Time.deltaTime;
				gameObject.transform.localScale = new Vector3 (size, size, size);
				lighting.range = size * 10;
                if (lighting.range < 0.5f) {
                    lighting.range = 0.5f;
                }
			}
		}
	}
}
