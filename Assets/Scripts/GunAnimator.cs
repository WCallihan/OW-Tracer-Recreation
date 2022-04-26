using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunAnimator : MonoBehaviour {

	[SerializeField] private Vector3 gunRecallingPos;
	[SerializeField] private float gunRecallingRotX;
	[SerializeField] private float gunRecallingRotY;
	[SerializeField] private float gunRecallingRotZ;
	[SerializeField] private float gunTravelTime;

	Quaternion gunRecallingRot;
	private Vector3 gunNormalPos;
	private Quaternion gunNormalRot;

    void Awake() {
		//set the recalling rotation
		gunRecallingRot = Quaternion.Euler(gunRecallingRotX, gunRecallingRotY, gunRecallingRotZ);

		//set the normal position for the gun to where it starts
		gunNormalPos = transform.localPosition;
		gunNormalRot = transform.localRotation;

		//add SetGun to the RecallAction event
		TracerRecall.RecallAction += SetGun;
    }

	//activated by the RecallAction event to set where the guns should move to
	private void SetGun(bool recalling) {
		if(recalling) {
			SetGunRecalling();
		} else {
			SetGunNormal();
		}
	}

	//move the gun outward
	private void SetGunRecalling() {
		StartCoroutine(GunLerpHelper(gunRecallingPos, gunRecallingRot));
	}

	//move the gun back inward
	private void SetGunNormal() {
		StartCoroutine(GunLerpHelper(gunNormalPos, gunNormalRot));
	}

	//helper function to Lerp the position and rotation of the gun in either direction
	private IEnumerator GunLerpHelper(Vector3 targetPos, Quaternion targetRot) {
		float t = 0;
		while(t < gunTravelTime) {
			transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, t / gunTravelTime);
			transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRot, t / gunTravelTime);
			t += Time.deltaTime;
			yield return null;
		}
	}
}