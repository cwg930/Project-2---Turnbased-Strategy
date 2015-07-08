using UnityEngine;
using System.Collections;

public class Knight : Unit {
	private Transform player; 

	public GameObject grass;

	// Use this for initialization
	protected override void Start () {
		player = GameObject.FindGameObjectWithTag("Player").transform;
		base.Start ();
	}
	
	// Update is called once per frame
	void Update () {

		//grassMove ();
		quickMove ();
	
	}

	void grassMove()
	{
		if (Input.GetMouseButtonDown (0)) {

			Debug.Log("Mouse is down");

			RaycastHit hitInfo = new RaycastHit();
			bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);
		

			if (hit) 
			{
				Debug.Log("Hit " + hitInfo.transform.gameObject.name);
				if (hitInfo.transform.gameObject.tag == "Construction")
				{
					Debug.Log ("It's working!");
				}
			}

			Vector3 clickpos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			clickpos.z = 0;


		}



	}

	void quickMove()
	{
		if (Input.GetMouseButtonDown (0)) {

			Vector3 new_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			new_pos.z = player.position.z;

<<<<<<< HEAD
			player.position = new_pos;
=======
			new_pos.x = Mathf.Round(new_pos.x / 1) * 1;
			new_pos.y = Mathf.Round(new_pos.y / 1) * 1;
>>>>>>> origin/knight-features



<<<<<<< HEAD
			Vector2 myMove = getPosition(horizontal, vertical);
			//player.position = myMove;
=======
			player.position = new_pos;
>>>>>>> origin/knight-features

			//base.Move (horizontal2, vertical2); // move along path to target
		}
	}

	private void OnTriggerEnter2D (Collider2D other)
	{
			
	}

	Vector2 getGridPosition (int x, int y)
	{
		// start pos = 288, 21 end pos = 586, 233
		int startx = 288;
		int starty = 21;
		int endx = 586;
		int endy = 233;
		int spriteSize = 21;
		int rows = 14;
		int cols = 10;
		Vector2 curPos = new Vector2 ();

		if (x < startx || y < starty || x > endx || y > endy)
			return curPos; // out of bounds

		else {
		
			for (int i=0; i<=rows; i++)
			{
				if (x > (i*spriteSize + startx) && x < ((i+1)*spriteSize + startx) )
					curPos.x = i;
			}
			for (int j=0; j<=cols; j++)
			{

				if (y > (j*spriteSize + starty) && y < ((j+1)*spriteSize + starty) )
					curPos.y = j;
				
			}
		}

		return curPos;

	}



	IEnumerable wait ()
	{
		yield return new WaitForSeconds (1);
	}
}
