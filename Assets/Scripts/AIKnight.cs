using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIKnight : MonoBehaviour {
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

	//Knight Specific
	private float speed;
	private bool ready = false;
	private bool swinging = false;
	private bool jumping = false;
	private int jumpFrames = 5;
	private int animationCount = 0;
	public Transform hammerPivot;
	private float animationTime = 0;
	public Transform knightModel;
	private float startSin = 0;
	public GameObject hammerHurtBox;

	//Body Parts
	public Transform helm;
	public Transform body;
	public Transform backPack;
	public Transform hammer;
	public Transform rightHand;
	public Transform leftHand;


	// Use this for initialization
	void Start () {
		gm = GameObject.FindGameObjectWithTag ("GameManager").GetComponent<GameManager>();
		leavePoint = GameObject.Find ("LeavePoint");
		agent = this.GetComponent<NavMeshAgent> ();
		chest = GameObject.FindGameObjectWithTag ("Chest");
		player = GameObject.FindGameObjectWithTag ("Player");
		rb = this.GetComponent<Rigidbody> ();
		startSin = Mathf.PI / 2;
		voiceClips = Resources.LoadAll ("Voices");
		audio = GetComponent<AudioSource> ();
	}
	
	// Update is called once per frame
	void Update () {

		if (!dead) {

			//Enter Combat if you have line of sight to the player and are within minimum distance
			if ((transform.position - player.transform.position).magnitude < 25) {
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

			if (swinging) {
				inCombat = true;
			}

			if (gold >= 300 || gm.gold == 0) {
				inCombat = false;
			}

			//OUT OF COMBAT
			if (!inCombat) {
				agent.destination = chest.transform.position;
	
				//stop at chest when next to it to start stealing gold
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
		
				//Return to normal speed if charging
				agent.speed = 4.5f;
				agent.acceleration = 8;

			//IN COMBAT
			}else {
				
				//Face the player
				if (!swinging) {
					agent.destination = player.transform.position;
					Quaternion playerDirection = Quaternion.LookRotation (player.transform.position - transform.position);
					transform.rotation = Quaternion.RotateTowards (transform.rotation, playerDirection, 10f);
				}

				//Increase run speed
				agent.speed = 10f;
				agent.acceleration = 50;
				speed = agent.velocity.magnitude;

				//Decrease turn speed as velocity increases
				rb.maxAngularVelocity = (float)1/rb.velocity.magnitude * 300;

				//Stop and swing hammer when close
				if ((transform.position - player.transform.position).magnitude < 5 && !swinging) {
					transform.rotation = Quaternion.LookRotation (player.transform.position - transform.position);
					swinging = true;
					animationTime = Time.time + 0.4f;
					agent.enabled = false;
				}


			}
		} else {//What to do when dead
			if(!onDeath){
				audio.clip = (AudioClip)voiceClips [Random.Range (0, voiceClips.Length)];
				//audio.Play ();

				onDeath = true;
			}
		}
	}

	void FixedUpdate(){
		if (swinging) {

			//Jump up and Raise Hammer
			if (animationCount == 0) {
				hammerPivot.localRotation = Quaternion.RotateTowards (hammerPivot.localRotation, Quaternion.Euler (-74.4f, 122f, -60f), 4f);
				knightModel.transform.position += new Vector3 (0, 0.2f, 0);
				transform.position += transform.forward/5f;
				if (Time.time > animationTime) {
					animationCount++;
					animationTime = Time.time + 0.1f;
				}
			}

			//Pause Midair
			if (animationCount == 1) {
				
				if (Time.time > animationTime) {
					animationCount++;
					animationTime = Time.time + 0.01f;
				}
			}

			//Swing Hammer down halfway to avoid rotating the opposite direction
			if (animationCount == 2) {
				hammerPivot.localRotation = Quaternion.RotateTowards (hammerPivot.localRotation, Quaternion.Euler (-24f, -4f, 64f), 30f);
				if (Time.time > animationTime){
					animationCount++;
					animationTime = Time.time + 0.05f;
					GameObject hammerDown = (GameObject)Instantiate(Resources.Load ("HammerDownEffect"),transform.position + transform.forward * 2.5f,Quaternion.identity);
					Destroy (hammerDown, 2);
					hammerHurtBox.SetActive (true);
				}
			}
			//Swing Hammer down the rest of the way and drop back down
			if (animationCount == 3) {
				hammerPivot.localRotation = Quaternion.RotateTowards (hammerPivot.localRotation, Quaternion.Euler (62f, -134f, -39f), 40f);
				knightModel.transform.position += new Vector3 (0, -1.6f, 0);
				if (Time.time > animationTime){
					knightModel.transform.localPosition = new Vector3 (knightModel.transform.localPosition.x, 0.11f, knightModel.transform.localPosition.z);
					animationCount++;
					animationTime = Time.time + 0.5f;
					hammerHurtBox.SetActive(false);
				}
			}
			//Pause
			if (animationCount == 4) {
				hammerPivot.localRotation = Quaternion.RotateTowards (hammerPivot.localRotation, Quaternion.Euler (62f, -134f, -39f), 20f);
				if (Time.time > animationTime){
					animationCount++;
					animationTime = Time.time + 0.25f;
				}
			}
			//Original position
			if (animationCount == 5) {
				hammerPivot.localRotation = Quaternion.RotateTowards (hammerPivot.localRotation, Quaternion.Euler (0f, 30f, 0f), 30f);
				if (Time.time > animationTime){
					swinging = false;
					agent.enabled = true;
					animationCount = 0;
					knightModel.transform.localPosition = new Vector3 (0, 0, 0);
				}
			}
		}
	}
}

