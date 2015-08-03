using UnityEngine;
using System.Collections;

public class TurnManager : Photon.MonoBehaviour {

	public int movesPerTurn;


	public  int currentPlayer;
	//public  int previousPlayer;

	public GameObject [] player1Team;
	public GameObject [] player2Team;

	private int readyCheck;

	public bool gameOver;
	public bool gameStarted;

	// Use this for initialization


		//currentMove = movesPerTurn;
		//Debug.Log ("current player is" + currentPlayer);
		//ReallySwitchPlayer ();
	

	void Start()
	{
		//Debug.Log ("Turn Manager initialized");
		currentPlayer = 0; //pregame setup
		readyCheck = 0;
		movesPerTurn = 2;
		gameOver = false;
		gameStarted = false;
	}


	public void playerReady()
	{
		readyCheck++;
		if (readyCheck == 2) {
			currentPlayer = 1;
			gameStarted = true;
		}
			
	}

	public void endGame()
	{
		gameOver = true;
		currentPlayer = 0;
	}

	public int getTurn()
	{
		//Debug.Log ("it is player"+currentPlayer+"'s turn");
		return currentPlayer;
	}

	public int getCurrentMoves()
	{
		return movesPerTurn;
	}

	public void madeMove()
	{
		Debug.Log ("player "+currentPlayer+" has made a move");
		movesPerTurn--;
		if (movesPerTurn == 0) {
			currentPlayer++;
			movesPerTurn = 2;
			if (currentPlayer > 2)
				currentPlayer = 1;
		}


		Debug.Log ("it is player"+currentPlayer+"'s turn");
	}

	public void startGame()
	{

	}


	public void initializeUnits(GameObject [] units)
	{
		player1Team = units;
		player2Team = units;
	}

	/*
	public void HasMoved()
	{
		currentMove--;
		if (currentMove <= 0) {
			previousPlayer = currentPlayer;
			currentPlayer = 0;
			currentMove = movesPerTurn;
		}
		SwitchPlayer ();
	}

	public void SwitchPlayer()
	{
		currentPlayer = previousPlayer + 1;
		if (previousPlayer > 2) {
			currentPlayer = 1;
		}
		ReallySwitchPlayer ();
	}

	public void ReallySwitchPlayer()
	{
		Debug.Log ("current player is" + currentPlayer);
		if (currentPlayer == 1) {
			foreach (GameObject obj in player1Team)
				obj.SetActive(true);
			foreach (GameObject obj in player2Team)
				obj.SetActive(false);
		}
		else if (currentPlayer == 2) {
			foreach (GameObject obj in player1Team)
				obj.SetActive(false);
			foreach (GameObject obj in player2Team)
				obj.SetActive(true);
		}


	}

	*/


}
