using UnityEngine;
using System.Collections;

public class CabbageBear : LivingEntity {

	public LayerMask seekableObjects;
	public AnimationClip attackingAnimation;

	public GameObject fire;

	bool isAttacking;

	NavMeshAgent pathfinder;

	public GameObject target;
	float myCollisionRadius = 2;
	float targetCollisionRadius = 2;


	// Use this for initialization
	protected override void Start () {
		base.Start();
		StartCoroutine(Grow());
		pathfinder = GetComponent<NavMeshAgent>();
		FindClosestSprout();

	}

	void Update(){

		//if the target has been reached and it is not on fire, attack it
		if (target != null && target.GetComponent<WaterableObject>() != null && !target.GetComponent<WaterableObject>().isOnFire || target.GetComponent<WateredObject>() != null && !target.GetComponent<WateredObject>().isOnFire){
			if(!pathfinder.pathPending){
				if(pathfinder.remainingDistance <= pathfinder.stoppingDistance){
					if(!pathfinder.hasPath || pathfinder.velocity.sqrMagnitude == 0f){
						if (isAttacking == false){
							StartCoroutine(Attack(target.transform.position));
						}

					}
				}
			}
		}

		if (target == null || target.GetComponent<WaterableObject>() != null && target.GetComponent<WaterableObject>().isOnFire || target.GetComponent<WateredObject>() != null && target.GetComponent<WateredObject>().isOnFire){
//			pathfinder.Stop();
			print("Finding New Target");
			FindClosestSprout();
		}

	}

	void FindClosestSprout() {
		isAttacking = false;

		Collider[] existingSprouts = Physics.OverlapSphere(transform.position, 50, seekableObjects);

		GameObject closest = null;
       	float distance = Mathf.Infinity;
        Vector3 position = transform.position;


        if (existingSprouts.Length > 0){
	        foreach (Collider sprout in existingSprouts) {

	        	//only if the sprout is not on fire, consider it
				if (sprout.gameObject.GetComponent<WaterableObject>() != null && !sprout.gameObject.GetComponent<WaterableObject>().isOnFire || sprout.gameObject.GetComponent<WateredObject>() != null && !sprout.gameObject.GetComponent<WateredObject>().isOnFire){
		          	Vector3 diff = sprout.transform.position - position;
		            float curDistance = diff.sqrMagnitude;
		            if (curDistance < distance) {
		            	closest = sprout.gameObject;
		               	distance = curDistance;
		               	target = closest;

						print("Found target");

		               	NavToClosestSprout(closest);
	
		            }
				}
	        } 
		} 

		//if no sprouts exist or all are on fire, do an idle animation
		else if (existingSprouts.Length <= 0 || IfAllOnFire(existingSprouts) == true){
			print("No more targets");

			pathfinder.Stop();
        	GetComponent<Animator>().Play("Idle");
        }
	}

	bool IfAllOnFire(Collider[] existingSprouts){
		for(int i = 0; i < existingSprouts.Length; i++){
			if (existingSprouts[i].gameObject.GetComponent<WaterableObject>() != null && existingSprouts[i].gameObject.GetComponent<WaterableObject>().isOnFire || existingSprouts[i].gameObject.GetComponent<WateredObject>() != null && existingSprouts[i].gameObject.GetComponent<WateredObject>().isOnFire){
				continue;
			} else {
				return false;
			}
		}
		return true;
	}

	void NavToClosestSprout(GameObject closestTarget){
		if(target != null){
			print("Moving to Target");

			Vector3 dirToTarget = (target.transform.position - transform.position).normalized;

			Vector3 targetPosition = target.transform.position - dirToTarget * (targetCollisionRadius + myCollisionRadius);

			if(!dead){
				GetComponent<Animator>().Play("Waddling");
				pathfinder.SetDestination(targetPosition);
			}
		}
	}

	void FaceTarget(){
		transform.LookAt(target.transform.position);
	}

	IEnumerator Attack(Vector3 attackPosition){

		if (target != null && isAttacking == false){
			isAttacking = true;
			print("attacking");

			if (target.GetComponent<WaterableObject>() != null && target.GetComponent<WaterableObject>().isOnFire == false){

				Instantiate(fire, attackPosition, Quaternion.identity);

//				if (target.GetComponent<WaterableObject>() != null){
					target.GetComponent<WaterableObject>().isOnFire = true;
//				} else if (target.GetComponent<WateredObject>() != null){
//					target.GetComponent<WateredObject>().isOnFire = true;
//				}


				GetComponent<Animator>().Play("Attacking");
				yield return new WaitForSeconds(attackingAnimation.length * 2);
				GetComponent<Animator>().Stop();


			} else if (target.GetComponent<WateredObject>() != null && target.GetComponent<WateredObject>().isOnFire == false){
				Instantiate(fire, attackPosition, Quaternion.identity);

				target.GetComponent<WateredObject>().isOnFire = true;
				GetComponent<Animator>().Play("Attacking");
				yield return new WaitForSeconds(attackingAnimation.length * 2);
				GetComponent<Animator>().Stop();
			}

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
