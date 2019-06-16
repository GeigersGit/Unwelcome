using UnityEngine;
using System.Collections;

public class GridPlacer : MonoBehaviour {
    public bool hover = false;
	public bool tapped = false;

    private Object SquarePrefab;

    private Renderer rend;

    public bool front = false;
    public bool back = false;
    public bool right = false;
    public bool left = false;
	public bool wall = false;
	public bool floor = false;

    Vector3 offset;
	private GameManager gm;
    


	// Use this for initialization
	void Start ()
    {
		SquarePrefab = Resources.Load ("Square");//GameObject.Find("SquareOriginal");
        rend = GetComponent<Renderer>();
        rend.enabled = false;
		gm = GameObject.FindGameObjectWithTag ("GameManager").GetComponent<GameManager> ();
    }

    void OnMouseEnter()
    {

        hover = true;
        rend.enabled = true;
    }
    void OnMouseExit()
    {
        hover = false;
        rend.enabled = false;
    }


	// Update is called once per frame
	void Update ()
    {


		if (hover && (Input.GetMouseButtonDown(0))) {
			tapped = true;
			//rend.enabled = true;
			//hover = false;

		} else {
			//rend.enabled = false;
		}


		if (right || left || back || front) {
			if (tapped && gm.gold >= 100) {
				if (front)
					offset = new Vector3 (0, 0, 10);

				if (back)
					offset = new Vector3 (0, 0, -10);

				if (right)
					offset = new Vector3 (10, 0, 0);

				if (left)
					offset = new Vector3 (-10, 0, 0);

				Instantiate (SquarePrefab, transform.parent.position + offset, transform.parent.rotation);
				gm.gold -= 100;
				Destroy (this.gameObject);
			}
		}

		if (wall) {
			if (tapped && gm.gold >= 100) {
				Instantiate (Resources.Load ("Mirror"), new Vector3 (transform.position.x, transform.position.y, transform.position.z), this.gameObject.transform.rotation, this.gameObject.transform.parent.transform);
				gm.gold -= 100;
				Destroy (this.gameObject);
			}
		}
			

	}
}
