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
		frontier.Enqueue (start, 0);

		LinkedList<Vector2> cameFrom = new LinkedList<Vector2> ();
		Dictionary<Vector2,int> costSoFar = new Dictionary<Vector2, int> ();
		cameFrom.AddFirst(start);
		costSoFar[start] = 0;

		while (!frontier.Empty) {
			Vector2 current = frontier.Dequeue();

			if(current.Equals(end))
				break;

			foreach (Vector2 next in GetNeighbors(current)){
				int cost = costSoFar[current] + (int)Vector2.Distance(current, next);
				if(!costSoFar.ContainsKey(next) || cost < costSoFar[next]){
					costSoFar[next] = cost;
					int priority = cost + (int)(Mathf.Abs(current.x - next.x) + Mathf.Abs(current.y - next.y));
					frontier.Enqueue(next,priority);
					cameFrom.AddLast(current);
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
	protected ArrayList GetNeighbors(Vector2 location)
	{
		ArrayList neighbors = new ArrayList ();
		for (int x = -1; x <= 1; x++) {
			for(int y = -1; y <= 1; y++){
				//Get only non-diagonal neighbors
				if(((x == -1 || x == 1) && y == 0) || ((y == -1 || y == 1) && x == 0)){
					Vector2 target = new Vector2(location.x+x,location.y+y);
					circleCollider.enabled = false;
					RaycastHit2D hit = Physics2D.Linecast(location,target,blockingLayer);
					circleCollider.enabled = true;
					if(hit.transform == null){
						neighbors.Add(target);
					}
				}
			}
		}
		return neighbors;
	}

		

}
