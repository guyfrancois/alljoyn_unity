using UnityEngine;
using AllJoynUnity;
using basic_server;

public class AllJoynServer : MonoBehaviour
{
	void OnGUI ()
	{
		if(BasicServer.serverText != null){
			GUI.TextArea (new Rect (0, 0, Screen.width, Screen.height), BasicServer.serverText);
		}
	}
	
	// Use this for initialization
	void Start()
	{
		Debug.Log("Starting up AllJoyn service and client");
		basicServer = new BasicServer();
	}

	// Update is called once per frame
    void Update()
	{
        if (Input.GetKeyDown(KeyCode.Escape)) { Application.Quit(); }
	}

	BasicServer basicServer;
	bool gotReply = false;
}
