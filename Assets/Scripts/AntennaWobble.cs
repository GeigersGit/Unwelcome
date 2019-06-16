/// https://forum.unity3d.com/threads/car-antenna-physics.464619/
///
/// This script will make the antennas of your vehicle wiggle, by default its all set to auto so you just need to add the script, but if you dissable the autos it lets you define more stuff manually so you can adapt it.

/// If you improve the script please share it in the thread above
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntennaWobble : MonoBehaviour {
	public bool addRigidBody = true;    //automatically add rigidbody to target with default values
	public bool findAntenna = true;        //automatically find antennaObject, sets as antenna the first ofject in children with a renderer, so have only 1 and the target object.
	public bool automaticHelpers = false; // automatically generate neccesary gameobjects for the antenna effect to run, notice that you'll need the root object of the vehicle to not move with the vehicle, if its not your case dissble and generate these manually following the example prefab.

	public Transform standByTarget;     // when vehicle does not move, this is the target possition target should have.
	public Transform target;             // The object wich antena will look at, this objet must be outside hyerrchy of the moving element such as car. Can be child of root if root does not move.
	public GameObject antenna;            // The antenna object you want to wobble, it autoinitialices if "findantenna" is true;

	public float Drag =2.5f;                 // Target Drag
	public float SpringForce =80f;        //Target Spring
	private Rigidbody targetRB ;

	private Vector3 LocalDistance;    //Distance between the two points
	private Vector3 LocalVelocity;    //Velocity converted to local space

	public Vector3 lookAtAxis = new Vector3 (1,0,0);         //Adjust the axis of your antenna object for the look at operation


	void Awake(){
		//HELPERS
		if (automaticHelpers) {
			standByTarget = new GameObject("StandByTarget").transform;
			standByTarget.SetParent (this.transform);
			standByTarget.localPosition= new Vector3(0,1,0);


			target = new GameObject("Target").transform;
			target.SetParent (this.transform); // temporarily parent
			target.localPosition= new Vector3(0,1,0); // need initially i same position as standbytarget

			target.SetParent (this.transform.parent); // final parent

		}

		//RIGIDBODY
		if (target.GetComponent<Rigidbody>()) {
			targetRB = target.GetComponent<Rigidbody> ();
		}
		else {
			print ("target missing rigidBody, default RB added");
			targetRB = target.gameObject.AddComponent<Rigidbody>();
			targetRB.mass = 1f;
			targetRB.angularDrag = 0f;  
			targetRB.useGravity = false;
		}
		targetRB.drag = Drag; // alternativelly you could use the code bellow in fixed update with same ressults.

		//ANTENNA
		if(findAntenna)        antenna = transform.GetComponentInChildren<MeshRenderer>().gameObject;
		if (!antenna && !findAntenna)  print("antenna must be manually set or enable automatic antenna find");
	}


	void FixedUpdate () {
		//Calculate the distance between the two points
		LocalDistance = standByTarget.InverseTransformPoint(target.position);
		print("localdistance " + LocalDistance);
		targetRB.AddRelativeForce(-(LocalDistance.x)*SpringForce,-(LocalDistance.y)*SpringForce,-(LocalDistance.z)*SpringForce);//Apply Spring


		//Calculate the local velocity of the SpringObj point
		//LocalVelocity = (target.InverseTransformDirection(targetRB.velocity));
		//targetRB.AddRelativeForce(-LocalVelocity.x*Drag,-LocalVelocity.y*Drag,-LocalVelocity.z*Drag);//Apply Drag
	}

	void Update (){
		antenna.transform.LookAt(target.position,lookAtAxis);
		standByTarget.rotation = target.rotation;
	}
}
