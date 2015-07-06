using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	public static GameManager instance = null;

	private BoardManager boardScript;
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
	
	}
}
