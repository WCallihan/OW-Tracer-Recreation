using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(CharacterController))]
public class TracerRecall : MonoBehaviour {

	public static event Action<bool> RecallAction; //create event action with one bool parameter

	[SerializeField] private float recallDuration;
	[SerializeField] private float recallCooldown;
	[SerializeField] private float maxDataPoints = 50f;
	[SerializeField] private AudioClip recallSFX;

	private TracerHUD tracerHUD;
	private AudioSource audioSource;
	private MouseLook mouseLook;

	private List<RecallData> recallData = new List<RecallData>(); //create list so that the length can change
	private float timeBetweenDataPoints;
	private bool canRecall = true;
	private bool isRecalling = false;
	private float recallTimer = 0f;
	private float dataTimer = 0f;

	//create a basic class to hold the set of information needed
	private class RecallData {
		public Vector3 characterPosition;
		public Quaternion characterRotation;
		public Quaternion cameraRotation;
		public float timeCollected;
	}

	void Awake() {
		tracerHUD = GetComponent<TracerHUD>();
		audioSource = GetComponent<AudioSource>();
		mouseLook = GetComponentInChildren<MouseLook>();

		//establish the interval at which the character gets recall data based on the max list size
		timeBetweenDataPoints = 3 / maxDataPoints;
		//send the cooldown value to the HUD script so that it's slider and countdown is consistent
		tracerHUD.SetRecallCooldown(recallCooldown);
	}

	void Update() {
		dataTimer -= Time.deltaTime;
		if(!isRecalling && dataTimer <= 0) { //if the player is Recalling, don't change the recallData list
			//add the recall data point for the current frame
			recallData.Add(GetData());
			dataTimer = timeBetweenDataPoints; //reset the data collection timer

			//if the oldest point was collected 3 or more seconds ago or there are too many data points, remove it
			float timeOfFirstPoint = recallData[0].timeCollected;
			if(Time.time - timeOfFirstPoint >= 3 || recallData.Count > maxDataPoints) {
				recallData.RemoveAt(0);
			}
		}

		//draw the recall path in the editor
		for(int i = 0; i < recallData.Count - 1; i++) {
			Debug.DrawLine(recallData[i].characterPosition, recallData[i + 1].characterPosition);
		}

		//countdown the recall timer and set if the player can recall
		recallTimer -= Time.deltaTime;
		if(!isRecalling && recallTimer <= 0) {
			canRecall = true;
			tracerHUD.GiveRecall(); //call the HUD script to update the UI
		} else {
			canRecall = false;
		}

		//if E is pressed and the player is able, then Recall
		if(Input.GetKeyDown(KeyCode.E) && canRecall) {
			StartCoroutine(Recall(recallDuration));
		}
	}

	//used to get the current game data used when Recalling
	private RecallData GetData() {
		return new RecallData {
			characterPosition = transform.position,
			characterRotation = transform.rotation,
			cameraRotation = mouseLook.transform.rotation,
			timeCollected = Time.time
		};
	}

	//used to activate Tracer's Recall ability, sending her backwards on the path she has been on in the last 3 seconds
	private IEnumerator Recall(float duration) {
		isRecalling = true;										//set recall flag to true
		RecallAction(true);                                     //invoke the recallAction event with a value of true, telling other scripts that the Recall is starting
		gameObject.layer = LayerMask.NameToLayer("Blinking");	//change layer so that Tracer can pass through enemies but not walls
		audioSource.PlayOneShot(recallSFX);						//play recall sound effect

		//get the amount of time that the character will spend getting to each stored point
		float secondsBtwnPoints = recallDuration / recallData.Count;

		//get the oldest point in the list and pass it to RecallCameraRotation to Lerp the camera rotation
		RecallData lastDataPoint = recallData[0];
		StartCoroutine(RecallCameraRotation(duration, lastDataPoint));

		//iterate through the list and move the character to each point in reverse sequential order
		for(int i = recallData.Count - 1; i >= 0; i--) {
			RecallData currentPos = GetData();				//get the current location
			RecallData currentDataPoint = recallData[i];	//get the next point to got to

			//Lerp the character position and rotation in the designated time
			float t = 0;
			while(t < secondsBtwnPoints) {
				transform.position = Vector3.Lerp(currentPos.characterPosition, currentDataPoint.characterPosition, t / secondsBtwnPoints);
				transform.rotation = Quaternion.Lerp(currentPos.characterRotation, currentDataPoint.characterRotation, t / secondsBtwnPoints);
				t += Time.deltaTime;
				yield return null;
			}

			recallData.RemoveAt(i);	//remove the newest point in the list
		}

		isRecalling = false;									//set recall flag to false
		RecallAction(false);									//invoke the recallAction event with a value of false, telling other scripts that the Recall is ending
		gameObject.layer = LayerMask.NameToLayer("Player");		//switch the layer back so that Tracer can no longer go through enemies
		recallTimer = recallCooldown;							//set the recall timer to begin cooldown
	}

	//used to move the camera rotation during the Recall since it is handled differently than the character position and rotation
	private IEnumerator RecallCameraRotation(float duration, RecallData lastDataPoint) {
		mouseLook.SetLock(true); //lock the mouse input

		//Lerp the camera position to the ending rotation throughout the entire Recall duration
		float t = 0;
		while(t < duration) {
			mouseLook.transform.rotation = Quaternion.Lerp(mouseLook.transform.rotation, lastDataPoint.cameraRotation, t / duration);
			t += Time.deltaTime;
			yield return null;
		}

		mouseLook.SetLock(false); //unlock the mouse input
	}
}