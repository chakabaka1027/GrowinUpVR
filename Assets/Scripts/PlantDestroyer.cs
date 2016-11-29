using UnityEngine;
using System.Collections;

public class PlantDestroyer : LivingEntity {

	NavMeshAgent pathfinder;
	Transform target;


	// Use this for initialization
	protected override void Start () {
		base.Start();

		GetComponent<Animator>().Play("Waddling");

		pathfinder = GetComponent<NavMeshAgent>();
		target = GameObject.FindGameObjectWithTag("Player").transform;


		StartCoroutine(PathRefresh());

	}
	
	// Update is called once per frame
	void Update () {
	
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
}
