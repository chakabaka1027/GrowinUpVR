using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerUI : MonoBehaviour {

	public int pickleBearDeathCount;
	public int cabbageBearDeathCount;
	
	public GameObject gameplayUI;

	[Header("Cookie")]
	public GameObject cookieUI;

	[Header("Sounds")]
	public AudioClip heartBeat;
	public AudioSource heartBeatSource;
	public AudioClip menuSelectionSFX;
	public AudioClip weaponUnlockSFX;
	public AudioClip gainHealthSFX;
//	public AudioClip addAmmoSFX;
	public AudioClip gameValueSFX;
	public AudioClip scoreValueSFX;
	public AudioClip celebrationSFX;
	public AudioClip fireworksSFX;

	[Header("Intro Text")]
	public Text introText;
	public GameObject rightMouseButtonUI;
	public GameObject wasdUI;
	public GameObject shiftUI;

	[Header("HitMarker")]
	public GameObject hitMarkerSmall;
	public GameObject hitMarkerLarge;

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

	public GameObject paper;
	public GameObject allScoreText;

	public GameObject sproutGameValue;
	public GameObject sproutScoreValue;
	public GameObject bearGameValue;
	public GameObject bearScoreValue;
	public GameObject dayGameValue;
	public GameObject dayScoreValue;
	public GameObject totalScore;


	[Header("Waterable Object")]
	public LayerMask waterable;
	PlayerController player;
	GunController gun;

	[Header("Health")]
	public Image healthUI;
	public Text GrowCounter;
	int growCount = 0;
	public GameObject growCounterIcon;
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

		if(Physics.SphereCast(ray, 3f, viewCamera.transform.forward, out hit, 20, waterable) || Physics.Raycast(ray, viewCamera.transform.forward, out hit, 20, waterable)){
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

		StartCoroutine(SpawnHealthParticles());


	}

	public void AddGrowCount(){
		growCount ++;
		GrowCounter.text = "" + growCount;
		growCounterIcon.GetComponent<Animator>().Play("BackingGrow");
	}

	public void SubtractGrowCount(){
		growCount --;
		if (growCount <= 0){
			growCount = 0;
		}
		GrowCounter.text = "" + growCount;
		growCounterIcon.GetComponent<Animator>().Play("BackingShrink");

	}

	IEnumerator SpawnHealthParticles(){

		GameObject healthEffect = Instantiate(healthParticle, transform.position, transform.localRotation) as GameObject;
		healthEffect.transform.parent = Camera.main.transform;
		healthEffect.transform.localPosition += Vector3.forward * 0.4f;
		yield return new WaitForSeconds(0.2f);
		player.audioSourceSFX.PlayOneShot(gainHealthSFX, 0.5f);
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

//	void PlayReloadSound(){
//		FindObjectOfType<PlayerController>().audioSourceSFX.PlayOneShot(addAmmoSFX, 0.25f);
//	}

	public void AddAmmo(int shotgunAmmoAddition, int pistolAmmoAddition){

//		if (FindObjectOfType<PlayerController>().hasDoominator == true){
//			Invoke("PlayReloadSound", 0.55f);
//		}

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

		//erase game ui and fade in "your responsibilities consumed you" text
		gameplayUI.SetActive(false);
		gameOverBacking.SetActive(true);
		gameOverText.gameObject.SetActive(true);
		gameOverText.CrossFadeAlpha(0, 0.01f, false);
		gameOverText.CrossFadeAlpha(1, 3, false);

		yield return new WaitForSeconds(3.5f);

		//animate paper to move up the screen
		paper.SetActive(true);
		paper.GetComponent<Animator>().Play("MoveUp");

		yield return new WaitForSeconds(.1f);
		allScoreText.SetActive(true);
		allScoreText.GetComponent<Animator>().Play("MoveUp");

		yield return new WaitForSeconds(3);

		//Sprouts Bloomed Values
		StartCoroutine(CountTo(growCount, sproutGameValue));
		yield return new WaitForSeconds(1f);
//		sproutScoreValue.GetComponent<Text>().text = string.Format("{0:n0}", growCount * 50);
		ScoreValueAnimation(sproutScoreValue, growCount * 50);

		yield return new WaitForSeconds(1.5f);

		//Bears Rejected Values
		StartCoroutine(CountTo(pickleBearDeathCount + cabbageBearDeathCount, bearGameValue));
		yield return new WaitForSeconds(1f);
//		bearScoreValue.GetComponent<Text>().text = string.Format("{0:n0}", (pickleBearDeathCount * 2) + (cabbageBearDeathCount * 20));
		ScoreValueAnimation(bearScoreValue, (pickleBearDeathCount * 2) + (cabbageBearDeathCount * 20));


		yield return new WaitForSeconds(1.5f);

		//Days Survived Values
		StartCoroutine(CountTo(dayCount, dayGameValue));
		yield return new WaitForSeconds(1f);
		dayScoreValue.GetComponent<Text>().text = "x " + GetDayCombo(dayCount);
		player.audioSourceSFX.PlayOneShot(scoreValueSFX, 0.5f);
		dayScoreValue.GetComponent<Animator>().Play("Default");

		yield return new WaitForSeconds(2);

		//Total Score values
//		totalScore.GetComponent<Text>().text = string.Format("{0:n0}", ((growCount * 50) + ((pickleBearDeathCount * 2) + (cabbageBearDeathCount * 20))) * GetDayCombo(dayCount));
		StartCoroutine(CountTo(((growCount * 50) + ((pickleBearDeathCount * 2) + (cabbageBearDeathCount * 20))) * GetDayCombo(dayCount), totalScore));

		yield return new WaitForSeconds(.5f);

		if ((growCount * 50) + ((pickleBearDeathCount * 2) + (cabbageBearDeathCount * 20)) * GetDayCombo(dayCount) > 1){
			Celebration();
		}

		yield return new WaitForSeconds(1);
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.Confined;
		retry.gameObject.SetActive(true);
		menu.gameObject.SetActive(true);

	}

	IEnumerator CountTo (int target, GameObject uiElement) {
		int score = 0;
		float duration = 0.4f;

        int start = score;
        for (float timer = 0; timer < duration; timer += Time.deltaTime) {
            float progress = timer / duration;
            score = (int)Mathf.Lerp (start, target, progress);
            if (score > 0){
				player.audioSourceSFX.PlayOneShot(gameValueSFX, 0.75f);
			}
			uiElement.GetComponent<Text>().text = string.Format("{0:n0}", score);
            yield return null;
        }
        score = target;
		uiElement.GetComponent<Text>().text = string.Format("{0:n0}", target);

    }

    void ScoreValueAnimation(GameObject uiElement, int value){
		uiElement.GetComponent<Text>().text = value + "";
		uiElement.GetComponent<Animator>().Play("Default");
		player.audioSourceSFX.PlayOneShot(scoreValueSFX, 0.75f);
    }

    void Celebration(){
		player.audioSourceSFX.PlayOneShot(celebrationSFX, 0.5f);
    }

	int GetDayCombo(int dayCount){
		if (dayCount == 1 || dayCount == 2 || dayCount == 3){
			return 1;
		}
		else if (dayCount == 4 || dayCount == 5 || dayCount == 6){
			return 2;
		}
		else if (dayCount == 7 || dayCount == 8 || dayCount == 9){
			return 3;
		}
		else if (dayCount == 10 || dayCount == 11 || dayCount == 12){
			return 4;
		}
		else if (dayCount == 13 || dayCount == 14 || dayCount == 15){
			return 5;
		} 
		else if (dayCount == 16 || dayCount == 17 || dayCount == 18){
			return 6;
		}
		else if (dayCount == 19 || dayCount == 20 || dayCount == 21){
			return 7;
		}
		else if (dayCount == 22 || dayCount == 23 || dayCount == 24){
			return 8;
		}
		else if (dayCount == 25 || dayCount == 26 || dayCount == 27){
			return 9;
		}
		else {
			return 10;
		}
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

		player.audioSourceSFX.PlayOneShot(weaponUnlockSFX, 0.5f);
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

	public IEnumerator AnimateSmallHitMarker(){
		hitMarkerSmall.SetActive(true);
		float percent = 0;
		float time = 0.5f;
		float speed = 1/time;

		while(percent < 1){
			percent += Time.deltaTime * speed;
			hitMarkerSmall.GetComponent<Image>().color = Color.Lerp(Color.white, Color.clear, percent);

			yield return null;
		}

	}

	public IEnumerator AnimateLargeHitMarker(){
		hitMarkerLarge.SetActive(true);
		float percent = 0;
		float time = 0.5f;
		float speed = 1/time;

		while(percent < 1){
			percent += Time.deltaTime * speed;
			hitMarkerLarge.GetComponent<Image>().color = Color.Lerp(Color.white, Color.clear, percent);

			yield return null;
		}

	}

}
