using UnityEngine;
using System.Collections;

[RequireComponent(typeof (NavMeshAgent))]
public class Enemy : LivingEntity {

	[Header("Sounds")]
	public AudioClip[] deathSounds;
	public GameObject audioPlayer;
	AudioSource audioSource;

	public ParticleSystem deathParticles;
	public ParticleSystem deathCloud;

	public LayerMask ground;
	public float damage = 10;
	public Material enemyMat;

	public GameObject hugMeBubble;

	float growTime = 1;
	NavMeshAgent pathfinder;
	Transform target;
	Camera viewCamera;
	DayAndNightCycle dayAndNightCycle;
	PlayerController player;
	PlayerUI playerUI;

	float nightSpeed = 15;
	float daySpeed = 10;

	// Use this for initialization
	protected override void Start () {
		base.Start();
		player = FindObjectOfType<PlayerController>();
		dayAndNightCycle = FindObjectOfType<DayAndNightCycle>();
		viewCamera = Camera.main;
		pathfinder = GetComponent<NavMeshAgent>();
		target = GameObject.FindGameObjectWithTag("Player").transform;
		playerUI = FindObjectOfType<PlayerUI>();
		audioSource = audioPlayer.GetComponent<AudioSource>();


		StartCoroutine(PathRefresh());
		StartCoroutine(Grow());

	}

	void Update(){
		if (dayAndNightCycle.isNight == true){
			StartCoroutine(NightChange());
//			if (player.isSelecting == true){
//				pathfinder.speed = daySpeed / 2;
//			} else if (player.isSelecting == false){
//				pathfinder.speed = daySpeed;
//			}
		}
		if (dayAndNightCycle.isDay == true){
			StartCoroutine(DayChange());
//			if (player.isSelecting == true){
//				pathfinder.speed = nightSpeed / 2;
//			} else if (player.isSelecting == false){
//				pathfinder.speed = nightSpeed;
//			}
		}

	}

	public override void TakeHit(float damage, Vector3 hitPoint, Vector3 hitDirection){
		if (damage >= health){
			Instantiate(deathParticles, hitPoint, Quaternion.FromToRotation(Vector3.forward, hitDirection));
		}

		base.TakeHit(damage, hitPoint, hitDirection);

	}

	public override void Die (){
		int percentChance = Random.Range(1, 101);
		Instantiate(deathCloud, gameObject.transform.position + Vector3.down * 0.75f, Quaternion.Euler(Vector3.left * 90));

		//if you kill the enemy and the tutorial is over
		if (percentChance < 30 && player.isTutorial2 == false){
			GrowSprout();
		}

		//grow a sprout every time the tutorial enemy is killed
		if (gameObject.tag == "TutorialEnemy"){
			GrowSprout();
		}

		int index = Random.Range(0, 5);
		audioSource.PlayOneShot(deathSounds[index], 0.1f);
		audioPlayer.transform.parent = null;
		Destroy(audioPlayer,2);

		base.Die ();

		//update the kill counter bar

		player.killCycleCount ++;
		playerUI.AddKillCount(player.killCycleCount);
		//unlock doominator
		if (player.killCycleCount == player.doominatorKillCount && player.hasDoominator == false){

			FindObjectOfType<PlayerUI>().UnlockWeaponText("Doominator Unlocked!", 0);
			FindObjectOfType<PlayerUI>().StartCoroutine(FindObjectOfType<PlayerUI>().UnlockAnimation());


			//unlock doominator weapon ui
			playerUI.UnlockWeapon(playerUI.doominatorUI);

			player.weaponNames[0] = "Doominator";


			player.WeaponSwitch(0);
			playerUI.doominatorUI.SetActive(true);
			player.hasDoominator = true;
			player.killCycleCount = 0;
			playerUI.ammoAdditionDoominator.SetActive(true);
			playerUI.AddKillCount(player.killCycleCount);
			playerUI.doominatorIcon.SetActive(false);
			playerUI.rhmIcon.SetActive(true);

		//unlock rhm
		} else if (player.killCycleCount == player.rhmKillCount && player.hasRHM == false){

			FindObjectOfType<PlayerUI>().UnlockWeaponText("Reverend Unlocked!", 0);
			FindObjectOfType<PlayerUI>().StartCoroutine(FindObjectOfType<PlayerUI>().UnlockAnimation());


			//unlock rhm weapon ui
			playerUI.UnlockWeapon(playerUI.rhmUI);

			player.weaponNames[2] = "Reverend";


			player.WeaponSwitch(2);
			player.killCycleCount = 0;
			playerUI.AddKillCount(player.killCycleCount);
			playerUI.rhmUI.SetActive(true);
			player.hasRHM = true;
			playerUI.ammoAdditionRHM.SetActive(true);

			playerUI.rhmIcon.SetActive(false);
			playerUI.bigRedIcon.SetActive(true);


	
			FindObjectOfType<Spawner>().StopCoroutine("SlowSpawner");
			FindObjectOfType<Spawner>().StartCoroutine("SpawnEnemy");

		//unlock bigred and allow to continually get ammo for it after getting it first
		} else if (player.killCycleCount == player.bigRedInitialKillCount && player.hasRHM == true && player.hasBigRed == false){

			FindObjectOfType<PlayerUI>().UnlockWeaponText("Big Red Unlocked!", 0);
			FindObjectOfType<PlayerUI>().StartCoroutine(FindObjectOfType<PlayerUI>().UnlockAnimation());


			//unlock doominator weapon ui
			playerUI.UnlockWeapon(playerUI.BigRedUI);

			player.weaponNames[4] = "Big Red";


			player.WeaponSwitch(4);
			player.killCycleCount = 0;
			playerUI.AddKillCount(player.killCycleCount);
			playerUI.BigRedUI.SetActive(true);
			player.hasBigRed = true;
		} else if (player.killCycleCount == player.bigRedKillCount && player.hasBigRed == true){

			FindObjectOfType<PlayerUI>().UnlockWeaponText("Big Red Ammo Gained!", 0);

			player.ammo[2] = player.bigRedMax;
			player.killCycleCount = 0;
			playerUI.AddKillCount(player.killCycleCount);
			player.audioSourceSFX.PlayOneShot(player.BigRedAmmo, 0.5f);
		}

		Destroy(gameObject);
	}

