using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WateredObject : MonoBehaviour {

	public bool isOnFire = false;
	public GameObject fire;

	[Header("Sounds")]
	public AudioClip growSound;

	[Header("Attributes")]
	public float heal = 20;
	float growTime = 1.5f;
	public float randomGrowthMin= 0.7f;
	public float randomGrowthMax= 1f;


	[Header("UI Elements")]
	public GameObject damageableUI;
	public Image damageableFill;

	[Header("Effects")]
	public GameObject explosion;
	public bool isDamaged = false;

	public float damageFillPercentage = 0f;
	Camera viewCamera;
	PlayerController player;


	// Use this for initialization
	void Start () {
		StartCoroutine(Grow());

		if (damageableFill != null){
			damageableFill.fillAmount = 0;
		}

		viewCamera = Camera.main;
		player = FindObjectOfType<PlayerController>();

	}

	public void FillDamage(float damageAmount){

		if (isDamaged == false){
			damageFillPercentage += Time.deltaTime * damageAmount;

//			damageableFill.fillAmount += Time.deltaTime * damageAmount;
//			damageFillPercentage = damageableFill.fillAmount;
		}

		if (damageFillPercentage >= 1){
			GetComponent<BoxCollider>().enabled = false;
			GetComponent<MeshRenderer>().enabled = false;
			gameObject.SetActive(false);

			DestroyWateredObject();
		}

	}

	public void DestroyWateredObject(){
		player.GainHealth(heal);
		isOnFire = false;

			FindObjectOfType<PlayerUI>().SubtractGrowCount();
			Destroy(Instantiate(explosion, this.gameObject.transform.position + Vector3.up * 1f, Quaternion.Euler(-90, 0, 0)) as GameObject, 5f);
			damageableUI.SetActive(false);
			Destroy(gameObject);
	}

	public void SetDamageableVisible(){
		damageableUI.SetActive(true);

		if (gameObject.tag == "Pumpkin"){
			Vector3 uiLocation = viewCamera.WorldToScreenPoint(transform.position + new Vector3(0f, 1.25f, 0f));
			damageableUI.transform.position = uiLocation;
		} else if (gameObject.tag == "Tree"){
			Vector3 uiLocation = viewCamera.WorldToScreenPoint(transform.position + new Vector3(0f, 3f, 0f));
			damageableUI.transform.position = uiLocation;
		} 
	}

	public IEnumerator Grow(){
		
		float randomGrowth = Random.Range(randomGrowthMin, randomGrowthMax);
		float percent = 0;
		float growSpeed = 1 / growTime; 

		while (percent < 1){
			percent += Time.deltaTime * growSpeed;
			gameObject.transform.localScale = Vector3.Lerp(Vector3.zero, new Vector3(randomGrowth, randomGrowth, randomGrowth), percent);
			yield return null;
		}

	}

	public IEnumerator OnFire(){
		if (isOnFire == true){
			GameObject flame = Instantiate(fire, gameObject.transform.position, Quaternion.identity) as GameObject;
			flame.transform.parent = gameObject.transform;

			while(isOnFire == true){
				
				yield return new WaitForSeconds(0.5f);
				FillDamage(1);
			}

		}
	}

}