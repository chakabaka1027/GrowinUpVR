using UnityEngine;
using System.Collections;

public class FaceCamera : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Grow();
	}
	
	// Update is called once per frame
	void Update () {
		gameObject.transform.LookAt(Camera.main.gameObject.transform);
	}

	void Grow(){
		float percent = 0;
		float growTime = 0.5f;
		float growSpeed = 1/growTime;

		while (percent > 1){
		percent += Time.deltaTime * growSpeed;
			gameObject.transform.localScale = Vector3.Lerp(Vector3.zero, new Vector3(0.25f, 0.25f, 0.25f), percent);

		}
	}
}
