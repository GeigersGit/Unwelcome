using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAt : MonoBehaviour {
	
	public Transform lookParent;
	public Transform target;
	
	void Update (){
		lookParent.transform.LookAt(target.position,new Vector3(0,0,1));
	}
}
