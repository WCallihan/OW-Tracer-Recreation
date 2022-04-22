using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour {

	[SerializeField] Transform playerBody;
	[SerializeField] private float mouseSensitivity;

	private float cameraXRotation = 0f;
	private bool isLocked = false;

	void Awake() {
		//lock cursor
		Cursor.lockState = CursorLockMode.Locked;

		//add SetLock to the recallAction event
		TracerRecall.RecallAction += SetLock;
	}

	void Update() {
		//if locked, do nothing
		if(isLocked) {
			return;
		}

		//get the input from the mouse
		float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
		float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

		//gets the rotation of the camera
		cameraXRotation -= mouseY;
			
		//rotate the camera and player
		if((cameraXRotation >= -90f) && (cameraXRotation <= 90f)) { //essentially clamp the x rotation between -90 and 90 degrees
			transform.Rotate(-mouseY, 0, 0);						//rotate the camera up and down
		} else {
			cameraXRotation += mouseY;								//undo the adding of the mouse Y delta
		}
		playerBody.Rotate(Vector3.up * mouseX);                     //rotate the entire play horizontally
	}

	//sets whether the player can move affect the camera or character rotation at all
	public void SetLock(bool locked) {
		isLocked = locked;
	}
}