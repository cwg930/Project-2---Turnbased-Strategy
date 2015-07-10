using UnityEngine;
using System.Collections;

public class Knight : Unit {

	protected override void Start () {

		base.Start ();
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

/*
	// Update is called once per frame

	void Update () {

	}

	void OnMouseDown()
	{

		player = this.gameObject.transform;
		Debug.Log ("object: " + this.gameObject.name);
		moved = false;
		while (!moved)
			makeMove ();
		/*bool heroSelected = false;

		while (!heroSelected) {
			var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if(Physics.Raycast(ray,out hit)){
				Debug.Log("Hero was hit by ray");
				player = hit.collider.gameObject.transform;
				heroSelected = true;
			}
		
		} 

		Debug.Log ("Unit was clicked");
		//StartCoroutine ("wait");

	} */

	private void OnTriggerEnter2D (Collider2D other)
	{
		//if (other.tag == "Player1")
		// used for collision detection	

	}

	 /*public override Unit getUnitType <T> ()
	{
		return gameObject.GetComponent<Knight> ();
	} */


}
