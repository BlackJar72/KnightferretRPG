using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildGameObjects : MonoBehaviour {

	public List<Transform> childrenGO = new List<Transform> ();


	void Start () {
		//CloseAllChildren ();
	}

	public void CloseAllChildren() {
		foreach (Transform child in transform) {
			childrenGO.Clear ();
			childrenGO.Add (child);

			if (child.gameObject.activeSelf) {
				child.gameObject.SetActive (false);
			}

		}
			
	}

	public void ToggleCurrentGameObject (GameObject go){
		if (go.gameObject.activeSelf) {
			go.gameObject.SetActive (false);
		} else {
			go.gameObject.SetActive (true);
		}
	}

	void Update () {
		
	}
}
