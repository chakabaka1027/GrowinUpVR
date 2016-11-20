using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;

public class GunController : MonoBehaviour {

	[Header("Sounds")]
	public AudioClip[] doominatorSFX;
	public AudioClip[] pewPewSFX;
	public AudioClip[] rhmSFX;
	public AudioClip[] wetWillieSFX;
	public AudioClip[] bigRedSFX;


	public enum Firemode{Single, Automatic};
	[Header("Gun Modes")]
	public Firemode firemode;
	public bool canDamage;
	public enum FireAttribute {Default, Penetrate, Shotgun, RocketLauncher, WaterGun, Lazer};
	public FireAttribute fireAttribute;

	[Header("Shot Attributes")]
	public float msBetweenShots = 100;
	public float damage = 1;
	public float shotForce = 50f;
	public int shotgunSpreadCount = 8;
	public float waterRange = 5f;
	public LayerMask shootable;

	float nextShotTime;
	bool triggerReleasedSinceLastShot = true;

	[Header("Shot Effects")]
	public GameObject[] waterParticles;
	public GameObject bulletImpact;
	public GameObject grenade;
	public Transform shellEjector;
	public GameObject shell;
	public Transform barrel;
	public GameObject[] muzzleFlash;
	public GameObject muzzleLight;

	[Header("Recoil")]
	public float cameraRecoilTime = 0.2f;
	public float cameraShakeY = 5;
	public float cameraShakeX = 2;
	public Vector2 kickMinMax = new Vector2(.05f, 0.2f);
	public Vector2 recoilAngleMinMax = new Vector2(3, 5);
	public float recoilMoveSettleTime = 0.1f;
	public float recoilRotationSettleTime = 0.1f;

	Vector3 recoilSmoothDampVelocity;
	float recoilRotSmoothDampVelocity;
	float recoilAngle;


	[Header("Weapon Switching")]
	public float WeaponReadyAnimationTime = 1;
	public Vector3 readyPosition;

	Camera viewCamera;
	bool canShoot = false;
	PlayerController player;

	void Start () {
		readyPosition = transform.localPosition;
		StartCoroutine(ReadyWeapon());
		viewCamera = GetComponentInParent<Camera>();
		player = GetComponentInParent<PlayerController>();
	}
	
	void Update () {
		if (Input.GetMouseButton(0) && canShoot == true && player.isSelecting == false){
			OnTriggerHold();
		}

		if (Input.GetMouseButtonUp(0) && canShoot == true && player.isSelecting == false){
			OnTriggerRelease();
		}
	}
	void LateUpdate(){
	//animate recoil
		transform.localPosition = Vector3.SmoothDamp(transform.localPosition, readyPosition, ref recoilSmoothDampVelocity, recoilMoveSettleTime);
		recoilAngle = Mathf.SmoothDamp(recoilAngle, 0, ref recoilRotSmoothDampVelocity, recoilRotationSettleTime);
		transform.localEulerAngles = transform.localEulerAngles + Vector3.left * recoilAngle;
	}

	IEnumerator Shoot(){
		if (Time.time > nextShotTime){

			if (firemode == Firemode.Single){
				if(!triggerReleasedSinceLastShot){
					yield break;
				}
			}

			nextShotTime = Time.time + msBetweenShots / 1000;
			
			Vector3 ray = viewCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0));
			RaycastHit hit;

			if(Physics.Raycast(ray, viewCamera.transform.forward, out hit, Mathf.Infinity, shootable)){
				if(canDamage){
					OnDamageObject(hit);

				} else {
					OnMoveObject(hit);
				}
			}
			Instantiate(shell, shellEjector.position, shellEjector.rotation);

