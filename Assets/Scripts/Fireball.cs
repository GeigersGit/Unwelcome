using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Fireball : MonoBehaviour {
	Rigidbody rb;
	Vector3 direction;
	// Use this for initialization
	void Start () {
		rb = this.GetComponent<Rigidbody> ();
	}
	
	// Update is called once per frame
	void Update () {
	}

	void OnTriggerEnter(Collider other){
		if (other.gameObject.tag == "Knight" || other.gameObject.tag == "Wizard" || other.gameObject.tag == "Rogue") {
			Rigidbody otherRB = other.gameObject.GetComponent<Rigidbody> ();
			otherRB.constraints = RigidbodyConstraints.None;
			otherRB.isKinematic = false;
			other.gameObject.GetComponent<NavMeshAgent> ().enabled = false;
			direction = rb.velocity;
			//otherRB.AddForce (rb.velocity * 300);

			if (other.gameObject.tag == "Knight") {
				AIKnight knightAI = other.gameObject.GetComponent<AIKnight> ();
				knightAI.dead = true;
				knightAI.gameObject.GetComponent<CapsuleCollider> ().enabled = false;
				knightAI.hammer.gameObject.AddComponent<BoxCollider> ();
				knightAI.hammer.gameObject.AddComponent <Rigidbody> ().AddForce (direction * Random.Range (20, 50));
				knightAI.helm.gameObject.AddComponent<BoxCollider> ();
				knightAI.helm.gameObject.AddComponent <Rigidbody> ().AddForce (direction * Random.Range (20, 50));
				knightAI.backPack.gameObject.AddComponent<BoxCollider> ();
				knightAI.backPack.gameObject.AddComponent <Rigidbody> ().AddForce (direction * Random.Range (20, 50));
				knightAI.body.gameObject.AddComponent<BoxCollider> ();
				knightAI.body.gameObject.AddComponent <Rigidbody> ().AddForce (direction * Random.Range (20, 50));
				knightAI.leftHand.gameObject.AddComponent<BoxCollider> ();
				knightAI.leftHand.gameObject.AddComponent <Rigidbody> ().AddForce (direction * Random.Range (20, 50));
				knightAI.rightHand.gameObject.AddComponent<BoxCollider> ();
				knightAI.rightHand.gameObject.AddComponent <Rigidbody> ().AddForce (direction * Random.Range (20, 50));
				while (knightAI.gold >= 100) {
					knightAI.gold -= 100;
					GameObject goldCoin = (GameObject) Instantiate (Resources.Load ("GoldCoin"), other.transform.position, Quaternion.identity);
					goldCoin.GetComponent<Rigidbody>().AddForce (direction * Random.Range (20, 50));
				}
			}
			if (other.gameObject.tag == "Wizard") {
				AIWizard wizardAI = other.gameObject.GetComponent<AIWizard> ();
				wizardAI.dead = true;
				wizardAI.gameObject.GetComponent<CapsuleCollider> ().enabled = false;
				wizardAI.staff.gameObject.AddComponent<BoxCollider> ();
				wizardAI.staff.gameObject.AddComponent <Rigidbody> ().AddForce (direction * Random.Range (20, 50));
				wizardAI.head.gameObject.AddComponent<BoxCollider> ();
				wizardAI.head.gameObject.AddComponent <Rigidbody> ().AddForce (direction * Random.Range (20, 50));
				wizardAI.backPack.gameObject.AddComponent<BoxCollider> ();
				wizardAI.backPack.gameObject.AddComponent <Rigidbody> ().AddForce (direction * Random.Range (20, 50));
				wizardAI.body.gameObject.AddComponent<BoxCollider> ();
				wizardAI.body.gameObject.AddComponent <Rigidbody> ().AddForce (direction * Random.Range (20, 50));
				wizardAI.leftHand.gameObject.AddComponent<BoxCollider> ();
				wizardAI.leftHand.gameObject.AddComponent <Rigidbody> ().AddForce (direction * Random.Range (20, 50));
				wizardAI.rightHand.gameObject.AddComponent<BoxCollider> ();
				wizardAI.rightHand.gameObject.AddComponent <Rigidbody> ().AddForce (direction * Random.Range (20, 50));
				while (wizardAI.gold >= 100) {
					wizardAI.gold -= 100;
					GameObject goldCoin = (GameObject) Instantiate (Resources.Load ("GoldCoin"), other.transform.position, Quaternion.identity);
					goldCoin.GetComponent<Rigidbody>().AddForce (direction * Random.Range (20, 50));
				}
			}
			if (other.gameObject.tag == "Rogue") {
				AIRogue rogueAI = other.gameObject.GetComponent<AIRogue> ();
				rogueAI.dead = true;
				rogueAI.gameObject.GetComponent<CapsuleCollider> ().enabled = false;
				rogueAI.leftSword.gameObject.AddComponent<BoxCollider> ();
				rogueAI.leftSword.gameObject.AddComponent <Rigidbody> ().AddForce (direction * Random.Range (20, 50));
				rogueAI.rightSword.gameObject.AddComponent<BoxCollider> ();
				rogueAI.rightSword.gameObject.AddComponent <Rigidbody> ().AddForce (direction * Random.Range (20, 50));
				rogueAI.head.gameObject.AddComponent<BoxCollider> ();
				rogueAI.head.gameObject.AddComponent <Rigidbody> ().AddForce (direction * Random.Range (20, 50));
				rogueAI.backPack.gameObject.AddComponent<BoxCollider> ();
				rogueAI.backPack.gameObject.AddComponent <Rigidbody> ().AddForce (direction * Random.Range (20, 50));
				rogueAI.body.gameObject.AddComponent<BoxCollider> ();
				rogueAI.body.gameObject.AddComponent <Rigidbody> ().AddForce (direction * Random.Range (20, 50));
				rogueAI.leftHand.gameObject.AddComponent<BoxCollider> ();
				rogueAI.leftHand.gameObject.AddComponent <Rigidbody> ().AddForce (direction * Random.Range (20, 50));
				rogueAI.rightHand.gameObject.AddComponent<BoxCollider> ();
				rogueAI.rightHand.gameObject.AddComponent <Rigidbody> ().AddForce (direction * Random.Range (20, 50));
				while (rogueAI.gold >= 100) {
					rogueAI.gold -= 100;
					GameObject goldCoin = (GameObject) Instantiate (Resources.Load ("GoldCoin"), other.transform.position, Quaternion.identity);
					goldCoin.GetComponent<Rigidbody>().AddForce (direction * Random.Range (20, 50));
				}
			}

			Destroy (other.gameObject, 3);
			Explode ();
		}

		if (other.gameObject.tag == "Player") {

			direction = rb.velocity;

			PlayerControls player = other.gameObject.GetComponent<PlayerControls> ();
			player.dead = true;
			player.head.gameObject.AddComponent<BoxCollider> ();
			player.head.gameObject.AddComponent <Rigidbody>().AddForce (direction * Random.Range(20,50));
			player.backPack.gameObject.AddComponent<BoxCollider> ();
			player.backPack.gameObject.AddComponent <Rigidbody>().AddForce (direction * Random.Range(20,50));
			player.body.gameObject.AddComponent<BoxCollider> ();
			player.body.gameObject.AddComponent <Rigidbody>().AddForce (direction * Random.Range(20,50));
			player.leftHand.gameObject.AddComponent<BoxCollider> ();
			player.leftHand.gameObject.AddComponent <Rigidbody>().AddForce (direction * Random.Range(20,50));
			player.rightHand.gameObject.AddComponent<BoxCollider> ();
			player.rightHand.gameObject.AddComponent <Rigidbody>().AddForce (direction * Random.Range(20,50));

			Explode ();
		}
	}
		
	void OnCollisionEnter(Collision col){


		if (col.gameObject.tag == "Mirror") {
			
		}
		else
		{
			Explode ();
		}
	}

	void Explode(){
		GameObject explosion = (GameObject) Instantiate (Resources.Load ("PlasmaExplosionEffect"),this.transform.position,this.transform.rotation);
		Destroy (explosion, 2);
		Destroy (this.gameObject);
	}
}
