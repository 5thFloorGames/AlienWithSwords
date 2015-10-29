using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {

	Rigidbody rb;
	int allowedLayers;
	public float speed;
	public float tilt;

	void Start () {
		allowedLayers |= LayerMask.NameToLayer("Default");
		allowedLayers = ~allowedLayers;
		rb = gameObject.GetComponent<Rigidbody> ();
	}
	
	void Update () {
		RaycastHit rayhit;
		float turnHorizontal = 0.0f;
		float moveVertical = 0.0f;

		bool grounded = Physics.Raycast (transform.position, Vector3.down, out rayhit, 1.2f, allowedLayers);

		Vector3 movement = new Vector3 (0, 0, 0);
		turnHorizontal = Input.GetAxis ("Horizontal");
		moveVertical = Input.GetAxis ("Vertical");

		if (grounded && Input.GetKeyDown(KeyCode.Space)) {
			rb.velocity += new Vector3(0, 5, 0);
		}
		
		movement += new Vector3 (0.0f, 0.0f, moveVertical);
		Vector3 tmp = rb.rotation * movement * speed;
		rb.velocity = new Vector3 (tmp.x, rb.velocity.y, tmp.z);

		rb.rotation = rb.rotation * Quaternion.AngleAxis ((turnHorizontal * 2.0f), Vector3.up);
	}

	void FixedUpdate() {
		rb.AddForce (Vector3.down*2, ForceMode.Acceleration);
	}
}
