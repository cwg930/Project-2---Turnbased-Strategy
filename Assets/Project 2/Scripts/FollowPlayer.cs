using UnityEngine;
using System.Collections;

public class FollowPlayer : MonoBehaviour {


	public Transform myCamera;
	public Transform target;
	
	// Update is called once per frame
	void Update () {
	
		myCamera.position = target.position + new Vector3 (0, 0, -1);
	}
}
