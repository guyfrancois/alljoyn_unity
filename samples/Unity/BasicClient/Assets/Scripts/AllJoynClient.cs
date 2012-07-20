using UnityEngine;
using AllJoynUnity;
using basic_client;

public class AllJoynClient : MonoBehaviour
{
	void OnGUI ()
	{
		if(BasicClient.clientText != null){
			GUI.TextArea (new Rect (0, 0, Screen.width, Screen.height), BasicClient.clientText);
		}
	}
	
	// Use this for initialization
	void Start()
	{
		Debug.Log("Starting up AllJoyn client");
		basicClient = new BasicClient();
	}

	// Update is called once per frame
    void Update()
	{
		if(basicClient != null && basicClient.FoundAdvertisedName)
		{
			basicClient.ConnectToFoundName();	
		}
		else if(basicClient != null && basicClient.Connected)
		{
		    Debug.Log("BasicClient.CallRemoteMethod returned '" + basicClient.CallRemoteMethod() + "'");
		}
        if (Input.GetKeyDown(KeyCode.Escape)) { Application.Quit(); }
	}

	BasicClient basicClient;
}
