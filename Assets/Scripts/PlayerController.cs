﻿using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;
using UnityEngine.UI;

public class PlayerController : LivingEntity {

	public event System.Action OnWeaponSwitch;

	public LayerMask groundedMask;
	public GameObject recoilHandler;

	[Header("Sounds")]
	public GameObject musicPlayer;
	public AudioSource audioSourceMusic;
	public AudioSource audioSourceSFX;

	public AudioClip BigRedAmmo;


	[Header("Movement")]
	public float walkSpeed = 5f;
	public float runSpeed = 100f;
	HeadBob headBob;
	Vector3 targetWalkAmount;
	Vector3 walkAmount;
	Vector3 runAmount;
	Vector3 smoothDampMoveRef;
	Rigidbody rb;
	bool isGrounded;

	[Header("Jumping")]
	public float jumpForce = 220f;
	public GameObject weaponHolder;
	bool landAnimPlaying;

	[Header("Look Controls")]
	[Range (-5, 5)]
	public float mouseSensitivityX = 6f;
	[Range (-5, 5)]
	public float mouseSensitivityY = 6f;
	Transform cam;
	float verticalLookRotation;

	[Header("Weapons")]
	public GameObject[] allGuns;
	public Transform weaponSpawner;
	public int[] ammo;
	public int doominatorMax = 30;
	public int rhmMax = 7;
	public int bigRedMax = 3;
	GameObject equippedGun;
	//int shotgunWatergunSwitch = 0;

	[Header("Weapon Switching")]
	public float blurTime = 1;
	public GameObject WeaponWheelUI;
	public GameObject crosshairHolder;
	public GameObject[] crosshairs;
	public bool isSelecting = false;
	public Text weaponNameText;
	public string[] weaponNames;

	WeaponSway weaponSway;
	public DepthOfField depthOfField;

	[Header("Pause")]
	public bool isPaused = false;

	[Header("Weapon Cycler")]

	public bool isTutorial1 = true;
	public bool isTutorial2 = true;

	public int killCycleCount = 0;
	public bool hasDoominator = false;
	public bool hasRHM = false;
	public bool hasBigRed = false;
	public int rhmKillCount = 10;
	public int doominatorKillCount = 4;
	public int bigRedInitialKillCount = 25;
	public int bigRedKillCount = 50;



	// Use this for initialization
	protected override void Start () {
		headBob = FindObjectOfType<HeadBob>();

		if (weaponSway != null){
			weaponSway = FindObjectOfType<WeaponSway>();
		}
		depthOfField = Camera.main.gameObject.GetComponent<DepthOfField>(); 
		audioSourceMusic = musicPlayer.GetComponent<AudioSource>();
		audioSourceSFX = GetComponent<AudioSource>();

		cam = Camera.main.transform;
		Cursor.visible = false;
		rb = GetComponent<Rigidbody>();

		//begin game with watergun
		WeaponSwitch(1);

		base.Start();
	}
	
	// Update is called once per frame
	void Update () {
		if(!dead && !isPaused){
			//Look Controls
			if(isSelecting == false){
				transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * mouseSensitivityX);
				verticalLookRotation += Input.GetAxis("Mouse Y") * mouseSensitivityY;
				verticalLookRotation = Mathf.Clamp(verticalLookRotation, -90f, 90f);
				cam.localEulerAngles = Vector3.left * verticalLookRotation;
			}

			//Movement
			if (isGrounded){
				Vector3 moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0 , Input.GetAxisRaw("Vertical")).normalized;

				//UNCOMMENT THE CODE BELOW IF YOU WANT TO RUN
				//if(Input.GetKey(KeyCode.LeftShift)){
				//	targetWalkAmount = moveDir * runSpeed;
				//} else{
					targetWalkAmount = moveDir * walkSpeed;
				//}
				//Walk Smoothing
				walkAmount = Vector3.SmoothDamp(walkAmount, targetWalkAmount, ref smoothDampMoveRef, 0.1f);
			}

			//Jumping
			if (Input.GetButtonDown("Jump")){
				if (isGrounded && landAnimPlaying == false){
					rb.AddForce(transform.up * jumpForce);
					StartCoroutine(JumpAnimation());
				}
			}

			isGrounded = false;
			Ray ray = new Ray(transform.position, -transform.up);
			RaycastHit hit;

			if(Physics.Raycast(ray, out hit, 1f + 0.05f, groundedMask)){
				isGrounded = true;
				headBob.enabled = true;
			}

