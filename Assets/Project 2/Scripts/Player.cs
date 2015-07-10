using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	private string playerID;
	private GameObject [] units;
	private int index;
	private int cols = BoardManager.columns;
	private int rows = BoardManager.rows;
	private Transform myPlayer;

	private NetworkManager client;

	public GameObject Knight;

	/*void Start()
	{
		myPlayer = null;
		Debug.Log ("Player created");
		//playerID = id;
		index = 0;
		units = new GameObject[6];
	}*/

	
	public Player(string id)
	{
		Debug.Log ("Player created");
		playerID = id;
		index = 0;
		units = new GameObject[6];
		myPlayer = new GameObject (id).transform;
		//StartCoroutine (Wait ());
	}

	public void addUnit(GameObject newUnit)
	{
		Debug.Log (newUnit.name);
		units [index] = newUnit;
		index++;
		//if (playerID.Equals("Player2")
		//GameObject instance = Instantiate(newUnit,new Vector3(2,Random.Range(2,rows),0f),Quaternion.identity) as GameObject;
		GameObject instance = PhotonNetwork.Instantiate(newUnit.name, Vector3.right * 0 + Vector3.up * 0, Quaternion.identity, 0) as GameObject;
		instance.transform.SetParent (myPlayer); // sets new unit as child of the player
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
