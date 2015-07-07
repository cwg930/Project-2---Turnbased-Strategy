using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Unit : MonoBehaviour {

	public float moveTime = 0.1f;
	public int moves;
	public LayerMask blockingLayer;

	private CircleCollider2D circleCollider;
	private Rigidbody2D rb2D;
	private float inverseMoveTime;

	enum Facing  {right, up, left , down};

	protected virtual void Start()
	{
		circleCollider = GetComponent<CircleCollider2D> ();
		rb2D = GetComponent<Rigidbody2D> ();
		inverseMoveTime = 1f / moveTime;
	}
	
	protected bool Move(int xLoc, int yLoc)
	{
		Vector2 start = transform.position;
		Vector2 end = new Vector2 (xLoc, yLoc);
		int dist = (int) Vector2.Distance (start, end);
		if (dist > moves) {
			//TODO: tell user target is too far for the unit
			return false;
		}

		LinkedList<Vector2> path = FindPath (start, end);
		foreach (Vector2 next in path) {
			StartCoroutine(SmoothMovement(next));

		}

		return true;
	}
	
	protected IEnumerator SmoothMovement(Vector3 end)
	{
		float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

		while (sqrRemainingDistance > float.Epsilon) {
			Vector3 newPosition = Vector3.MoveTowards(rb2D.position, end, inverseMoveTime * Time.deltaTime);
			rb2D.MovePosition(newPosition);
			sqrRemainingDistance = (transform.position - end).sqrMagnitude;

			yield return null;
		}
	}
	/*	calculates the area of the circle that corresponds to
		the unit's movement radius
	 */
	private int CalcMoveArea()
	{
		//straight line distance all around plus origin
		int area = 4 * moves + 1;
		int temp = 0;
		//the num of possible locs for each quadrant if you turn is the sum of numbers up to move radius
		for (int i = 0; i < moves; i++) {
			temp += i;
		}
		area += 4 * temp;

		return area;
	}

	/* Finds path with A* algorithm
	*  Heuristic Function: Manhattan Distance
	* 		H(a,b) = |a.x - b.x| + |a.y - b.y|
	* 
	*/
	private LinkedList<Vector2> FindPath (Vector2 start, Vector2 end)
	{

		int moveArea = CalcMoveArea ();

		PriorityQueue<Vector2> frontier = new PriorityQueue<Vector2> (moveArea);
		frontier.Insert (start, 0);

		LinkedList<Vector2> path = new LinkedList<Vector2>();
		int[] currentCost = new int[moveArea];
		path.AddFirst(null);
		currentCost[0] = 0;

		while (!frontier.IsEmpty()) {
			Tuple<Vector2,int> current = frontier.RemoveWithPriority();

			if(current.first.Equals(end))
				break;

			foreach (Vector2 next in GetNeighbors(current.first)){
				int cost = current.second + (int)Vector2.Distance(current.first, next);


			}

		}

		return path;
	}
	/*
	 * Get the list of <location>'s non-diagonal neighbors
	 * 
	 * TODO: Check for occupied neighbors that block the path
	*/
	protected ArrayList GetNeighbors(Vector2 location)
	{
		ArrayList neighbors = new ArrayList ();
		for (int x = -1; x <= 1; x++) {
			for(int y = -1; y <= 1; y++){
				//Get only non-diagonal neighbors
				if(((x == -1 || x == 1) && y == 0) || ((y == -1 || y == 1) && x == 0))
					neighbors.Add(new Vector2(location.x+x,location.y+y));
			}
		}
		return neighbors;
	}

		

}
