using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingSimulation : MonoBehaviour {

	public GameObject openWindowAfterLoading;
	public GameObject closeWindowAfterLoading;
	public float loadingSpeed = 1f;
	Slider sliderComp;

	// Use this for initialization
	void OnEnable () {
		sliderComp = GetComponent<Slider> ();
		sliderComp.value = 0f;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		Mathf.Clamp(sliderComp.value += 1f * loadingSpeed, 0f, 100f);
		if (sliderComp.value >= 100f) {
			openWindowAfterLoading.gameObject.SetActive (true);
			closeWindowAfterLoading.gameObject.SetActive (false);
		}
	}
}
