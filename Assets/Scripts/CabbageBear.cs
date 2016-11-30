using UnityEngine;
using System.Collections;

public class CabbageBear : LivingEntity {

	public LayerMask seekableObjects;

	NavMeshAgent pathfinder;
	Transform target;


	// Use this for initialization
	protected override void Start () {
		base.Start();
		GetComponent<Animator>().Play("Waddling");
		pathfinder = GetComponent<NavMeshAgent>();
		target = FindClosestSprout().transform;

		NavToClosestSprout();
	}

	void NavToClosestSprout(){


		Vector3 targetPosition = new Vector3(target.position.x, target.position.y, target.position.z);
		if(!dead){
			pathfinder.SetDestination(targetPosition);
		}
	}

	GameObject FindClosestSprout() {
		Collider[] existingSprouts = Physics.OverlapSphere(transform.position, 15, seekableObjects);


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
		
}
