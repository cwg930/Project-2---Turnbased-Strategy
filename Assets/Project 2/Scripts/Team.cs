using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Team  {

	private List<Unit> myTeam;

	public Team ()
	{
		myTeam = new List<Unit> ();
	}

	// Use this for initialization
	void Start () {
		myTeam = new List<Unit> ();
	}

	public Unit getUnit()
	{
		 //TODO get unit from list and then remove that unit from the list
		return new Knight();
	}

	public Knight getKnight()
	{
		return new Knight();
	}

	public void AddUnit (Unit unit)
	{
		myTeam.Add(unit);
	}

}
