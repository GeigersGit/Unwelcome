using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionShake : MonoBehaviour {
	GameObject cameraTarget;
	float shake = .1f;
	float shakeAmount = 0.6f;
	float decreaseFactor = 1.0f;

	// Use this for initialization
	void Start () {
		cameraTarget = GameObject.FindGameObjectWithTag ("CameraTarget");
	}
	
	// Update is called once per frame
	void Update () {
		if (shake > 0) {
			cameraTarget.transform.localPosition = Random.insideUnitSphere * shakeAmount;
			shake -= Time.deltaTime * decreaseFactor;
		}
		if (shake <= 0) {
			cameraTarget.transform.localPosition = new Vector3 (0, 0.3f, 0);
		}
	}
}
