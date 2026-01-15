using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUISoundManager : MonoBehaviour {

	AudioSource GUIAudioSource;

	public AudioClip buttonEnter;
	public AudioClip buttonDown;
	public AudioClip buttonUp;

	public AudioClip sliderEnter;
	public AudioClip sliderDown;
	public AudioClip sliderUp;
	public AudioClip sliderValueChange;

	void Start () {
		GUIAudioSource = GetComponent<AudioSource> ();
	}

	public void PlayAudioClip(AudioClip audioClip){
		GUIAudioSource.clip = audioClip;
		GUIAudioSource.Play ();
	}

	public void PlayButtonUp(){
		GUIAudioSource.clip = buttonUp;
		GUIAudioSource.Play ();
	}

	public void PlayButtonOver(){
		GUIAudioSource.clip = buttonEnter;
		GUIAudioSource.Play ();
	}

	public void PlayButtonDown(){
		GUIAudioSource.clip = buttonDown;
		GUIAudioSource.Play ();
	}

	void Update () {
		
	}
}
