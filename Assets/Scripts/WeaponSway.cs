using UnityEngine;
using System.Collections;

public class WeaponSway : MonoBehaviour {

	[Header("Aim Sway")]
	public float aimSpeed = 10;

	[Header("Player Movement Sway")]
	public float cameraMoveTime = .1f;
	public float moveAmountX = 0.035f;
	public float moveAmountY = 0.035f;

	float mouseX;
	float mouseY;

	Quaternion rotationSpeed;


	void LateUpdate () {
	//aim sway
		mouseX = Input.GetAxis("Mouse X");
		mouseX = Mathf.Clamp(mouseX, -7, 7);
		mouseY = Input.GetAxis("Mouse Y");
		mouseY = Mathf.Clamp(mouseY, -7, 7);


		rotationSpeed = Quaternion.Euler(mouseY * 1, -mouseX * 1, 0);
		transform.localRotation = Quaternion.Slerp(transform.localRotation, rotationSpeed, aimSpeed * Time.deltaTime); 

	//player movement sway
		if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0){
			StartCoroutine(PlayerMovementSway());
		}
	}

	IEnumerator PlayerMovementSway(){
		float percent = 0f;
		float moveSpeed = 1 / cameraMoveTime;
		Vector3 currentPos = transform.parent.localPosition;
		float playerX = Input.GetAxis("Horizontal");
		float playerY = Input.GetAxis("Vertical");
		Vector3 targetPos = new Vector3(-playerX * moveAmountX, 0, -playerY * moveAmountY);

		while(percent < 1){
			percent += Time.deltaTime * moveSpeed;
			transform.parent.localPosition = Vector3.Lerp(currentPos, targetPos, percent);
			yield return null;

			if (Input.GetAxis("Horizontal") == 0 || Input.GetAxis("Vertical") == 0){
				break;
			}
		}
	}


}
