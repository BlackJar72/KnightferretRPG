using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Required when Using UI elements.

public class Validate : MonoBehaviour {

	public List<InputField> inputFields = new List<InputField> ();

	void Start ()
	{

	}

	public void ValidateField()
	{
		foreach (InputField child in inputFields) {
			if (child.text == "") {

			}
		}
	}
}
