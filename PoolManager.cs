using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour {
	public static PoolManager Instance { get; set;}
	public void Awake(){
		Instance = this;
	}
	Dictionary<int,Queue<GameObject>> pool = new Dictionary<int, Queue<GameObject>> ();
	public void CreatePool(GameObject gameObj,int numberOfObj){
		int gameObjKey = gameObj.GetInstanceID ();
		if (!pool.ContainsKey (gameObjKey)) {
			pool.Add (gameObjKey, new Queue<GameObject> ());
			for (int i = 0; i < numberOfObj; i++) {
				GameObject go = Instantiate (gameObj) as GameObject;
				go.SetActive (false);
				pool [gameObjKey].Enqueue (go);
			}
		}
	}
	public void Reuse(int gameObjKey, Vector3 position, Quaternion orientation){
		GameObject go = pool [gameObjKey].Dequeue ();
		go.SetActive (true);
		go.transform.position = position;
		go.transform.rotation = orientation;
		pool [gameObjKey].Enqueue (go);
	}
	public void Reuse(int gameObjKey){
		GameObject go = pool [gameObjKey].Dequeue ();
		go.SetActive (true);
		pool [gameObjKey].Enqueue (go);
	}
	public void DestroyObj(int gameObjKey){
		bool isActive = false;
		while (!isActive) {
			GameObject gop = pool [gameObjKey].Dequeue ();
			isActive = gop.activeSelf;
			if (isActive) {
				gop.SetActive (false);
			}
			pool [gameObjKey].Enqueue (gop);
		}
	}
}
