using UnityEngine;
using AllJoynUnity;
using basic_clientserver;

public class AllJoynClientServer : MonoBehaviour
{
	void OnGUI ()
	{
		if(BasicServer.serverText != null){
			GUI.TextArea (new Rect (0, 0, Screen.width, (Screen.height / 3)), BasicServer.serverText);
		}
		if(BasicClient.clientText != null){
			GUI.TextArea (new Rect (0, (Screen.height / 3), Screen.width, (Screen.height * 2 / 3)), BasicClient.clientText);
		}
	}
	
	// Use this for initialization
	void Start()
	{
		Debug.Log("Starting up AllJoyn service and client");
		basicServer = new BasicServer();
		basicClient = new BasicClient();

		//basicClient.Connect();
	}

	// Update is called once per frame
    void Update()
	{
            System.GC.Collect();
		if(basicClient.FoundAdvertisedName)
		{
			basicClient.ConnectToFoundName();	
		}
		else if(basicClient.Connected)
		{
			//gotReply = true;
		    Debug.Log("BasicClient.CallRemoteMethod returned '" + basicClient.CallRemoteMethod() + "'");
			basicServer.SendTheMsg();
		}
        if (Input.GetKeyDown(KeyCode.Escape)) { Application.Quit(); }
	}

	BasicServer basicServer;
	BasicClient basicClient;
	bool gotReply = false;
}
