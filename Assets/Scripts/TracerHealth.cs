using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TracerHealth : MonoBehaviour {

	private TracerHUD tracerHUD;
	private int currentHealth;

    void Awake() {
		tracerHUD = GetComponent<TracerHUD>();
		currentHealth = 150;
    }

	public void TakeDamage(int damage) {
		currentHealth -= damage;								//hurt the player
		currentHealth = Mathf.Clamp(currentHealth, 0, 150);		//make sure the health doesn't go past 0
		tracerHUD.UpdateHealthBar(currentHealth);				//call the HUD script to update the UI
		if(currentHealth == 0) {
			Die();												//kill the player if health hits 0
		}
	}

	//reloads the level when Tracer dies
	private void Die() {
		int activeSceneIndex = SceneManager.GetActiveScene().buildIndex;
		SceneManager.LoadScene(activeSceneIndex);
	}
}