			StartCoroutine(GunRecoil());
		}
	}

	IEnumerator ShootLazer(){
		if (Time.time > nextShotTime){

			if (firemode == Firemode.Single){
				if(!triggerReleasedSinceLastShot){
					yield break;
				}
			}

			nextShotTime = Time.time + msBetweenShots / 1000;
			
			Vector3 ray = viewCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0));
			RaycastHit hit;

			if(Physics.Raycast(ray, viewCamera.transform.forward, out hit, Mathf.Infinity, shootable)){
				if(canDamage){
					OnDamageObject(hit);

				} else {
					OnMoveObject(hit);
				}
				StartCoroutine(GunRecoil());

				LineRenderer lineRenderer = gameObject.GetComponent<LineRenderer>();
				lineRenderer.material.color = Color.green;
				lineRenderer.SetColors(Color.green, Color.green);
				lineRenderer.SetPosition(0, barrel.position);
				lineRenderer.SetPosition(1, hit.point);
				lineRenderer.enabled = true;;
				yield return new WaitForSeconds(0.05f);
				lineRenderer.enabled = false;
			}
			int index = Random.Range(0, pewPewSFX.Length);
			player.audioSourceSFX.PlayOneShot(pewPewSFX[index], .1f);

		}
	}

	IEnumerator ShootPenetrate(){
		if (Time.time > nextShotTime && player.ammo[1] > 0){
			if (firemode == Firemode.Single){
				if(!triggerReleasedSinceLastShot){
					yield break;
				}
			}

			nextShotTime = Time.time + msBetweenShots / 1000;
			
			Vector3 ray = viewCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0));
			RaycastHit[] hits;
			hits = Physics.RaycastAll(ray, viewCamera.transform.forward, Mathf.Infinity, shootable );
			for (int i = 0; i < hits.Length; i++){	
				RaycastHit hit = hits[i];
				if(canDamage){
					OnDamageObject(hit);

				} else {
					OnMoveObject(hit);
				}
			}
			Instantiate(shell, shellEjector.position, shellEjector.rotation);

			StartCoroutine(GunRecoil());
			player.ammo[1] --;
			FindObjectOfType<PlayerUI>().UpdateAmmo();
			int index = Random.Range(0, rhmSFX.Length);
			player.audioSourceMusic.PlayOneShot(rhmSFX[index], 2f);
		}
	}

	IEnumerator ShootShotgun(){
		if (Time.time > nextShotTime && player.ammo[0] > 0){

			if (firemode == Firemode.Single){
				if(!triggerReleasedSinceLastShot){
					yield break;
				}
			}
			nextShotTime = Time.time + msBetweenShots / 1000;

			int spreadCount = shotgunSpreadCount;
			float spreadAngle = 10.0f;

			for(int i = 0; i < spreadCount; i++){
				RaycastHit hit;
				Quaternion fireRotation = Quaternion.LookRotation(viewCamera.transform.forward);
				Quaternion randomRotation = Random.rotation;
				fireRotation = Quaternion.RotateTowards(fireRotation, randomRotation, Random.Range(0.0f, spreadAngle));

				if(Physics.Raycast(viewCamera.transform.position, fireRotation * Vector3.forward, out hit, Mathf.Infinity, shootable)){
					if(canDamage){
						OnDamageObject(hit);
					} else {
						OnMoveObject(hit);
					}
				}
			}
			Instantiate(shell, shellEjector.position, shellEjector.rotation);
			StartCoroutine(GunRecoil());
			player.ammo[0]--;
			FindObjectOfType<PlayerUI>().UpdateAmmo();
			player.audioSourceMusic.PlayOneShot(doominatorSFX[0], 1.5f);
		} 
	}

	IEnumerator ShootWater(){
		if (Time.time > nextShotTime){

			if (firemode == Firemode.Single){
				if(!triggerReleasedSinceLastShot){
					yield break;
				}
			}
			nextShotTime = Time.time + msBetweenShots / 1000;
			
			Vector3 ray = viewCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0));
			RaycastHit hit;

//			if(Physics.Raycast(ray, viewCamera.transform.forward, out hit, 15, shootable)){
			if(Physics.SphereCast(ray, 2f, viewCamera.transform.forward, out hit, 15, shootable)){

				WaterableObject waterableObject = hit.collider.GetComponent<WaterableObject>();
				if (waterableObject != null){
					waterableObject.FillWater();
				}
			}

			for (int i = 0; i < 2; i++){
				Destroy(Instantiate(waterParticles[i], barrel.position, barrel.rotation) as GameObject, 0.7f);
			}

			transform.localPosition -= Vector3.forward * Random.Range(kickMinMax.x, kickMinMax.y);
			recoilAngle += Random.Range(recoilAngleMinMax.x, recoilAngleMinMax.y);
			recoilAngle = Mathf.Clamp(recoilAngle, 0f, 30f);

			//Camera Recoil
