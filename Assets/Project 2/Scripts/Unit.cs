using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Unit : Photon.MonoBehaviour {

	public float moveTime = 1f;
	public int moves;
	public int attackRange = 1;
	public LayerMask blockingLayer;
	public bool newAnimation;

	private ActionMenu actionMenu;
	private Rigidbody2D rb2D;
	private bool moved;
	private bool attacked;
	public Player myPlayer;

	protected CircleCollider2D circleCollider;
	protected float inverseMoveTime;
	public string myDirection;
	public bool stopped;

	private string [] directions = {"right", "up", "left" , "down" , "attack"};
	enum Facing  {right, up, left , down};
	//enum String  facing {"right", "up", "left" , "down"};
	public enum Action {move, attack, ability, wait};
	private float lastSynchronizationTime = 0f;
	private float syncDelay = 0f;
	private float syncTime = 0f;
	private Vector3 syncStartPosition = Vector3.zero;
	private Vector3 syncEndPosition = Vector3.zero;


	protected virtual void Start()
	{
		circleCollider = GetComponent<CircleCollider2D> ();
		rb2D = GetComponent<Rigidbody2D> ();
		inverseMoveTime = 1f / moveTime;
		moved = false;
		stopped = true;
		newAnimation = false;
		myDirection = directions [0];
		myPlayer = this.GetComponentInParent<Player> (); // gets the photon view of parent player class
		if (myPlayer == null) { // Player2 denotes enemy, still unused but can be changed later
			transform.gameObject.tag = "Player2";
			//transform.SetParent(GameObject.FindGameObjectWithTag("Player").transform);
			transform.SetParent(GameObject.FindGameObjectsWithTag("Player")[1].transform);
			myPlayer = this.GetComponentInParent<Player> ();
		}

		GameObject go = GameObject.FindGameObjectWithTag ("Canvas");
		actionMenu = (ActionMenu)go.transform.FindChild ("ActionMenu(Clone)").GetComponent<ActionMenu>();
	}

	void Update ()
	{
	
		// unused
	}

	public void StartAction(Action action)
	{
		switch (action) {
		case Action.move:
			StartCoroutine("WaitForMove");
			break;
		case Action.attack:
			StartCoroutine("WaitForAttack");
			break;
		case Action.ability:
		case Action.wait:
		default:
			break;
		}
	}

	void OnMouseDown()
	{

		if (!myPlayer.photonView.isMine) {
			Debug.Log ("This is not your unit to move buddy");
			return;
		}

		if (!myPlayer.ready) // player's team is not set up/ready, do not move selected unit
			return;

		if (myPlayer.photonView.isMine && myPlayer.myTurn.getTurn() == myPlayer.turn && !myPlayer.unitIsMoving) { // if it is player's unit and is player's turn and a unit is not already moving
			if ( myPlayer.photonView.isMine ) { //possibly not needed but good to have just in case
//				StartCoroutine ("WaitForMove");
				actionMenu.ShowMenu(this);
			}

		}
			
		else
			Debug.Log ("it is not my turn"); // if unit has not parent that means it is not the players unit	
	}

	/* // updated movement of unit over network (not used but possibly useful)
	[PunRPC] void updateMovement(Vector3 newPosition)
	{
		transform.position = newPosition;
	}
	*/

	protected bool Move(Dictionary<IntegerLocation,IntegerLocation> locations, IntegerLocation end)
	{
		IntegerLocation start = new IntegerLocation(transform.position);
		int dist = IntegerLocation.Distance (start, end);
		if (dist > moves) {
			//TODO: tell user target is too far for the unit
			return false;
		}

//		Dictionary<IntegerLocation,IntegerLocation> result = FindPath (start, end);
		LinkedList<Vector2> path = new LinkedList<Vector2>();
		path.AddLast (end.toVector2());
		if (locations.ContainsKey (end)) {
			var current = locations [end];
			while (current != (new IntegerLocation(-1,-1))) {
				path.AddFirst (current.toVector2());
				current = locations [current];
			}
		} else {
			//couldn't find a path to end
			return false;
		}
		/*
		foreach (Vector3 loc in path)
			transform.position = loc;
			*/
		StartCoroutine (SmoothMovement (path));
		//photonView.RPC("makeSmoothMovement", PhotonTargets.MasterClient, path);

		return true;
	}

	protected bool Attack(Dictionary<IntegerLocation, Unit> targets, IntegerLocation targetLoc)
	{
		if (targets.ContainsKey (targetLoc)) {
			return true;
		} else
			return false;
	}

	protected virtual IEnumerator SmoothMovement(LinkedList<Vector2> path) // using vector2
	{


		photonView.RPC("setStopped", PhotonTargets.AllBufferedViaServer, false);
		myPlayer.unitIsMoving = true;
		foreach (Vector3 loc in path) {
			newAnimation = true;

			if (rb2D.position.x > loc.x) // if next location is left of starting location
				photonView.RPC("setDirection", PhotonTargets.AllBufferedViaServer, directions[2]);
				
			if ( rb2D.position.x < loc.x ) // right
				photonView.RPC("setDirection", PhotonTargets.AllBufferedViaServer, directions[0]);

			if ( rb2D.position.y > loc.y ) // down
				photonView.RPC("setDirection", PhotonTargets.AllBufferedViaServer, directions[3]);

			if ( rb2D.position.y < loc.y ) // up
				photonView.RPC("setDirection", PhotonTargets.AllBufferedViaServer, directions[1]);

			//Debug.Log(myDirection);
			




			float sqrRemainingDistance = (transform.position - loc).sqrMagnitude;

			while (sqrRemainingDistance > float.Epsilon) {

				Vector3 newPosition = Vector3.MoveTowards (rb2D.position, loc, inverseMoveTime * Time.deltaTime);
				transform.position = newPosition;
				sqrRemainingDistance = (transform.position - loc).sqrMagnitude;
				yield return new WaitForSeconds(.01f);
				yield return null;
			}
		}
		myPlayer.photonView.RPC("makingMove", PhotonTargets.AllBuffered); // player has made a move update the turnmanager on the server
		photonView.RPC("setStopped", PhotonTargets.AllBufferedViaServer, true); // update animation on server to stop animating when stopped
		yield return new WaitForSeconds(.1f);
		newAnimation = false;
		myPlayer.unitIsMoving = false;
	}

	[PunRPC] public void makeSmoothMovement(LinkedList<Vector2> path)
	{
		StartCoroutine (SmoothMovement (path));
	}

	[PunRPC] public void setDirection(string newDirection)
	{
		Debug.Log ("new direction: " + newDirection);
		myDirection = newDirection;
	}

	[PunRPC] public void setStopped(bool hasStopped)
	{
		Debug.Log ("has unit stopped ?: " + hasStopped);
		stopped = hasStopped;
	}

	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting) {
			stream.SendNext (GetComponent<Rigidbody2D> ().transform.position);
		}
		
		else {
			
			syncEndPosition = (Vector3)stream.ReceiveNext();
			syncStartPosition = GetComponent<Rigidbody2D>().transform.position;
			syncTime = 0f;
			syncDelay = Time.time - lastSynchronizationTime;
			lastSynchronizationTime = Time.time;
			
			//GetComponentInChildren<Rigidbody2D>().transform.position = (Vector3)stream.ReceiveNext();
		}
		
	}

	private void SyncedMovement()
	{
		syncTime += Time.deltaTime;
		GetComponent<Rigidbody2D>().transform.position = Vector3.Lerp(syncStartPosition, syncEndPosition, syncTime / syncDelay);
	}

	protected IEnumerator WaitForMove ()
	{
		//Debug.Log ("waiting for move target");
		yield return new WaitForSeconds (.1f);
		var validMoves = FindPath(new IntegerLocation(transform.position));
		HighlightMoveArea(validMoves, Color.cyan);
		moved = false;

		while (!moved) {
			if (Input.GetMouseButtonDown (0))
			{
				var target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				moved = Move(validMoves,new IntegerLocation (target));
			}	
			yield return null;
		}

		HighlightMoveArea (validMoves, Color.white);
		
	}

	protected IEnumerator WaitForAttack()
	{
		yield return new WaitForSeconds (.1f);
		var validTargets = FindTargets(attackRange, true);
		HighlightTargets(validTargets, Color.red);
		attacked = false;

		while (!attacked) {
			if (Input.GetMouseButtonDown (0))
			{
				var target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				attacked = Attack(validTargets,new IntegerLocation (target));
			}	
			yield return null;
		}
		
		HighlightTargets (validTargets, Color.white);
	}

	/*	--may not be needed anymore
	 *  calculates the area of the circle that corresponds to
		the unit's movement radius 
	 */
