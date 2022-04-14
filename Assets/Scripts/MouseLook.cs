using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour {

    [SerializeField] Transform playerBody;
    [SerializeField] private float mouseSensitivity = 100f;

    private float xRotation = 0f;

    void Update() {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); //clamps the up and down rotation between -90 and 90 degrees

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f); //looks up and down
        playerBody.Rotate(Vector3.up * mouseX); //rotates the entire player
    }
}