using UnityEngine;
using System.Collections;

public class ActionMenu : MonoBehaviour {

	public GameObject actionMenu;
	public Unit actingUnit;
	// Use this for initialization
	void Start ()
	{

	}
	public void ShowMenu(Unit unit)
	{
		actingUnit = unit;

	}
	// Update is called once per frame
	void Update () {
	
	}

	public void OnMoveClicked()
	{
		
	}
}
