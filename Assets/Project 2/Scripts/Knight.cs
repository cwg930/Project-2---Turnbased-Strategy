using UnityEngine;
using System.Collections;

public class Knight : Unit {

	private Animator animator;
	private bool facingRight;

	private string newDirection;
	private bool attack;

	protected override void Start () {

		base.Start ();
		animator = GetComponent<Animator> ();
		animator.SetBool ("walk", false);
		animator.SetBool ("down", false);
		animator.SetBool ("up", false);
		animator.SetBool ("down_attack", false);
		animator.SetBool ("up_attack", false);
		setColor ();
		facingRight = true;
		attack = false;
		newDirection = "right";

	}

	private void setColor()
	{
		if (gameObject.name.Equals("Knight-Red(Clone)")) { // red
			animator.SetLayerWeight (2, 1f);
			animator.SetLayerWeight (1, 0f);
			animator.SetLayerWeight (0, 0f);
		} else if (gameObject.name.Equals("Knight-Green(Clone)")) { // green
			animator.SetLayerWeight (1, 1f);
			animator.SetLayerWeight (0, 0f);
			animator.SetLayerWeight (2, 0f);
		} else if (gameObject.name.Equals("Knight-Blue(Clone)")){// blue
			animator.SetLayerWeight (0, 1f);
			animator.SetLayerWeight (1, 0f);
			animator.SetLayerWeight (2, 0f);
		}
			


	}

	void Update()

	{
		if (!myPlayer.photonView.isMine)
			Debug.Log ("This is not your unit");

		Debug.Log ("my direction: "+myDirection+ "new direction: " + newDirection + "move? "+ base.newAnimation);

		// myPlayer.turn == myPlayer.myTurn.getTurn() &&
		if (base.newAnimation && myPlayer.photonView.isMine) {
			Debug.Log ("update animation here");
			photonView.RPC("updateAnimation", PhotonTargets.AllBufferedViaServer);
			newDirection = myDirection;
			//updateAnimation();
		} else if (myPlayer.photonView.isMine && Input.GetKeyDown ("f")) {
				attack = true;
				photonView.RPC("updateAnimation", PhotonTargets.AllBufferedViaServer);
		}
		else if (myPlayer.photonView.isMine && Input.GetKeyDown ("s")) {
			animator.SetBool ("attack", false);
			animator.SetBool ("down_attack", false);
			animator.SetBool ("up_attack", false);
		}
		else if (myPlayer.photonView.isMine && Input.GetKeyDown ("r")) {
			photonView.RPC("updateAnimation", PhotonTargets.AllBufferedViaServer);
		}
			
			
			

	}

	[PunRPC] public void updateAnimation()
	{
		Debug.Log ("updating");
		if (myDirection.Equals ("right")) {
			if (!facingRight) // flip
			{
				Vector3 theScale = transform.localScale;
				theScale.x *= -1;
				transform.localScale = theScale;
				facingRight = true;
			}
			animator.SetBool ("up", false);
			animator.SetBool ("down", false);
			animator.SetBool ("walk", true);
		} 
		else if (myDirection.Equals ("left")) {
			if (facingRight)
			{
				Vector3 theScale = transform.localScale;
				theScale.x *= -1;
				transform.localScale = theScale;
				facingRight = false;
			}
			animator.SetBool ("down", false);
			animator.SetBool ("up", false);
			animator.SetBool ("walk", true);
		} 
		else if (myDirection.Equals ("up")) {
			animator.SetBool ("down", false);
			animator.SetBool ("walk", false);
			animator.SetBool ("up", true);
		} 
		else if (myDirection.Equals ("down")) {
			animator.SetBool ("up", false);
			animator.SetBool ("walk", false);
			animator.SetBool ("down", true);
		} 
		if (stopped) {
			animator.SetBool ("walk", false);
			animator.SetBool ("down", false);
			animator.SetBool ("up", false);
			animator.SetBool ("attack", false);
			animator.SetBool ("down_attack", false);
			animator.SetBool ("up_attack", false);
		}
		
		if (attack) {
			if (myDirection.Equals("down"))
				animator.SetBool ("down_attack", true);
			else if (myDirection.Equals("up"))
				animator.SetBool ("up_attack", true);
			else
				animator.SetBool ("attack", true);
			attack = false;
		}
	}


//"right", "up", "left" , "down" , "attack"

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