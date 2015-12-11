using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour {
	
	private AudioSource[] audios;
	private AudioSource music;
	
	void Awake() {
		DontDestroyOnLoad(transform.gameObject);
		audios = gameObject.GetComponents<AudioSource>();
		music = audios[0];
	}
	
	void OnLevelWasLoaded(int level) {
		if ((level == 0)) {
			music.Stop();
		}
		if (level == 1 && !music.isPlaying) {
			music.Play();
		}
	}
}