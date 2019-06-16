using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour {

	private int health = 100;
	private int stolenWood = 0;
	private int stolenMetal = 0;
	public bool dead = false;

	private NavMeshAgent agent;
	private Rigidbody rb;
	private bool inCombat = false;
	private float chargeTimeStamp = 0;
	private float coolDownStamp = 0;

	GameObject player;
	GameObject chest;

	public bool knight = false;
		private ParticleSystem fire;
		private float speed;
		private ParticleSystem fireLight;

	public bool wizard = false;
		private float chargeTime = 1.5f; //seconds 
		private bool charging = false;
		private float fireballCooldown = 1f; //seconds
		private bool offCooldown = true;

	public bool rogue = false;

	// Use this for initialization
	void Start () {
		agent = this.GetComponent<NavMeshAgent> ();
		chest = GameObject.FindGameObjectWithTag ("Chest");
		player = GameObject.FindGameObjectWithTag ("Player");
		rb = this.GetComponent<Rigidbody> ();
		fire = GameObject.FindGameObjectWithTag("FireParticleEffect").GetComponent<ParticleSystem>();
		fireLight = GameObject.FindGameObjectWithTag ("FireLight").GetComponent<ParticleSystem>();

	}



	// Update is called once per frame
	void Update () {

		if (!dead) {
			//Only enter combat if player is within line of sight
			if ((transform.position - player.transform.position).magnitude < 15) {
				RaycastHit hit;
				Vector3 rayDirection = (player.transform.position - transform.position);
				if (Physics.Raycast (transform.position, rayDirection, out hit)) {
					if (hit.collider.gameObject.tag == "Player") {
						inCombat = true;
					} else
						inCombat = false;
				}
			} else
				inCombat = false;

			if (charging == true) {
				inCombat = true;
			}

			//Move toward chest if not in combat, steal wood and metal from chest, then escape
			if (!inCombat) {
				agent.destination = chest.transform.position;

				//stop at chest when next to it to start stealing supplies
				if ((transform.position - chest.transform.position).magnitude < 3) {
					agent.destination = transform.position;
				}


				if (knight) {
					agent.speed = 4.5f;
					agent.acceleration = 8;
					var mainModule = fire.main;
					mainModule.startSize = 0;
				}


			}
		//IN COMBAT
		else {
				if (knight) {
					//Face the player
					agent.destination = player.transform.position;
					Quaternion playerDirection = Quaternion.LookRotation (player.transform.position - transform.position);
					transform.rotation = Quaternion.RotateTowards (transform.rotation, playerDirection, 10f);

					//Increase run speed
					agent.speed = 50f;
					agent.acceleration = 25;
					speed = agent.velocity.magnitude;

					var mainModule = fire.main;
					var lightModule = fireLight.lights;
					if (speed > 20) {
						speed /= 10;

						mainModule.startSize = speed;
						lightModule.intensityMultiplier = speed;
					} else {
						mainModule.startSize = 0;
						lightModule.intensityMultiplier = 0;
					}
			

				}
				if (wizard) {

					//Face the player
					agent.destination = transform.position;
					Quaternion playerDirection = Quaternion.LookRotation (player.transform.position - transform.position);
					transform.rotation = Quaternion.RotateTowards (transform.rotation, playerDirection, 3f);


					if (!charging && offCooldown) {
						//start charging
						chargeTimeStamp = Time.time + chargeTime;
						charging = true;
						GameObject chargingfireBall = (GameObject)Instantiate (Resources.Load ("FireballCharging"), transform.position + (this.transform.forward * 2) + (this.transform.up), playerDirection, this.transform);
						Destroy (chargingfireBall, 1.5f);
					}
					if (charging && chargeTimeStamp < Time.time) {
						//fire
						charging = false;
						offCooldown = false;
						coolDownStamp = Time.time + fireballCooldown;
						GameObject fireBall = (GameObject)Instantiate (Resources.Load ("Fireball"), transform.position + (this.transform.forward * 3) + (this.transform.up), playerDirection);
						fireBall.GetComponent<Rigidbody> ().AddForce (this.transform.forward * 2000);
						Destroy (fireBall, 3);
					} else {
						//continue charging
					}
					if (coolDownStamp < Time.time) {
						//cooldown is finished
						offCooldown = true;
					}
				}
				if (rogue) {
					if ((transform.position - chest.transform.position).magnitude < 2) {
						agent.destination = transform.position;
					}
				}
			}
		}
	}
}