//			StartCoroutine(CameraRecoil());
			player.CameraRecoilHandler(cameraRecoilTime, cameraShakeX, cameraShakeY);

			int index = Random.Range(0, wetWillieSFX.Length);
			player.audioSourceMusic.PlayOneShot(wetWillieSFX[index], 1f);

		}
	}

	IEnumerator ShootGrenade(){
		if (Time.time > nextShotTime && player.ammo[2] > 0){

			if (firemode == Firemode.Single){
				if(!triggerReleasedSinceLastShot){
					yield break;
				}
			}

			nextShotTime = Time.time + msBetweenShots / 1000;

			Instantiate(grenade, barrel.position, Quaternion.FromToRotation(Vector3.forward, viewCamera.gameObject.transform.forward));

			StartCoroutine(GunRecoil());

			player.ammo[2]--;
			FindObjectOfType<PlayerUI>().UpdateAmmo();


			int index = Random.Range(0, bigRedSFX.Length);
			player.audioSourceMusic.PlayOneShot(bigRedSFX[index], 2f);
		}
	}

	IEnumerator GunRecoil(){

		for(int i = 0; i < muzzleFlash.Length;i ++){
			GameObject muzzleEffect = Instantiate(muzzleFlash[i], barrel.position, barrel.rotation) as GameObject;
			muzzleEffect.transform.parent = barrel;
			Destroy(muzzleEffect, 1f);
		}

		muzzleLight.SetActive(true);

		transform.localPosition -= Vector3.forward * Random.Range(kickMinMax.x, kickMinMax.y);
		recoilAngle += Random.Range(recoilAngleMinMax.x, recoilAngleMinMax.y);
		recoilAngle = Mathf.Clamp(recoilAngle, 0f, 30f);

		//Camera Recoil
//		StartCoroutine(CameraRecoil());
		player.CameraRecoilHandler(cameraRecoilTime, cameraShakeX, cameraShakeY);

		yield return new WaitForSeconds(0.05f);
		muzzleLight.SetActive(false);
	}

	//Causes damage to shot objects
	void OnDamageObject(RaycastHit hit){

		//damage plants
		WaterableObject waterableObject = hit.collider.GetComponent<WaterableObject>();
		if (waterableObject != null){
			waterableObject.damageableFill.fillAmount = waterableObject.damageFillPercentage;
			waterableObject.FillDamage();
		}

		WateredObject wateredObject = hit.collider.GetComponent<WateredObject>();

		if (wateredObject != null){
			wateredObject.damageableFill.fillAmount = wateredObject.damageFillPercentage;
			wateredObject.FillDamage();
		}

		IDamageable damageableObject = hit.collider.GetComponent<IDamageable>();
		if (damageableObject != null){
			Camera viewCamera = Camera.main;
			damageableObject.TakeHit(damage, hit.point, viewCamera.gameObject.transform.forward);
		}

		if(hit.rigidbody != null){
			Vector3 hitDirection = -viewCamera.transform.forward;
			hit.rigidbody.AddForce(-hitDirection * shotForce);
		}
		Destroy(Instantiate(bulletImpact, hit.point, Quaternion.FromToRotation(Vector3.forward, hit.normal)) as GameObject, 3f);
	}

	//Only adds force to shot objects
	void OnMoveObject(RaycastHit hit){
		if(hit.rigidbody != null){
			Vector3 hitDirection = -viewCamera.transform.forward;
			hit.rigidbody.AddForce(-hitDirection * shotForce);
		}
		Destroy(Instantiate(bulletImpact, hit.point, Quaternion.FromToRotation(Vector3.forward, hit.normal)) as GameObject, 1f);
	}

	IEnumerator ReadyWeapon(){
		float percent = 0;
		float readySpeed = 1 / WeaponReadyAnimationTime;
		Vector3 readyPos= transform.parent.localPosition;
		Vector3 unreadyPos= transform.parent.localPosition + Vector3.down * 1f;


		while (percent < 1){
			percent += Time.deltaTime * readySpeed;
			transform.parent.localPosition = Vector3.Lerp(unreadyPos, readyPos, percent);
			//transform.localPosition = Vector3.Lerp(unreadyPosition.localPosition, readyPosition.localPosition, percent);
			yield return null;
		}
		canShoot = true;
	}

	public void OnTriggerHold(){
		if(fireAttribute == FireAttribute.Default){
			StartCoroutine(Shoot());
		} else if(fireAttribute == FireAttribute.Penetrate){
			StartCoroutine(ShootPenetrate());
		} else if (fireAttribute == FireAttribute.RocketLauncher){
			StartCoroutine(ShootGrenade());
		} else if (fireAttribute == FireAttribute.Shotgun){
			StartCoroutine(ShootShotgun());
		} else if (fireAttribute == FireAttribute.WaterGun){
			StartCoroutine(ShootWater());
		} else if (fireAttribute == FireAttribute.Lazer){
			StartCoroutine(ShootLazer());
		}
		triggerReleasedSinceLastShot = false;
	}

	public void OnTriggerRelease(){
		triggerReleasedSinceLastShot = true;
	}

}
