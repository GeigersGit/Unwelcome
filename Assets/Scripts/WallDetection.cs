using UnityEngine;
using System.Collections;

public class WallDetection : MonoBehaviour {
    // Use this for initialization
    public bool front = false;
    public bool back = false;
    public bool left = false;
    public bool right = false;

    public GameObject frontwall;
    public GameObject backwall;
    public GameObject leftwall;
    public GameObject rightwall;

    void Start () {
        
	}

    void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.name == "Floor")
        {
            if (front)
                Destroy(frontwall);

            if (back)
                Destroy(backwall);

            if (left)
                Destroy(leftwall);

            if (right)
                Destroy(rightwall);

        }

    }
    
}
