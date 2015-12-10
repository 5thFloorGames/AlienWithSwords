using UnityEngine;
using System.Collections;

public class BulletDamages : MonoBehaviour {

	AudioClip clip;


	public void SetHitSFX(AudioClip[] clips) {
		if (clips.Length > 0) {
			int n = Random.Range (0, clips.Length);
			clip = clips[n];
		}
	}

    void OnTriggerEnter(Collider other) {
		CreateHitEffect ();
		Destroy (gameObject);
	}

	void CreateHitEffect() {
		GameObject prefab = (GameObject) Resources.Load("BulletHitEffect");
		GameObject fx = (GameObject)Instantiate (prefab, transform.position + (transform.rotation * new Vector3(0, 0, -0.5f)), Quaternion.identity);
		if (clip != null) {
			fx.GetComponent<SingleAudioClipPlayer> ().PlayThisClip (clip);
		}
	}
}
