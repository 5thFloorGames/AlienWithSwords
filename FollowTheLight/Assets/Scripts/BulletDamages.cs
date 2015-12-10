using UnityEngine;
using System.Collections;

public class BulletDamages : MonoBehaviour {

	int damage;
	AudioClip clip;

	public void setDamage(int amount) {
		damage = amount;
	}

	public void SetHitSFX(AudioClip[] clips) {
		if (clips.Length > 0) {
			int n = Random.Range (0, clips.Length);
			clip = clips[n];
		}
	}

    void OnTriggerEnter(Collider other) {
        if (((other.GetType() == typeof(CapsuleCollider)) && other.tag == "Enemy") || (other.tag == "Player" && (other.GetType() == typeof(CapsuleCollider)))) {
            if (other.tag == "Enemy" || (other.tag == "Player" && (other.GetType() == typeof(CapsuleCollider)))) {
                other.SendMessageUpwards("TakeDamage", damage);
            }
		}
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
