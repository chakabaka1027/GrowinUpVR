using UnityEngine;
using System.Collections;

public class Shell : MonoBehaviour {

	public Rigidbody rb;
	public float minForce;
	public float maxForce;
	public Color trailColor;

	float fadeTime = 2;
	float lifeTime = 2;

	// Use this for initialization
	void Start () {

		GetComponent<TrailRenderer>().material.SetColor("_TintColor", trailColor);


		float force = Random.Range(minForce, maxForce);

		rb.AddForce(transform.right * force);
		rb.AddTorque(Random.insideUnitSphere * force);

		StartCoroutine(Fade());
	}

	IEnumerator Fade () {
		yield return new WaitForSeconds(lifeTime);

		float percent = 0;
		float fadeSpeed = 1 / fadeTime;
		Material mat = GetComponent<Renderer>().material;
		Color initialColor = mat.color;

		while (percent < 1){
			percent += Time.deltaTime * fadeSpeed;
			mat.color = Color.Lerp(initialColor, Color.clear, percent);
			yield return null;
		}
		Destroy(gameObject);

	}

}
