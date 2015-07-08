using UnityEngine;
using System.Collections;

public class Knight : Unit {
	private Transform player;
	private float inverseMoveTime2;
	private bool moving = false;
	private CircleCollider2D circleCollider2;

	private int rows = BoardManager.rows;
	private int cols = BoardManager.columns;




	// Use this for initialization
	protected override void Start () {
		player = GameObject.FindGameObjectWithTag("Player").transform;
		circleCollider2 = GetComponent <CircleCollider2D> ();
		inverseMoveTime2 = 1f / moveTime;
		base.Start ();
	}
	
	// Update is called once per frame
	void Update () {

		//StartCoroutine(wait ());
		quickMove ();

	
	}




	void quickMove()
	{
		if (Input.GetMouseButtonDown (0) && !moving) {

			//RaycastHit2D hit;

			Vector3 new_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			new_pos.z = player.position.z;
			new_pos.x = Mathf.Round(new_pos.x / 1) * 1;
			new_pos.y = Mathf.Round(new_pos.y / 1) * 1;

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

			//StartCoroutine(slideMove(player.position, new_pos));
			//slideMove(player.position, new_pos);
			//player.position = new_pos;

			//base.Move (horizontal2, vertical2); // move along path to target
		}
	}

	private void OnTriggerEnter2D (Collider2D other)
	{
		// used for collision detection	


	}


	IEnumerator wait ()
	{
		Debug.Log ("waiting");
		yield return new WaitForSeconds (1);


	}

	protected IEnumerator SmoothMovement (Vector3 end)
	{
		moving = true;
		//Calculate the remaining distance to move based on the square magnitude of the difference between current position and end parameter. 
		//Square magnitude is used instead of magnitude because it's computationally cheaper.
		float sqrRemainingDistance = (transform.position - end).sqrMagnitude;
		float compareDist = 0;
		
		//While that distance is greater than a very small amount (Epsilon, almost zero):
		while(sqrRemainingDistance > float.Epsilon && sqrRemainingDistance != compareDist)
		{
			compareDist = sqrRemainingDistance;
			//Find a new position proportionally closer to the end, based on the moveTime
			Vector3 newPostion = Vector3.MoveTowards(player.position, end, inverseMoveTime2 * Time.deltaTime);
			
			//Call MovePosition on attached Rigidbody2D and move it to the calculated position.
			player.position = newPostion;
			
			//Recalculate the remaining distance after moving.
			sqrRemainingDistance = (transform.position - end).sqrMagnitude;
			
			//Return and loop until sqrRemainingDistance is close enough to zero to end the function
			yield return null;
		}
		moving = false;
	}

	public override Unit getUnitType <T> ()
	{
		return gameObject.GetComponent<Knight> ();
	}
}
