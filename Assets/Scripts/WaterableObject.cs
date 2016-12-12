using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WaterableObject : MonoBehaviour {

	public bool isOnFire = false;
	public GameObject fire;

	[Header("Sounds")]
	public AudioClip flameIgnite;
	public AudioClip flameExtinguish;
	public GameObject audioPlayer;
	AudioSource audioSource;

	[Header("Attributes")]
	public LayerMask waterable;
	public float heal = 10;
	public float waterRange = 5f;
	float growTime = 1.5f;

	[Header("UI Elements")]
	public GameObject waterableUI;
	public GameObject damageableUI;
	public Image waterableFill;
	public Image damageableFill;

	[Header("Effects")]
	public GameObject waterShockwave;
	public GameObject explosion;
	public bool isWatered = false;
	public bool isDamaged = false;

	public float waterFillPercentage = 0f;
	public float damageFillPercentage = 0f;
	Camera viewCamera;
	bool hasPlayed = false;
	PlayerController player;

	[Header("Growable Plants")]
	public GameObject tree1;
	public GameObject tree2;
	public GameObject pumpkin;



	// Use this for initialization
	void Start () {
		StartCoroutine(Grow());
		audioSource = audioPlayer.GetComponent<AudioSource>();
		
		if(waterableFill != null){
			waterableFill.fillAmount = 0;
		}
		if (damageableFill != null){
			damageableFill.fillAmount = 0;
		}

		viewCamera = Camera.main;
		player = FindObjectOfType<PlayerController>();
	}

	// Update is called once per frame
	void Update () {		
		if (waterFillPercentage < 1){
			waterFillPercentage += -Time.deltaTime * 0.2f;
			if (waterFillPercentage < 0){
				waterFillPercentage = 0;
			}
		}
	}

	public void FillWater(){

		if (isWatered == false){
			waterableFill.fillAmount +=  .2f;
			waterFillPercentage = waterableFill.fillAmount;
		}

		if (waterFillPercentage == 1){
			if(hasPlayed == false){
				int randomPlant = Random.Range(0, 101);

				FindObjectOfType<PlayerUI>().AddGrowCount();

				Destroy(Instantiate(waterShockwave, this.gameObject.transform.position + Vector3.up * 1f, Quaternion.Euler(-90, 0, 0)) as GameObject, 2f);

				if (randomPlant < 40){
					GameObject currentPlant = Instantiate(tree1, this.gameObject.transform.position, Quaternion.Euler(0, Random.Range(0, 360), 0)) as GameObject;
					currentPlant.GetComponent<AudioSource>().PlayOneShot(FindObjectOfType<WateredObject>().growSound, .5f);
		
				} else if(randomPlant > 40 && randomPlant < 80){
					GameObject currentPlant = Instantiate(tree2, this.gameObject.transform.position, Quaternion.Euler(0, Random.Range(0, 360), 0)) as GameObject;
					currentPlant.GetComponent<AudioSource>().PlayOneShot(FindObjectOfType<WateredObject>().growSound, .5f);

				} else if (randomPlant > 20){
					GameObject currentPlant = Instantiate(pumpkin, this.gameObject.transform.position, Quaternion.Euler(0, Random.Range(0, 360), 0)) as GameObject;
					currentPlant.GetComponent<AudioSource>().PlayOneShot(FindObjectOfType<WateredObject>().growSound, .5f);

				}

				hasPlayed = true;
				IncreaseAmmo();
			}
			isWatered = true;
			waterFillPercentage = 0f;
			waterableUI.SetActive(false);

			//if tutorial sprout
			if(player.isTutorial1 == true){
				player.isTutorial1 = false;
				FindObjectOfType<Spawner>().StartCoroutine("TutorialSpawner");

				//raygun ui unlocked text
				FindObjectOfType<PlayerUI>().UnlockWeaponText("Dr. Ray, PhD Unlocked!", 0);
				player.weaponNames[3] = "Dr. Ray, PhD";

				//unlock raygun weapon ui
				FindObjectOfType<PlayerUI>().UnlockWeapon(FindObjectOfType<PlayerUI>().thePewPewUI);
				FindObjectOfType<PlayerUI>().StartCoroutine(FindObjectOfType<PlayerUI>().UnlockAnimation());

				player.WeaponSwitch(3);
				player.raygunUnlocked = true;
				FindObjectOfType<PlayerUI>().thePewPewUI.SetActive(true);
			}

			else if(player.isTutorial2 == true && player.isTutorial1 == false){
				player.isTutorial2 = false;
				FindObjectOfType<Spawner>().StartCoroutine("SlowSpawner");
				FindObjectOfType<DayAndNightCycle>().enabled = true;
			}


			Destroy(gameObject);

		}
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

			DestroySprout();
		}

	}

	public void DestroySprout(){
		if(!isOnFire){
			player.GainHealth(heal);
		}

		Destroy(Instantiate(explosion, this.gameObject.transform.position + Vector3.up * 1f, Quaternion.Euler(-90, 0, 0)) as GameObject, 5f);
		damageableUI.SetActive(false);


		//if you damage the sprout
		if(player.isTutorial2 == true && player.isTutorial1 == false){
			player.isTutorial2 = false;
			FindObjectOfType<Spawner>().StartCoroutine("SlowSpawner");
			FindObjectOfType<DayAndNightCycle>().enabled = true;
		}

		
		Destroy(gameObject);
	}

	public void SetWaterableVisible(){
		waterableUI.SetActive(true);

		Vector3 uiLocation = viewCamera.WorldToScreenPoint(transform.position + new Vector3(0f, 1f, 0f));
		waterableUI.transform.position = uiLocation;
	}

	public void SetDamageableVisible(){
		damageableUI.SetActive(true);

		Vector3 uiLocation = viewCamera.WorldToScreenPoint(transform.position + new Vector3(0f, 1f, 0f));
		damageableUI.transform.position = uiLocation;
	}

	public IEnumerator Grow(){
		float percent = 0;
		float growSpeed = 1 / growTime; 
		while (percent < 1){
			percent += Time.deltaTime * growSpeed;
			gameObject.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, percent);
			yield return null;
		}
	}

	public void IncreaseAmmo(){
		int randomShotgun = Random.Range(5, 10);
		int randomPistol = Random.Range(1, 3);

		int initialShotgunAmmo = player.ammo[0];
		int initialPistolAmmo = player.ammo[1];

		for (int i = 0; i < 2; i ++){
			if (i < 1){
				player.ammo[i] += randomShotgun;
				if (player.ammo[i] >= player.doominatorMax){
					player.ammo[i] = player.doominatorMax;
				}
			} 

			if (i > 0){
				player.ammo[i] += randomPistol;
				if (player.ammo[i] >= player.rhmMax){
					player.ammo[i] = player.rhmMax;
				}
			}
		}

		int shotgunAmmoAddition = player.ammo[0] - initialShotgunAmmo;
		int pistolAmmoAddition = player.ammo[1] - initialPistolAmmo;

		FindObjectOfType<PlayerUI>().AddAmmo(shotgunAmmoAddition, pistolAmmoAddition);

		FindObjectOfType<PlayerUI>().AddAmmo(shotgunAmmoAddition, pistolAmmoAddition);

		FindObjectOfType<PlayerUI>().UpdateAmmo();
	}

	public IEnumerator OnFire(){
		if(audioSource != null){
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
		audioSource.PlayOneShot(flameExtinguish, 0.2f);
		GameObject flame = gameObject.transform.FindChild("FireComplex(Clone)").gameObject;
		if (flame != null){
			Destroy(flame);
		}
		isOnFire = false;
	}
}
