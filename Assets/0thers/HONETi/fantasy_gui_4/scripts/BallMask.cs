using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BallMask : MonoBehaviour {

	public Image mask;
	public GameObject fillEnding;
	RectTransform ballFillTopTransform;

	float ballFill;
	Vector3 ballFillTopPosition = new Vector3 ();
	float ballFillX;

	void Start () {
		ballFill = mask.fillAmount;
		ballFillTopTransform = fillEnding.GetComponent<Image> ().rectTransform;
	}
		
	void LateUpdate () {
		ballFill = mask.fillAmount;

		ballFillX += 1f;
		if (ballFillX >= 100f) {
			ballFillX = 0f;
		}

		ballFillTopPosition.Set(ballFillX, 172f * ballFill, 0);
		ballFillTopTransform.localPosition = ballFillTopPosition;
	}
}
