using UnityEngine;
using System.Collections;

public class Navmehs : MonoBehaviour {

	public Transform target;
	NavMeshAgent nav;

	// Use this for initialization
	void Start () {
		nav = GetComponent<NavMeshAgent> ();
	}
	
	// Update is called once per frame
	void Update () {
		nav.destination = target.position;
	}
}
