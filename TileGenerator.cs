using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGenerator : MonoBehaviour {
	public GameObject[] tiles;
	Transform cameraTransform;
	int amountOfTiles = 4;
	int numberOfTiles=0;
	float zPos=0.0f;
	public static int index=0;
	List<int> activeTiles;
	//PoolManager poolmanager;
	void Start () {
		activeTiles = new List<int> ();
		cameraTransform = GameObject.FindGameObjectWithTag ("MainCamera").transform;
		PoolManager.Instance.CreatePool (tiles[0],amountOfTiles);
		PoolManager.Instance.CreatePool (tiles[1],amountOfTiles);
		PoolManager.Instance.CreatePool (tiles[2],1);
		PoolManager.Instance.CreatePool (tiles[3],1);
		PoolManager.Instance.CreatePool (tiles[4],3);
		PoolManager.Instance.CreatePool (tiles[5],3);
		PoolManager.Instance.CreatePool (tiles[6],3);
		PoolManager.Instance.CreatePool (tiles[7],3);
		PoolManager.Instance.CreatePool (tiles[8],3);
		PoolManager.Instance.CreatePool (tiles[9],1);
		PoolManager.Instance.CreatePool (tiles[10],3);
		PoolManager.Instance.CreatePool (tiles[11],3);

		for (int i = 0; i < amountOfTiles; i++) {
			PoolManager.Instance.Reuse(tiles[0].GetInstanceID(),new Vector3(0f,0f,zPos),Quaternion.Euler(0f,0f,0f));
			zPos += 10f;
			numberOfTiles++;
			activeTiles.Add (tiles[0].GetInstanceID());
		}
	}

	void Update () {
		if (cameraTransform.position.z + 25f > zPos) {
			if (numberOfTiles < 10) {
				index = 0;
			} else if (numberOfTiles == 10) {
				index = 2;
			} else if (numberOfTiles < 20) {
				index = 1;
			} else if (numberOfTiles < 30) {
				index = 11;
			} else if (numberOfTiles == 30) {
				index = 3;
			} else if (numberOfTiles < 40) {
				index = Random.Range (4, 9);
			} else if (numberOfTiles == 40) {
				index = 9;
			} else if (numberOfTiles < 50) {
				index = 10;
			} else if (numberOfTiles == 50) {
				numberOfTiles = 0;
			}
			PoolManager.Instance.Reuse (tiles [index].GetInstanceID(), new Vector3 (0f, 0f, zPos), Quaternion.Euler (0f, 0f, 0f));
			shiftActiveTiles (tiles [index].GetInstanceID ());
			zPos += 10f;
			numberOfTiles++;
		}
	}
	void shiftActiveTiles(int i){
		if (i != activeTiles [0]) {
			//PoolManager.Instance.DestroyObj (activeTiles [0]);
		}
		activeTiles.RemoveAt (0);
		activeTiles.Add (i);
	}
}
