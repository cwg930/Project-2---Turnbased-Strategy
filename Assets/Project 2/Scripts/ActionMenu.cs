using UnityEngine;
using System.Collections;

public class ActionMenu : MonoBehaviour {

	private GameObject canvas;
	private Unit actingUnit;
	// Use this for initialization
	void Start ()
	{
		canvas = GameObject.FindGameObjectWithTag ("Canvas");
		gameObject.transform.SetParent (canvas.transform);
		gameObject.SetActive (false);
	}
	public void ShowMenu(Unit unit)
	{
		actingUnit = unit;
		gameObject.SetActive (true);
	}
	// Update is called once per frame
	void Update () {
	
	}

	public void OnMoveClicked()
	{
		actingUnit.StartAction ('M');
		gameObject.SetActive (false);
	}
}
