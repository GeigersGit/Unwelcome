using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerControls : MonoBehaviour {

	private GameManager gm;
	Vector2 joystickPosition;
	public bool dead = false;
	bool rolling = false;
	bool rollOffCooldown = true;
	int rollFrames = 15;
	int frameCount = 0;
	float playerMovementSpeed = 10;
	float playerRollSpeed = 40;
	float hInput;
	float vInput;
	float timeStamp;
	float rollCoolDown = .5f; //seconds
	float originalY = 0;
	float offset = 0;
	Rigidbody rb;
	Vector3 movementDir;
	int deadFrames = 100;
	public GameObject playerBody;
	FollowTarget ft;
	CapsuleCollider collider;
	public GameObject cameraTarget;

	public GameObject head;
	public GameObject body;
	public GameObject backPack;
	public GameObject leftHand;
	public GameObject rightHand;

	void Start () {
		rb = this.GetComponent<Rigidbody> ();
		originalY = playerBody.transform.position.y;
		ft = Camera.main.GetComponent<FollowTarget>();
		collider = GetComponent<CapsuleCollider> ();
		gm = GameObject.FindGameObjectWithTag ("GameManager").GetComponent<GameManager> ();
	}

	void Update () {

		if (!dead) {
			
			//Get joystick positions
			joystickPosition = SimpleJoystick.GetPosition("Movement");
			movementDir = new Vector3 (joystickPosition.x * playerMovementSpeed, 0, joystickPosition.y * playerMovementSpeed);

			//Set velocity and direction
			rb.velocity = movementDir;
			if (movementDir == Vector3.zero) {
				rb.freezeRotation = true;
			} else {
				rb.freezeRotation = false;
				transform.rotation = Quaternion.LookRotation (movementDir);
			}

			//Walking Bob
			//if (rb.velocity.magnitude > 1 && !rolling) {
			//	offset = Mathf.Sin (Time.time * rb.velocity.magnitude * 3) / 20;
			//	playerBody.transform.position = new Vector3 (transform.position.x, originalY + offset, transform.position.z);
			//} else {
			//	playerBody.transform.position = new Vector3 (transform.position.x, originalY, transform.position.z);
			//}

			//Roll Button Pressed
			if (CrossPlatformInputManager.GetButtonDown ("Jump") && rollOffCooldown) {
				rolling = true;
				rollOffCooldown = false;
				timeStamp = Time.time + rollCoolDown;
			}

			if (!rollOffCooldown && Time.time > timeStamp)
				rollOffCooldown = true;

			//Attack Button Pressed
			if (CrossPlatformInputManager.GetButtonDown ("Fire1")) {
			
			}

			//Build Button Pressed
			if (CrossPlatformInputManager.GetButtonDown ("Fire2")) {

			}
		}
    }
		
	void FixedUpdate()
	{
		if (rolling && !dead) {
			rb.velocity = transform.forward * playerRollSpeed;
			frameCount++;
			playerBody.transform.Rotate (360/rollFrames, 0, 0);
			if (frameCount == rollFrames) {
				rolling = false;
				frameCount = 0;
			}
		}
		if(dead){
			//ft.target = null;
			collider.enabled = false;
			cameraTarget.transform.parent = head.transform;
			deadFrames--;
			if (deadFrames == 0) {
				gm.gold = 1000;
				Application.LoadLevel(Application.loadedLevel);
			}
		}
	}
}
