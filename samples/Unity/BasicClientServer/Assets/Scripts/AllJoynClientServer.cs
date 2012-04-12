using UnityEngine;
using AllJoynUnity;
using basic_clientserver;

public class AllJoynClientServer : MonoBehaviour
{
	void OnGUI ()
	{
		if(BasicServer.serverText != null){
			GUI.TextArea (new Rect (0, 0, Screen.width, (Screen.height / 2)), BasicServer.serverText);
		}
		if(BasicClient.clientText != null){
			GUI.TextArea (new Rect (0, (Screen.height / 2), Screen.width, (Screen.height * 1 / 2)), BasicClient.clientText);
		}
	}
	
	// Use this for initialization
	void Start()
	{
		Debug.Log("Starting up AllJoyn service and client");
		basicServer = new BasicServer();
		basicClient = new BasicClient();

		basicClient.Connect();
	}

	// Update is called once per frame
	void Update()
	{
		if(basicClient.Connected && !gotReply)
		{
			gotReply = true;
			Debug.Log("BasicClient.CallRemoteMethod returned '" + basicClient.CallRemoteMethod() + "'");
		}
	}

	BasicServer basicServer;
	BasicClient basicClient;
	bool gotReply = false;
}
