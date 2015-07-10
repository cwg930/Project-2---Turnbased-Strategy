using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Unit : MonoBehaviour {

	public float moveTime = 0.1f;
	public int moves;
	public LayerMask blockingLayer;
	public Transform player;
	public bool moved;


	private Rigidbody2D rb2D;
	private int rows = BoardManager.rows;
	private int cols = BoardManager.columns;
	private bool moving;
	/*private GameObject [] Team1;
	private GameObject [] Team2;
	private int teamIndex; */

	protected CircleCollider2D circleCollider;
	protected float inverseMoveTime;

	enum Facing  {right, up, left , down};

	protected virtual void Start()
	{
		circleCollider = GetComponent<CircleCollider2D> ();
		rb2D = GetComponent<Rigidbody2D> ();
		inverseMoveTime = 1f / moveTime;
		moving = false;
		player = GameObject.FindGameObjectWithTag("Player1").transform;
		
		/*Team1 = GameObject.FindGameObjectsWithTag ("Player1");
		teamIndex = 0;*/
		
		//Debug.Log (Team1[0].transform.name);
		
		
		//player = Team1[0].transform;
	}

	void OnMouseDown()
	{
		Debug.Log ("Mouse Clicked");
		moved = false;
		StartCoroutine ("Wait");
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

		StartCoroutine (SmoothMovement (path));

		return true;
	}

	public void makeMove() 
	{
		//StartCoroutine(Wait (1));
		Debug.Log ("making move");
		/*Input.GetMouseButtonDown (0)*/
		if (!moving) {
			Vector3 new_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			new_pos.z = player.position.z;
			new_pos.x = Mathf.Round(new_pos.x / 1) * 1;
			new_pos.y = Mathf.Round(new_pos.y / 1) * 1;
			
			if (new_pos.x < 0 || new_pos.y < 0 || new_pos.x >= cols || new_pos.y >= rows) // stays within bounds
			{
				Debug.Log ("move was out of bounds, returning...");
				return;
			}

			/*	player = Team1[teamIndex].transform; 
			teamIndex++;
			if (teamIndex == Team1.Length)
				teamIndex = 0; */
			
			//Move ((int)new_pos.x, (int)new_pos.y); //used when move function works properly
			//StartCoroutine (SmoothMovement (new_pos));
			var tmp = new IntegerLocation(new_pos);
			moved = Move(tmp.x,tmp.y);
		}
	}
	
	protected virtual IEnumerator SmoothMovement(LinkedList<Vector2> path) // using vector2
	{

		foreach (Vector3 loc in path) {

			float sqrRemainingDistance = (transform.position - loc).sqrMagnitude;

			while (sqrRemainingDistance > float.Epsilon) {
				Vector3 newPosition = Vector3.MoveTowards (rb2D.position, loc, inverseMoveTime * Time.deltaTime);
				rb2D.MovePosition (newPosition);
				sqrRemainingDistance = (transform.position - loc).sqrMagnitude;

				yield return null;
			}
		}
	}

	protected virtual IEnumerator SmoothMovement(Vector3 path) // using vector3
	{
		moving = true;
			
			float sqrRemainingDistance = (transform.position - path).sqrMagnitude;
			
			while (sqrRemainingDistance > float.Epsilon) {
				Vector3 newPosition = Vector3.MoveTowards (rb2D.position, path, inverseMoveTime * Time.deltaTime);
				rb2D.MovePosition (newPosition);
				sqrRemainingDistance = (transform.position - path).sqrMagnitude;
				
				yield return null;
			}
		moving = false;
		Debug.Log ("move made");
	}

	protected IEnumerator Wait ()
	{
		Debug.Log ("waiting");
		yield return new WaitForSeconds (.1f);


		while (!moved) {
			if (Input.GetMouseButtonDown (0))
			{
				makeMove ();
			}
				
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

		Queue<IntegerLocation> frontier = new Queue<IntegerLocation> (moveArea);
		ArrayList discovered = new ArrayList();
		frontier.Enqueue (start);
		discovered.Add(start);

		Dictionary<IntegerLocation,IntegerLocation> cameFrom = new Dictionary<IntegerLocation, IntegerLocation> ();

		//invalid loc used as sentinel value to prevent loop overflow
		cameFrom[start] = new IntegerLocation(-1,-1);

		while (frontier.Count > 0) {
			var current = frontier.Dequeue();

			if(Mathf.CeilToInt(Vector2.Distance(start.toVector2(),current.toVector2())) >= moves){
				return null;
			}

			if(current == end){
				cameFrom[end] = cameFrom[current];
				break;
			}
			foreach (IntegerLocation next in GetNeighbors(current)){
				if(!discovered.Contains(next)){
					frontier.Enqueue(next);
					discovered.Add(next);
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

	/*public abstract Unit getUnitType <T> ()
		where T : Component;

	public Knight getKnight()
	{
		return new Knight ();
	} */

		

}
