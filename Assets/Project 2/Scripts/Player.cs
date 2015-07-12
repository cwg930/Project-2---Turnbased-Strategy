using UnityEngine;
using System.Collections;

public class Player : Photon.MonoBehaviour {

	private string playerID;
	private GameObject [] units;
	private int index;
	private int cols = BoardManager.columns;
	private int rows = BoardManager.rows;


	private NetworkManager client;
	private float lastSynchronizationTime = 0f;
	private float syncDelay = 0f;
	private float syncTime = 0f;
	private Vector2 syncStartPosition = Vector2.zero;
	private Vector2 syncEndPosition = Vector2.zero;
	
	public GameObject Knight;
	private Transform myPlayer; 

	public TurnManager myTurn;

	public int turn;



	/*public Player()
	{
		Debug.Log ("Player created");
		index = 0;
		units = new GameObject[6];
		//myPlayer = new GameObject (id).transform;
		//StartCoroutine (Wait ());
	} */

	void Start()
	{
		//myTurn = GetComponentInParent<TurnManager> ();
		photonView.RPC("setMyParent", PhotonTargets.AllBuffered);
		Debug.Log (photonView.ownerId);

		myPlayer = gameObject.transform;
		turn = photonView.owner.ID;

		Debug.Log ("Player "+turn+" created");
		//playerID = id;
		index = 0;
		units = new GameObject[6];
		//addUnit (Knight);
	}
	

	void Update()
	{
		if (Input.GetKeyDown("k") && photonView.isMine) {
			Debug.Log("Knight was added");

			addUnit(Knight);
			//myTurn.startGame();
			//myTurn.ReallySwitchPlayer();
		}

		//if (!photonView.isMine)
		//	SyncedMovement ();

		if (photonView.isMine)
			Debug.Log ("is my view");
		else
			Debug.Log ("is not my view");
	}

	public TurnManager getTurnManager()
	{
		return myTurn;
	}

	[PunRPC] public void setMyParent()
	{
		transform.SetParent (GameObject.Find ("TurnManager").transform);
		myTurn = GetComponentInParent<TurnManager> ();
		//myTurn.madeMove ();
	}

	[PunRPC] public void makingMove()
	{
		GetComponentInParent<TurnManager> ().madeMove ();
		//myTurn.madeMove ();
	}

	[PunRPC] void updateMovement(Vector3 newPosition)
	{
		
			transform.position = newPosition;
		//renderer.material.color = new Color(color.x, color.y, color.z, 1f);
		/*if (myPlayer.photonView.isMine) {
			photonView.RPC("sendMove", PhotonTargets.OthersBuffered);
		}*/
		
		//if (photonView.isMine)
		//photonView.RPC("sendMove", PhotonTargets.OthersBuffered, color);
	}

	[PunRPC] void updateUnit()
	{
		this.addUnit(Knight);
		//renderer.material.color = new Color(color.x, color.y, color.z, 1f);
		/*if (myPlayer.photonView.isMine) {
			photonView.RPC("sendMove", PhotonTargets.OthersBuffered);
		}*/
		
		//if (photonView.isMine)
		//photonView.RPC("sendMove", PhotonTargets.OthersBuffered, color);
	}


	
	private void SyncedMovement()
	{
		syncTime += Time.deltaTime;
		GetComponentInChildren<Rigidbody2D>().transform.position = Vector2.Lerp(syncStartPosition, syncEndPosition, syncTime / syncDelay);

	}
	
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
		
	}


	


	public void addUnit(GameObject newUnit)
	{
		if (index >= 6)
			return;
		Debug.Log (newUnit.name);
		units [index] = newUnit;
		index++;
		Debug.Log ("index =" + index);
		//if (playerID.Equals("Player2")
		//GameObject instance = Instantiate(newUnit,new Vector3(2,Random.Range(2,rows),0f),Quaternion.identity) as GameObject;
		GameObject instance = PhotonNetwork.Instantiate(newUnit.name, Vector3.right * Random.Range(2,cols) + Vector3.up * Random.Range(2, rows), Quaternion.identity, 0) as GameObject;
		instance.transform.SetParent (myPlayer); // sets new unit as child of the player
	}

	private void initializePlayer()
	{

	}




	// TODO create a button that will add a knight to a player's team
	public void SelectKnight() 
	{
		addUnit (Knight);
	}

	protected IEnumerator Wait ()
	{
		yield return new WaitForSeconds (1);
		
		addUnit (Knight);
		addUnit (Knight);
	}

	public GameObject getUnitatIndex(int i)
		{
			return units[i];
		}


}
