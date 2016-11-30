using UnityEngine;
using System.Collections;

public class CabbageBear : LivingEntity {

	public LayerMask seekableObjects;
	public AnimationClip attackingAnimation;

	public GameObject fire;

	NavMeshAgent pathfinder;
	GameObject target;
	float myCollisionRadius = 2;
	float targetCollisionRadius = 2;


	// Use this for initialization
	protected override void Start () {
		base.Start();
		StartCoroutine(Grow());
		GetComponent<Animator>().Play("Waddling");
		pathfinder = GetComponent<NavMeshAgent>();
		target = FindClosestSprout();

		NavToClosestSprout();
	}

	void Update(){

		//if the target has been reached
	
		if(!pathfinder.pathPending){
			if(pathfinder.remainingDistance <= pathfinder.stoppingDistance){
				if(!pathfinder.hasPath || pathfinder.velocity.sqrMagnitude == 0f){

					StartCoroutine(Attack(target.transform.position));

				}
			}
		}

	}

	void NavToClosestSprout(){

		Vector3 originalPosition = transform.position;
		Vector3 dirToTarget = (target.transform.position - transform.position).normalized;

		Vector3 targetPosition = target.transform.position - dirToTarget * (targetCollisionRadius + myCollisionRadius);

//		Vector3 targetPosition = new Vector3(target.position.x, target.position.y, target.position.z);
		if(!dead){
			pathfinder.SetDestination(targetPosition);
		}

	}



	GameObject FindClosestSprout() {
		Collider[] existingSprouts = Physics.OverlapSphere(transform.position, 50, seekableObjects);


//		GameObject[] sprouts = GameObject.FindGameObjectsWithTag("TreeDuplicator");

		GameObject closest = null;
       	float distance = Mathf.Infinity;
        Vector3 position = transform.position;

        if (existingSprouts.Length > 0){

	        foreach (Collider sprout in existingSprouts) {
	          	Vector3 diff = sprout.transform.position - position;
	            float curDistance = diff.sqrMagnitude;
	            if (curDistance < distance) {
	            	closest = sprout.gameObject;
	               	distance = curDistance;
	            }
	        }
	    }

        return closest;
	}

	void FaceTarget(){
		transform.LookAt(target.transform.position);
	}

	IEnumerator Attack(Vector3 attackPosition){
		if (target != null){

			if (target.GetComponent<WaterableObject>().isOnFire == false){

				Instantiate(fire, attackPosition, Quaternion.identity);

				target.GetComponent<WaterableObject>().isOnFire = true;
				GetComponent<Animator>().Play("Attacking");
				yield return new WaitForSeconds(attackingAnimation.length * 2);
				GetComponent<Animator>().Stop();


			}

		//if the target is destroyed on the way there, look again for another one -> not working yet
		} else if (target == null) {
			target = FindClosestSprout();
			StartCoroutine(Attack(target.transform.position));

		}
	}

	public IEnumerator Grow(){
		float percent = 0;
		float growTime = 1;
		float growSpeed = 1 / growTime; 
		while (percent < 1){
			percent += Time.deltaTime * growSpeed;
			gameObject.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one * 0.2265497f, percent);
			yield return null;
		}
	}
		
}
