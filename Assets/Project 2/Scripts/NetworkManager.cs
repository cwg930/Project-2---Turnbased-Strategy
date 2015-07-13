using UnityEngine;
using System.Collections;

public class NetworkManager : Photon.MonoBehaviour {

	public GameObject Knight;//moved to player class
	public GameObject playerPrefab;

	//private int cols = BoardManager.columns;
	//private int rows = BoardManager.rows;//moved to player class
	private const string roomName = "RoomName";
	private RoomInfo[] roomsList;



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
		GameObject turnManager = GameObject.Find("TurnManager");
		GameObject instance = PhotonNetwork.Instantiate(playerPrefab.name, Vector3.right * 0 + Vector3.up * 0, Quaternion.identity, 0) as GameObject;
		instance.transform.SetParent (turnManager.transform); // sets new unit as child of the player
	}

}