			//Weapon Switching
			if (Input.GetKey(KeyCode.LeftShift)){
				weaponSway = FindObjectOfType<WeaponSway>();
				weaponSway.enabled = false;

				WaterableObject uiElement = FindObjectOfType<WaterableObject>();
				if (uiElement != null){
					uiElement.waterableUI.SetActive(false);
				}

				FindObjectOfType<PlayerUI>().ammoAddition.SetActive(false);

				WeaponSwitchActive();
			} else { 
				WeaponSwitchInactive();
			}
		}


		//Pause Game
		if (Input.GetKeyDown(KeyCode.Escape) && !dead){
			isPaused = !isPaused;
		}
		if (isPaused){
			Time.timeScale = 0;
		} 

			//Weapon Hotkeys
	//		if(Input.GetKeyDown(KeyCode.F)){
	//			shotgunWatergunSwitch = 1 - shotgunWatergunSwitch;
	//			WeaponSwitch(shotgunWatergunSwitch);
	//		}
	//
	//		if(Input.GetKeyDown(KeyCode.R)){
	//			WeaponSwitch(2);
	//			shotgunWatergunSwitch = 1 - shotgunWatergunSwitch;
	//		}

		
	}

	void FixedUpdate(){
		rb.MovePosition(rb.position + transform.TransformDirection(walkAmount) * Time.fixedDeltaTime * Time.timeScale);
	}

	//Weapon Switch UI Commands
	public void WeaponSwitch(int weaponIndex){

		if (equippedGun != null){
			Destroy(equippedGun);
		}
		equippedGun = Instantiate(allGuns[weaponIndex], weaponSpawner.position, weaponSpawner.rotation) as GameObject;
		equippedGun.transform.parent = weaponSpawner;

		foreach(GameObject crosshair in crosshairs){
			crosshair.SetActive(false);
		}
		crosshairs[weaponIndex].SetActive(true);

		//weaponswitch event placed
		if( OnWeaponSwitch != null){
			OnWeaponSwitch();
		}
	}

	public override void TakeDamage (float damage){
		FindObjectOfType<PlayerUI>().LoseHealth(health, damage);
		FindObjectOfType<PlayerUI>().TakeDamageUI();

		base.TakeDamage (damage);
	}

	public override void Die (){
		base.Die ();
		DeathAnimation();
		StartCoroutine(FindObjectOfType<PlayerUI>().GameOver());
	}


	public void GainHealth(float heal){
		health += heal;
		if (health > startingHealth){
			health = startingHealth;
		}
		FindObjectOfType<PlayerUI>().GainHealth(health, heal);
	}

	public void WeaponSwitchUIChange(int weaponIndex){
		weaponNameText.text = weaponNames[weaponIndex];
	}

	void WeaponSwitchActive(){
		if (Time.timeScale == 1.0f){
			StartCoroutine(SlowFade());

			WeaponWheelUI.SetActive(true);
			FindObjectOfType<PlayerUI>().rightMouseButtonUI.SetActive(false);
			FindObjectOfType<PlayerUI>().wasdUI.SetActive(false);
			FindObjectOfType<PlayerUI>().shiftUI.SetActive(false);

			FindObjectOfType<PlayerUI>().introText.gameObject.SetActive(false);
			crosshairHolder.SetActive(false);
			isSelecting = true;
			//disable weapon sway
			weaponSway.enabled = false;
			//enable DOF
			depthOfField.enabled = true;
			//slow time
			Time.timeScale = 0.25f;
			//reveal and center cursor
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.lockState = CursorLockMode.Confined;
			Cursor.visible = true;
		}
	}

	void WeaponSwitchInactive(){
		Cursor.lockState = CursorLockMode.Locked;
		WeaponWheelUI.SetActive(false);
		crosshairHolder.SetActive(true);

		if(weaponSway != null){
			weaponSway.enabled = true;
		}

		isSelecting = false;
		Time.timeScale = 1.0f;
		depthOfField.enabled = false;
		Cursor.visible = false;

	}

	IEnumerator SlowFade(){
		float percent = 0f;
		float blurSpeed = 1/blurTime;
		while (percent < 1){
			percent += Time.deltaTime * blurSpeed;
			depthOfField.maxBlurSize = Mathf.Lerp(0f, 3.94f, percent);
			yield return null;
		}
	}

	IEnumerator JumpAnimation(){
		yield return new WaitForSeconds(0.05f);
		headBob.enabled = false;

		float percent = 0;
		float animationTime = 0.7f;
		float animationSpeed = 1 / animationTime;
		Vector3 currentPos = weaponHolder.transform.localPosition;
		float randomX = 0.2f;
		Vector3 targetPos = currentPos + new Vector3(Random.Range(-randomX, randomX), 1, 0) * 0.08f;

		Vector3 currentRot = weaponHolder.transform.localEulerAngles;
		Vector3 targetRot = currentRot + new Vector3(-2,0,0);

		while (percent <= 1){
			percent += Time.deltaTime * animationSpeed;
			float interpolation = (-Mathf.Pow(percent, 2) + percent) * 4;
			weaponHolder.transform.localPosition = Vector3.Lerp(currentPos, targetPos, interpolation);
			weaponHolder.transform.localEulerAngles = Vector3.Slerp(currentRot, targetRot, interpolation);
			if (isGrounded){
				break;
			}
			yield return null;
		}
	}

	void OnCollisionEnter(Collision col){
		Ray ray = new Ray(transform.position, -transform.up);
		RaycastHit hit;

		if(Physics.Raycast(ray, out hit, 1.0f + 0.05f, groundedMask)){
			if (col.collider.gameObject == hit.collider.gameObject){
				StartCoroutine(LandCameraShake());
				StartCoroutine(LandAnimation());
			}
		} 

//		if(col.collider.gameObject.layer == LayerMask.NameToLayer("Ground")){
//			StartCoroutine(LandCameraShake());
//			StartCoroutine(LandAnimation());
//		}

	}

	IEnumerator LandCameraShake(){
		float cameraRecoilTime = 0.25f;
		GameObject jumpShaker = GameObject.FindGameObjectWithTag("JumpShaker");
		float cameraShakeX = .25f;
		float randomY = Random.Range(-cameraShakeX, cameraShakeX);
		float cameraShakeY = 2f;

		float percent = 0;
		float landShakeSpeed = 1 / cameraRecoilTime;
		Vector3 currentPos = jumpShaker.transform.localEulerAngles;
		while (percent <= 1){
			percent += Time.deltaTime * landShakeSpeed;
			float interpolation = (-Mathf.Pow(percent, 2) + percent) * 4;
			jumpShaker.transform.localEulerAngles = Vector3.Slerp(currentPos, new Vector3(1 * cameraShakeY, randomY, 0), interpolation);
			yield return null;
		}
	}

	IEnumerator LandAnimation(){
		landAnimPlaying = true;
		float percent = 0;
		float animationTime = 0.3f;
		float animationSpeed = 1 / animationTime;
		Vector3 currentPos = weaponHolder.transform.localPosition;
		float randomX = 0.2f;
		Vector3 targetPos = currentPos + new Vector3(Random.Range(-randomX, randomX), -1, 0) * 0.03f;

		Vector3 currentRot = weaponHolder.transform.localEulerAngles;
		Vector3 targetRot = currentRot + new Vector3(6,0,0);

		while (percent <= 1){
			percent += Time.deltaTime * animationSpeed;
			float limitInterpolation = (-Mathf.Pow(percent, 2) + percent) * 4;
			weaponHolder.transform.localPosition = Vector3.Lerp(currentPos, targetPos, limitInterpolation);
			weaponHolder.transform.localEulerAngles = Vector3.Slerp(currentRot, targetRot, limitInterpolation);

			yield return null;
		}
		landAnimPlaying = false;
	} 

	public void DeathAnimation(){
		Time.timeScale = 1;
		Destroy(equippedGun);
		GameObject player = GameObject.FindGameObjectWithTag("Player");
		player.transform.DetachChildren();
		Rigidbody rb = cam.GetComponent<Rigidbody>();
		rb.isKinematic = false;
		cam.GetComponent<BoxCollider>().enabled = true;

	}

	public void CameraRecoilHandler(float cameraRecoilTime, float cameraShakeX, float cameraShakeY){
		StartCoroutine(CameraRecoil(cameraRecoilTime, cameraShakeX,  cameraShakeY));
	}

	public IEnumerator CameraRecoil(float cameraRecoilTime, float cameraShakeX, float cameraShakeY){
		float percent = 0;
		float recoilSpeed = 1 / cameraRecoilTime;
		Vector3 currentPos = recoilHandler.transform.localEulerAngles;
		float randomY = Random.Range(-cameraShakeX, cameraShakeX);
		while (percent <= 1){
			percent += Time.deltaTime * recoilSpeed;
			float interpolation = (-Mathf.Pow(percent, 2) + percent) * 4;
			recoilHandler.transform.localEulerAngles = Vector3.Slerp(currentPos, new Vector3(-1 * cameraShakeY, randomY, 0), interpolation);
			yield return null;
		}
	}
}