using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldCoin : MonoBehaviour {

	private GameManager gm;

	// Use this for initialization
	void Start () {
		gm = GameObject.FindGameObjectWithTag ("GameManager").GetComponent<GameManager>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter(Collider other){
		if (other.gameObject.tag == "Player") {
			gm.gold += 100;
			Destroy (this.gameObject);
		}
	}
}
