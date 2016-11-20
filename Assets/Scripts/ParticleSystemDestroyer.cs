using UnityEngine;
using System.Collections;

public class ParticleSystemDestroyer : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

	// Update is called once per frame
	void Update () {
		ParticleSystem ps = gameObject.GetComponent<ParticleSystem>();
		Destroy(gameObject, ps.startLifetime);
	}
}
