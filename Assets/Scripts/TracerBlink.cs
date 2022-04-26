using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class TracerBlink : MonoBehaviour {

	[SerializeField] private float blinkDistance;
	[SerializeField] private float blinkDuration;
	[SerializeField] private float blinkCooldown;
	[SerializeField] private TrailRenderer blinkTrail;
	[SerializeField] private float maxTrailDuration;

	private TracerHUD tracerHUD;
	private CharacterController characterController;
	private TracerAudioManager tracerAudio;

	private bool isBlinking = false;
	private float[] blinkTimers = new float[] { 0, 0, 0 };
	private bool canBlink = true;
	private float trailTimer;
	private bool isLocked = false;

	void Awake() {
		tracerHUD = GetComponent<TracerHUD>();
		characterController = GetComponent<CharacterController>();
		tracerAudio = GetComponent<TracerAudioManager>();

		//make sure blink trail is deactivated
		blinkTrail.enabled = false;

		//add SetLock to the RecallAction event
		TracerRecall.RecallAction += SetLock;
	}

	void Update() {
		//get directional input
		float inputX = Input.GetAxisRaw("Horizontal");
		float inputZ = Input.GetAxisRaw("Vertical");

		//set the blink direction to the input
		Vector3 blinkDirection = (transform.right * inputX) + (transform.forward * inputZ);
		//if there is no input, set the blink direction to staright forward
		if(blinkDirection == Vector3.zero) {
			blinkDirection = transform.forward;
		}

		//countdown the blink cooldowns and check if the player is able to blink
		for(int i = 0; i < blinkTimers.Length; i++) {               //iterate through each blink cooldown
			float newCooldownTime = blinkTimers[i] - Time.deltaTime;//decrement each cooldown and set it to new variable
			if((blinkTimers[i] > 0) && (newCooldownTime <= 0)) {    //if the cooldown is now 0 or lower, then it will allow the player to blink
				canBlink = true;                                    //set can blink to true (for now)
				tracerHUD.GiveBlink();								//call the HUD script to give a blink to the UI
			} else if(blinkTimers[i] <= 0) {                        //if the cooldown was already 0 or lower, then set can blink to true
				canBlink = true;
			}
			blinkTimers[i] = newCooldownTime;                       //set the cooldown to the new time (regardless of if it is 0 or lower)
		}

		//if current blinking or blinking is locked, then you can't blink
		if(isBlinking || isLocked) {
			canBlink = false;
		}

		//decrement the trail timer and check if it has exceeded its timer and should be deactivated
		trailTimer -= Time.deltaTime;
		if(trailTimer <= 0) {
			blinkTrail.enabled = false;
		}

		//if Shift or Right Click is pressed and the player is able, then blink
		if((Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.Mouse1)) && canBlink) {            
			StartCoroutine(Blink(blinkDirection, blinkDistance, blinkDuration));
		}
	}

	//used to activate Tracer's blink ability, dashing her forward a short distance very quickly
	private IEnumerator Blink(Vector3 direction, float distance, float duration) {
		isBlinking = true;                                      //set blinking flag to true
		gameObject.layer = LayerMask.NameToLayer("Blinking");   //change layer so that Tracer can pass through enemies but not walls
		blinkTrail.enabled = true;								//turn on the trail
		trailTimer = maxTrailDuration;                          //reset the timer to allow the trail to linger for the maxTrailDuration
		tracerAudio.Blink();									//use audio manager script to play the blinking sound effect

		//look for the first cooldown that is zero or less and reset it
		for(int i = 0; i < blinkTimers.Length; i++) {
			if(blinkTimers[i] <= 0) {
				blinkTimers[i] = blinkCooldown;
				break;
			}
		}
		//call the HUD script to take a blink from the UI
		tracerHUD.TakeBlink();

		Vector3 startPosition = transform.position;             //set blink start position to measure distance traveled
		float elapsedTime = 0f;                                 //make elapsed time variable to measure the time since the blink started
		float speed = distance / duration;                      //get the speed of the blink based on the duration and distance

		//move the character along the blink direction at the blink speed until it has either reached the blink distance or has hit the blink duration
		while((distance > Vector3.Distance(startPosition, transform.position)) && (duration > elapsedTime)) {
			characterController.Move(direction * speed * Time.deltaTime);
			elapsedTime += Time.deltaTime;
			yield return null;
		}

		gameObject.layer = LayerMask.NameToLayer("Player");     //switch the layer back so that Tracer can no longer go through enemies
		isBlinking = false;                                     //set blinking flag to false so that Tracer can blink again
	}

	public void SetLock(bool locked) {
		isLocked = locked;
	}
}