using UnityEngine;
using System.Collections;

public class AppleBomb : MonoBehaviour {

	public GameObject explosion;
	public GameObject audioPlayer;
	public LayerMask damageableObjects;

	public AudioClip[] explosionSFX;

	Collider[] hitColliders;
	Rigidbody rb;
	AudioSource audioSource;
	float blastRadius = 8;

	void Start(){
		rb = GetComponent<Rigidbody>();
		rb.AddForce(Camera.main.gameObject.transform.forward * 30, ForceMode.Impulse);
		StartCoroutine(Explode());
		audioSource = audioPlayer.GetComponent<AudioSource>();
	}

//	void OnCollisionEnter(Collision col){
//		Instantiate(explosion, gameObject.transform.position, Quaternion.identity);
//		Destroy(gameObject);
//	}

	IEnumerator Explode(){
		yield return new WaitForSeconds(1);
		Destroy(Instantiate(explosion, gameObject.transform.position, Quaternion.identity) as GameObject, 3f);
		int index = Random.Range(0, explosionSFX.Length);
		audioSource.PlayOneShot(explosionSFX[index], 0.25f);
		audioPlayer.transform.parent = null;
		CheckRadius(audioPlayer.transform.position);
		Destroy(audioPlayer,2);
		Destroy(gameObject);
	}

	void CheckRadius(Vector3 explosionPoint){
		hitColliders = Physics.OverlapSphere(explosionPoint, blastRadius, damageableObjects);

		foreach(Collider col in hitColliders){
			if (col.gameObject.tag == "Tree" || col.gameObject.tag == "Pumpkin"){
				col.GetComponent<WateredObject>().DestroyWateredObject();
			}

			if (col.gameObject.tag == "Enemy"){
				col.GetComponent<Enemy>().Die();
			}

			if (col.gameObject.tag == "TreeDuplicator"){
				col.GetComponent<WaterableObject>().DestroySprout();
			}
		}
	}
}
