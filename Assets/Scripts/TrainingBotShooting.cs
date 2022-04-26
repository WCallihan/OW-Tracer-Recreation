using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingBotShooting : MonoBehaviour {

	[Header("Raycast Settings")]
	[SerializeField] float weaponRange;
	[SerializeField] Transform leftRayOrigin;
	[SerializeField] Transform rightRayOrigin;
	[SerializeField] float lineMaxDuration = 0.1f;

	[Header("Weapon Settings")]
	[SerializeField] float attackCooldown;
	[SerializeField] int weaponDamage;
	[SerializeField] LayerMask layerToHit;
	[SerializeField] AudioClip shootingSFX;

	private RaycastHit objectHit;
	private LineRenderer lineRenderer;
	private AudioSource audioSource;
	private Transform currentRayOrigin;
	private float attackTimer;
	private float lineTimer;
	private int layerToHitInt;

    void Awake() {
		lineRenderer = GetComponent<LineRenderer>();
		audioSource = GetComponent<AudioSource>();
		currentRayOrigin = leftRayOrigin;
		layerToHitInt = layerToHit.value << 8; //convert layer to hit to an 8 bit value to be inverted
    }

    void Update() {
		//make sure the line renderer is enabled on each frame
		lineRenderer.enabled = true;

		//decrease attack timer
		attackTimer -= Time.deltaTime;

		//shoot forward from the curren attack origin
		if(attackTimer <= 0) {
			attackTimer = attackCooldown;   //reset attack timer
			lineTimer = lineMaxDuration;    //reset line timer
			Shoot();
		}

		//disable line renderer after max duration to make line disappear
		lineTimer -= Time.deltaTime;
		if(lineTimer <= 0) {
			lineRenderer.enabled = false;
		}
    }

	private void Shoot() {
		//make the ray direction and end point
		Vector3 rayDirection = -transform.forward;
		Vector3 endPoint = currentRayOrigin.position + (rayDirection * weaponRange);

		//set beginning of the visual line
		lineRenderer.SetPosition(0, currentRayOrigin.position);

		//shoot raycast to hit something on the Player layer (exclude everything except the layer to hit (player))
		if(Physics.Raycast(currentRayOrigin.position, rayDirection, out objectHit, weaponRange, ~layerToHitInt)) {
			lineRenderer.SetPosition(1, objectHit.point);   //set end of visual line if it hits
			TracerHealth player = objectHit.transform.gameObject.GetComponent<TracerHealth>();
			player?.TakeDamage(weaponDamage);				//damage the player if it is them
		} else {
			lineRenderer.SetPosition(1, endPoint);			//set end of visual line if it misses
		}

		//play shooting sound effect
		audioSource.PlayOneShot(shootingSFX);

		//switch the current ray origin
		if(currentRayOrigin == leftRayOrigin) {
			currentRayOrigin = rightRayOrigin;
		} else {
			currentRayOrigin = leftRayOrigin;
		}
	}
}