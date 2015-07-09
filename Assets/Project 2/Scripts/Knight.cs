using UnityEngine;
using System.Collections;

public class Knight : Unit {

	private Transform player;
	private float inverseMoveTime2;
	private bool moving = false;
	private bool clicked = false;
	private bool makingMove = false;
	private CircleCollider2D circleCollider2;

	private int rows = BoardManager.rows;
	private int cols = BoardManager.columns;

	private GameObject [] Team1;
	private GameObject [] Team2;
	private int teamIndex;




	// Use this for initialization
	protected override void Start () {

		Team1 = GameObject.FindGameObjectsWithTag ("Player1");
		teamIndex = 0;

		Debug.Log (Team1[0].transform.name);

		player = GameObject.FindGameObjectWithTag ("Player1").transform;

		//player = Team1[0].transform;

		circleCollider2 = GetComponent <CircleCollider2D> ();
		inverseMoveTime2 = 1f / moveTime;
		base.Start ();
	}
	
	// Update is called once per frame
	void Update () {
		//OnClickedHero ();
		quickMove ();
	
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
		makingMove = true;
		//StartCoroutine ("wait");

	}



	private void OnClickedHero()
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
	}




	private void quickMove()
	{
		/*Input.GetMouseButtonDown (0) && */
		if (Input.GetMouseButtonDown (0) && !moving) {

			//RaycastHit2D hit;

			Vector3 new_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

			new_pos.z = player.position.z;
			new_pos.x = Mathf.Round(new_pos.x / 1) * 1;
			new_pos.y = Mathf.Round(new_pos.y / 1) * 1;

			//player.position = new_pos;

			//Debug.Log("x ="+ new_pos.x + ", y=" + new_pos.y + "cols= "+ cols + "rows= " + rows);
			//Debug.Log(player.name);

			if (new_pos.x < 0 || new_pos.y < 0 || new_pos.x >= cols || new_pos.y >= rows) // stays within bounds
			{
				return;
			}

			player = Team1[teamIndex].transform;
			teamIndex++;
			if (teamIndex == Team1.Length)
				teamIndex = 0;

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

			//StartCoroutine(slideMove(player.position, new_pos));
			//slideMove(player.position, new_pos);
			//player.position = new_pos;

			//base.Move (horizontal2, vertical2); // move along path to target
		}
	}

	private void OnTriggerEnter2D (Collider2D other)
	{
		if (other.tag == "Player1")
			StopCoroutine ("wait");
		// used for collision detection	


	}


	IEnumerator wait ()
	{
		yield return new WaitForSeconds (1);
		while (makingMove && Input.GetMouseButtonDown (0)) {
			//yield return new WaitForSeconds (.05f);
			//quickMove();

		}



	}

	protected IEnumerator SmoothMovement (Vector3 end)
	{
		Debug.Log ("in smooth movement");
		moving = true;
		//Calculate the remaining distance to move based on the square magnitude of the difference between current position and end parameter. 
		//Square magnitude is used instead of magnitude because it's computationally cheaper.
		float sqrRemainingDistance = (player.position - end).sqrMagnitude;
		
		//While that distance is greater than a very small amount (Epsilon, almost zero):
		while(sqrRemainingDistance > float.Epsilon )
		{
			//Find a new position proportionally closer to the end, based on the moveTime
			Vector3 newPostion = Vector3.MoveTowards(player.position, end, inverseMoveTime2 * Time.deltaTime);
			
			//Call MovePosition on attached Rigidbody2D and move it to the calculated position.
			player.position = newPostion;
			
			//Recalculate the remaining distance after moving.
			sqrRemainingDistance = (player.position - end).sqrMagnitude;
			
			//Return and loop until sqrRemainingDistance is close enough to zero to end the function
			yield return null;
		}
		moving = false;
		clicked = false;
		//makingMove = false;
	}

	public override Unit getUnitType <T> ()
	{
		return gameObject.GetComponent<Knight> ();
	}
}
