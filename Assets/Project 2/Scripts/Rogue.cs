﻿using UnityEngine;
using System.Collections;

public class Rogue : Unit {

	public int abilityUses;
	private Animator animator;
	private bool facingRight;
	
	private bool attack;
	private bool amDead;
	private bool dancing;
	
	private bool selectedHighlighted;
	private bool lowHealth;
	
	private int cols = BoardManager.columns;
	
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
		amDead = false;
		dancing = false;
		unitHighlight = false;
		lowHealth = false;
		selectedHighlighted = false;
		if (transform.position.x > (cols / 2))
			flip ();
		
	}
	
	private void setColor()
	{
		if (gameObject.name.Equals("Rogue-Red(Clone)")) { // red
			animator.SetLayerWeight (2, 1f);
			animator.SetLayerWeight (1, 0f);
			animator.SetLayerWeight (0, 0f);
		} else if (gameObject.name.Equals("Rogue-Green(Clone)")) { // green
			animator.SetLayerWeight (1, 1f);
			animator.SetLayerWeight (0, 0f);
			animator.SetLayerWeight (2, 0f);
		} else if (gameObject.name.Equals("Rogue-Blue(Clone)")){// blue
			animator.SetLayerWeight (0, 1f);
			animator.SetLayerWeight (1, 0f);
			animator.SetLayerWeight (2, 0f);
		}
		
		
		
	}

	protected override IEnumerator WaitForAbility ()
	{
		//Debug.Log ("waiting for move target");
		yield return new WaitForSeconds (.1f);
		var validTargets = FindTargets(abilityRange, true);
		HighlightTargets(validTargets, Color.blue);
		isAttacking = true;
		myPlayer.unitHasAttacked = true;
		
		if (hasAttacked) {
			Debug.Log ("you have already attacked this turn");
			HighlightTargets (validTargets, Color.white);
			isAttacking = false;
		} else if (abilityUses == 0) {
			Debug.Log("Cannot use this ability anymore");
			HighlightTargets(validTargets, Color.white);
			isAttacking = false;
		}
		
		while (isAttacking) {
			if (Input.GetMouseButtonDown (0))
			{
				Vector3 pos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
				RaycastHit2D hit = Physics2D.Raycast(pos, transform.position);
				int distance = (int)Vector3.Distance(transform.position,pos);
				if (distance > abilityRange)
				{
					Debug.Log("this attack is too far" + distance);
					HighlightTargets (validTargets, Color.white);
					isAttacking = false;
					yield return null;
				}
				
				
				else if (hit.collider != null && attackRange >= distance && hit.collider.transform.tag.Equals("Player2"))
				{
					Debug.Log("this unit ( " + transform.name + ") is switching positions with friendly unit (" + hit.collider.transform.name + ") hit is being confirmed" );
					//hit.collider.gameObject.GetComponent<Unit>().healthValue -= attackValue;
					photonView.RPC("reduceDamage", PhotonTargets.AllBufferedViaServer, hit.collider.transform.position);
					Debug.Log("friend selected" );
					HighlightTargets (validTargets, Color.white);
					HighlightUnit(Color.yellow);
					photonView.RPC("setAttackAnimation", PhotonTargets.AllBufferedViaServer);
					hasAttacked = true;
					isAttacking = false;
				}
				else{
					Debug.Log("you have clicked on something within your range that is not an enemy unit");
					HighlightTargets (validTargets, Color.white);
				}
				
				//moved = Move(validMoves,new IntegerLocation (target));
			}	
			yield return null;
		}
		yield return new WaitForSeconds (1);
		photonView.RPC("stopAttackAnimation", PhotonTargets.AllBufferedViaServer);
	}

	[PunRPC] void reduceDamage(Vector3 targetPos)
	{
		RaycastHit2D hit = Physics2D.Raycast(targetPos, transform.position);
		if (hit.collider != null) {
			Unit targetUnit = hit.collider.gameObject.GetComponent<Unit>();
			Debug.Log("reducing damage value of " + gameObject.name);
			targetUnit.attackValue -= abilityPower;
			abilityUses--;
		}
	}

	void Update()
		
	{
		Debug.Log ("selectedHighlighted: " + selectedHighlighted + " " + photonView.viewID + " - " + gameObject.name);
		
		//if the player has not selected a unit and this unit is not already (unselected) highlighted then highlight
		if (myPlayer.photonView.isMine && !myPlayer.unitSelected && !selected && !unitHighlight && myPlayer.myTurn.getTurn () == myPlayer.turn && !amDead) {
			HighlightUnit (Color.blue);
			unitHighlight = true;
			selectedHighlighted = false;
			Debug.Log ("unselected highlighting unit");
		}
		//else if the player has selected a unit and the unit is not (selected) highlighted then highlight
		else if (myPlayer.photonView.isMine && myPlayer.myTurn.getTurn () == myPlayer.turn && selected && !selectedHighlighted && myPlayer.unitSelected) {
			Debug.Log ("selected highlighting unit");
			HighlightUnit (Color.yellow);
			unitHighlight = false;
			selectedHighlighted = true;
		} 
		
		
		//else if the player has selected a unit (not this unit) and this unit is still (unselected) highlighted  or this has a (selected) highlight 
		//and the player has not chosen this unit then remove the highlight
		else if (myPlayer.photonView.isMine && myPlayer.myTurn.getTurn () == myPlayer.turn && (unitHighlight || selectedHighlighted) && myPlayer.unitSelected && !selected) {
			HighlightUnit (Color.white);
			unitHighlight = false;
			selectedHighlighted = false;
			Debug.Log ("unhighlighting unit");
		} 
		// game is over remove any remaining highlights
		else if (myPlayer.photonView.isMine && myPlayer.myTurn.gameOver && (selectedHighlighted || unitHighlight)) {
			HighlightUnit (Color.white);
			selectedHighlighted = false;
			unitHighlight = false;
		}
		
		/* // low health to give a damage boost
		if (healthValue < 5 && !lowHealth && !amDead && !getDeathStatus ()) {
			Debug.Log("critically low health");
			//attackValue = attackValue + (int) attackValue/2;
			attackValue *= 10;
			var img = GetComponent<SpriteRenderer>();
			img.color = Color.red;
			lowHealth = true;
		}
		*/
		
		if (getDeathStatus () && !amDead) {
			var img = GetComponent<SpriteRenderer>();
			img.color = Color.white;
			Lifebar_Dead();
		}
		
		if (myPlayer.wonGame && myPlayer.photonView.isMine && !dancing) {
			photonView.RPC("updateVictoryAnimation", PhotonTargets.AllBufferedViaServer);
		}
		
		if (myPlayer.photonView.isMine)
			Debug.Log ("my " + transform.name + " has " + getHealth () + "health");
		else if (!myPlayer.photonView.isMine) {
			Debug.Log ("enemy " + transform.name + " has " + getHealth () + "health");
		}
		//Debug.Log ("my health" + base.healthValue);
		if (!myPlayer.photonView.isMine)
			Debug.Log ("This is not your unit");
		
		//		Debug.Log ("my direction: "+myDirection+ "new direction: " + newDirection + "move? "+ base.newAnimation);
		
		// myPlayer.turn == myPlayer.myTurn.getTurn() &&
		if (base.newAnimation && myPlayer.photonView.isMine && myPlayer.myTurn.getTurn() == myPlayer.turn) { // updates server with new animation
			Debug.Log ("update animation here");
			photonView.RPC("updateAnimation", PhotonTargets.AllBufferedViaServer);
			//updateMyAnimations();
		} 
		
		/*
		else if (myPlayer.photonView.isMine && Input.GetKeyDown ("f")) { // can attack with sword 'f'
				attack = true;
				photonView.RPC("updateAnimation", PhotonTargets.AllBufferedViaServer);
		}
		else if (myPlayer.photonView.isMine && Input.GetKeyDown ("s")) { // stop it with 's'
			animator.SetBool ("attack", false);
			animator.SetBool ("down_attack", false);
			animator.SetBool ("up_attack", false);
		}
		else if (myPlayer.photonView.isMine && Input.GetKeyDown ("r")) { // refresh with 'r' (debugging for now)
			photonView.RPC("updateAnimation", PhotonTargets.AllBufferedViaServer);
		}
		*/	
	}
	
	public void HighlightUnit(Color color)
	{
		GameObject[] board = GameObject.FindGameObjectsWithTag ("Floor");
		
		foreach(GameObject loc in board)
		{
			if(gameObject.transform.position == loc.transform.position){
				var img = loc.GetComponent<SpriteRenderer>();
				img.color = color;
			}
		}
	}
	
	void Lifebar_Dead(){
		
		Debug.Log ("you are dead");
		myPlayer.unitDied ();
		amDead = true;
		animator.SetBool("dead",true);
		this.gameObject.GetComponent<Collider2D>().enabled=false;
		Destroy(Lifebar_group);
		StartCoroutine("waitForDeath");
		
		/*if((Lifebar.GetComponent<Renderer>().bounds.size.x<=0)||(Lifebar.transform.localScale.x<=0)){
				Debug.Log("your lifebar is depleted");
				amDead = true;
				animator.SetBool("dead",true);
				this.gameObject.GetComponent<Collider2D>().enabled=false;
				Destroy(Lifebar_group);
				StartCoroutine("waitForDeath");
			} */
	}
	
	IEnumerator waitForDeath()
	{
		yield return new WaitForSeconds (5);
		Destroy (gameObject);
	}
	
	[PunRPC] public void updateVictoryAnimation()
	{
		dancing = true;
		StartCoroutine("victoryAnimation");
	}
	
	IEnumerator victoryAnimation()
	{
		animator.SetBool ("walk", false);
		animator.SetBool ("down", false);
		animator.SetBool ("up", false);
		var img = GetComponent<SpriteRenderer>();
		img.color = Color.white;
		if (!facingRight)
			flip ();
		while (true) {
			yield return new WaitForSeconds (.4f); // right
			img.color = Color.grey;
			animator.SetBool ("up_attack", false);
			animator.SetBool("walk", true);
			yield return new WaitForSeconds (.4f); // right attack
			img.color = Color.blue;
			animator.SetBool("walk", false);
			animator.SetBool("attack", true);
			yield return new WaitForSeconds (.4f); // down
			img.color = Color.black;
			animator.SetBool("attack", false);
			animator.SetBool("down", true);
			yield return new WaitForSeconds (.4f); // down attack
			img.color = Color.gray;
			//animator.SetBool("down", false);
			animator.SetBool ("down_attack", true);
			yield return new WaitForSeconds (.4f); // left
			img.color = Color.cyan;
			animator.SetBool("down", false);
			animator.SetBool ("down_attack", false);
			flip ();
			animator.SetBool("walk", true);
			yield return new WaitForSeconds (.4f); // left attack
			img.color = Color.green;
			animator.SetBool("walk", false);
			animator.SetBool("attack", true);
			yield return new WaitForSeconds (.4f); // up
			img.color = Color.magenta;
			animator.SetBool("attack", false);
			animator.SetBool("up", true);
			yield return new WaitForSeconds (.4f); // up attack
			img.color = Color.red;
			animator.SetBool("up", false);
			animator.SetBool ("up_attack", true);
			yield return new WaitForSeconds (.4f); // back to right
			img.color = Color.yellow;
			flip ();
			yield return null;
		}
	}
	
	[PunRPC] public void updateAnimation()
	{
		updateMyAnimations ();
	}
	
	private void updateMyAnimations()
	{
		Debug.Log ("updating");
		if (myDirection.Equals ("right")) {
			if (!facingRight) // flip
			{
				flip ();
			}
			animator.SetBool ("up", false);
			animator.SetBool ("down", false);
			animator.SetBool ("walk", true);
		} 
		else if (myDirection.Equals ("left")) {
			if (facingRight)
			{
				flip ();
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
	
	public void flip ()
	{
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
		if (facingRight)
			facingRight = false;
		else
			facingRight = true;
	}
	
}