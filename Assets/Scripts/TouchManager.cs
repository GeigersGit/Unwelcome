using UnityEngine;
using System.Collections;

public class TouchManager : MonoBehaviour {

	Vector3 touchPosWorld;
	RaycastHit hit;
	string text = "nothing";

	//Change me to change the touch phase used.
	TouchPhase release = TouchPhase.Ended;
	TouchPhase hold = TouchPhase.Stationary;
	TouchPhase moved = TouchPhase.Moved;
	TouchPhase begin = TouchPhase.Began;

	void OnGUI()
	{
		int w = Screen.width, h = Screen.height;

		GUIStyle style = new GUIStyle();
		style.normal.textColor = new Color (1.0f, 1.0f, 1.0f, 1.0f);
		Rect rect = new Rect(0, 300, w, h * 2 / 100);
		GUI.Label(rect, text, style);
	}
	void Update() {
		//We check if we have more than one touch happening.
		//We also check if the first touches phase is Ended (that the finger was lifted)
		if (Input.touchCount > 0) {
			for (int i = 0; i < Input.touchCount; i++) {

				//finger is on the screen
				if (Input.GetTouch (i).phase == hold || Input.GetTouch (i).phase == moved || Input.GetTouch (i).phase == begin) {
					//transform the touch position into word space from screen space and store it.
					Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch (i).position); 
					//raycast with this information. If we have hit something we can process it.
					Physics.Raycast (ray, out hit, 200f);

					if (hit.collider != null && hit.transform.gameObject.tag == "Panel") {
						GameObject touchedObject = hit.transform.gameObject;
						touchedObject.GetComponent<GridPlacer> ().hover = true;
						text = "Hovering over " + touchedObject.name + ".";
					} 

					else if (hit.collider != null) {
						GameObject touchedObject = hit.transform.gameObject;
						text = "Hovering over " + touchedObject.name + ".";
					}
				}

				//Finger has lifted up
				else if (Input.GetTouch (i).phase == release) {
					//transform the touch position into word space from screen space and store it.
					Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch (i).position); 
					//raycast with this information. If we have hit something we can process it.
					Physics.Raycast (ray, out hit, 200f);

					if (hit.collider != null && hit.transform.gameObject.tag == "Panel") {
						GameObject touchedObject = hit.transform.gameObject;
						touchedObject.GetComponent<GridPlacer> ().tapped = true;
						text = "Released from " + touchedObject.name + ".";
					} 

					else if (hit.collider != null) {
						GameObject touchedObject = hit.transform.gameObject;
						text = "Released from " + touchedObject.name + ".";
					}
				}
			}
		}
	}
}
