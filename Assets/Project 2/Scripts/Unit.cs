using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Unit : Photon.MonoBehaviour {

	public float moveTime = 1f;
	public int moves;
	public LayerMask blockingLayer;
	public bool newAnimation;
	public int attackRange;

	private ActionMenu actionMenu;
	private Rigidbody2D rb2D;
	private bool moved;
	public Player myPlayer;

	private bool hasMoved;
	private bool hasAttacked;


	protected CircleCollider2D circleCollider;
	protected float inverseMoveTime;
	public string myDirection;
	public bool stopped;
	private bool isdead;

	private string [] directions = {"right", "up", "left" , "down" , "attack"};
	enum Facing  {right, up, left , down};
	//enum String  facing {"right", "up", "left" , "down"};
	public enum Action {move, attack, ability, wait};
	private float lastSynchronizationTime = 0f;
	private float syncDelay = 0f;
	private float syncTime = 0f;
	private Vector3 syncStartPosition = Vector3.zero;
	private Vector3 syncEndPosition = Vector3.zero;

	public bool isAttacking;
	private int attackerDamage;

	public int attackValue;
	public int healthValue;
	private int startingHealth;

	public bool selected;
	public bool unitHighlight;

	//About progress-bar --
	private float aux_d=0;//Initial lifebar width
	public GameObject Lifebar;
	public GameObject Lifebar_group;
	private Vector3 initial_localscale;
	//private float Lifebar_distance_y=0;
	//private float Lifebar_back_distance_y=0;
	//--


	protected virtual void Start()
	{
		//Lifebar_distance_y = this.transform.position.y-Lifebar.transform.position.y;
		aux_d = Lifebar.GetComponent<Renderer>().bounds.size.x;//lifebar width
		initial_localscale = new Vector3 (Lifebar.transform.localScale.x,Lifebar.transform.localScale.y,Lifebar.transform.localScale.z);
		//--
		circleCollider = GetComponent<CircleCollider2D> ();
		rb2D = GetComponent<Rigidbody2D> ();
		inverseMoveTime = 1f / moveTime;
		moved = true;
		isAttacking = false;
		attackerDamage = 0;
		hasMoved = false;
		hasAttacked = false;
		isdead = false;
		stopped = true;
		selected = false;

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

		//healthValue = 50; //TODO read in health value by calculating from level
		startingHealth = healthValue;
	}

	public bool getDeathStatus()
	{
		return isdead;
	}

	public int getHealth()
	{
		return healthValue;
	}

	public void StartAction(Action action, GameObject actionMenu)
	{
		switch (action) {
		case Action.move:
			if (hasMoved)
				Debug.Log("you have already moved this unit");
			else if (isAttacking)
				Debug.Log("you must finish your attack before you can make a move");
			else
				StartCoroutine("WaitForMove");
			break;
		case Action.attack:
			if (!moved)
				Debug.Log("you must finish your move before you can make an attack");

			else if (hasAttacked)
				Debug.Log("you have already attacked this turn");
				else
					StartCoroutine("WaitForAttack");

			break;
		case Action.ability:
		case Action.wait:
			if (myPlayer.myTurn.gameOver)
				actionMenu.SetActive(false);
			else if (!moved)
				Debug.Log("you must finish your move before you can end your turn");
			else if (isAttacking)
				Debug.Log("you must finish your attack before you can end your turn");
			else{
				myPlayer.photonView.RPC("makingMove", PhotonTargets.AllBuffered); // player has made a move update the turnmanager on the server
				hasMoved = false;
				hasAttacked = false;
				myPlayer.unitHasMoved = false;
				myPlayer.unitHasAttacked = false;
				myPlayer.isActionMenuActive = false;

				actionMenu.SetActive(false);
				selected = false;
				HighlightUnit(Color.white);
				myPlayer.unitSelected = false;
				unitHighlight = false;
			}

			break;
		default:
			break;
		}
	}

	protected IEnumerator WaitForAttack ()
	{

		//Debug.Log ("waiting for move target");
		yield return new WaitForSeconds (.1f);
		var validTargets = FindTargets(attackRange, true);
		HighlightTargets(validTargets, Color.red);
		isAttacking = true;
		myPlayer.unitHasAttacked = true;

		if (hasAttacked)
		{
			Debug.Log("you have already attacked this turn");
			HighlightTargets (validTargets, Color.white);
			isAttacking = false;
		}
		
		while (isAttacking) {
			if (Input.GetMouseButtonDown (0))
			{
				Vector3 pos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
				RaycastHit2D hit = Physics2D.Raycast(pos, transform.position);
				int distance = (int)Vector3.Distance(transform.position,pos);
				if (distance > attackRange)
				{
					Debug.Log("this attack is too far" + distance);
					HighlightTargets (validTargets, Color.white);
					isAttacking = false;
					yield return null;
				}



				else if (hit.collider != null && attackRange >= distance && hit.collider.transform.tag.Equals("Player2"))
				{
					Debug.Log("this unit ( " + transform.name + ") is attacking enemy unit (" + hit.collider.transform.name + ") hit is being confirmed" );
					//hit.collider.gameObject.GetComponent<Unit>().healthValue -= attackValue;
					photonView.RPC("damageEnemy", PhotonTargets.AllBufferedViaServer, hit.collider.transform.position);
					Debug.Log("enemy selected" );
					HighlightTargets (validTargets, Color.white);
					HighlightUnit(Color.yellow);
					photonView.RPC("setAttackAnimation", PhotonTargets.AllBufferedViaServer);
					hasAttacked = true;
					isAttacking = false;
				}
				else{
					Debug.Log("you have clicked on something within your range that is not an enemy unit");
					HighlightTargets (validTargets, Color.white);
				}

				//moved = Move(validMoves,new IntegerLocation (target));
			}	
			yield return null;
		}
		yield return new WaitForSeconds (1);
		photonView.RPC("stopAttackAnimation", PhotonTargets.AllBufferedViaServer);

		
	}


	[PunRPC] public void damageEnemy(Vector3 enemyPosition)
	{
		RaycastHit2D hit = Physics2D.Raycast(enemyPosition, transform.position);
		if (hit.collider != null) {
			Unit enemyUnit = hit.collider.gameObject.GetComponent<Unit>();
			Debug.Log("enemy is being damaged" + gameObject.name);
			hit.collider.gameObject.GetComponent<Unit>().healthValue -= attackValue;
			enemyUnit.attackerDamage = attackValue;
			enemyUnit.Life_Down();
			if (hit.collider.gameObject.GetComponent<Unit>().healthValue <= 0)
			{

				enemyUnit.isdead = true;

				if (myPlayer.myTurn.gameOver )
					HighlightUnit(Color.white);

			//	Animator enemyAnimator = hit.collider.gameObject.GetComponent<Animator>();
			//	StartCoroutine (waitForDeath(enemyAnimator));
			}
				
		}
			
	}


	[PunRPC] public void setAttackAnimation()
	{
		Animator animator = GetComponent<Animator> ();
		if (myDirection.Equals ("down")) {
			animator.SetBool ("attack", true);
			animator.SetBool ("down_attack", true);
		}
			
		else if (myDirection.Equals("up"))
			animator.SetBool ("up_attack", true);
		else
			animator.SetBool ("attack", true);
	}

	[PunRPC] public void stopAttackAnimation()
	{
		Animator animator = GetComponent<Animator> ();
		animator.SetBool ("attack", false);
		animator.SetBool ("down_attack", false);
		animator.SetBool ("up_attack", false);
	}


	void OnMouseDown()
	{

		if (!myPlayer.photonView.isMine) {
			Debug.Log ("This is not your unit to move buddy");
			return;
		}


		if (!myPlayer.ready) // player's team is not set up/ready, do not move selected unit
			return;

		if (myPlayer.photonView.isMine && myPlayer.myTurn.getTurn () == myPlayer.turn && !myPlayer.unitIsMoving && (myPlayer.unitHasMoved || myPlayer.unitHasAttacked)) {
			Debug.Log ("You cannot select another unit until you are done with this one");
			return;
		}
		// !myPlayer.isActionMenuActive //old bool for menu now just use attack and move bools

		// if it is player's unit and is this player's turn and any unit is not already moving, you can select the unit as long as they havent moved or attacked
		else if (myPlayer.photonView.isMine && myPlayer.myTurn.getTurn () == myPlayer.turn && !myPlayer.unitIsMoving && !myPlayer.unitHasMoved && !myPlayer.unitHasAttacked && !myPlayer.unitSelected) { 
			Debug.Log("selecting new unit");
//				StartCoroutine ("WaitForMove");
			//HighlightUnit (Color.yellow);
			selected = true;
			actionMenu.ShowMenu (this);
			myPlayer.isActionMenuActive = true;
			myPlayer.unitSelected = true;
		}

		//selecting a new unit
		else if (myPlayer.photonView.isMine && myPlayer.myTurn.getTurn () == myPlayer.turn && !myPlayer.unitIsMoving && !myPlayer.unitHasMoved && !myPlayer.unitHasAttacked && myPlayer.unitSelected) {
			Debug.Log("selecting another unit");
			myPlayer.DeSelectUnit();
			selected = true;
			actionMenu.ShowMenu (this);
			myPlayer.isActionMenuActive = true;
			myPlayer.unitSelected = true;
		}
			
		else
			Debug.Log ("it is not my turn"); // if unit has not parent that means it is not the players unit	
	}

	public void HighlightUnit(Color color)
	{
		GameObject[] board = GameObject.FindGameObjectsWithTag ("Floor");

		foreach(GameObject loc in board)
		{
			if(gameObject.transform.position == loc.transform.position){
				var img = loc.GetComponent<SpriteRenderer>();
				img.color = color;
			}
		}
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
		hasMoved = true;
		photonView.RPC("setStopped", PhotonTargets.AllBufferedViaServer, true); // update animation on server to stop animating when stopped
		yield return new WaitForSeconds(.1f);
		newAnimation = false;
		myPlayer.unitIsMoving = false;
		HighlightUnit(Color.yellow);
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
		var validMoves = FindPath(new IntegerLocation(transform.position), moves);
		HighlightMoveArea(validMoves, Color.cyan);
		moved = false;
		myPlayer.unitHasMoved = true;
		if (hasMoved) {
			moved = true;
			Debug.Log("you have already moved this turn");
		}

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

/*	protected IEnumerator WaitForAttack()
	{
		yield return new WaitForSeconds (.1f);
		var validTargets = FindTargets(attackRange);
		HighlightMoveArea(validMoves, Color.red);
		
		while (!moving) {
			if (Input.GetMouseButtonDown (0))
			{
				var target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				//moving = Move(validMoves,new IntegerLocation (target));
			}	
			yield return null;
		}
		
		HighlightMoveArea (validMoves, Color.white);
	}
*/
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
	private Dictionary<IntegerLocation, IntegerLocation> FindPath (IntegerLocation start, int limit)
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

			if(Mathf.CeilToInt(Vector2.Distance(start.toVector2(),current.toVector2())) >= limit){
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

	//--About_Lifebar--
	void Life_Down(){
		float aux_c = Lifebar.GetComponent<Renderer>().bounds.size.x;
		float divisor = startingHealth / attackerDamage;
		float newLifebarLength = aux_c;
		newLifebarLength -= aux_d / divisor;
		if (newLifebarLength < float.Epsilon)
			Lifebar.transform.localScale = new Vector3(0, 0, 0);
		else
			Lifebar.transform.localScale -= new Vector3(aux_d / divisor, 0, 0);// takes percentage based on damage
		//Lifebar.transform.localScale += new Vector3(-aux_d/divisor, 0, 0);// takes percentage based on damage
		
		Vector3 auxve = new Vector3(Lifebar.transform.position.x,Lifebar.transform.position.y,Lifebar.transform.position.z);
		auxve.x=auxve.x-(aux_c - Lifebar.GetComponent<Renderer>().bounds.size.x)/2;
		Lifebar.transform.position=auxve;
	}
	
	void Healing_(){
		if(isdead==false){
			if(Lifebar.transform.localScale.x>0){
				float aux_c = Lifebar.GetComponent<Renderer>().bounds.size.x;
				if(Lifebar.transform.localScale.x +aux_d/8 >= initial_localscale.x){
					//Debug.Log(initial_localscale + " - " + Lifebar.transform.localScale.x +aux_d/4);
					Lifebar.transform.localScale = new Vector3(initial_localscale.x, initial_localscale.y, initial_localscale.z);
				}else{
					Lifebar.transform.localScale += new Vector3(+aux_d/8, 0, 0);// 1/4 part
				}
				Vector3 auxve = new Vector3(Lifebar.transform.position.x,Lifebar.transform.position.y,Lifebar.transform.position.z);
				auxve.x=auxve.x-(aux_c - Lifebar.GetComponent<Renderer>().bounds.size.x)/2;
				Lifebar.transform.position=auxve;
			}
		}
		//is_healing=false;
	}
	
	//--



}
