using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Team  {

	private LinkedList<GameObject> myTeam;
	private LinkedListNode<GameObject> head;

	public GameObject myUnit;

	public Team ()
	{
		myTeam = new LinkedList<GameObject> ();
		head = null;
		head = myTeam.First;
	}

	public GameObject getUnit()
	{
		 //TODO get unit from list and then remove that unit from the list
		return myUnit;
	}

	public void AddUnit (GameObject unit)
	{
		myTeam.AddLast (unit);
		myUnit = unit;
	}

	public void addKnight ()
	{

	}

	public void removeUnit()
	{
		myTeam.RemoveFirst ();
	}

	public bool isEmpty()
	{
		Debug.Log (myTeam.ToString());
		if (myTeam.Contains (myUnit)) {

			return false;
		}
			
		else 
			return true;

	}
}
