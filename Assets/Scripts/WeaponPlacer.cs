using UnityEngine;
using System.Collections;

public class WeaponPlacer : MonoBehaviour {
	public bool floorTile = false;
	public bool wallTile = false;
    bool hover = false;
    private Renderer rend;
	private GameManager gm;

    // Use this for initialization
    void Start () {
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

    void Update () {

        if (hover){
			if (Input.GetMouseButtonUp (0) && gm.gold >= 100) {
				if (floorTile) {
				}
				if (wallTile) {
					Instantiate (Resources.Load ("Mirror"), new Vector3 (transform.position.x, transform.position.y, transform.position.z), this.gameObject.transform.rotation, this.gameObject.transform.parent.transform);
					gm.gold -= 100;
				}
			}
        }
	}
}
