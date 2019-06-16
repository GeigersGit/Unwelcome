using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIRogue : MonoBehaviour {
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

	//Rogue Specific
	private float jumpTimeStamp = 0;
	private float jumpTime = 3f;
	private bool jumping;
	private int jumpFrames = 30;
	private int currentFrame = 0;
	private float jumpStart = 0;
	private float jumpDistance = 0;
	public int animationCount = 0;
	private int animationFrames = 20;
	public bool swinging = false;
	public bool swingActive = false;
	private GameObject jumpTarget;
	public Transform leftArm;
	public Transform rightArm;
	public TrailRenderer rightTrailRenderer;
	public TrailRenderer leftTrailRenderer;
	public SwordScript leftSwordScript;
	public SwordScript rightSwordScript;
	private float swingSpeed = 0.7f;


	//Body Parts
	public Transform head;
	public Transform body;
	public Transform backPack;
	public Transform leftHand;
	public Transform rightHand;
	public Transform leftSword;
	public Transform rightSword;
	public Transform Model;

	// Use this for initialization
	void Start () {
		gm = GameObject.FindGameObjectWithTag ("GameManager").GetComponent<GameManager>();
		leavePoint = GameObject.Find ("LeavePoint");
		agent = this.GetComponent<NavMeshAgent> ();
		chest = GameObject.FindGameObjectWithTag ("Chest");
		player = GameObject.FindGameObjectWithTag ("Player");
		rb = this.GetComponent<Rigidbody> ();
		jumpTarget = GameObject.FindGameObjectWithTag ("JumpTarget");
		jumpStart = Mathf.PI / 2;
		voiceClips = Resources.LoadAll ("Voices");
		audio = GetComponent<AudioSource> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (!dead) {

			//Enter Combat if you have line of sight to the player and are within minimum distance
			if ((transform.position - player.transform.position).magnitude < 17) {
				RaycastHit hit;
				Vector3 rayDirection = (player.transform.position - transform.position);
				if (Physics.Raycast (transform.position, rayDirection, out hit)) {
					if (hit.collider.gameObject.tag == "Player") {
						inCombat = true;
					} else
						inCombat = false;
				}
			} else {
				inCombat = false;
			}

			//If jumping or swinging, stay in combat
			if (jumping || swinging)
				inCombat = true;

			if (gold >= 300 || gm.gold == 0) {
				inCombat = false;
			}

			//OUT OF COMBAT
			if (!inCombat) {
				agent.destination = chest.transform.position;
				agent.speed = 4.5f;
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
				agent.speed = 7;
				//Move toward the player
				if (!jumping) {
					agent.destination = player.transform.position;

					//Look at player
					Quaternion playerDirection = Quaternion.LookRotation (player.transform.position - transform.position);
					transform.rotation = Quaternion.RotateTowards (transform.rotation, playerDirection, 10f);
				}

				//If close enough, attempt to swing at player
				if ((transform.position - player.transform.position).magnitude < 7) {
					swinging = true;
				}



				//Jump toward the player every 3 seconds
				if(jumpTimeStamp < Time.time && !swinging){
					jumping = true;
					transform.rotation = Quaternion.LookRotation (jumpTarget.transform.position - transform.position);;
					jumpTimeStamp = Time.time + jumpTime;
					jumpDistance = Mathf.Abs ((transform.position - jumpTarget.transform.position).magnitude);
					agent.enabled = false;
				}


			}


		} else {//What to do when dead
			if(!onDeath){
				swinging = false;
				audio.clip = (AudioClip)voiceClips [Random.Range (0, voiceClips.Length)];
				//audio.Play ();

				onDeath = true;
			}
		}
	}

	void FixedUpdate(){
		//Jump animation
		if (jumping) {
			if (currentFrame < jumpFrames) {
				transform.position += new Vector3 (0, (Mathf.Sin (jumpStart))/2, 0);
				transform.position += transform.forward * jumpDistance / jumpFrames;
				Model.transform.Rotate (360 / (jumpFrames / 3), 0, 0);
				jumpStart += Mathf.PI / jumpFrames;
				currentFrame++;
			} else {
				jumping = false;
				agent.enabled = true;
				currentFrame = 0;
				jumpStart = Mathf.PI/2;
			}
		}

		if (swinging) {

			//Readies arm
			if(animationCount == 0){
				if (animationFrames == 0) {
					animationCount++;
					animationFrames = 5;
				}
				animationFrames--;
			}

			//Right arm swings
			if(animationCount == 1){
				rightTrailRenderer.enabled = true;
				swingActive = true;
				if (animationFrames == 0) {
					animationCount++;
					animationFrames = 10;
					rightTrailRenderer.enabled = false;
					swingActive = false;
				}
				rightArm.Rotate (0, 40, 0);
				transform.localPosition += transform.forward/2;
				animationFrames--;
			}

			//Pause
			if(animationCount == 2){
				if (animationFrames == 0) {
					animationCount++;
					animationFrames = 5;
				}
				animationFrames--;
			}

			//Left arm swings
			if(animationCount == 3){
				leftTrailRenderer.enabled = true;
				swingActive = true;
				if (animationFrames == 0) {
					leftTrailRenderer.enabled = false;
					swingActive = false;
					animationCount++;
					animationFrames = 10;
				}
				leftArm.Rotate (0, 40, 0);
				transform.localPosition += transform.forward/2;
				animationFrames--;
			}

			//Pause
			if(animationCount == 4){
				if (animationFrames == 0) {
					animationCount++;
					animationFrames = 10;
				}
				animationFrames--;
			}

			//Reset Arms
			if(animationCount == 5){
				if (animationFrames == 0) {
					animationCount++;
					animationFrames = 5;
				}
				leftArm.Rotate (0, -20, 0);
				rightArm.Rotate (0, -20, 0);
				animationFrames--;
			}

			//Pause
			if(animationCount == 6){
				if (animationFrames == 0) {
					swinging = false;
					animationCount = 0;
					animationFrames = 20;
				}
				animationFrames--;
			}
		}
	}
}
