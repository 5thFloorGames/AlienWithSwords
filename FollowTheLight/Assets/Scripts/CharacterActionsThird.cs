using UnityEngine;
using System.Collections;

public class CharacterActionsThird : MonoBehaviour {
	
	public int healing;
	public int maxActions;
	
	bool inCharacter;
	bool dead;
	int actions;
	bool characterInAim;

	GameObject cameraObj;
	GameObject overlay;
	
	UserInterfaceManager uim;
	
	void Awake() {
		characterInAim = true;
		uim = GameObject.Find("UserInterface").GetComponent<UserInterfaceManager>();
		overlay = transform.FindChild ("Overlay").gameObject;
		dead = false;
	}
	
	void Start () {
		UpdateActionsToUI();
	}
	
	void FixedUpdate () {
		if (GameState.playersTurn && inCharacter && actions > 0 && !dead) {
			CheckIfCharacterWithinAim();
			if (Input.GetButtonDown ("Fire1") && characterInAim){
				HealAimedCharacter();
				UpdateActionsToUI();
			}
			
		}
	}
	
	void UpdateActionsToUI() {
		uim.UpdateActionPoints(gameObject.transform.parent.name, actions, maxActions);
	}

	void CheckIfCharacterWithinAim () {
		Vector3 start = gameObject.transform.position;
		Vector3 direction = (gameObject.transform.rotation * new Vector3 (0, 0, 20f));
		RaycastHit hit;
		if (Physics.Raycast (start, direction, out hit, (direction.magnitude + 1.0f))) {
			if (hit.collider.tag == "Player") {
				if (!characterInAim) {
					CharacterAimedAt ();
				}
			} else {
				if (characterInAim) {
					CharacterNotAimedAt ();
				}
			}
		}
	}

	void HealAimedCharacter() {
		Vector3 start = gameObject.transform.position;
		Vector3 direction = (gameObject.transform.rotation * new Vector3 (0, 0, 20f));
		RaycastHit hit;
		if (Physics.Raycast (start, direction, out hit, (direction.magnitude + 1.0f))) {
			actions -= 1;
			if (hit.collider.tag == "Player") {
				hit.collider.gameObject.SendMessageUpwards ("Heal", healing);
			}
		}
	}

	void CharacterAimedAt() {
		characterInAim = true;
	}

	void CharacterNotAimedAt() {
		characterInAim = false;
	}
	
	
	// CharacterType Manager calls these with a broadcast message
	
	void CharacterDied() {
		dead = true;
	}
	
	void CharacterResurrected() {
		dead = false;
		ResetActions();
	}
	
	void ResetActions() {
		actions = maxActions;
		UpdateActionsToUI();
	}
	
	void EnterCharacter() {
		inCharacter = true;
		overlay.SetActive (true);
	}
	
	void LeaveCharacter() {
		inCharacter = false;
		overlay.SetActive (false);
	}
}
