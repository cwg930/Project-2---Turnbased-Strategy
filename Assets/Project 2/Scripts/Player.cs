﻿using UnityEngine;
using System.Collections;

public class Player : Photon.MonoBehaviour {

	private string playerID;
	private GameObject [] units;
	private int index;
	private int cols = BoardManager.columns;
	private int rows = BoardManager.rows;

	private bool selectedLocation; // checks to see if you have chosen a placement for spawned unit
	public bool ready; // checks to see if you have spawned all units and are ready to start the game
	private int unitChoice; // keeps track of which unit is chosen
	public bool unitIsMoving;

	public GameObject blueKnight;
	public GameObject redKnight;
	public GameObject greenKnight;

	public GameObject bluePaladin;
	public GameObject redPaladin;
	public GameObject greenPaladin;

	public GameObject blueMage;
	public GameObject redMage;
	public GameObject greenMage;

	public GameObject blueRogue;
	public GameObject greenRogue;
	public GameObject redRogue;
	
	[HideInInspector]	public TurnManager myTurn;
	public int turn;

	public bool lostGame;
	private bool wonGame;
	private int StartingUnitCount;
	private int DeadUnitCount;

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
		index = 0;
		units = new GameObject[6];
		selectedLocation = true;
		ready = false;
		unitChoice = 0;
		unitIsMoving = false;
		lostGame = false;
		StartingUnitCount = 0;
		DeadUnitCount = 0;
	}

	void OnGUI()
	{
		if (turn > 2)
			return;
		if (!ready && myTurn.getTurn () == 0 && photonView.isMine && (turn == 1 || turn == 2)) {
			if (GUI.Button (new Rect (10, 120, 150, 50), "Add Knight")) {
				SelectKnight();
			}
			
			if (GUI.Button (new Rect (10, 190, 150, 50), "Add Paladin")) {
				SelectPaladin();
			}
			
			if (GUI.Button (new Rect (10, 260, 150, 50), "Add Mage")) {
				SelectMage();
			}

			if (GUI.Button (new Rect (10, 330, 150, 50), "Add Rogue")) {
				SelectRogue();
			}
			
			if (GUI.Button (new Rect (10, 400, 150, 50), "Ready")) {
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
		else if (photonView.isMine && myTurn.getTurn () == 0 && ready) {
			GUIStyle myStyle = new GUIStyle ();
			myStyle.fontSize = 36;
			GUI.Label (new Rect (0, Screen.height - 40, 200, 40), "Waiting for Opponent...", myStyle);
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
			GUI.Label (new Rect (Screen.width/2, Screen.height/2, 200, 40), "You Win!", myStyle);
		}

			
	}

	

	void Update()
	{
		if (ready && photonView.isMine && StartingUnitCount == DeadUnitCount) {
			Debug.Log ("you lost");
			photonView.RPC("updateDeath", PhotonTargets.AllBuffered);
		}

			
		//adds a knight to current player's team. will be replaced by ui unit selection
		/*if (Input.GetKeyDown ("k") && myTurn.getTurn() == turn && photonView.isMine) { // player 1 recieves blue knight
			if (turn == 1)
				addUnit (redKnight);
			else if (turn == 2)
				addUnit (greenKnight);
		} */
	}

	[PunRPC] public void updateDeath()
	{
		lostGame = true;
		myTurn.endGame();
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
		
	//TODO give user option to select where new unit will be placed
	public void addUnit(GameObject newUnit, Vector3 loc)
	{
		if (index >= 6)
			return;
		//Debug.Log (newUnit.name);
		units [index] = newUnit; // adds unit to game object array
		index++;
		StartingUnitCount++;
		//Debug.Log ("index =" + index);
		GameObject instance = PhotonNetwork.Instantiate(newUnit.name, loc, Quaternion.identity, 0) as GameObject;
		//GameObject instance = PhotonNetwork.Instantiate(newUnit.name, Vector3.right * Random.Range(2,cols) + Vector3.up * Random.Range(2, rows), Quaternion.identity, 0) as GameObject;
		instance.transform.SetParent (transform); // sets new unit as child of the player
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
				GameObject [] unit1Map = GameObject.FindGameObjectsWithTag("Player1");
				GameObject [] unit2Map = GameObject.FindGameObjectsWithTag("Player2");

				for (int i=0; i< unit1Map.Length; i++)
				{
					if (unit1Map[i].transform.position == loc) // check if click is at same coordinates as unit from player 1
					{
						Debug.Log("you tried placing a unit where one already exists");
						blocked = true;
						break;
					}
						
				}

				for (int i=0; i<unit2Map.Length; i++)
				{
					if (unit2Map[i].transform.position == loc) // check if click is at same coordinates as unit from player 2
					{
						blocked = true;
						break;
					}
				}
					
				
				if (!blocked)
				{
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
				}



			}

			yield return null;
		}
	}

	public void unitDied()
	{
		DeadUnitCount++;
	}


}
