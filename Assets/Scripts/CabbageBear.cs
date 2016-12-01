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
		pathfinder = GetComponent<NavMeshAgent>();

		base.Start();
		StartCoroutine(Grow());
		FindClosestSprout();

	}

	void Update(){

		//if the target has been reached and it is not on fire, attack it
		if (target != null && target.GetComponent<WaterableObject>() != null && !target.GetComponent<WaterableObject>().isOnFire || target != null && target.GetComponent<WateredObject>() != null && !target.GetComponent<WateredObject>().isOnFire){
			if(!pathfinder.pathPending){
				if(pathfinder.remainingDistance <= pathfinder.stoppingDistance){
					if(!pathfinder.hasPath || pathfinder.velocity.sqrMagnitude <= 0f){
						StartCoroutine(Attack(target.transform.position));

					}
				}
			}
		}


		//if there is no target or the target is on fire
		if (target == null || target.GetComponent<WaterableObject>() != null && target.GetComponent<WaterableObject>().isOnFire && isAttacking == false || target != null && target.GetComponent<WateredObject>() != null && target.GetComponent<WateredObject>().isOnFire && isAttacking == false){
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
		print("Moving to Target");

		Vector3 dirToTarget = (target.transform.position - transform.position).normalized;

		Vector3 targetPosition = target.transform.position - dirToTarget * (targetCollisionRadius + myCollisionRadius);

		if(!dead){
			pathfinder.Resume();

			GetComponent<Animator>().Play("Waddling");
			pathfinder.SetDestination(targetPosition);
		}
	}

	IEnumerator FaceTarget(){

		Quaternion currentRotation = transform.rotation;
		Vector3 direction = (target.transform.position - transform.position).normalized;
		Quaternion lookRotation = Quaternion.LookRotation(direction);




		float percent = 0;
		float turnTime = 1;
		float turnSpeed = 1 / turnTime;

		while(percent < 1){
			percent +=  Time.deltaTime * turnSpeed;
			transform.rotation = Quaternion.Lerp(currentRotation, lookRotation, percent);
			yield return null;
		}


	}

	IEnumerator Attack(Vector3 attackPosition){
		StartCoroutine(FaceTarget());
		if (target != null && isAttacking == false){
			print("attacking");

			if (target.GetComponent<WaterableObject>() != null && target.GetComponent<WaterableObject>().isOnFire == false){
				isAttacking = true;

				Instantiate(fire, attackPosition, Quaternion.identity);

				target.GetComponent<WaterableObject>().isOnFire = true;



				GetComponent<Animator>().Play("Attacking");
				yield return new WaitForSeconds(attackingAnimation.length * 2);
				GetComponent<Animator>().Play("Idle");
				isAttacking = false;



			} else if (target.GetComponent<WateredObject>() != null && target.GetComponent<WateredObject>().isOnFire == false){
				isAttacking = true;

				Instantiate(fire, attackPosition, Quaternion.identity);

				target.GetComponent<WateredObject>().isOnFire = true;
				GetComponent<Animator>().Play("Attacking");
				yield return new WaitForSeconds(attackingAnimation.length * 2);
				GetComponent<Animator>().Play("Idle");
				isAttacking = false;

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
