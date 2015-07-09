using UnityEngine;
using System.Collections;

public class Knight : Unit {


	/*private GameObject [] Team1;
	private GameObject [] Team2;
	private int teamIndex; */


	//private Transform player; //moving to unit
	//private bool moving = false; //moving to unit

	//private int rows = BoardManager.rows;
	//private int cols = BoardManager.columns; // moving both to unit

	// Use this for initialization
	protected override void Start () {

		/*Team1 = GameObject.FindGameObjectsWithTag ("Player1");
		teamIndex = 0;*/

		//Debug.Log (Team1[0].transform.name);


		//player = Team1[0].transform;

		base.Start ();
	}
	
	// Update is called once per frame

	void Update () {
		
		//StartCoroutine(wait ());
		makeMove ();
	}

	void OnMouseDown()
	{
		/*bool heroSelected = false;

		while (!heroSelected) {
			var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if(Physics.Raycast(ray,out hit)){
				Debug.Log("Hero was hit by ray");
				player = hit.collider.gameObject.transform;
				heroSelected = true;
			}
		
		} */

		Debug.Log ("Unit was clicked");
		//StartCoroutine ("wait");

	}



	/*private void OnClickedHero()
	{
		if (Input.GetMouseButtonDown (0)) {
			Debug.Log("Pressed left click, casting ray.");
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;

			if (Physics.Raycast (ray, out hit, 100)) {
				Debug.Log (hit.transform.gameObject.name + " was hit by ray");
				player = hit.collider.transform;
			}
		}
	} */


/*	private void quickMove()
=======
	void makeMove() --MOVING TO UNIT SCRIPT
>>>>>>> develop
	{
		Input.GetMouseButtonDown (0) && 
		if (Input.GetMouseButtonDown (0) && !moving) {
			Vector3 new_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

			new_pos.z = player.position.z;
			new_pos.x = Mathf.Round(new_pos.x / 1) * 1;
			new_pos.y = Mathf.Round(new_pos.y / 1) * 1;

<<<<<<< HEAD
<<<<<<< HEAD
			//player.position = new_pos;

			//Debug.Log("x ="+ new_pos.x + ", y=" + new_pos.y + "cols= "+ cols + "rows= " + rows);
			//Debug.Log(player.name);

=======
>>>>>>> develop
			if (new_pos.x < 0 || new_pos.y < 0 || new_pos.x >= cols || new_pos.y >= rows) // stays within bounds
			{
				return;
			}
<<<<<<< HEAD
=======
			//Debug.Log("x ="+ new_pos.x + ", y=" + new_pos.y + "cols= "+ cols + "rows= " + rows);

			if (new_pos.x < 0 || new_pos.y < 0 || new_pos.x >= cols || new_pos.y >= rows) // stays within bounds
			{
				return;
			}
			StartCoroutine (SmoothMovement (new_pos));

			/*
			Vector2 start = transform.position;
			Vector2 end = start + new Vector2 (new_pos.x, new_pos.y); //creates vector 2 object of start and end transform

			circleCollider2.enabled = false; // disables the objects collider so the linecast doesnt hit it
			hit = Physics2D.Linecast (start, end, blockingLayer); // checks for a collision along the line from start to end
			circleCollider2.enabled = true;

			 // if path is clear then move

			if(hit.transform != null) // if linecast hit a collider
			{
				Debug.Log(hit.transform.ToString());
			}*/

		/*	player = Team1[teamIndex].transform; 
			teamIndex++;
			if (teamIndex == Team1.Length)
				teamIndex = 0; */


			/*
			Vector2 start = transform.position;
			Vector2 end = start + new Vector2 (new_pos.x, new_pos.y); //creates vector 2 object of start and end transform
=======
>>>>>>> develop

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
		if (other.tag == "Player1")
			StopCoroutine ("wait");
		// used for collision detection	

	}






	 /*public override Unit getUnitType <T> ()
	{
		return gameObject.GetComponent<Knight> ();
	} */
}
