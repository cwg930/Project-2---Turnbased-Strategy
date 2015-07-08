using UnityEngine;
using System.Collections;

public class AddUnit : MonoBehaviour {

	public Team myTeam;

	void addKnight()
	{
		Unit myUnit = new Knight ();
		myTeam.AddUnit(myUnit);
	}

	public Team getTeam()
	{
		return myTeam;
	}

	void Start()
	{
		myTeam = new Team ();
		addKnight ();
		addKnight ();
	}
}
