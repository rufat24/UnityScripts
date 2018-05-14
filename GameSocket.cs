using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SocketIO;
public class GameSocket : MonoBehaviour {
	private SocketIOComponent socket;
	public List<GameObject> enemies;
	public List<GameObject> soldiers;
	string id;
	public Text point1;
	public Text point2;
	public void Start() 
	{
		GameObject go = GameObject.Find("SocketIO");
		socket = go.GetComponent<SocketIOComponent>();

		socket.On("open", (SocketIOEvent e) => {
		});
		socket.On("error", (SocketIOEvent e) => {

		});
		socket.On("close", (SocketIOEvent e) => {

		});
		socket.On("id", (SocketIOEvent e) => {
			id=e.data.GetField("id").ToString();
		});
		socket.On("gensoldier", (SocketIOEvent e) => {
			Debug.Log(e.data);
			if(id==e.data.GetField("id").ToString()){
				Instantiate(soldiers[int.Parse(e.data.GetField("enid").ToString())]);
				point1.text=e.data.GetField("health").ToString();
			} else if(e.data.GetField("id").ToString()!=null){
				Instantiate(enemies[int.Parse(e.data.GetField("enid").ToString())]);
				point2.text=e.data.GetField("health").ToString();
			}
		});
	}
	public void GeneratePlayer(int enid){
		Dictionary<string, string> data = new Dictionary<string, string>();
		data["id"] = id;
		data["enid"] = enid.ToString();
		socket.Emit("gensold", new JSONObject(data));
	}

}
