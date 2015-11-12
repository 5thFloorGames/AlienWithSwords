using UnityEngine;
using System.Collections;

public class MovementMeasurements : MonoBehaviour {

	public float maximumMovement;

	bool movementAvailable;
	bool inCharacter;
	float distanceTravelled;
	float updatedDistance;
	Vector3 lastPosition;

	UserInterfaceManager uim;
	FPSController fpsc;

	public void enterCharacter() {
		inCharacter = true;
	}
	
	public void leaveCharacter() {
		inCharacter = false;
	}

	public void ResetMovement() {
		distanceTravelled = 0;
		updateDistanceToUI ();
		movementAvailable = true;
		fpsc.movementAvailable = true;
	}

	void Start() {

		uim = GameObject.Find ("UserInterface").GetComponent<UserInterfaceManager>();
		fpsc = gameObject.GetComponent<FPSController> ();

		movementAvailable = true;
		fpsc.movementAvailable = true;

		distanceTravelled = 0;
		updatedDistance = 0;
		lastPosition = transform.position;
		lastPosition.y = 1;
	}
	
	
	void Update() {
		if (GameState.playersTurn && inCharacter && movementAvailable) {
			detectMovement();
		}
	}

	void detectMovement() {
		Vector3 currentPosition = transform.position;
		currentPosition.y = 1;
		distanceTravelled += Vector3.Distance(currentPosition, lastPosition);

		if (distanceTravelled > maximumMovement) {
			distanceTravelled = maximumMovement;
			updateDistanceToUI ();
			movementAvailable = false;
			fpsc.movementAvailable = false;
		} else {
			lastPosition = currentPosition;
			if (updatedDistance - distanceTravelled <= -0.01f) {
				updateDistanceToUI ();
			}
		}
	}

	void updateDistanceToUI() {
		updatedDistance = (float)System.Math.Round (distanceTravelled, 2);
		uim.updateDistanceMeter (gameObject.name, updatedDistance);
	}

}