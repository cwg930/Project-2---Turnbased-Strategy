using UnityEngine;
using System.Collections;

public class Knight : Unit {

	protected override void Start () {

		base.Start ();
	}
	
	// Update is called once per frame

	void Update () {

		makeMove ();
	}

	void OnMouseDown()
	{
		/*bool heroSelected = false;

		while (!heroSelected) {
			var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if(Physics.Raycast(ray,out hit)){
				Debug.Log("Hero was hit by ray");
				player = hit.collider.gameObject.transform;
				heroSelected = true;
			}
		
		} */

		Debug.Log ("Unit was clicked");
		//StartCoroutine ("wait");

	}

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
