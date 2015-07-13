using UnityEngine;
using System.Collections;

public class TurnManager : Photon.MonoBehaviour {

	public int movesPerTurn = 2;

	private static int currentMove;

	private  int currentPlayer = 1;
	//public  int previousPlayer;

	public GameObject [] player1Team;
	public GameObject [] player2Team;

	// Use this for initialization


		//currentMove = movesPerTurn;
		//Debug.Log ("current player is" + currentPlayer);
		//ReallySwitchPlayer ();
	

	void Start()
	{
		//Debug.Log ("Turn Manager initialized");
	}

	public int getTurn()
	{
		Debug.Log ("it is player"+currentPlayer+"'s turn");
		return currentPlayer;
	}

	public void madeMove()
	{
		currentPlayer++;
		if (currentPlayer > 2)
			currentPlayer = 1;
		Debug.Log ("it is player"+currentPlayer+"'s turn");
	}

	public void startGame()
	{
		foreach (GameObject obj in player1Team)
			obj.SetActive(true);
		foreach (GameObject obj in player2Team)
			obj.SetActive(true);
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
