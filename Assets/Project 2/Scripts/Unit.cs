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
		IntegerLocation start = new IntegerLocation(transform.position);
		IntegerLocation end = new IntegerLocation (xLoc, yLoc);
		int dist = IntegerLocation.Distance (start, end);
		if (dist > moves) {
			//TODO: tell user target is too far for the unit
			return false;
		}

		Dictionary<IntegerLocation,IntegerLocation> result = FindPath (start, end);
		LinkedList<Vector2> path = new LinkedList<Vector2>();
		path.AddLast (new Vector2(end.x,end.y));
		if (result.ContainsKey (end)) {
			var current = result [end];
			while (current != (new IntegerLocation(-1,-1))) {
				path.AddFirst (new Vector2(current.x,current.y));
				current = result [current];
			}
		} else {
			//couldn't find a path to end
			return false;
		}

		foreach (Vector2 loc in path) {
			StartCoroutine(SmoothMovement(loc));
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
	private Dictionary<IntegerLocation, IntegerLocation> FindPath (IntegerLocation start, IntegerLocation end)
	{

		int moveArea = CalcMoveArea ();

		PriorityQueue<IntegerLocation> frontier = new PriorityQueue<IntegerLocation> (moveArea);
		frontier.Enqueue (start, 0);

		Dictionary<IntegerLocation,IntegerLocation> cameFrom = new Dictionary<IntegerLocation, IntegerLocation> ();
		Dictionary<IntegerLocation,int> costSoFar = new Dictionary<IntegerLocation, int> ();
		//invalid loc used as sentinel value to prevent loop overflow
		cameFrom[start] = new IntegerLocation(-1,-1);
		costSoFar[start] = 0;

		while (!frontier.Empty) {
			var current = frontier.Dequeue();

			if(current == end){
				cameFrom[end] = cameFrom[current];
				break;
			}
			foreach (IntegerLocation next in GetNeighbors(current)){
				int cost = costSoFar[current] + IntegerLocation.Distance(current, next);
				if(!costSoFar.ContainsKey(next)){
					costSoFar[next] = cost;
					int priority = cost + (int)(Mathf.Abs(current.x - next.x) + Mathf.Abs(current.y - next.y));
					frontier.Enqueue(next,priority);
					cameFrom[next] = current;
				}

			}

		}

		return cameFrom;
	}
	/*
	 * Get the list of <location>'s non-diagonal neighbors
	 * 
	 * Occupied neighbors will not be added to the list
	*/
	protected ArrayList GetNeighbors(IntegerLocation location)
	{
		ArrayList neighbors = new ArrayList ();
		for (int x = -1; x <= 1; x++) {
			for(int y = -1; y <= 1; y++){
				//Get only non-diagonal neighbors
				if(((x == -1 || x == 1) && y == 0) || ((y == -1 || y == 1) && x == 0)){
					Vector2 target = new Vector2(location.x+x,location.y+y);
					circleCollider.enabled = false;
					RaycastHit2D hit = Physics2D.Linecast(new Vector2(location.x,location.y),target,blockingLayer);
					circleCollider.enabled = true;
					if(hit.transform == null){
						neighbors.Add(new IntegerLocation(target));
					}
				}
			}
		}
		return neighbors;
	}

		

}
