using UnityEngine;
using System.Collections;
using Facebook;

public class NetworkManager : Photon.MonoBehaviour {

	public GameObject playerPrefab;

	private const string roomName = "RoomName";
	private RoomInfo[] roomsList;

	private bool inRoom;
	private string stringToEdit;

	enum NetworkStates {
		NotLoggedIn,
		InLobby,
		InRoom,
		Unknown
	}

	private NetworkStates networkState;

	// Use this for initialization
	void Start () {
		stringToEdit = "Enter a Room Name";
		inRoom = false;
		FB.Init (SetInit, OnHideUnity);
		networkState = NetworkStates.NotLoggedIn;
		//PhotonNetwork.ConnectUsingSettings("0.1");
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	private void SetInit()
	{
		if (FB.IsLoggedIn) {
			Debug.Log ("SetInit()");
			OnLoggedIn ();
		}
	}

	void LoginCallback(FBResult result)
	{
		if (FB.IsLoggedIn)
		{
			OnLoggedIn();
		}
	}

	void OnLoggedIn()
	{
		PhotonNetwork.AuthValues = new AuthenticationValues();
		PhotonNetwork.AuthValues.AuthType = CustomAuthenticationType.Facebook;
		PhotonNetwork.AuthValues.AddAuthParameter("username", FB.UserId);
		PhotonNetwork.AuthValues.AddAuthParameter("token", FB.AccessToken);
		PhotonNetwork.playerName = FB.UserId;
		PhotonNetwork.ConnectUsingSettings("1.0");
	}

	private void OnHideUnity(bool isGameShown)
	{
		Debug.Log("OnHideUnity()");
	}

	void OnGUI()
	{
		GUI.Label(new Rect(10, 10, 500, 30), PhotonNetwork.connectionStateDetailed.ToString());

		if (!PhotonNetwork.connected)
			networkState = NetworkStates.NotLoggedIn;
		else if (PhotonNetwork.room == null)
			networkState = NetworkStates.InLobby;
		else if (PhotonNetwork.room != null)
			networkState = NetworkStates.InRoom;
		else
			networkState = NetworkStates.Unknown;

		// Network state machine
		switch (networkState) {
			case NetworkStates.NotLoggedIn:
			{
				if (GUI.Button (new Rect (10, 10, 150, 70), "Login to Facebook")) {
					FB.Login ("email", LoginCallback);
				}
				if (GUI.Button(new Rect (10, 100, 300, 70), "Login")) { // TODO connect normal login to our database
					PhotonNetwork.playerName = "Player " + (int)(Random.value*100);
					PhotonNetwork.ConnectUsingSettings("1.0");
				}
				break;
			}
			case NetworkStates.InLobby:
			{
				inRoom = false;
				stringToEdit = GUI.TextField (new Rect (10, 10, 200, 70), stringToEdit);
			// Create game button
				if (GUI.Button (new Rect (10, 100, 150, 70), "Create Game"))
				{
				Debug.Log("room name ="+ stringToEdit);
					PhotonNetwork.CreateRoom (stringToEdit, true, true, 5);
				}
					

				// Join existing game button
				if (roomsList != null) {
					for (int i = 0; i < roomsList.Length; i++) {
						if (GUI.Button (new Rect (10, 180 + (80 * i), 150, 70), "Join " + roomsList [i].name))
							PhotonNetwork.JoinRoom (roomsList [i].name);
					}
				}
				break;
			}
			case NetworkStates.InRoom: // in room so instantiate player
			{
				inRoom = true;
				GUILayout.Label("Your name: " + PhotonNetwork.playerName);
				GUILayout.Label(PhotonNetwork.playerList.Length + " players in this room.");
				GUILayout.Label("The others are:");
				foreach (PhotonPlayer player in PhotonNetwork.otherPlayers)
				{
					GUILayout.Label(player.ToString());
				}
				
				if (GUI.Button(new Rect (10, 20, 150, 70), "Leave"))
				{
					PhotonNetwork.LeaveRoom();
				}
				break;

			}
			case NetworkStates.Unknown:
			{
				GUILayout.Label("Unknown network state!");
				break;
			}
		}
		
		if (PhotonNetwork.connected && !inRoom) {
			if (GUI.Button(new Rect(10, Screen.height - 90, 150, 70), "Logout")) 
			{
				if (FB.IsLoggedIn)
					FB.Logout();
				PhotonNetwork.Disconnect();
			}
		}
	}

	public void getRoomName()
	{
		GUI.TextField (new Rect (10, 10, 200, 20), stringToEdit);
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


