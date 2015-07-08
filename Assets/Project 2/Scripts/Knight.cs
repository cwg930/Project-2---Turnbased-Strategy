using UnityEngine;
using System.Collections;

public class Knight : Unit {
	private Transform player;
	private float inverseMoveTime2;
	private bool moving = false;

	// Use this for initialization
	protected override void Start () {
		player = GameObject.FindGameObjectWithTag("Player").transform;
		circleCollider = GetComponent <CircleCollider2D> ();
		inverseMoveTime2 = 1f / moveTime;
		base.Start ();
	}
	
	// Update is called once per frame
	void OnMouseDown () {
		Debug.Log ("mouse clicked");
		Move (1,2);
		/*
		int horizontal = 0;
		int vertical = 0;
	void Update () {

		//StartCoroutine(wait ());
		quickMove ();

	*/
	}



	/*
	void quickMove()
	{
		if (Input.GetMouseButtonDown (0) && !moving) {

			RaycastHit2D hit;

			Vector3 new_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			new_pos.z = player.position.z;
			new_pos.x = Mathf.Round(new_pos.x / 1) * 1;
			new_pos.y = Mathf.Round(new_pos.y / 1) * 1;

			Vector2 start = transform.position;
			Vector2 end = start + new Vector2 (new_pos.x, new_pos.y);

			circleCollider.enabled = false;
			hit = Physics2D.Linecast (start, end, blockingLayer);
			circleCollider.enabled = true;

			if(hit.transform == null)
			{
				StartCoroutine (SmoothMovement (new_pos)); // if path is clear then move
			}

			//StartCoroutine(slideMove(player.position, new_pos));
			//slideMove(player.position, new_pos);
			//player.position = new_pos;

			//base.Move (horizontal2, vertical2); // move along path to target
		}


	}
	*/



	private void OnTriggerEnter2D (Collider2D other)
	{
		// used for collision detection	


	}




	/*protected override IEnumerator SmoothMovement (Vector3 end)
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
	}*/
}
