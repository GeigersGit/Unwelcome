using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class chargingFireball : MonoBehaviour {
	ParticleSystem ps;
	Light light;
	float size = 0;
	// Use this for initialization
	void Start () {
		ps = this.GetComponent<ParticleSystem> ();
		light = this.GetComponentInChildren<Light> ();
	}

	// Update is called once per frame
	void FixedUpdate () {
		var mainModule = ps.main;
		mainModule.startSize = size;
		size += 0.04f;
		//light.intensity += 0.04f;
	}
}
