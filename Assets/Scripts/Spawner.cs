using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour {

	public GameObject enemy;
	public GameObject noSpawnZone;
	float slowSpawnRate = 3;
	float spawnRate = 2;

	public IEnumerator TutorialSpawner(){
		yield return new WaitForSeconds(1);
		
		GameObject tutorialEnemy = Instantiate(enemy, new Vector3(Camera.main.transform.forward.x, 0 , Camera.main.transform.forward.z) * 10 , Quaternion.identity) as GameObject;
		tutorialEnemy.tag = "TutorialEnemy";
		tutorialEnemy.transform.parent = gameObject.transform;

		StartCoroutine(tutorialEnemy.GetComponent<Enemy>().HugMe(tutorialEnemy));
	}

	public IEnumerator SlowSpawner(){
		yield return new WaitForSeconds(1.5f);

		while(true){

			yield return new WaitForSeconds(slowSpawnRate);

			GameObject enemyInstance = Instantiate(enemy, new Vector3(Random.Range(-30, 30), 0, Random.Range(-15, 15)), Quaternion.identity) as GameObject;
			enemyInstance.transform.parent = gameObject.transform;
			 if (enemyInstance.transform.position == noSpawnZone.transform.position){
			 	Destroy(enemyInstance);
			 }

		}
	}

	public IEnumerator SpawnEnemy(){

		while(true){

			yield return new WaitForSeconds(spawnRate);

			GameObject enemyInstance = Instantiate(enemy, new Vector3(Random.Range(-30, 30), 0, Random.Range(-15, 15)), Quaternion.identity) as GameObject;
			enemyInstance.transform.parent = gameObject.transform;

		}
	}


}
