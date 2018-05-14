using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuCamera : MonoBehaviour {
	public Light left;
	public Light right;	
	FastMobileBloom fmb;
	float averageBuffer=0f;
	float averageDecrease=0f;
	float lAverageBuffer=0f;
	float lAverageDecrease=0f;
	Camera cam;
	float destination=0f;
	float[] samples = new float[64];

	void Start () {
		fmb = gameObject.GetComponent<FastMobileBloom> ();
		cam = gameObject.GetComponent<Camera> ();
	}
	void Update () {
		parseSound ();
		RaycastMenu ();
		if (transform.position.x != destination) {
			if (destination == 25f) {
				transform.position = Vector3.Lerp (transform.position, new Vector3(destination,1f,transform.position.z), 0.05f);
			} else if (destination == 0f) {
				transform.position = Vector3.Lerp (transform.position, new Vector3(destination,1.9f,transform.position.z), 0.05f);
			}
		}
	}
	void parseSound(){
		float average = 0f;
		float lightAverage = 0f;
		AudioListener.GetSpectrumData (samples, 0, FFTWindow.Blackman);
		average = samples[0]+samples[1];
		if (average > averageBuffer) {
			averageBuffer = average;
			averageDecrease = 0.0005f;
		} else {
			averageBuffer -= averageDecrease;
			averageDecrease *= 1.2f;
		}
		left.intensity = averageBuffer * 4;
		cam.fieldOfView = 60+average / 2;	

		for (int i = 2; i <= 16; i++) {
			lightAverage += samples [i];
		}
		if (lightAverage > lAverageBuffer) {
			lAverageBuffer = lightAverage;
			lAverageDecrease = 0.0005f;
		} else {
			lAverageBuffer -= lAverageDecrease;
			lAverageDecrease *= 1.2f;
		}
		right.intensity = lAverageBuffer*4;
		fmb.threshold = 0.45f - lAverageBuffer/4;
	}
	void RaycastMenu(){
		if ((Input.touchCount > 0) && (Input.GetTouch(0).phase == TouchPhase.Began))
		{
			Ray raycast = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
			RaycastHit raycastHit;
			if (Physics.Raycast(raycast, out raycastHit))
			{
				
				if (raycastHit.collider.name=="Garage") {
					destination = 25f;
				} else if (raycastHit.collider.name=="BackButton") {
					destination = 0f;
				}
				else if (raycastHit.collider.name=="Play") {
					SceneManager.LoadScene (1);
				} else if (raycastHit.collider.name=="Quit") {
					Application.Quit ();
				}
			}
		}
	}
}
