using UnityEngine;
using System.Collections;

public class WeaponBreathing : MonoBehaviour {

	Vector3 startPosition;
	public float amplitude = 0.01f;
	public float interval = 1f;
	public float frequency = 2f;

	// Use this for initialization
	void Start () {
		startPosition = transform.localPosition;	
	}

	// Update is called once per frame
	void FixedUpdate () {
		float x = startPosition.x;
		float y = amplitude * Mathf.Sin (Time.timeSinceLevelLoad * frequency + Mathf.PI/(interval)) + startPosition.y;
		float z = startPosition.z;

		transform.localPosition = new Vector3 (x, y, z);

	}
}