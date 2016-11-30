using UnityEngine;
using System.Collections;

public class CabbageBear : LivingEntity {

	public LayerMask seekableObjects;

	NavMeshAgent pathfinder;
	Transform target;
	float myCollisionRadius = 2;
	float targetCollisionRadius = 2;


	// Use this for initialization
	protected override void Start () {
		base.Start();
		GetComponent<Animator>().Play("Waddling");
		pathfinder = GetComponent<NavMeshAgent>();
		target = FindClosestSprout().transform;

		NavToClosestSprout();
	}

	void Update(){

		//if the target has been reached
	
		if(!pathfinder.pathPending){
			if(pathfinder.remainingDistance <= pathfinder.stoppingDistance){
				if(!pathfinder.hasPath || pathfinder.velocity.sqrMagnitude == 0f){
					GetComponent<Animator>().Play("Attacking");
				}
			}
		}
	}

	void NavToClosestSprout(){

		Vector3 originalPosition = transform.position;
		Vector3 dirToTarget = (target.position - transform.position).normalized;

		Vector3 targetPosition = target.position - dirToTarget * (targetCollisionRadius + myCollisionRadius);

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
        foreach (Collider sprout in existingSprouts) {
          	Vector3 diff = sprout.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance) {
            	closest = sprout.gameObject;
               	distance = curDistance;
            }
        }

        return closest;
	}

	void FaceTarget(){
		transform.LookAt(target);
	}
		
}
