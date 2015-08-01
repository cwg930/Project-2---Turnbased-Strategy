using UnityEngine;
using System.Collections;
using Facebook;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Internal;
using PlayFab;



public class NetworkManager : Photon.MonoBehaviour
{
	public GameObject playerPrefab;
	public GameObject gameManager;
	public GameObject background;
	public GameObject title;
	public TurnManager turnManager;

	private const string roomName = "RoomName";
	private RoomInfo[] roomsList;

	private string createRoomName;
	private string username;
	private string email;
	private string password;
	private string playfabUserID;
	private PlayFabClientAPI playfab;

	private string myErrorMessage;
	private bool hasError = false;
	private bool inRoom = false;
	private bool joinedRoom = false;

	// Offical Battle Knights ID's
	private const string PHOTON_APP_ID 		= "a89a5e03-2c3b-4fc0-b01f-ce7d87bf5b5d";	

	private UserData userData;

	enum NetworkStates
	{
		NotLoggedIn,
		InLobby,
		InRoom,
		Unknown
	};
	
	private NetworkStates networkState;

	// Use this for initialization
	void Start ()
	{
		createRoomName = "Enter a Room Name";
		username = "Username";
		email = "Email";
		password = "Password";
		playfabUserID = string.Empty;
		FB.Init (SetInit, OnHideUnity);
		//PlayFabSettings.UseDevelopmentEnvironment = false;
		networkState = NetworkStates.NotLoggedIn;
		//PhotonNetwork.ConnectUsingSettings("0.1");
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	private void SetInit ()
	{
		if (FB.IsLoggedIn) {
			Debug.Log ("SetInit()");
			OnLoggedIn ();
		}
	}

	private void LoginCallback (FBResult result)
	{
		if (FB.IsLoggedIn) {
			OnLoggedIn ();
		}
	}

	private void OnLoggedIn ()
	{
		PhotonNetwork.AuthValues = new AuthenticationValues ();
		PhotonNetwork.AuthValues.AuthType = CustomAuthenticationType.Facebook;
		PhotonNetwork.AuthValues.AddAuthParameter ("username", FB.UserId);
		PhotonNetwork.AuthValues.AddAuthParameter ("token", FB.AccessToken);
		PhotonNetwork.playerName = FB.UserId;
		PhotonNetwork.ConnectUsingSettings ("1.0");
	}

	private void OnHideUnity (bool isGameShown)
	{
		Debug.Log ("OnHideUnity()");
	}

	private void OnGUI ()
	{
		GUI.Label (ResizeGUI(new Rect (10, 10, 500, 30)), PhotonNetwork.connectionStateDetailed.ToString ());

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
			/*
				if (GUI.Button (new Rect (10, 10, 150, 70), "Login to Facebook")) {
					FB.Login ("email", LoginCallback);
				}
			*/
			if (hasError) {

				StartCoroutine("waitForError");
				GUIStyle myStyle = new GUIStyle ();
				myStyle.fontSize = 36;
				GUI.Label (ResizeGUI(new Rect (10, 50, 200, 20)), myErrorMessage, myStyle);
			}

			username = GUI.TextField (ResizeGUI(new Rect (10, 100, 200, 20)), username);
			email = GUI.TextField (ResizeGUI(new Rect (10, 130, 200, 20)), email);
			password = GUI.PasswordField (ResizeGUI(new Rect (10, 160, 200, 20)), password, '*');

			if (GUI.Button (ResizeGUI(new Rect (10, 210, 150, 30)), "Create Account")) {
					RegisterPlayFabUserRequest request = new RegisterPlayFabUserRequest ();
					request.TitleId = PlayFabSettings.TitleId;

					request.Username = username;
					request.Email = email;
					request.Password = password;

					PlayFabClientAPI.RegisterPlayFabUser (request, OnPlayFabRegisterSuccess, OnPlayFabError);
				}

			if (GUI.Button (ResizeGUI(new Rect (10, 250, 150, 30)), "Login with Username")) {
				LoginWithPlayFabRequest request = new LoginWithPlayFabRequest ();
				request.Username = username;
				request.Password = password;
				request.TitleId = PlayFabData.TitleId;  
				PlayFabClientAPI.LoginWithPlayFab (request, OnPlayFabLoginSuccess, OnPlayFabError);
			}

			if (GUI.Button (ResizeGUI(new Rect (10, 290, 150, 30)), "Login with Email")) {
				LoginWithEmailAddressRequest request = new LoginWithEmailAddressRequest();
				request.Email = email;
				request.Password = password;
				request.TitleId = PlayFabData.TitleId;
				PlayFabClientAPI.LoginWithEmailAddress(request, OnPlayFabLoginSuccess, OnPlayFabError);
			}
			
			break;
			}
		case NetworkStates.InLobby:
			{
				if (joinedRoom)
			{
				GameObject board = GameObject.Find("Board");
				Destroy(board);
				background.SetActive(true);
				title.SetActive(true);
				turnManager.currentPlayer = 0; // resets game when you leave
				turnManager.gameOver = false;

				GameObject go = GameObject.FindGameObjectWithTag ("Canvas");
				ActionMenu actionMenu = (ActionMenu)go.transform.FindChild ("ActionMenu(Clone)").GetComponent<ActionMenu>();

				actionMenu.DestroyActionMenu();
				joinedRoom = false;

			}
				inRoom = false;
			createRoomName = GUI.TextField (ResizeGUI(new Rect (10, 110, 150, 30)), createRoomName);
				// Create game button
			if (GUI.Button (ResizeGUI(new Rect (10, 50, 150, 50)), "Create Game")) {
					Debug.Log ("room name = " + createRoomName);
					PhotonNetwork.CreateRoom (createRoomName, true, true, 5);
				}
					

				// Join existing game button
				if (roomsList != null) {
					for (int i = 0; i < roomsList.Length; i++) {
					if (GUI.Button (ResizeGUI(new Rect (10, 180 + (80 * i), 150, 70)), "Join " + roomsList [i].name))
							PhotonNetwork.JoinRoom (roomsList [i].name);
					}
				}
				break;
			}
		case NetworkStates.InRoom: // in room so instantiate player
			{
				inRoom = true;
				GUILayout.Label ("Your name: " + PhotonNetwork.playerName + " - " + playfabUserID );
				GUILayout.Label (PhotonNetwork.playerList.Length + " players in this room.");
				GUILayout.Label ("The others are:");
				foreach (PhotonPlayer player in PhotonNetwork.otherPlayers) {
					GUILayout.Label (player.ToString ());
				}
				
			if (GUI.Button (ResizeGUI(new Rect (10, 70, 150, 30)), "Leave")) {
					PhotonNetwork.LeaveRoom ();
				}
				break;

			}
		case NetworkStates.Unknown:
			{
				GUILayout.Label ("Unknown network state!");
				break;
			}
		}
		
