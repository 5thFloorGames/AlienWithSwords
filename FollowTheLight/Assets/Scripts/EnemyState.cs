using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class EnemyState : MonoBehaviour {

    public EnemyType type;
    public SecondaryEnemyType secondaryType;
    public int maximumHealth;
    public bool dead;

    public bool willSpawnLater;
    public int spawnNumber;

    int currentHealth;

	Image healthMeter;
	EnemyManager em;
    AnnouncementManager am;
	Animator animator;
    EnemyMovement move;
	EnemySoundController esc;
	SpriteRenderer spriteRenderer;

	public void Init(GameObject manager) {
		dead = false;
		healthMeter = transform.FindChild("EnemyInfo").FindChild("HealthMeter").GetComponent<Image>();
        em = manager.GetComponent<EnemyManager>();
        am = manager.GetComponent<AnnouncementManager>();
        esc = GetComponent<EnemySoundController>();
        HealthInit();
		UpdateHealthToHealthMeter ();
	}
	
	void Start () {
		spriteRenderer = transform.FindChild ("Sprite").GetComponent<SpriteRenderer> ();

        HealthInit();
		animator = gameObject.GetComponentInChildren<Animator>();
        move = gameObject.GetComponent<EnemyMovement>();
        move.DisableNva();
	}

	void Update () {

	}

    void HealthInit() {

        if (maximumHealth == 0) {
            maximumHealth = 30;
        }
        currentHealth = maximumHealth;
    }

	void TakeDamage (List<object> info) {

        if (info[0] == null || info[1] == null) {
            Debug.Log("WARNING: correct info not given for dealing damage!");
        }

        if (!dead) {

            // Handling the info
            int amount = int.Parse(info[0].ToString());
            GameObject source = (GameObject)info[1];
            bool sourceIsCharacter = false;
            CharacterState sourceCs = source.GetComponent<CharacterState>();

            if (sourceCs != null) {
                sourceIsCharacter = true;
            }

            currentHealth -= amount;

            am.EnemyTookDamageFromCharacter(type, amount, sourceCs.type);

			if (currentHealth <= 0) {
				StartDying();

                if (sourceIsCharacter) {
                    sourceCs.YouKilledAnEnemy();
                    am.EnemyDiedFromCharacter(type, sourceCs.type);
                } else {
                    
                }
            }
			if (!dead) {
				animator.SetTrigger("IsHit");
			}
            UpdateHealthToHealthMeter ();
		}
	}

	void AimedAt(GameObject character) {
		esc.PlayAimedQuote ();
		//animator.SetBool ("AimedAt", true);
        move.CharacterAimedYou(character);
		spriteRenderer.color = new Vector4 (0.5f, 0.5f, 0.5f, 1);
	}

	void NotAimedAt() {
		//animator.SetBool ("AimedAt", false);
		spriteRenderer.color = new Vector4 (1, 1, 1, 1);
	}

	void UpdateHealthToHealthMeter() {
		healthMeter.fillAmount = ((float)currentHealth/maximumHealth);
	}

	void StartDying() {
        currentHealth = 0;
        dead = true;
        gameObject.GetComponentInChildren<Collider> ().enabled = false;
		animator.SetBool ("Dying", true);
		em.DeleteEnemyFromList(gameObject);
		Invoke ("AdjustSpriteForDying", 0.1f);
		Invoke ("Death", 0.5f);
	}

	void AdjustSpriteForDying() {
		transform.FindChild ("Sprite").transform.position += new Vector3 (0, 0.2f, 0);
	}

	void Death() {
        Invoke("ColliderDeactivate", 1.0f);
		GameObject prefab = (GameObject) Resources.Load("BloodPool");
		transform.FindChild ("EnemyInfo").gameObject.SetActive (false);
		Instantiate (prefab, transform.position, Quaternion.identity);
	}

    void ColliderDeactivate() {
        GetComponentInChildren<Collider>().enabled = false;
    }

}
