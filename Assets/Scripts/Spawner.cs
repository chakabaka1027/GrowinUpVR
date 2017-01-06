using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour {

	public GameObject pickleBear;
	public GameObject cabbageBear;

    public LayerMask spawnableArea;

	public GameObject noSpawnZone;
	float slowSpawnRate = 3;
	float spawnRate = 2;

	void Start(){
	}

	public IEnumerator TutorialSpawner(){
		yield return new WaitForSeconds(1);
		
		GameObject tutorialEnemy = Instantiate(pickleBear, new Vector3(Camera.main.transform.forward.x, 0 , Camera.main.transform.forward.z) * 10 , Quaternion.identity) as GameObject;
		tutorialEnemy.tag = "TutorialEnemy";
		tutorialEnemy.transform.parent = gameObject.transform;

		StartCoroutine(tutorialEnemy.GetComponent<PickleBear>().HugMe(tutorialEnemy));
	}

	public IEnumerator SlowSpawner(){
		yield return new WaitForSeconds(1.5f);

		while(true){

			yield return new WaitForSeconds(slowSpawnRate);

            while (true)
            {
                Vector3 spawnPoint = new Vector3(Random.Range(-30, 30), 0, Random.Range(-15, 15));

                Collider[] spawnLocation = Physics.OverlapSphere(spawnPoint, 2, spawnableArea);

                if (spawnLocation.Length <= 0)
                {
                    GameObject enemyInstance = Instantiate(pickleBear, spawnPoint, Quaternion.identity) as GameObject;
                    enemyInstance.transform.parent = gameObject.transform;
                    break;
                }
                else {
                    print("retrying");
                    continue;
                }

                //GameObject enemyInstance = Instantiate(pickleBear, new Vector3(Random.Range(-30, 30), 0, Random.Range(-15, 15)), Quaternion.identity) as GameObject;
                //enemyInstance.transform.parent = gameObject.transform;
                //if (enemyInstance.transform.position == noSpawnZone.transform.position){
                // 	Destroy(enemyInstance);
                //}
            }

		}
	}

	public IEnumerator SpawnPickleBear(){
		StartCoroutine(SpawnCabbageBear());

		while(true){

			yield return new WaitForSeconds(spawnRate);

            while (true){
                Vector3 spawnPoint = new Vector3(Random.Range(-30, 30), 0, Random.Range(-15, 15));

                Collider[] spawnLocation = Physics.OverlapSphere(spawnPoint, 2, spawnableArea);

                if (spawnLocation.Length <= 0){
                    GameObject enemyInstance = Instantiate(pickleBear, spawnPoint, Quaternion.identity) as GameObject;
                    enemyInstance.transform.parent = gameObject.transform;
                    break;
                }
                else{
                    print("retrying");

                    continue;
                }

                //GameObject enemyInstance = Instantiate(pickleBear, new Vector3(Random.Range(-30, 30), 0, Random.Range(-15, 15)), Quaternion.identity) as GameObject;
                //enemyInstance.transform.parent = gameObject.transform;
            }
        }
	}

	public IEnumerator SpawnCabbageBear(){

		while(true){
			float interval = Random.Range(120, 181);
			yield return new WaitForSeconds(interval);

            while (true)
            {
                Vector3 spawnPoint = new Vector3(Random.Range(-30, 30), 0, Random.Range(-15, 15));

                Collider[] spawnLocation = Physics.OverlapSphere(spawnPoint, 2, spawnableArea);

                if (spawnLocation.Length <= 0)
                {
                    GameObject enemyInstance = Instantiate(cabbageBear, spawnPoint, Quaternion.identity) as GameObject;
                    enemyInstance.transform.parent = gameObject.transform;
                    break;
                }
                else
                {
                    print("retrying");

                    continue;
                }
            }



           // GameObject enemyInstance = Instantiate(cabbageBear, new Vector3(Random.Range(-30, 30), 0, Random.Range(-15, 15)), Quaternion.identity) as GameObject;
			//enemyInstance.transform.parent = gameObject.transform;	
		}
	}


}
