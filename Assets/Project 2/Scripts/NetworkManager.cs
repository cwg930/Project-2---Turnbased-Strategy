using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour {

	private const string roomName = "RoomName";
	private RoomInfo[] roomsList;
	public GameObject playerPrefab;
	public GameObject UnitManager;

	private int cols = BoardManager.columns;
	private int rows = BoardManager.rows;

	// Use this for initialization
	void Start () {

			PhotonNetwork.ConnectUsingSettings("0.1");
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI()
	{
		if (!PhotonNetwork.connected)
		{
			GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
		}
		else if (PhotonNetwork.room == null)
		{
			// Create Room
			if (GUI.Button(new Rect(100, 100, 250, 100), "Start Server"))

				PhotonNetwork.CreateRoom(roomName , true, true, 5);
			// Guid.NewGuid().ToString("N") this was added to room name
			
			// Join Room
			if (roomsList != null)
			{
				for (int i = 0; i < roomsList.Length; i++)
				{
					if (GUI.Button(new Rect(100, 250 + (110 * i), 250, 100), "Join " + roomsList[i].name))
						PhotonNetwork.JoinRoom(roomsList[i].name);
				}
			}
		}
	}
	

	void OnReceivedRoomListUpdate()
	{
		roomsList = PhotonNetwork.GetRoomList();
	}
	void OnJoinedRoom()
	{
		Instantiate (UnitManager, new Vector3(0f,0f,0f), Quaternion.identity);
		Debug.Log("Connected to Room");
		//AddUnit newUnits = new AddUnit ();
		Team myTeam = new Team ();
		myTeam.AddUnit (playerPrefab);
		myTeam.AddUnit (playerPrefab);



		while (!myTeam.isEmpty()) {
			PhotonNetwork.Instantiate(playerPrefab.name, Vector3.right * Random.Range(0,cols) + Vector3.up * Random.Range(0,rows), Quaternion.identity, 0);
			myTeam.removeUnit();
			//playerPrefab.tag = "Player2";

		}



		//TODO set up team and spawn all team members

		//Debug.Log (myTeam.GetType);



		//PhotonNetwork.Instantiate(playerPrefab.name, Vector3.right * Random.Range(0,cols) + Vector3.up * Random.Range(0,rows), Quaternion.identity, 0);
	}
}
