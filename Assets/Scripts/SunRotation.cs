using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunRotation : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	void FixedUpdate(){
		transform.Rotate (.005f, 0, 0);
	}
}
