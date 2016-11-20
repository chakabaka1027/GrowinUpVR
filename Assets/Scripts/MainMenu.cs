using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

	public GameObject blackFade;
	public GameObject companyLogo;

	public void RestartGame(){
		SceneManager.LoadScene("Game");
	}

	public void Menu(){
		SceneManager.LoadScene("Menu");
	}

	public void QuitGame(){
		Application.Quit();
	}

	void Start(){

		Scene currentScene = SceneManager.GetActiveScene ();
 
        string sceneName = currentScene.name;
 
        if (sceneName == "Menu") {
			StartCoroutine(LogoAnimation());
        }

        else if (sceneName == "Game"){
			StartCoroutine(FadeIn());
        }
	}

	IEnumerator LogoAnimation(){
		yield return new WaitForSeconds(2);

		Animator animator = blackFade.GetComponent<Animator>();
		animator.Play("FadeIn");

		yield return new WaitForSeconds(3);

		animator.Play("FadeOut");
		yield return new WaitForSeconds(3);
		Destroy(companyLogo);
		animator.Play("FadeIn");
		yield return new WaitForSeconds(2);
		blackFade.SetActive(false);
	}

	IEnumerator FadeIn(){
		Animator animator = blackFade.GetComponent<Animator>();
		animator.Play("FadeIn");
		yield return new WaitForSeconds(2);
		blackFade.SetActive(false);

	}
}
