﻿using UnityEngine;
using System.Collections;

public class Knight : Unit {
	//private Transform player; //moving to unit
	//private bool moving = false; //moving to unit
	private float inverseMoveTime2;

	//private int rows = BoardManager.rows;
	//private int cols = BoardManager.columns; // moving both to unit

	// Use this for initialization
	protected override void Start () {
		 //player = GameObject.FindGameObjectWithTag("Player").transform; // moving ot unit
		//inverseMoveTime2 = 1f / moveTime; // set for deletion
		base.Start ();
	}

	void Update () {
		
		//StartCoroutine(wait ());
		makeMove ();
		
	}


	/*void makeMove() --MOVING TO UNIT SCRIPT
	{
		if (Input.GetMouseButtonDown (0) && !moving) {
			Vector3 new_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			new_pos.z = player.position.z;
			new_pos.x = Mathf.Round(new_pos.x / 1) * 1;
			new_pos.y = Mathf.Round(new_pos.y / 1) * 1;

			if (new_pos.x < 0 || new_pos.y < 0 || new_pos.x >= cols || new_pos.y >= rows) // stays within bounds
			{
				return;
			}

			Move ((int)new_pos.x, (int)new_pos.y);

		}
	} */
	
	//when the Unit is clicked
	/*void OnMouseDown () {
		Debug.Log ("mouse clicked");
		Move (1,2);
	} */




	private void OnTriggerEnter2D (Collider2D other)
	{
		// used for collision detection	

	}


}