		if (PhotonNetwork.connected && !inRoom) {
			if (GUI.Button (ResizeGUI(new Rect (10, Screen.height - 70, 150, 30)), "Logout")) {
				if (FB.IsLoggedIn)
					FB.Logout ();
				PhotonNetwork.Disconnect ();
			}
		}
	}

	private IEnumerator waitForError()
	{
		yield return new WaitForSeconds(1);
		hasError = false;
	}

	public void OnPlayFabRegisterSuccess (RegisterPlayFabUserResult result)
	{
		Debug.Log ("PlayFab Register Success");
		Debug.Log (result.Username);
		LoginWithPlayFabRequest request = new LoginWithPlayFabRequest ();
		request.Username = username;
		request.Password = password;
		request.TitleId = PlayFabData.TitleId;
		PlayFabClientAPI.LoginWithPlayFab (request, OnPlayFabLoginSuccess, OnPlayFabError);
	}

	private void OnPlayFabLoginSuccess (LoginResult result)
	{
		Debug.Log ("PlayFab Login Success");
		playfabUserID = result.PlayFabId;	// record our playfab user ID

		StartCoroutine(GetPlayerXP ());	// request the XP for this user

		GetPhotonAuthenticationTokenRequest request = new GetPhotonAuthenticationTokenRequest ();
		request.PhotonApplicationId = PHOTON_APP_ID;
		// get an authentication ticket to pass on to Photon 
		PlayFabClientAPI.GetPhotonAuthenticationToken (request, OnPhotonAuthenticationSuccess, OnPlayFabError);
	}

	private void OnPhotonAuthenticationSuccess (GetPhotonAuthenticationTokenResult result)
	{
		Debug.Log ("Photon Authentication Success");
		ConnectToMasterServer (playfabUserID, result.PhotonCustomAuthenticationToken);
	}

	private void ConnectToMasterServer (string id, string token)
	{
		Debug.Log ("logging in... " + id);
		PhotonNetwork.AuthValues = new AuthenticationValues ();
		PhotonNetwork.AuthValues.AuthType = CustomAuthenticationType.Custom;
		PhotonNetwork.AuthValues.AddAuthParameter (id, token);
		PhotonNetwork.ConnectUsingSettings ("1.0");
	}

