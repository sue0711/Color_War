using UnityEngine;
using UnityEditor;
using System.Collections;

public class NetworkManager : MonoBehaviour {
	private const string gameTypeName = "ColorWar";

	private HostData[] hostDatas;

	private string roomName = "room1";
	private bool isConnect;

	/* Set Component Position */
	private float btnX;
	private float btnY;
	/* Set Component Size */
	private float btnW;
	private float btnH;
	
	private float timer;
	private int waitingTime;

	/*
	 * Author - Jongsu
	 * 
	 * Description - Use this for initialization
	 * 
	 * State - Stable
	 */
	void Start () {
		isConnect = false;

		btnX = Screen.width * 0.1f;
		btnY = Screen.height * 0.1f;
		btnW = btnH = Screen.width * 0.1f;

		timer = 0.0f;
		waitingTime = 5;

		refreshHostList ();
	}

	/*
	 * Author - Jongsu
	 * 
	 * Description - Update is called once per frame
	 * 
	 * State - Implementing
	 */
	void Update () {
		timer += Time.deltaTime;
		if (timer > waitingTime) {
			refreshHostList();
			timer = 0;
		}
	}

	/*
	 * Author - Jongsu
	 * 
	 * Description - 
	 * 
	 *  State - Implementing
	 */
	void OnGUI() {
		roomName = GUI.TextField (new Rect (btnX, btnY, btnW * 2, btnH * 0.5f), roomName);

		if (hostDatas != null) {
			for (int i = 0; i < hostDatas.Length; i++) {
				if (GUI.Button (new Rect (btnX, btnY * 2f + (btnH * i), btnW * 3, btnH * 0.5f), hostDatas [i].gameName)) {
					Network.Connect (hostDatas[i]);
				}
			}
		}

		if (GUI.Button (new Rect (btnX + btnW * 2.2f, btnY, btnW * 2, btnH * 0.5f), "Create Room")) {
			createRoom();
		}

		if (GUI.Button (new Rect (btnX + btnW * 4.4f, btnY, btnW * 2, btnH * 0.5f), "Refresh")) {
			refreshHostList();
		}
	}

	/*
	 * Author - Jongsu
	 * 
	 * Description - Create a rooom
	 * 
	 * State - Before adding error handling 
	 */
	void createRoom() {
		if (isConnect) {
			EditorUtility.DisplayDialog ("Warning", "Already connected game", "OK");
			return ;
		}

		if (hostDatas != null) {
			for (int i = 0; i < hostDatas.Length; i++) {
				if (hostDatas [i].gameName.Equals (roomName)) {
					EditorUtility.DisplayDialog ("Warning", "Already exist room name.", "OK");
					return;
				}
			}
		}

		Network.InitializeServer (2, 8888, !Network.HavePublicAddress());
		MasterServer.RegisterHost (gameTypeName, roomName);
	}

	/*
	 * Author - Jongsu
	 * 
	 * Description - Get created room lists.
	 * 
	 * State - Stable
	 */
	void refreshHostList() {
		MasterServer.RequestHostList (gameTypeName);
		hostDatas = MasterServer.PollHostList();
	}

	/*
	 * Author - Jongsu
	 * 
	 * Description - 
	 * 
	 * State - implementing
	 */
	void OnServerInitialized(){
		/* TODO: Wait for other player */
	}
	
	/*
	 * Author - Jongsu
	 * 
	 * Description - 
	 * 
	 * State - implementing
	 */
	void OnConnectedToServer() {
		/* TODO: Send signal for game to start */
	}

	/*
	 * Author - Jongsu
	 * 
	 * Description - 
	 * 
	 * State - implementing
	 */
	void OnMasterServerEvent(MasterServerEvent msg) {
		if (msg == MasterServerEvent.RegistrationSucceeded) {
			isConnect = true;
		}
	}
}
