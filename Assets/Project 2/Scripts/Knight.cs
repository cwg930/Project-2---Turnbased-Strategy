﻿using UnityEngine;
using System.Collections;

public class Knight : Unit {

	// Use this for initialization
	protected override void Start () {
		base.Start ();
	}
	
	// Update is called once per frame
	void OnMouseDown () {
		Debug.Log ("mouse clicked");
		Move (1,2);
		/*
		int horizontal = 0;
		int vertical = 0;

		if (Input.GetMouseButtonDown (0)) {
			int horizontal2 = (int)Input.mousePosition.x;
			int vertical2 = (int)Input.mousePosition.y;

			Debug.Log (" horizontal: " + horizontal2 + " vertical " + vertical2);
			
			base.Move (horizontal2, vertical2);
		}
		*/

	}

}
