using UnityEngine;
using UnityEngine.UI; 

using System.Collections;
using Facebook;

public class NetworkManager : MonoBehaviour {

	private const string roomName = "RoomName";
	private RoomInfo[] roomsList;
	public GameObject playerPrefab;
	public GameObject UnitManager;

	public GameObject loginFacebook;
	public GameObject loginNormal;
	public GameObject CreateGame;
	public GameObject Logout;
	public GameObject Leave;

	private int cols = BoardManager.columns;
	private int rows = BoardManager.rows;



	enum NetworkStates {
		NotLoggedIn,
		InLobby,
		InRoom,
		Unknown
	}

	private NetworkStates networkState;


	// Use this for initialization
	void Start () {
		FB.Init (SetInit, OnHideUnity);

		loginFacebook = GameObject.Find("Login");
		loginNormal = GameObject.Find ("LoginNormal");
		CreateGame = GameObject.Find ("Create Game");
		Logout = GameObject.Find("Logout");
		Leave = GameObject.Find ("Leave");

		networkState = NetworkStates.NotLoggedIn;
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
				loginFacebook.SetActive(true);				
				loginNormal.SetActive(true);
				Logout.SetActive(false);
				CreateGame.SetActive(false);
				Leave.SetActive(false);

				break;
			}
			case NetworkStates.InLobby:
			{
				loginFacebook.SetActive(false);				
				loginNormal.SetActive(false);
				Logout.SetActive(true);
				CreateGame.SetActive (true);
				Leave.SetActive(false);

				// Join existing game button
				if (roomsList != null) {
					for (int i = 0; i < roomsList.Length; i++) {
						if (GUI.Button (new Rect (10, 70 + (110 * i), 150, 30), "Join " + roomsList [i].name))
							PhotonNetwork.JoinRoom (roomsList [i].name);
					}
				}
				break;
			}
			case NetworkStates.InRoom:
			{

				loginFacebook.SetActive(false);				
				loginNormal.SetActive(false);
				CreateGame.SetActive(false);
				Logout.SetActive(false);
				Leave.SetActive(true);	

				GUILayout.Label("Your name: " + PhotonNetwork.playerName);
				GUILayout.Label(PhotonNetwork.playerList.Length + " players in this room.");
				GUILayout.Label("The others are:");
				foreach (PhotonPlayer player in PhotonNetwork.otherPlayers)
				{
					GUILayout.Label(player.ToString());
				}
				break;
			}
			case NetworkStates.Unknown:
			{

				loginFacebook.SetActive(false);				
				loginNormal.SetActive(false);
				CreateGame.SetActive(false);
				Logout.SetActive(false);
				Leave.SetActive(false);

				GUILayout.Label("Unknown network state!");
				break;
			}
		}
		
		if (PhotonNetwork.connected) {
			loginFacebook.SetActive(false);				
			loginNormal.SetActive(false);

		}
	}
	

	void OnReceivedRoomListUpdate()
	{
		roomsList = PhotonNetwork.GetRoomList();
	}

	void OnJoinedRoom()
	{
		//Instantiate (UnitManager, new Vector3(0f,0f,0f), Quaternion.identity);
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

	 public void loginFBClicked()
     {
         FB.Login ("email", LoginCallback);
     }

	public void loginNormalClicked()
	{
		PhotonNetwork.playerName = "Player " + (int)(Random.value*100);
		PhotonNetwork.ConnectUsingSettings("1.0");
	}

	public void CreateGameClicked()
	{
		PhotonNetwork.CreateRoom (roomName, true, true, 5);
	}

	public void LogoutClicked()
	{
		if (FB.IsLoggedIn)
			FB.Logout();
		PhotonNetwork.Disconnect();
	}

	public void LeaveClicked()
	{
		PhotonNetwork.LeaveRoom();
	}
}
