using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerUI : MonoBehaviour {

	public GameObject gameplayUI;

	[Header("Sounds")]
	public AudioClip heartBeat;
	public AudioSource heartBeatSource;

	[Header("Intro Text")]
	public Text introText;
	public GameObject rightMouseButtonUI;
	public GameObject wasdUI;
	public GameObject shiftUI;

	[Header("FlashLight")]
	public GameObject flashLight;
	bool flashHasActivated = false;
	float flashSpeed = 0.005f;

	[Header("GameOver UI")]
	public GameObject gameOverBacking;
	public Image gameOverText;
	public Button retry;
	public Button menu;
	public Text gameOverDayCount;
	public Text gameOverGrowCount;

	[Header("Waterable Object")]
	public LayerMask waterable;
	PlayerController player;
	GunController gun;

	[Header("Health")]
	public Image healthUI;
	public Text GrowCounter;
	int growCount = 0;
	public GameObject damageUI;
	public GameObject lowHealthAnimationUI;

	[Header("Ammo")]
	public GameObject ammoAddition;
	public Text[] ammoAdditionCount;
	public Image[] ammoAdditionIcons;
	public Text[] ammoCount;

	public GameObject ammoAdditionRHM;
	public GameObject ammoAdditionDoominator;

	[Header("Gun UI")]
	public GameObject rhmUI;
	public GameObject doominatorUI;
	public GameObject thePewPewUI;
	public GameObject BigRedUI;

	[Header("Particle Effects")]
	public GameObject healthParticle;

	[Header("Day Counter")]
	public Text dayCounter;

	Camera viewCamera;
	WaterableObject growable;

	[Header("Kill Progression")]
	public Image killUI;
	public GameObject rhmIcon;
	public GameObject doominatorIcon;
	public GameObject bigRedIcon;
	public GameObject padlock;


	// Use this for initialization
	void Start () {
		viewCamera = Camera.main;
		player = FindObjectOfType<PlayerController>();
		gun = FindObjectOfType<GunController>();
		FindObjectOfType<PlayerController>().OnWeaponSwitch += CheckWaterRifle;
		growable = FindObjectOfType<WaterableObject>();
		healthUI.fillAmount = 1.0f;

		StartCoroutine(ActivateIntroText("Water the Sprout!", 1));
		StartCoroutine(InstructionFade(wasdUI, 3f, 3.5f));
		StartCoroutine(InstructionFade(rightMouseButtonUI, 3.5f, 3));
		StartCoroutine(InstructionFade(shiftUI, 4f, 2.5f));

		AddKillCount(player.killCycleCount);

		GrowCounter.text = "" + growCount;
		ammoCount[0].text = "" + player.ammo[0];
		ammoCount[1].text = "" + player.ammo[1];
	}
	
	// Update is called once per frame
	void Update () {

		Vector3 ray = viewCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0f));
		RaycastHit hit;

		if (growable != null){
			growable.waterableUI.SetActive(false);
			growable.damageableUI.SetActive(false);

		}

		if(Physics.SphereCast(ray, 1f, viewCamera.transform.forward, out hit, 20, waterable)){
			WaterableObject waterableObject = hit.collider.gameObject.GetComponent<WaterableObject>();
			WateredObject wateredObject = hit.collider.gameObject.GetComponent<WateredObject>();

			if (waterableObject != null){
				if(waterableObject.isWatered == false && player.isSelecting == false && gun.fireAttribute == GunController.FireAttribute.WaterGun){
					waterableObject.SetWaterableVisible();
					waterableObject.waterableFill.fillAmount = waterableObject.waterFillPercentage;
				} 

				if(waterableObject.isDamaged == false && player.isSelecting == false && gun.fireAttribute != GunController.FireAttribute.WaterGun){
					waterableObject.SetDamageableVisible();
					waterableObject.damageableFill.fillAmount = waterableObject.damageFillPercentage;
				}
			}

			if (wateredObject != null){

				if(wateredObject.isDamaged == false && player.isSelecting == false && gun.fireAttribute != GunController.FireAttribute.WaterGun){
					wateredObject.SetDamageableVisible();
					wateredObject.damageableFill.fillAmount = wateredObject.damageFillPercentage;
				}
			}
		}
	}

	void CheckWaterRifle(){
		gun = FindObjectOfType<GunController>();
	}

	public void LoseHealth(float health, float damage){
		health = health * 0.01f;
		damage = damage * .01f;
		healthUI.fillAmount = health - damage;
	}

	public void GainHealth(float health, float heal){
		health = health * 0.01f;
		heal = heal * .01f;
		healthUI.fillAmount = health + heal;
		SpawnHealthParticles();
	}

	public void AddGrowCount(){
		growCount ++;
		GrowCounter.text = "" + growCount;
	}

	public void SubtractGrowCount(){
		growCount --;
		if (growCount <= 0){
			growCount = 0;
		}
		GrowCounter.text = "" + growCount;
	}

	void SpawnHealthParticles(){
		GameObject healthEffect = Instantiate(healthParticle, transform.position, transform.localRotation) as GameObject;
		healthEffect.transform.parent = Camera.main.transform;
		healthEffect.transform.localPosition += Vector3.forward * 0.4f;
		Destroy(healthEffect, 4);
	}

	public void TakeDamageUI(){
		damageUI.SetActive(true);
		damageUI.GetComponent<Image>().CrossFadeAlpha(1, 0.01f, false);
		damageUI.GetComponent<Image>().CrossFadeAlpha(0, 1, false);

	}

	public void LowHealth(){
		lowHealthAnimationUI.SetActive(true);
		if (heartBeatSource.isPlaying == false){
			heartBeatSource.Play();
		}
	}

	public void Healthy(){
		lowHealthAnimationUI.SetActive(false);
		heartBeatSource.Stop();
	}

	public void AddAmmo(int shotgunAmmoAddition, int pistolAmmoAddition){

		if (shotgunAmmoAddition == 0){
			ammoAdditionCount[0].text = "Ammo Full";
		} else{
			ammoAdditionCount[0].text = "+ " + shotgunAmmoAddition + " Ammo";
		}

		if (pistolAmmoAddition == 0){
			ammoAdditionCount[1].text = "Ammo Full";
		} else{
			ammoAdditionCount[1].text = "+ " + pistolAmmoAddition + " Ammo";
		}

		ammoAddition.SetActive(true);
		Invoke("AmmoUIDisappear", 2);
	} 

	void AmmoUIDisappear(){
		ammoAddition.SetActive(false);

	}

	public void UpdateAmmo(){
		for (int i = 0; i < ammoCount.Length; i ++){
			ammoCount[i].text = "" + player.ammo[i];
		}
	}

	public IEnumerator GameOver(){
		int dayCount = FindObjectOfType<DayAndNightCycle>().dayCounter;
		player.depthOfField.enabled = true;


		gameplayUI.SetActive(false);
		gameOverBacking.SetActive(true);
		gameOverText.gameObject.SetActive(true);
		gameOverText.CrossFadeAlpha(0, 0.01f, false);
		gameOverText.CrossFadeAlpha(1, 3, false);

		yield return new WaitForSeconds(1);
		gameOverDayCount.gameObject.SetActive(true);
		gameOverDayCount.CrossFadeAlpha(0, 0.01f, false);
		gameOverDayCount.CrossFadeAlpha(1, 2f, false);
		gameOverDayCount.text = "Days Survived: " + dayCount;

		yield return new WaitForSeconds(1f);
		gameOverGrowCount.gameObject.SetActive(true);
		gameOverGrowCount.CrossFadeAlpha(0, 0.01f, false);
		gameOverGrowCount.CrossFadeAlpha(1, 2f, false);
		gameOverGrowCount.text = "Sprouts Bloomed: " + growCount;

		yield return new WaitForSeconds(2);
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.Confined;
		retry.gameObject.SetActive(true);
		menu.gameObject.SetActive(true);

	}

	public void IncreaseDayCount(int dayCount){
		dayCounter.text = "Day: " + dayCount;
		FindObjectOfType<DayAndNightCycle>().DaySound();

	}

	public IEnumerator FlashLightActivate(){
		if(!flashHasActivated){
			flashHasActivated = true;
			flashLight.SetActive(true);	
			yield return new WaitForSeconds(flashSpeed);
			flashLight.SetActive(false);
			yield return new WaitForSeconds(flashSpeed);
			flashLight.SetActive(true);	
			yield return new WaitForSeconds(flashSpeed);
			flashLight.SetActive(false);
			yield return new WaitForSeconds(flashSpeed);
			flashLight.SetActive(true);

			//random flickers
			for(int i = 0; i < 10; i++){
				yield return new WaitForSeconds(Random.Range(8, 12));
				flashLight.SetActive(false);
				yield return new WaitForSeconds(flashSpeed);
				flashLight.SetActive(true);	
				yield return new WaitForSeconds(flashSpeed);
				flashLight.SetActive(false);
				yield return new WaitForSeconds(flashSpeed);
				flashLight.SetActive(true);
				yield return new WaitForSeconds(flashSpeed);
				flashLight.SetActive(false);
				yield return new WaitForSeconds(flashSpeed);
				flashLight.SetActive(true);
				if (!flashHasActivated){
					break;
				}
			}
		}
	}

	public void FlashLightDeactivate(){
		flashLight.SetActive(false);
		flashHasActivated = false;
	}

	public void UnlockWeaponText(string text, float waitTime){
		StartCoroutine(ActivateIntroText(text, waitTime));
	}

	public IEnumerator ActivateIntroText(string text, float waitTime){
		StopCoroutine(GrowText(waitTime));
		StartCoroutine(GrowText(waitTime));

		yield return new WaitForSeconds(waitTime);

		introText.gameObject.SetActive(true);
		introText.text = text;

		introText.CrossFadeAlpha(0, 0.01f, false);
		introText.CrossFadeAlpha(1, 0.5f, false);
		yield return new WaitForSeconds(2);
		introText.CrossFadeAlpha(0, 1, false);
	}

	public IEnumerator GrowText(float waitTime){
		yield return new WaitForSeconds(waitTime);

		float growTime = 0.75f;
		float percent = 0;
		float growSpeed = 1 / growTime; 
		while (percent < 1){
			percent += Time.deltaTime * growSpeed;
			introText.gameObject.transform.localScale = Vector3.Lerp(new Vector3(0.95f, 0.95f, 0.95f), Vector3.one, percent);
			yield return null;
		}
	}

	public void AddKillCount(int killCycleCount){
		if (player.hasDoominator == false){
			killUI.fillAmount = (float)killCycleCount / player.doominatorKillCount;
		} else if (player.hasRHM == false && player.hasDoominator == true){
			killUI.fillAmount = (float)killCycleCount / player.rhmKillCount;
		} else if (player.hasBigRed == false && player.hasRHM == true){
			killUI.fillAmount = (float)killCycleCount / player.bigRedInitialKillCount;
		} else if (player.hasBigRed == true){
			killUI.fillAmount = (float)killCycleCount / player.bigRedKillCount;
		}
	}

	IEnumerator InstructionFade(GameObject instructionText, float waitTime, float stayTime){
		yield return new WaitForSeconds(waitTime);

		instructionText.SetActive(true);
		instructionText.GetComponent<Animator>().Play("FadeIn");

		yield return new WaitForSeconds(stayTime);

		instructionText.GetComponent<Animator>().Play("FadeOut");
		yield return new WaitForSeconds(2);

		instructionText.SetActive(false);
	}

	public void UnlockWeapon(GameObject weaponUI){
		weaponUI.GetComponent<Button>().interactable = true;
		weaponUI.transform.GetChild(0).gameObject.SetActive(false);
	}

	public IEnumerator UnlockAnimation(){
		padlock.SetActive(true);

		Color color = padlock.GetComponent<Image>().color;
		color = Color.white;
		color.a = 192;

		padlock.GetComponent<Image>().color = color;

		yield return new WaitForSeconds(0.25f);
		padlock.GetComponent<Animator>().Play("Unlock");

		float percent = 0;
		float time = 1.5f;
		float speed = 1 / time;

		yield return new WaitForSeconds(1.1f);
		while (percent < 1){
			percent += Time.deltaTime * speed;
			padlock.GetComponent<Image>().color = Color.Lerp(Color.white, Color.clear, percent);
			yield return null;
		}
	}

}
