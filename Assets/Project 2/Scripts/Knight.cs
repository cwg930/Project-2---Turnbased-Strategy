using UnityEngine;
using System.Collections;

public class Knight : Unit {

	private Animator animator;
	private bool facingRight;

	private string newDirection;
	private bool attack;
	private bool amDead;

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
		newDirection = "right";
		if (transform.position.x > (cols / 2))
			flip ();

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
		if (getDeathStatus () && !amDead) {
			Lifebar_Dead();
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

	void Lifebar_Dead(){

		Debug.Log ("you are dead");
		myPlayer.unitDied ();
			if((Lifebar.GetComponent<Renderer>().bounds.size.x<=0)||(Lifebar.transform.localScale.x<=0)){
				Debug.Log("your lifebar is depleted");
				amDead = true;
				animator.SetBool("dead",true);
				this.gameObject.GetComponent<Collider2D>().enabled=false;
				Destroy(Lifebar_group);
				StartCoroutine("waitForDeath");
			}
	}

	IEnumerator waitForDeath()
	{
		yield return new WaitForSeconds (5);
		Destroy (gameObject);
	}

	[PunRPC] public void updateAnimation()
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
		facingRight = false;
	}

}