	public void GrowSprout(){
		Ray ray = new Ray (transform.position, -transform.up);
		GameObject existingSprout = GameObject.FindWithTag("TreeDuplicator");
		RaycastHit hit;

		if (Physics.Raycast(ray, out hit, Mathf.Infinity, ground)){
			GameObject newSprout = Instantiate(existingSprout, hit.point + new Vector3(viewCamera.gameObject.transform.forward.x, 0 , viewCamera.gameObject.transform.forward.z) * 7, Quaternion.Euler(0, Random.Range(0, 360), 0)) as GameObject;
			newSprout.GetComponent<WaterableObject>().damageFillPercentage = 0;
			newSprout.GetComponent<WaterableObject>().waterFillPercentage = 0;
		}
	}


	IEnumerator PathRefresh(){
		float refreshRate = .1f;

		while (target != null){
			Vector3 targetPosition = new Vector3(target.position.x, target.position.y, target.position.z);
			if(!dead){
				pathfinder.SetDestination(targetPosition);
			}
			yield return new WaitForSeconds(refreshRate);
		}
	}

	void OnCollisionEnter(Collision col){
		if(col.gameObject.tag == "Player"){
			player.TakeDamage(damage);
		}
	}

	public IEnumerator NightChange(){
		float changeTime = 1;
		float changeSpeed = 1 / changeTime;
		float percent = 0;

		while (percent < 1){
			percent += Time.deltaTime * changeSpeed;
//			enemyMat.color = Color.Lerp(blue, red, percent);
			yield return null;
		}
		pathfinder.speed = nightSpeed;
		pathfinder.acceleration = 18;


	}

	public IEnumerator DayChange(){
		float changeTime = 1;
		float changeSpeed = 1 / changeTime;
		float percent = 0;

		while (percent < 1){
			percent += Time.deltaTime * changeSpeed;
//			enemyMat.color = Color.Lerp(red, blue, percent);
			yield return null;
		}
		pathfinder.speed = daySpeed;
		pathfinder.acceleration = 8;
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

	public IEnumerator HugMe(GameObject enemyHugger){
		yield return new WaitForSeconds(1);
		GameObject hug = Instantiate(hugMeBubble, gameObject.transform.position + Vector3.up, Quaternion.identity) as GameObject;
		hug.transform.parent = enemyHugger.gameObject.transform;
	}
}
