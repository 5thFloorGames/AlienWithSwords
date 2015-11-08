using UnityEngine;
using System.Collections;

public class MainMenuManager : MonoBehaviour {
	public GameObject mainOptions;
	public GameObject credits;

	private GameObject musicObject;
	private AudioSource[] audios;
	private AudioSource hoverSound;
	
	void Start () {
		audios = gameObject.GetComponents<AudioSource>();
		hoverSound = audios [0];
		CheckMusic();
	}
	
	void Update () {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			PlayExitSoundAndExit();
		}
	}
	
	public void ShowCredits() {
		PlayChoosingSound();
		mainOptions.SetActive(false);
		credits.SetActive(true);
	}
	
	public void ShowMainMenu() {
		PlayChoosingSound();
		mainOptions.SetActive(true);
		credits.SetActive(false);
	}

	public void Play() {
		PlayChoosingSound ();
		StartCoroutine(PlayPlaySoundAndPlay());
	}
	
	public void ExitGame() {
		StartCoroutine(PlayExitSoundAndExit());
	}

	public void PlayHoverSound() {
		hoverSound.Play ();
	}
	
	IEnumerator PlayExitSoundAndExit() {
		PlayChoosingSound ();
		yield return new WaitForSeconds(0.5f);
		Application.Quit();
	}

	IEnumerator PlayPlaySoundAndPlay() {
		PlayChoosingSound ();
		yield return new WaitForSeconds(0.5f);
		Application.LoadLevel(1);
	}

	void CheckMusic() {
		musicObject = GameObject.Find("MusicManager");
				if (musicObject == null) {
					musicObject = Instantiate<GameObject>((GameObject)Resources.Load("MusicManager"));
					musicObject.name = "MusicManager";
				}
	}
	
	void PlayChoosingSound() {
		audios [1].PlayOneShot (audios [1].clip);
	}


}