//	private void OnPlayFabGetUserInfo(GetUserCombinedInfoResult result)
//	{
//		Debug.Log ("Received Player Info");
//
//		// Get player's XP
//		UserDataRecord xp = null;
//		result.Data.TryGetValue ("XP", out xp);
//
//		// If player has no XP value, initialize it to 0
//		if (xp == null) {
//			UpdatePlayerXP (0);
//			userData.xp = 0;
//		} else {
//			userData.xp = int.Parse (xp.Value);
//		}
//		Debug.Log ("User XP = " + userData.xp);
//	}

	private void OnPlayFabGetUserStatistics(GetUserStatisticsResult result) {
		Debug.Log ("Received Player Stats");

		int xp;
		if (result.UserStatistics.TryGetValue ("XP", out xp)) {
			userData.xp = xp;
			Debug.Log ("User XP = " + userData.xp);
		} else {
			UpdatePlayerXP(0);
		}
	}

	
	public void UpdatePlayerXP(int xp) {
		UpdateUserStatisticsRequest updateReq = new UpdateUserStatisticsRequest ();
		updateReq.UserStatistics = new System.Collections.Generic.Dictionary<string, int> ();
		updateReq.UserStatistics.Add("XP", xp);
		PlayFabClientAPI.UpdateUserStatistics (updateReq, OnUpdatePlayerXPSuccess, OnPlayFabError);
	}

//	public void UpdatePlayerXP(int xp) {
//		UpdateUserDataRequest updateReq = new UpdateUserDataRequest ();
//		updateReq.Data = new System.Collections.Generic.Dictionary<string, string> ();
//		updateReq.Data.Add("XP", xp.ToString());
//		PlayFabClientAPI.UpdateUserData (updateReq, OnUpdatePlayerXPSuccess, OnPlayFabError);
//	}

//	private IEnumerator GetPlayerData(float sec = 0) {
//		yield return new WaitForSeconds (sec);
//		GetUserCombinedInfoRequest infoReq = new GetUserCombinedInfoRequest ();
//		PlayFabClientAPI.GetUserCombinedInfo (infoReq, OnPlayFabGetUserInfo, OnPlayFabError);
//	}

	private IEnumerator GetPlayerXP(float sec = 0) {
		yield return new WaitForSeconds (sec);
		GetUserStatisticsRequest xpReq = new GetUserStatisticsRequest ();
		PlayFabClientAPI.GetUserStatistics (xpReq, OnPlayFabGetUserStatistics, OnPlayFabError);
	}

//	private void OnUpdatePlayerXPSuccess(UpdateUserDataResult result) {
//		Debug.Log ("Updated player's XP value");
//	}

	private void OnUpdatePlayerXPSuccess(UpdateUserStatisticsResult result) {
		Debug.Log ("Updated player's XP value");
	}

	// Generic PlayFab callback for errors.
	private void OnPlayFabError (PlayFabError error)
	{
		Debug.Log (error.ErrorMessage);
		myErrorMessage = error.ErrorMessage;
		hasError = true;
	}

	public void getRoomName ()
	{
		GUI.TextField (ResizeGUI(new Rect (10, 10, 200, 20)), createRoomName);
	}

	private void OnReceivedRoomListUpdate ()
	{
		roomsList = PhotonNetwork.GetRoomList ();
	}

	private void OnJoinedRoom ()
	{
		Instantiate (gameManager, new Vector3 (0, 0, 0), Quaternion.identity);
		background.SetActive (false);
		title.SetActive (false);
		GameObject turnManager = GameObject.Find ("TurnManager");
		GameObject instance = PhotonNetwork.Instantiate (playerPrefab.name, Vector3.right * 0 + Vector3.up * 0, Quaternion.identity, 0) as GameObject;
		instance.transform.SetParent (turnManager.transform); // sets the player as child of the turn manager;
		joinedRoom = true;
	}

	public UserData GetUserData() {
		return userData;
	}

	Rect ResizeGUI(Rect rect)
	{
		var FilScreenWidth = rect.width / 800;
		var rectWidth = FilScreenWidth * Screen.width;
		var FilScreenHeight = rect.height / 600;
		var rectHeight = FilScreenHeight * Screen.height;
		var rectX = (rect.x / 800) * Screen.width;
		var rectY = (rect.y / 600) * Screen.height;
		
		return new Rect(rectX,rectY,rectWidth,rectHeight);
	}
}


