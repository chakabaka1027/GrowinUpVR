using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRMovement : MonoBehaviour {
    public GameObject cameraObject;
	
	// Update is called once per frame
	void Update () {
		cameraObject.transform.position = gameObject.transform.position;

        gameObject.transform.eulerAngles = new Vector3(gameObject.transform.eulerAngles.x, cameraObject.transform.eulerAngles.y, gameObject.transform.eulerAngles.z);
	}
}
