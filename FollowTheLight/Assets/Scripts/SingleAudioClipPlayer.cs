using UnityEngine;
using System.Collections;

public class SingleAudioClipPlayer : MonoBehaviour {

	public void PlayThisClip(AudioClip clip) {
		AudioSource source = GetComponent<AudioSource>();
		source.clip = clip;
		source.PlayOneShot (source.clip);
	}
	
}
