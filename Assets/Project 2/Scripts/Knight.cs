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
		if (base.newAnimation && myPlayer.photonView.isMine) { // updates server with new animation
			Debug.Log ("update animation here");
			photonView.RPC("updateAnimation", PhotonTargets.AllBufferedViaServer);
			newDirection = myDirection;
			//updateAnimation();
		} else if (myPlayer.photonView.isMine && Input.GetKeyDown ("f")) { // can attack with sword 'f'
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

}