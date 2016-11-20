using UnityEngine;
using System.Collections;

public class DayAndNightCycle : MonoBehaviour {

	public GameObject[] clouds;

	public AudioClip roosterCall;
	public AudioClip wolfHowl;

	public Gradient nightDayColor;

	public float maxIntensity = 3f;
	public float minIntensity = 0f;
	public float minPoint = -0.2f;

	public float maxAmbient = 1f;
	public float minAmbient = 0f;
	public float minAmbientPoint = -0.2f;

	public Gradient nightDayFogColor;
	public AnimationCurve fogDensityCurve;
	public float fogScale = 1f;

	public float dayAtmosphereThickness = 0.4f;
	public float nightAtmosphereThickness = 0.87f;

	public Vector3 dayRotateSpeed;
	public Vector3 nightRotateSpeed;

	//Counter
	public bool isNight;
	public bool isDay;
	public bool canCount = false;
	public int dayCounter = 0;

	float skySpeed = 1;

	Light mainLight;
	Skybox sky;
	Material skyMat;
	PlayerUI playerUI;
	PlayerController player;

	void Awake(){
		StartCoroutine(TurnOffCycle());
	}

	// Use this for initialization
	void Start () {
		playerUI = FindObjectOfType<PlayerUI>();
		mainLight = GetComponent<Light>();
		skyMat = RenderSettings.skybox;
		player = FindObjectOfType<PlayerController>();
	}
	
	// Update is called once per frame
	void Update () {
		float tRange = 1 - minPoint;
		float dot = Mathf.Clamp01 ((Vector3.Dot (mainLight.transform.forward, Vector3.down) - minPoint)/ tRange);
		float i = ((maxIntensity - minIntensity) * dot) + minIntensity;

		mainLight.intensity = i;

		tRange = 1 - minAmbientPoint;
		dot = Mathf.Clamp01 ((Vector3.Dot (mainLight.transform.forward, Vector3.down) - minAmbientPoint) / tRange);
		i = ((maxAmbient - minAmbient) * dot) + minAmbient;
		RenderSettings.ambientIntensity = i;

		mainLight.color = nightDayColor.Evaluate(dot);
		RenderSettings.ambientLight = mainLight.color;

		RenderSettings.fogColor = nightDayFogColor.Evaluate(dot);
		RenderSettings.fogDensity = fogDensityCurve.Evaluate(dot) * fogScale;

		i = ((dayAtmosphereThickness - nightAtmosphereThickness) * dot) + nightAtmosphereThickness;
		skyMat.SetFloat ("_AtmosphereThickness", i);

		if (dot > 0)
			transform.Rotate (dayRotateSpeed * Time.deltaTime * skySpeed);
		else
			transform.Rotate (nightRotateSpeed * Time.deltaTime * skySpeed);
		
//			control the cycle for testing purposes
//			if (Input.GetKeyDown (KeyCode.Q)) skySpeed *= 0.5f;
//			if (Input.GetKeyDown (KeyCode.E)) skySpeed *= 2f;

		if (mainLight.transform.eulerAngles.x > 200 && mainLight.transform.eulerAngles.x < 350){
			isNight = true;
			isDay = false;
			canCount = false;
			StartCoroutine(playerUI.FlashLightActivate());

//			//turn off clouds
//			for(int c = 0; c < clouds.Length; c ++){
//				ParticleSystem[] cloud = clouds[c].GetComponentsInChildren<ParticleSystem>();
//				foreach(ParticleSystem ps in cloud){
//					ps.loop = false;
//				}
//			}

		}

		if (mainLight.transform.eulerAngles.x > 200 || mainLight.transform.eulerAngles.x < 30 ){
			//turn off clouds

			for(int c = 0; c < clouds.Length; c ++){
				ParticleSystem[] cloud = clouds[c].GetComponentsInChildren<ParticleSystem>();
				foreach(ParticleSystem ps in cloud){
					ps.loop = false;

				}
			}
		}


		if (mainLight.transform.eulerAngles.x > 0 && mainLight.transform.eulerAngles.x < 90){
			isDay = true;
			isNight = false;
			if (canCount == false){
				dayCounter ++;
				playerUI.IncreaseDayCount(dayCounter);
				canCount = true;
			}
			playerUI.FlashLightDeactivate();


			//turn on clouds
			for(int c = 0; c < clouds.Length; c ++){
				ParticleSystem[] cloud = clouds[c].GetComponentsInChildren<ParticleSystem>();
				foreach(ParticleSystem ps in cloud){
					ps.loop = true;
				}
			}
		}
	}

	IEnumerator TurnOffCycle(){
		yield return new WaitForSeconds(0.1f);
		FindObjectOfType<DayAndNightCycle>().enabled = false;
	}

	public void NightSound(){
		player.audioSourceSFX.PlayOneShot(wolfHowl, 0.05f);
	}

	public void DaySound(){
		player.audioSourceSFX.PlayOneShot(roosterCall, 0.05f);
	}
}
