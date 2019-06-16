using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialChanger : MonoBehaviour {
	public bool enemyColor = false;
	Renderer rend;
	Color colorStart = Color.cyan;
	Color colorEnd = Color.cyan;
	float duration = 1F;
	float solidTime = 2f;
	float flickerTime = .5f;
	float timeStamp = 0;
	bool flickering = false;
	// Use this for initialization
	void Start () {
		rend = GetComponent<Renderer> ();
		timeStamp = Time.time + solidTime;
		colorStart = new Color (0f, 1f, 1f);
		if (enemyColor) {
			colorStart = Color.magenta;
		}
	}

	// Update is called once per frame
	void Update () {
		//float lerp = Mathf.PingPong(Time.time, duration) / duration;
		//rend.material.color = Color.Lerp(colorStart, colorEnd, lerp);

		if (timeStamp < Time.time) {
			if (!flickering) {
				timeStamp = Time.time + flickerTime;
				flickering = true;
			} else {
				timeStamp = Time.time + solidTime;
				flickering = false;
			}
		}

		if (flickering) {
			rend.material.color = colorEnd;
			colorEnd = colorStart * Random.Range (0.2f, 1f);
		} else {
			rend.material.color = colorStart;
		}



	}
}
