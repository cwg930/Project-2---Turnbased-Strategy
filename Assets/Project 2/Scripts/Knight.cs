using UnityEngine;
using System.Collections;

public class Knight : Unit {
	private Transform player;
	private bool moving = false;

	// Use this for initialization
	protected override void Start () {
		player = GameObject.FindGameObjectWithTag("Player").transform;
		base.Start ();
	}
	
	//when the Unit is clicked
	void OnMouseDown () {
		Debug.Log ("mouse clicked");
		Move (1,2);
	}


	private void OnTriggerEnter2D (Collider2D other)
	{
		// used for collision detection	


	}


}
