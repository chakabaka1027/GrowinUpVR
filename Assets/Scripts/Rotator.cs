using UnityEngine;
using System.Collections;

public class Rotator : MonoBehaviour {

	public AudioClip collect;
	AudioSource audioSource;
	GameObject lightSource;
	GameObject cookieVisual;

	// Use this for initialization
	void Start () {
		audioSource = gameObject.transform.FindChild("AudioSource").GetComponent<AudioSource>();
		lightSource = gameObject.transform.FindChild("Point light").gameObject;
		cookieVisual = gameObject.transform.FindChild("Cookie").gameObject;

	}


	// Update is called once per frame
	void Update () {

		transform.Rotate (new Vector3 (15, 30, 45) * Time.deltaTime);

	}

	void OnTriggerEnter(Collider col){
		if(col.gameObject.tag == "Player" && !FindObjectOfType<PlayerController>().hasCookie){
			gameObject.transform.DetachChildren();
			audioSource.PlayOneShot(collect, 0.5f);
			Destroy(audioSource.gameObject, 2);
			Destroy(lightSource);
			Destroy(cookieVisual);

			FindObjectOfType<PlayerController>().HasCookie();

            FindObjectOfType<PlayerUI>().StartCoroutine(FindObjectOfType<PlayerUI>().ActivateIntroText("Cookie Obtained!", .1f));

			Destroy(gameObject);
		}

	}

}
