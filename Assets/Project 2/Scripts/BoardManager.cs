using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour {

	public int columns = 15;
	public int rows = 10;

	public GameObject[] floorTiles;
	public GameObject leftWall;
	public GameObject rightWall;
	public GameObject topWall;
	public GameObject bottomWall;
	public GameObject topLeftWall;
	public GameObject topRightWall;
	public GameObject bottomLeftWall;
	public GameObject bottomRightWall;

	
	private Transform boardHolder;


	void BoardSetup()
	{
		boardHolder = new GameObject ("Board").transform;
		GameObject toInstantiate;
		for (int x = -1; x < columns + 1; x++) {

			for(int y = -1; y < rows + 1; y++) {

				toInstantiate = floorTiles[Random.Range(0,floorTiles.Length)];

				if(x == -1 || y == -1 || x == columns || y == rows){
					if(x == -1 && y == -1){
						toInstantiate = bottomLeftWall;
					}else if(x==-1 && y == rows){
						toInstantiate = topLeftWall;
					}else if(x == columns && y == -1){
						toInstantiate = bottomRightWall;
					}else if(x == columns && y == rows){
						toInstantiate = topRightWall;
					}else if(x == -1){
						toInstantiate = leftWall;
					}else if(x == columns){
						toInstantiate = rightWall;
					}else if(y == -1){
						toInstantiate = bottomWall;
					}else if(y == rows){
						toInstantiate = topWall;
					}
				}

				GameObject instance = Instantiate(toInstantiate,new Vector3(x,y,0f),Quaternion.identity) as GameObject;


				instance.transform.SetParent(boardHolder);

			}

		}

	}

	public void SetupScene()
	{
		BoardSetup ();
	}
}
