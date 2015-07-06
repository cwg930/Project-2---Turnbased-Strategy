using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	public static GameManager instance = null;
	[HideInInspector] public bool player1Turn = true;
	public float turnDelay = .1f;

	private BoardManager boardScript;
	private bool player2moving;
	// Use this for initialization
	void Awake () {
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy (gameObject);

		DontDestroyOnLoad (gameObject);

		boardScript = GetComponent<BoardManager> ();

		InitGame ();
		
	}

	void InitGame()
	{
		boardScript.SetupScene ();
	}
	
	// Update is called once per frame
	void Update () {
		if (player1Turn || player2moving)
			return;
		StartCoroutine (wait());

	}

	IEnumerator wait ()
	{
		yield return new WaitForSeconds (turnDelay);
		yield return new WaitForSeconds (turnDelay);
		yield return new WaitForSeconds (turnDelay);
		yield return new WaitForSeconds (turnDelay);
		player1Turn = true;
		player2moving = false;
	}
}
