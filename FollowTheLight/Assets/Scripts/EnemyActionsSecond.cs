using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyActionsSecond : MonoBehaviour {

    public int actionDamage;

    EnemyManager em;
    EnemyMovement move;
    Animator animator;

    float shotLifetime;
    GameObject shotPrefab;

    void Start() {
        move = GetComponent<EnemyMovement>();
        animator = gameObject.GetComponentInChildren<Animator>();

        shotLifetime = 2.0f;
        shotPrefab = (GameObject)Resources.Load("EnemyShot");

        if (actionDamage == 0) {
            actionDamage = 5;
        }

    }

    void Update() {

    }

    public void InitActions(GameObject manager) {
        em = manager.GetComponent<EnemyManager>();
    }

    public void TriggerActions() {
        move.Go();
    }

    public void MovingCompleteStartAttack(GameObject target) {
        StartCoroutine(ShootAtCharacter(target));
    }

    IEnumerator ShootAtCharacter(GameObject target) {
        Vector3 start = transform.position + new Vector3(0, 0.8f, 0);
        Vector3 end = target.transform.position + target.transform.rotation * new Vector3(0, 0.7f, 0);
        Vector3 direction = (end - start).normalized;

        animator.SetTrigger("Attack");
        Invoke("ActionsCompletedInformManager", shotLifetime);
        yield return new WaitForSeconds(0.5f);
        GameObject shot = (GameObject)Instantiate(shotPrefab, (start + direction), Quaternion.Inverse(transform.rotation));
        shot.name = gameObject.name + "Shot";
        shot.GetComponent<EnemyShotDamages>().Init(actionDamage, 0.0f, shotLifetime);
        Rigidbody shotrb = shot.GetComponent<Rigidbody>();
        shotrb.AddForce(direction * 500.0f);
    }

    void ActionsCompletedInformManager() {
        em.EnemyActionsCompleted();
    }
}