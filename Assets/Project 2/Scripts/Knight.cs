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
		IntegerLocation loc = new IntegerLocation (transform.position);
		int tarX = Random.Range (loc.x - moves, loc.x + moves);
		int tarY = Random.Range (loc.y - moves, loc.y + moves);
		if (Move (tarX, tarY)) {
			Debug.Log ("moved");
		} else {
			Debug.Log("nope");
		}
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


>>>>>>> origin/pathfinding-features
	}


	private void OnTriggerEnter2D (Collider2D other)
	{
		// used for collision detection	


	}


}
