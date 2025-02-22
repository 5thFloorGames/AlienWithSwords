﻿using UnityEngine;
using System.Collections;

public class CharacterMovement : MonoBehaviour {

	public float maximumMovement;

    bool dead;

	bool movementAvailable;
	bool inCharacter;
	float distanceTravelled;
	float updatedDistance;
	Vector3 lastPosition;

	CharacterSoundController csc;
	UserInterfaceManager uim;
	FPSController fpsc;

	public void ResetMovement() {
        if (!dead) {
			csc.outOfMovement = false;
            distanceTravelled = 0;
            lastPosition = transform.position;
            lastPosition.y = 1;
            UpdateDistanceToUI();
            movementAvailable = true;
            fpsc.movementAvailable = true;
        }
	}

	void Awake() {
        dead = false;
		uim = GameObject.Find ("UserInterface").GetComponent<UserInterfaceManager>();
		fpsc = gameObject.GetComponent<FPSController> ();
		csc = GetComponentInChildren<CharacterSoundController>();
	}
	
	
	void Update() {
		if (GameState.playersTurn && inCharacter && movementAvailable) {
			DetectMovement();
		}
	}

	void DetectMovement() {
		Vector3 currentPosition = transform.position;
		currentPosition.y = 1;
		distanceTravelled += Vector3.Distance(currentPosition, lastPosition);

		if (distanceTravelled > maximumMovement) {
			distanceTravelled = maximumMovement;
			UpdateDistanceToUI ();
			OutOfMovement();
		} else {
			lastPosition = currentPosition;
			if (updatedDistance - distanceTravelled <= -0.01f) {
				UpdateDistanceToUI ();
			}
		}
	}

	void UpdateDistanceToUI() {
		updatedDistance = distanceTravelled;
		uim.UpdateDistanceMeter (gameObject.name, updatedDistance, maximumMovement);
	}

    void CharacterDied() {
        dead = true;
        movementAvailable = false;
        fpsc.movementAvailable = false;
    }

    void CharacterResurrected() {
        dead = false;
        ResetMovement();
    }

	void OutOfMovement() {
		movementAvailable = false;
		fpsc.movementAvailable = false;
		csc.outOfMovement = true;
		csc.PlayOutOfActionsQuote ();
	}


    // CharacterType managers calls these with a broadcast message

    void EnterCharacter() {
        inCharacter = true;
    }

    void LeaveCharacter() {
        inCharacter = false;
    }

}