//-----------------------------------------------------------------------
// <copyright file="AllJoynServer.cs" company="Qualcomm Innovation Center, Inc.">
// Copyright 2012, Qualcomm Innovation Center, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
//-----------------------------------------------------------------------

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
