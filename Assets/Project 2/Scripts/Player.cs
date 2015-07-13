using UnityEngine;
using System.Collections;

public class Player : Photon.MonoBehaviour {

	private string playerID;
	private GameObject [] units;
	private int index;
	private int cols = BoardManager.columns;
	private int rows = BoardManager.rows;

	public GameObject Knight;
	public TurnManager myTurn;
	public int turn;

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
		index = 0;
		units = new GameObject[6];
	}
	

	void Update()
	{
		//adds a knight to current player's team. will be replaced by ui unit selection
		if (Input.GetKeyDown("k") && photonView.isMine) {
			addUnit(Knight);
		}
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

	/* // possibly useful, didn't actually do anything. tried to smooth the movement over the server
	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
				stream.SendNext (GetComponentInChildren<Rigidbody2D> ().transform.position);
		else {
			syncEndPosition = (Vector2)stream.ReceiveNext();
			syncStartPosition = GetComponent<Rigidbody2D>().transform.position;
			syncTime = 0f;
			syncDelay = Time.time - lastSynchronizationTime;
			lastSynchronizationTime = Time.time;
			//GetComponent<Rigidbody2D>().transform.position = (Vector2)stream.ReceiveNext();
		}
		
	} */
		
	//TODO give user option to select where new unit will be placed
	public void addUnit(GameObject newUnit)
	{
		if (index >= 6)
			return;
		//Debug.Log (newUnit.name);
		units [index] = newUnit; // adds unit to game object array
		index++;
		//Debug.Log ("index =" + index);
		GameObject instance = PhotonNetwork.Instantiate(newUnit.name, Vector3.right * Random.Range(2,cols) + Vector3.up * Random.Range(2, rows), Quaternion.identity, 0) as GameObject;
		instance.transform.SetParent (transform); // sets new unit as child of the player
	}

	// TODO create a button that will add a knight to a player's team
	public void SelectKnight() 
	{
		addUnit (Knight);
	}

	public GameObject getUnitatIndex(int i)
	{
		return units[i];
	}


}
