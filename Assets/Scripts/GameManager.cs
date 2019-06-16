using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Uninvited Guests

public class GameManager : MonoBehaviour {

	//Public Variables
	public int gold = 1000;

	//Public References
	public GameObject GameUI;
	public GameObject MenuUI;
	public GameObject enemySpawnPoint;
	public GameObject menuCameraTarget;
	public GameObject playerCameraTarget;

	//Private Variables
	private Text goldText;
	private float spawnTime = 0;
	private bool inMenu;
	private bool inGame = true;
	private Quaternion GameCameraRotation;
	private Quaternion MenuCameraRotation;

	// Use this for initialization
	void Start () {
		//singleton pattern
		GameObject other = GameObject.FindGameObjectWithTag ("GameManager");
		if (other != this.gameObject) {
			Destroy (this.gameObject);
		}
		DontDestroyOnLoad (this);

		//initialization
		inMenu = true;
		GameCameraRotation = Quaternion.Euler (64, 18, 5);
		MenuCameraRotation = Quaternion.Euler (0, 0, 0);
		spawnTime = Time.time + 2;
		//goldText = GameObject.Find ("GoldText").GetComponent<Text> ();
		//Physics.gravity = new Vector3 (0, -90.81f, 0);
	}
	  
	// Update is called once per frame
	void Update () {
		//prevents nullreference on scene reload
		if (!enemySpawnPoint) {
			enemySpawnPoint = GameObject.Find ("Enemy Spawn Point");
		}
		if (!goldText) {
			//goldText = GameObject.Find ("GoldText").GetComponent<Text>();
		}
		//update gold amount to screen
	    //goldText.text = "" + gold;

		//Show Main Menu
		if (inMenu) {
			//Camera.main.GetComponent<FollowTarget> ().target = menuCameraTarget.transform;
			//if(Camera.main.transform.rotation != MenuCameraRotation)
			//Camera.main.transform.rotation = Quaternion.RotateTowards (Camera.main.transform.rotation, MenuCameraRotation, 10f);
		}

		if (inGame) {
			if(Camera.main.transform.rotation != GameCameraRotation)
				Camera.main.transform.rotation = Quaternion.RotateTowards (MenuCameraRotation, GameCameraRotation, 5f);

			if (spawnTime < Time.time) {
				SpawnRandomEnemy ();
				spawnTime = Time.time + Random.Range (3, 6);
			}
		}
	}

	void SpawnRandomEnemy (){
		int selection = Random.Range (1, 4);
		if (selection == 1) {
			Instantiate(Resources.Load ("Knight"), enemySpawnPoint.transform.position, Quaternion.identity, enemySpawnPoint.transform);
		}
		if (selection == 2) {
			Instantiate(Resources.Load ("Wizard"), enemySpawnPoint.transform.position, Quaternion.identity, enemySpawnPoint.transform);
		}
		if (selection == 3) {
			Instantiate(Resources.Load ("Rogue"), enemySpawnPoint.transform.position, Quaternion.identity, enemySpawnPoint.transform);
		}
	}
}
