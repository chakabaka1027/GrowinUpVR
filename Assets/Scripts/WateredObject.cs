using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WateredObject : MonoBehaviour {

	public bool isOnFire = false;
	public GameObject fire;

	[Header("Sounds")]
	public AudioClip growSound;

	public AudioClip flameIgnite;
	public AudioClip flameExtinguish;
	public GameObject audioPlayer;
	AudioSource audioSource;

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

    public GameObject navmeshObs;
    GameObject currentNavMeshObs;


	// Use this for initialization
	void Start () {
		StartCoroutine(Grow());
        StartCoroutine(SpawnNavMeshObs());


        audioSource = audioPlayer.GetComponent<AudioSource>();

		if (damageableFill != null){
			damageableFill.fillAmount = 0;
		}

		viewCamera = Camera.main;
		player = FindObjectOfType<PlayerController>();

	}

    IEnumerator SpawnNavMeshObs()
    {
        currentNavMeshObs = Instantiate(navmeshObs, gameObject.transform.position, Quaternion.identity) as GameObject;
        yield return new WaitForSeconds(1);
        currentNavMeshObs.transform.parent = gameObject.transform;

    }

    public void FillDamage(float damageAmount){

		if (isDamaged == false){
			damageFillPercentage += .1f * damageAmount;

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
        if (!isOnFire)
        {
            if (gameObject.tag == "Pumpkin") {
                player.GainHealth(100, true);
            } else if (gameObject.tag == "Tree")
                {
                    player.GainHealth(20, false);
                }

        }

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

    public IEnumerator Grow() {

        float randomGrowth = Random.Range(randomGrowthMin, randomGrowthMax);
        float percent = 0;
        float growSpeed = 1 / growTime;

        while (percent < 1) {
            percent += Time.deltaTime * growSpeed;
            gameObject.transform.localScale = Vector3.Lerp(Vector3.zero, new Vector3(randomGrowth, randomGrowth, randomGrowth), percent);
            yield return null;
        }

    }

	public IEnumerator OnFire(){
		if (audioSource != null){
			audioSource.PlayOneShot(flameIgnite, 0.25f);
		}

		if (isOnFire == true){
			GameObject flame = Instantiate(fire, gameObject.transform.position, Quaternion.identity) as GameObject;
			flame.transform.parent = gameObject.transform;

			while(isOnFire == true){
				
				yield return new WaitForSeconds(.75f);

				if (this != null){
					FillDamage(.5f);
				} else if (this == null){
					break;
				}
			}

		}
	}

	public void RemoveFire(){
		if (audioSource != null){
			audioSource.PlayOneShot(flameExtinguish, 0.2f);
		}

		GameObject flame = gameObject.transform.FindChild("FireComplex(Clone)").gameObject;
		if (flame != null){
			Destroy(flame);
		}
		isOnFire = false;
	}

}