using UnityEngine;
using System.Collections;

public class Cookie : MonoBehaviour {
	Rigidbody rb;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody>();
		rb.AddForce(Camera.main.gameObject.transform.forward * 30, ForceMode.Impulse);
		StartCoroutine(CookieCrumbleTime());
	}
	

	IEnumerator CookieCrumbleTime(){
		yield return new WaitForSeconds(20);

		Vector3 currentSize = gameObject.transform.localScale;
		float percent = 0;
		float shrinkTime = 1;
		float shrinkSpeed = 1 / shrinkTime; 
		while (percent < 1){
			percent += Time.deltaTime * shrinkSpeed;
			gameObject.transform.localScale = Vector3.Lerp(currentSize, Vector3.zero, percent);
			yield return null;
		}	

		Destroy(gameObject);
	}
}
