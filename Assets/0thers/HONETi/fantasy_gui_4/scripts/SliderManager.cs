using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SliderManager : MonoBehaviour {
	Slider sliderComp;
	public bool noDecimals;

	void Start () {
		sliderComp = GetComponent<Slider> ();
	}

	public void SliderValueChange(Text textComponent){
		if (noDecimals == false) {
			textComponent.text = sliderComp.value.ToString ("F1");
		} else {
			textComponent.text = sliderComp.value.ToString ("N0");
		}
	}

	void Update () {
		
	}
}
