using UnityEngine;
using System.Collections;

public class EscapeMenu : MonoBehaviour {

	public GameObject menu; // Assign in inspector
	private bool isShowing = false;

	
	// Update is called once per frame
	void Update () {

		if (Input.GetKeyDown("escape")) {
			isShowing = !isShowing;
			menu.SetActive(isShowing);
		}
	
	}
}