/*	private int CalcMoveArea()
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
*/
	/* Finds path with A* algorithm
	*  Heuristic Function: Manhattan Distance
	* 		H(a,b) = |a.x - b.x| + |a.y - b.y|
	* 
	*/
	private Dictionary<IntegerLocation, IntegerLocation> FindPath (IntegerLocation start)
	{

		Queue<IntegerLocation> frontier = new Queue<IntegerLocation> ();
		ArrayList discovered = new ArrayList();
		frontier.Enqueue (start);
		discovered.Add(start);

		Dictionary<IntegerLocation,IntegerLocation> cameFrom = new Dictionary<IntegerLocation, IntegerLocation> ();

		//invalid loc used as sentinel value to prevent loop overflow
		cameFrom[start] = new IntegerLocation(-1,-1);

		while (frontier.Count > 0) {
			var current = frontier.Dequeue();

			if(Mathf.CeilToInt(Vector2.Distance(start.toVector2(),current.toVector2())) >= moves){
				return cameFrom;
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
	
	void HighlightMoveArea(Dictionary<IntegerLocation, IntegerLocation> area, Color color)
	{
		GameObject[] board = GameObject.FindGameObjectsWithTag ("Floor");
		foreach(GameObject loc in board)
		{
			if(area.ContainsKey(new IntegerLocation(loc.transform.position))){
				var img = loc.GetComponent<SpriteRenderer>();
				img.color = color;
			}
		}
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

	private Dictionary<IntegerLocation,Unit> FindTargets(int range, bool hostile)
	{
		GameObject[] unitObjects;
		if (hostile)
			unitObjects = GameObject.FindGameObjectsWithTag ("Player2");
		else
			unitObjects = GameObject.FindGameObjectsWithTag("Player1");

		ArrayList units = new ArrayList ();
		foreach (GameObject go in unitObjects) {
			units.Add((Unit)go.GetComponent<Unit>());
		}


		Dictionary<IntegerLocation, Unit> targets = new Dictionary<IntegerLocation, Unit> ();
		//Get enemies for attacks, friends for heals

		foreach (Unit u in units){
			if(Mathf.CeilToInt(Vector2.Distance(transform.position,u.transform.position)) <= range)
			{
				targets.Add(new IntegerLocation(u.transform.position),u);
			}
		}
		return targets;
	}

	void HighlightTargets(Dictionary<IntegerLocation, Unit> targets, Color color)
	{
		GameObject[] board = GameObject.FindGameObjectsWithTag ("Floor");
		foreach(GameObject loc in board)
		{
			if(targets.ContainsKey(new IntegerLocation(loc.transform.position))){
				var img = loc.GetComponent<SpriteRenderer>();
				img.color = color;
			}
		}
	}
}
