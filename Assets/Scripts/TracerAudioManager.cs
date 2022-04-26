using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class TracerAudioManager : MonoBehaviour {

	[SerializeField] private AudioClip jumpSFX;
	[SerializeField] private AudioClip[] blinkSFX;
	[SerializeField] private AudioClip blinkGainSFX;
	[SerializeField] private AudioClip[] recallSFX;
	[SerializeField] private AudioClip recallGainSFX;

	private AudioSource audioSource;

	void Awake() {
		audioSource = GetComponent<AudioSource>();
    }

	//play jump sound effect
    public void Jump() {
		audioSource.PlayOneShot(jumpSFX);
	}

	//play random Blink sound effect
	public void Blink() {
		int i = Random.Range(0, blinkSFX.Length);
		audioSource.PlayOneShot(blinkSFX[i]);
	}

	//play gaining Blink sound effect
	public void GainBlink() {
		audioSource.PlayOneShot(blinkGainSFX);
	}

	//play random Recall sound effect
	public void Recall() {
		int i = Random.Range(0, recallSFX.Length);
		audioSource.PlayOneShot(recallSFX[i]);
	}

	//play gaining Recall sound effect
	public void GainRecall() {
		audioSource.PlayOneShot(recallGainSFX);
	}
}