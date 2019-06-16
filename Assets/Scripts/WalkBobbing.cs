using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WalkBobbing : MonoBehaviour {
	float originalY = 0;
	float offset = 0;
	public NavMeshAgent agent;
	// Use this for initialization
	void Start () {
		originalY = transform.position.y;
	}

	// Update is called once per frame
	void Update () {
		if (agent && agent.velocity.magnitude > 1) {
			offset = Mathf.Sin (Time.time * agent.velocity.magnitude * 5) / 10;
			transform.position = new Vector3 (transform.position.x, originalY + offset, transform.position.z);
		}
	}
}
