using UnityEngine;
using System.Collections;

public class DisplayMessage : MonoBehaviour {

	public string message;

	private bool displayed = true;
	private GUIStyle myStyle;

	void Start()
	{
		myStyle = new GUIStyle ();
		myStyle.fontSize = 20;
	}

	void OnGUI()
	{
		if (!displayed) {
			GUI.Label (new Rect (Screen.width/2 - 200, Screen.height/3, 200, 40), message, myStyle);
		}
	}
	
	// Update is called once per frame

	public void displayErrorMessage(string displayMessage, int seconds)
	{
		message = displayMessage;
		displayed = false;
		StartCoroutine (waitToDestroyMessage (seconds));
	}

	private IEnumerator waitToDestroyMessage(int seconds)
	{
		yield return new WaitForSeconds (seconds);
		displayed = true;
	}
}
