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
		int dist = Vector2.Distance (start, end);
		if (dist > moves) {
			//tell user it's too far
			return false;
		}

		ArrayList path = FindPath (start, end);
		Vector2 current = path [start];
		foreach (Vector2 next in path) {
			StartCoroutine(SmoothMovement(next));

		}
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

	//Finds path with A* algorithm
	private ArrayList FindPath (Vector2 start, Vector2 end)
	{

		int moveArea = CalcMoveArea ();

		PriorityQueue<Vector2> frontier = new PriorityQueue<Vector2> (moveArea);
		frontier.Insert (start, 0);

		ArrayList prev_loc = new ArrayList();
		int[] currentCost = new int[moveArea];
		prev_loc [start] = null;
		currentCost [start] = 0;

		while (!frontier.IsEmpty()) {
			Vector2 current = frontier.Remove();

			if(current == end)
				break;

			foreach (Vector2 next in GetNeighbors(current)){
				int cost = currentCost[current] + Vector2.Distance(current, next);
				if(currentCost[next] == null || cost < currentCost[next]){
					currentCost[next] = cost;
					int priority = cost + (Mathf.Abs(current.x - next.x) + Mathf.Abs(current.y - next.y));
					frontier.Insert(next,priority);
					prev_loc[next]  = current;
				}

			}

		}

		return prev_loc;
	}
	//TODO: Check for occupied neighbors that block the path
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
