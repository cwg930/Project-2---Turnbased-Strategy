using UnityEngine;
using System.Collections;

public class Player : MovingObject {

	private int health;

	private Animator animator;
	
	// Use this for initialization
	protected override void Start () 
	{
		animator = GetComponent<Animator> ();
		health = 100;
		base.Start();

			//animator = GetComponent<Animator> ();

	}
	
	// Update is called once per frame
	void Update () {

		if (!GameManager.instance.player1Turn)
			return;


		int horizontal = 0;
		int vertical = 0;
		
		horizontal = (int)Input.GetAxisRaw ("Horizontal");
		vertical = (int)Input.GetAxisRaw ("Vertical");
		
		if (horizontal != 0)
			vertical = 0;
		else if (vertical != 0)
			horizontal = 0;
		
		if (horizontal != 0 || vertical != 0)
			AttemptMove<Player> (horizontal, vertical);



	
	}

	protected override void AttemptMove <T> (int xDir, int yDir)
	{
		base.AttemptMove <T> (xDir, yDir);
		RaycastHit2D hit;
		//if (Move (xDir, yDir, out hit))
		//	SoundManager.instance.RandomizeSfx (moveSound1, moveSound2);
		//CheckIfGameOver();
		GameManager.instance.player1Turn = false;
		
	}

	protected override void OnCantMove <T> (T component)
	{
		animator.SetTrigger ("dead");
		return;
	}

}
