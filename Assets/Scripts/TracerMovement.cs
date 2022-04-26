using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class TracerMovement : MonoBehaviour {

    [SerializeField] private float speed = 12f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float jumpHeight = 3f;

    private CharacterController characterControler;
	private TracerAudioManager tracerAudio;

    private Vector3 velocity;
    private bool isGrounded;
	private bool isLocked = false;

    void Awake() {
        characterControler = GetComponent<CharacterController>();
        tracerAudio = GetComponent<TracerAudioManager>();

		//add SetLock to the RecallAction event
		TracerRecall.RecallAction += SetLock;
    }

    void Update() {
        //check if on ground
        isGrounded = Physics.CheckSphere(groundCheck.position, 0.6f, groundMask);
        if(isGrounded && velocity.y < 0) {
            velocity.y = -2f; //just makes sure the player sticks to the ground
        }

		//if locked, do nothing
		if(isLocked) {
			return;
		}

		//take in inputs
		float inputX = Input.GetAxis("Horizontal");
		float inputZ = Input.GetAxis("Vertical");

		//use character controller to move
		Vector3 moveVector = (transform.right * inputX) + (transform.forward * inputZ);
		if(moveVector.magnitude > 1) {
			moveVector /= moveVector.magnitude; //normalizes magnitude so the character doesn't move faster when moving diagonally
		}
		characterControler.Move(moveVector * speed * Time.deltaTime);
		
		//jump
		if(Input.GetButtonDown("Jump") && isGrounded) {
			//jump upwards
			velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
			//use audio manager script to play jump sound effect
			tracerAudio.Jump();
		}

		//simulate gravity
		velocity.y += gravity * Time.deltaTime;
		//move the character
		characterControler.Move(velocity * Time.deltaTime);
    }

	public void SetLock(bool locked) {
		isLocked = locked;
	}
}