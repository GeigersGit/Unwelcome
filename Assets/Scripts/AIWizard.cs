using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIWizard : MonoBehaviour {
	//Inherited
	public bool dead = false;
	public int gold = 200;

	private float stealTime = 0;
	private bool inCombat = false;
	private bool onDeath = false;
	private NavMeshAgent agent;
	private Rigidbody rb;
	private GameObject player;
	private GameObject chest;
	private GameManager gm;
	private GameObject leavePoint;
	private Object[] voiceClips;
	private AudioSource audio;

	//Wizard Specific
	private float chargeTime = 1.5f; //seconds 
	private bool charging = false;
	private float chargeTimeStamp = 0;
	private float coolDownStamp = 0;
	private float fireballCooldown = 1f; //seconds
	private bool offCooldown = true;

	//Body Parts
	public Transform staff;
	public Transform body;
	public Transform head;
	public Transform backPack;
	public Transform leftHand;
	public Transform rightHand;

	// Use this for initialization
	void Start () {
		gm = GameObject.FindGameObjectWithTag ("GameManager").GetComponent<GameManager>();
		leavePoint = GameObject.Find ("LeavePoint");
		agent = this.GetComponent<NavMeshAgent> ();
		chest = GameObject.FindGameObjectWithTag ("Chest");
		player = GameObject.FindGameObjectWithTag ("Player");
		rb = this.GetComponent<Rigidbody> ();
		voiceClips = Resources.LoadAll ("Voices");
		audio = GetComponent<AudioSource> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (!dead) {

			//Enter Combat if you have line of sight to the player and are within minimum distance
			if ((transform.position - player.transform.position).magnitude < 15) {
				RaycastHit hit;
				Vector3 rayDirection = (player.transform.position - transform.position);
				if (Physics.Raycast (transform.position, rayDirection, out hit)) {
					if (hit.collider.gameObject.tag == "Player") {
						inCombat = true;
					//Player is out of line of sight
					} else {
						inCombat = false;
					}
				}
			//Player is out of range
			} else {
				inCombat = false;
			}
			//Finish the cast before exiting combat
			if (charging == true) {
				inCombat = true;
			}

			if (gold >= 300 || gm.gold == 0) {
				inCombat = false;
			}

			//OUT OF COMBAT
			if (!inCombat) {
				agent.destination = chest.transform.position;

				//stop at chest when next to it to start stealing supplies
				if ((transform.position - chest.transform.position).magnitude < 3) {
					agent.destination = transform.position;
					stealTime += Time.deltaTime;
					if (stealTime >= 1 && gm.gold >= 100) {
						gold += 100;
						gm.gold -= 100;
						stealTime = 0;
					}
				}

				if (gold >= 300 || gm.gold == 0) {
					agent.destination = leavePoint.transform.position;
				}
			//IN COMBAT
			} else {
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
			}
			if (coolDownStamp < Time.time) {
				//cooldown is finished
				offCooldown = true;
			}

		} else {//What to do when dead
			if(!onDeath){
				audio.clip = (AudioClip)voiceClips [Random.Range (0, voiceClips.Length)];
				//audio.Play ();

				onDeath = true;
			}
		}
	}
}
