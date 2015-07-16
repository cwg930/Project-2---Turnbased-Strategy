using UnityEngine;
using System.Collections;

public class GameUIManager : MonoBehaviour {

	public static GameUIManager instance = null;

	public GameObject actionMenu;

	// Use this for initialization
	void Awake()
	{
		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy(gameObject);
		}

		Instantiate (actionMenu);
	}
}
