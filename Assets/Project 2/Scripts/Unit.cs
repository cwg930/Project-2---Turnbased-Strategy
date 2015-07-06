using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Unit : MonoBehaviour {

	public int moves;
	public LayerMask blockingLayer;

	private CircleCollider2D circleCollider;
	private Rigidbody2D rb2D;

	protected virtual void Start()
	{
		circleCollider = GetComponent<CircleCollider2D> ();
		rb2D = GetComponent<Rigidbody2D> ();
	}
	
	protected bool Move(int xLoc, int yLoc)
	{
		Vector2 start = transform.position;
		Vector2 end = new Vector2 (xLoc, yLoc);
		int dist = Vector2.Distance (start, end);
		if (dist > moves) {
			//tell user it's too far
			return false;
		}

		ArrayList path = FindPath (start, end);
	}

	//Finds path with A* algorithm
	private ArrayList FindPath (Vector2 start, Vector2 end)
	{
		List<Vector2> priorityQueue = new List<Vector2> ();
	}


}
