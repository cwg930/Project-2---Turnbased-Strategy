using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : Photon.MonoBehaviour {

	private string playerID;
	private GameObject [] units;
	private int cols = BoardManager.columns;
	private int rows = BoardManager.rows;

	private NetworkManager networkManager;
	private UserData userData;

	private bool selectedLocation; // checks to see if you have chosen a placement for spawned unit
	public bool ready; // checks to see if you have spawned all units and are ready to start the game
	private int unitChoice; // keeps track of which unit is chosen
	public bool unitIsMoving;
	public bool isActionMenuActive;

	public GameObject blueKnight;
	public GameObject greenKnight;
	public GameObject redKnight;

	public GameObject bluePaladin;
	public GameObject greenPaladin;
	public GameObject redPaladin;

	public GameObject blueMage;
	public GameObject greenMage;
	public GameObject redMage;

	public GameObject blueRogue;
	public GameObject greenRogue;
	public GameObject redRogue;

	public GameObject[] unitPrefabs;
		
	[HideInInspector]	public TurnManager myTurn;
	public int turn;

	public bool lostGame;
	public bool wonGame;
	private int StartingUnitCount;
	private int DeadUnitCount;

	public bool unitHasMoved;
	public bool unitHasAttacked;

	public bool unitSelected;

	private bool gainedExp = false;


	/*
	private float lastSynchronizationTime = 0f;
	private float syncDelay = 0f;
	private float syncTime = 0f;
	private Vector2 syncStartPosition = Vector2.zero;
	private Vector2 syncEndPosition = Vector2.zero;
	*/

	void Start()
	{
		photonView.RPC("setMyParent", PhotonTargets.AllBuffered);
		turn = photonView.owner.ID;
		Debug.Log ("Player " + turn);
		units = new GameObject[6];
		selectedLocation = true;
		ready = false;
		unitChoice = 0;
		unitIsMoving = false;
		lostGame = false;
		StartingUnitCount = 0;
		DeadUnitCount = 0;
		isActionMenuActive = false;
		unitHasMoved = false;
		unitHasAttacked = false;
		unitSelected = false;

		networkManager = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<NetworkManager>();
		userData = networkManager.GetUserData ();
		Debug.Log ("Player XP is " + userData.xp);
	}

	void OnGUI()
	{
		if (turn > 2 && photonView.isMine) {
			GUIStyle myStyle = new GUIStyle ();
			myStyle.fontSize = 36;
			GUI.Label (new Rect (0, Screen.height - 40, 200, 40), "Spectating...", myStyle);
			return;
		}
			
		if (!ready && myTurn.getTurn () == 0 && photonView.isMine && (turn == 1 || turn == 2)) {
			int screenHeight = Screen.height - 110; // accounts for leave button
			int buttonHeight = screenHeight/5;
			if (GUI.Button (new Rect (10,Screen.height - screenHeight, 150, buttonHeight), "Add Knight")) {
				SelectKnight();
			}
			
			if (GUI.Button (new Rect (10,Screen.height- screenHeight + buttonHeight, 150, buttonHeight), "Add Paladin")) {
				SelectPaladin();
			}
			
			if (GUI.Button (new Rect (10, Screen.height - screenHeight + buttonHeight*2, 150, buttonHeight), "Add Mage")) {
				SelectMage();
			}

			if (GUI.Button (new Rect (10, Screen.height - screenHeight + buttonHeight*3, 150, buttonHeight), "Add Rogue")) {
				SelectRogue();
			}
			
			if (GUI.Button (new Rect (10, Screen.height - screenHeight + buttonHeight*4, 150, buttonHeight), "Ready")) {
				PlayerReady();
			}
		}


		if (!selectedLocation) {
			GUIStyle myStyle = new GUIStyle();
			myStyle.fontSize = 36;
			GUI.Label(new Rect(200,10, 100, 30), "Place your Unit", myStyle);
		}



		if (photonView.isMine && myTurn.getTurn () == 0 && !ready) {
			GUIStyle myStyle = new GUIStyle ();
			myStyle.fontSize = 36;
			GUI.Label (new Rect (0, Screen.height - 40, 200, 40), "Please Add a Unit", myStyle);
		} 
		else if (photonView.isMine && myTurn.getTurn () == 0 && ready && !myTurn.gameOver) {
			GUIStyle myStyle = new GUIStyle ();
			myStyle.fontSize = 36;
			GUI.Label (new Rect (0, Screen.height - 40, 200, 40), "Waiting for Opponent...", myStyle);
		} 
		else if (photonView.isMine && myTurn.getTurn () == 0 && myTurn.gameOver) 
		{
			GUIStyle myStyle = new GUIStyle ();
			myStyle.fontSize = 36;
			GUI.Label (new Rect (0, Screen.height - 40, 200, 40), "Game has ended", myStyle);
		}

		else if (myTurn.getTurn () == turn && photonView.isMine) {
			GUIStyle myStyle = new GUIStyle ();
			myStyle.fontSize = 36;
			GUI.Label (new Rect (0, Screen.height - 40, 200, 40), "It is your turn", myStyle);
			GUI.Label (new Rect (Screen.width/2, 0, 200, 40), "You have "+myTurn.movesPerTurn+ " move(s) left", myStyle);
		} 
		else if (myTurn.getTurn () != turn && photonView.isMine){
			GUIStyle myStyle = new GUIStyle ();
			myStyle.fontSize = 36;
			GUI.Label (new Rect (0, Screen.height - 40, 200, 40), "It is not your turn", myStyle);
		}

		if (photonView.isMine && lostGame) {
			GUIStyle myStyle = new GUIStyle ();
			myStyle.fontSize = 72;
			GUI.Label (new Rect (Screen.width/2, Screen.height/2, 200, 40), "You Lose", myStyle);
		}
		else if (photonView.isMine && !lostGame && myTurn.gameOver) {
			GUIStyle myStyle = new GUIStyle ();
			myStyle.fontSize = 72;
			GUI.Label (new Rect (Screen.width/2 - 200, Screen.height/3, 200, 40), "You Win!", myStyle);
			myStyle.fontSize = 24;
			GUI.Label (new Rect (100, Screen.height/2 + 60, 200, 40), "You've Gained 100 Experience points!", myStyle);
			if (!gainedExp)
			{
				userData.xp += 100;
				gainedExp = true;
			}

			GUI.Label (new Rect (100, (Screen.height/2) + 100, 200, 40), "You now have "+ userData.xp + " Experience points", myStyle);

		}

			
	}



	void Update()
	{
		if (ready && photonView.isMine && StartingUnitCount == DeadUnitCount && !lostGame) {
			Debug.Log ("you lost");
			photonView.RPC("updateDeath", PhotonTargets.AllBuffered);
		}

		if (photonView.isMine && !lostGame && myTurn.gameOver && !wonGame) {
			Debug.Log ("you won");
			wonGame = true;
		}

			
		//adds a knight to current player's team. will be replaced by ui unit selection
		/*if (Input.GetKeyDown ("k") && myTurn.getTurn() == turn && photonView.isMine) { // player 1 recieves blue knight
			if (turn == 1)
				addUnit (redKnight);
			else if (turn == 2)
				addUnit (greenKnight);
		} */
	}

	public void DeSelectUnit()
	{
		Unit [] myUnits = gameObject.GetComponentsInChildren<Unit> ();
		Debug.Log ("deselecting unit...");
		foreach (Unit unit in myUnits) {
			if (unit.selected)
			{
				Debug.Log(unit.name + " is deselected");
				unit.selected = false;
			}
				
		}
	}

	[PunRPC] public void updateDeath()
	{
		lostGame = true;
		myTurn.endGame();
	}

	[PunRPC] public void updateAdditionalUnit()
	{
		StartingUnitCount++;
	}

	[PunRPC] public void setMyParent()
	{
		transform.SetParent (GameObject.Find ("TurnManager").transform);
		myTurn = GetComponentInParent<TurnManager> ();
	}

	[PunRPC] public void makingMove()
	{
		GetComponentInParent<TurnManager> ().madeMove (); // manages the turn over the network
	}

	/* // an RPC method to update the movement over the network, also doesn't do anything
	[PunRPC] void updateMovement(Vector3 newPosition)
	{
			transform.position = newPosition;
	}
	*/

	 // possibly useful, didn't actually do anything. tried to smooth the movement over the server
	/*void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting) {
			//stream.SendNext (GetComponentInChildren<Rigidbody2D> ().transform.position);
			stream.SendNext (GetComponentInChildren<Animator> ());
		}
				
		else {

			syncEndPosition = (Vector2)stream.ReceiveNext();
			syncStartPosition = GetComponent<Rigidbody2D>().transform.position;
			syncTime = 0f;
			syncDelay = Time.time - lastSynchronizationTime;
			lastSynchronizationTime = Time.time;

			//GetComponentInChildren<Rigidbody2D>().transform.position = (Vector3)stream.ReceiveNext();
			GetComponentInChildren<Animator> () = (Animator) stream.ReceiveNext();
		}
		
	}
	*/
		
	public void addUnit(GameObject newUnit, Vector3 loc)
	{
		if (StartingUnitCount >= 6)
			return;
		GameObject[] board = GameObject.FindGameObjectsWithTag ("Floor");
		foreach (GameObject pos in board) {
			if (pos.transform.position == loc)
			{
				var img = pos.GetComponent<SpriteRenderer> ();
				img.color = Color.white;
			}
				
		}
		//Debug.Log (newUnit.name);
		photonView.RPC("updateStartingUnits", PhotonTargets.AllBuffered, newUnit.name);
		//Debug.Log ("index =" + StartingUnitCount);
		GameObject instance = PhotonNetwork.Instantiate(newUnit.name, loc, Quaternion.identity, 0) as GameObject;
		//GameObject instance = PhotonNetwork.Instantiate(newUnit.name, Vector3.right * Random.Range(2,cols) + Vector3.up * Random.Range(2, rows), Quaternion.identity, 0) as GameObject;
		instance.transform.SetParent (transform); // sets new unit as child of the player
	}

	[PunRPC] public void updateStartingUnits(string newUnit)
	{

		foreach(GameObject myUnit in unitPrefabs)
		{
			if (myUnit.name.Equals(newUnit))
			{
				units [StartingUnitCount] = myUnit; // adds unit to game object array
				StartingUnitCount++;
			}
		}
	}

	public void SelectKnight() 
	{
		unitChoice = 1;
		if (selectedLocation && myTurn.getTurn() == 0 && photonView.isMine)
			StartCoroutine ("WaitForClick");
	}

	public void SelectPaladin() 
	{
		unitChoice = 2;
		if (selectedLocation && myTurn.getTurn() == 0 && photonView.isMine)
			StartCoroutine ("WaitForClick");
	}

	public void SelectMage() 
	{
		unitChoice = 3;
		if (selectedLocation && myTurn.getTurn() == 0 && photonView.isMine)
			StartCoroutine ("WaitForClick");
	}
	public void SelectRogue() 
	{
		unitChoice = 4;
		if (selectedLocation && myTurn.getTurn() == 0 && photonView.isMine)
			StartCoroutine ("WaitForClick");
	}

	public void PlayerReady()
	{
		photonView.RPC("setPlayerReady", PhotonTargets.AllBufferedViaServer);
		ready = true;
		//highlightUnitPlacement ();
	}

	[PunRPC] public void setPlayerReady()
	{
		ready = true;
		myTurn.playerReady ();
	}

	public GameObject getUnitatIndex(int i)
	{
		return units[i];
	}

	protected IEnumerator WaitForClick ()
	{
		yield return new WaitForSeconds (.1f);
		selectedLocation = false;
		Debug.Log ("waiting");
		highlightUnitPlacement (); 

		while (!selectedLocation) {
			bool blocked = false;
			if (Input.GetMouseButtonDown(0))
			{
				Debug.Log ("clicked");
				Vector3 loc = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				loc.x = (int) Mathf.Round(loc.x*1)/1;
				loc.y = (int) Mathf.Round(loc.y*1)/1;
				loc.z = 0;
				if (loc.x < 0 || loc.y < 0 || loc.x >= cols || loc.y >= rows) // check if click is within bounds
				{
					yield return new WaitForSeconds (.1f);
					continue;
				}

				if (photonView.isMine && turn == 1)
				{
					if (loc.x > cols/2)
					{
						yield return new WaitForSeconds (.1f);
						continue;
					}
				}
				else if (photonView.isMine && turn == 2)
				{
					if (loc.x < cols/2)
					{
						yield return new WaitForSeconds (.1f);
						continue;
					}
				}

				GameObject [] unit1Map = GameObject.FindGameObjectsWithTag("Player1");
				GameObject [] unit2Map = GameObject.FindGameObjectsWithTag("Player2");

				foreach(GameObject unit in unit1Map)
					if (unit.transform.position == loc)
						blocked = true;	

				foreach(GameObject unit in unit2Map)
					if (unit.transform.position == loc)
						blocked = true;	

				Debug.Log("past unit check");
				//Vector3 [] myStartingPlacements = new Vector3 [rows*cols];
				List<Vector3> myStartingPlacements = new List<Vector3>();
				if (photonView.isMine && turn == 1)
				{
					for (int i=1; i< cols/2; i++)
						for (int j=1; j<rows; j++)
							if (i==1 || i==2 || i==4 || i==5)
								if (j==1 || j==2 || j==4 || j==5 || j==7|| j==8)
							{
								myStartingPlacements.Add(new Vector3(i,j,0));
							}
				}
				else if (photonView.isMine && turn == 2)
				{
					for (int i=cols/2; i< cols; i++)
						for (int j=1; j<rows; j++)
							if (i==9 || i==10 || i==12 || i==13)
								if (j==1 || j==2 || j==4 || j==5 || j==7|| j==8)
							{
								myStartingPlacements.Add(new Vector3(i,j,0));
							}
				}


				foreach(Vector3 pos in myStartingPlacements)
				{
					if (loc == pos && !blocked)
					{
						Debug.Log("you have picked an allocated location");
						if (turn == 1)
						{
							if (unitChoice == 1)
								addUnit (redKnight, loc);
							else if (unitChoice == 2)
								addUnit (redPaladin, loc);
							else if (unitChoice == 3)
								addUnit(redMage, loc);
							else if (unitChoice == 4)
								addUnit(redRogue, loc);
						}
						
						else if (turn == 2)
						{
							if (unitChoice == 1)
								addUnit (greenKnight, loc);
							else if (unitChoice == 2)
								addUnit (greenPaladin, loc);
							else if (unitChoice == 3)
								addUnit(greenMage, loc);
							else if (unitChoice == 4)
								addUnit(greenRogue, loc);
						}
						
						selectedLocation = true;
						unhighlightUnitPlacement();
					}
				}
					
			}

			yield return null;
		}
	}

	private void highlightUnitPlacement()
	{
		GameObject[] board = GameObject.FindGameObjectsWithTag ("Floor");
		if (photonView.isMine && turn == 1 && !ready) {

			List<Vector3> myStartingPlacements = new List<Vector3>();
				for (int i=1; i< cols/2; i++)
					for (int j=1; j<rows; j++)
						if (i==1 || i==2 || i==4 || i==5)
							if (j==1 || j==2 || j==4 || j==5 || j==7|| j==8)
						{
							myStartingPlacements.Add(new Vector3(i,j,0));
						}
			
					
			foreach (GameObject loc in board) {

				if (loc.transform.position.x <= cols / 2 && loc.transform.position.y > 0) { // first 3rd of arena
					foreach(Vector3 pos in myStartingPlacements)
					{
						if (loc.transform.position == pos){
							var img = loc.GetComponent<SpriteRenderer> ();
							img.color = Color.green;
						}
					}

				}
			}
		} 

		else if (photonView.isMine && turn == 2 && !ready) {

			List<Vector3> myStartingPlacements = new List<Vector3>();
			for (int i=cols/2; i< cols; i++)
				for (int j=1; j<rows; j++)
					if (i==9 || i==10 || i==12 || i==13)
						if (j==1 || j==2 || j==4 || j==5 || j==7|| j==8)
					{
						myStartingPlacements.Add(new Vector3(i,j,0));
					}

			foreach (GameObject loc in board) {
				
				if (loc.transform.position.x >= cols / 2 && loc.transform.position.y > 0) { // first 3rd of arena
					foreach(Vector3 pos in myStartingPlacements)
					{
						if (loc.transform.position == pos){
							var img = loc.GetComponent<SpriteRenderer> ();
							img.color = Color.green;
						}
					}
					
				}
			}
		}
	}

	private void unhighlightUnitPlacement()
	{
		GameObject[] board = GameObject.FindGameObjectsWithTag ("Floor");
		if (photonView.isMine) {
			foreach(GameObject loc in board)
			{
				var img = loc.GetComponent<SpriteRenderer> ();
				img.color = Color.white;
			}
		}
	}

	public void unitDied()
	{
		DeadUnitCount++;
	}


}
