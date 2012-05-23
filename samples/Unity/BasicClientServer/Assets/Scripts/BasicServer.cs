using UnityEngine;
using AllJoynUnity;
using System.Runtime.InteropServices;

namespace basic_clientserver
{
	class BasicServer
	{
		private const string INTERFACE_NAME = "org.alljoyn.Bus.method_sample";
		private const string SERVICE_NAME = "org.alljoyn.Bus.method_sample";
		private const string SERVICE_PATH = "/method_sample";
		private const ushort SERVICE_PORT = 25;
		
		private static readonly string[] connectArgs = {"unix:abstract=alljoyn",
														"tcp:addr=127.0.0.1,port=9955",
														"launchd:"};
        

		private AllJoyn.BusAttachment msgBus;
		private MyBusListener busListener;
		private MySessionPortListener sessionPortListener;
		private TestBusObject testObj;
		private AllJoyn.InterfaceDescription testIntf;
		public static string serverText;
        public AllJoyn.SessionOpts opts;
       
		class TestBusObject : AllJoyn.BusObject
		{
			private AllJoyn.InterfaceDescription.Member chatMember;
			
			public TestBusObject(AllJoyn.BusAttachment bus, string path) : base(bus, path, false)
			{
			
				AllJoyn.InterfaceDescription exampleIntf = bus.GetInterface(INTERFACE_NAME);
				AllJoyn.QStatus status = AddInterface(exampleIntf);
				if(!status)
				{
					serverText += "Server Failed to add interface " + status.ToString() + "\n";
					Debug.Log("Server Failed to add interface " + status.ToString());
				}

				AllJoyn.InterfaceDescription.Member catMember = exampleIntf.GetMember("cat");
				status = AddMethodHandler(catMember, this.Cat);
				if(!status)
				{
					serverText +="Server Failed to add method handler " + status.ToString() + "\n";
					Debug.Log("Server Failed to add method handler " + status.ToString());
				}
				
				chatMember = exampleIntf.GetMember("chat");
			}

			protected override void OnObjectRegistered ()
			{
			
				serverText += "Server ObjectRegistered has been called\n";
				Debug.Log("Server ObjectRegistered has been called");
			}

			protected void Cat(AllJoyn.InterfaceDescription.Member member, AllJoyn.Message message)
			{
			
				string outStr = (string)message[0] + (string)message[1];
				AllJoyn.MsgArgs outArgs = new AllJoyn.MsgArgs(1);
				outArgs[0] = outStr;

				AllJoyn.QStatus status = MethodReply(message, outArgs);
				if(!status)
				{
					serverText += "Server Ping: Error sending reply\n";
					Debug.Log("Server Ping: Error sending reply");
				}
			}
			
			public void SendChatSignal(string msg) {
				AllJoyn.MsgArgs payload = new AllJoyn.MsgArgs(1);
				payload[0] = msg;
				AllJoyn.QStatus status = Signal(null, theSessionId, chatMember, payload, 0, 0);
				if(!status) {
					Debug.Log("Server failed to send signal: "+status.ToString());	
				}
			}
		}

		class MyBusListener : AllJoyn.BusListener
		{

            protected override void ListenerRegistered(AllJoyn.BusAttachment busAttachment)
            {
                serverText += "Server ListenerRegistered: busAttachment=" + busAttachment + "\n";
                Debug.Log("Server ListenerRegistered: busAttachment=" + busAttachment);
            }

			protected override void NameOwnerChanged(string busName, string previousOwner, string newOwner)
			{
			
				if(string.Compare(SERVICE_NAME, busName) == 0)
				{
					serverText += "Server NameOwnerChanged: name=" + busName + ", oldOwner=" +
						previousOwner + ", newOwner=" + newOwner + "\n";
					Debug.Log("Server NameOwnerChanged: name=" + busName + ", oldOwner=" +
						previousOwner + ", newOwner=" + newOwner);
				}
			}
		}

		class MySessionPortListener : AllJoyn.SessionPortListener
		{
			protected override bool AcceptSessionJoiner(ushort sessionPort, string joiner, AllJoyn.SessionOpts opts)
			{
			
				if (sessionPort != SERVICE_PORT)
				{
					serverText += "Server Rejecting join attempt on unexpected session port " + sessionPort + "\n";
					Debug.Log("Server Rejecting join attempt on unexpected session port " + sessionPort);
					return false;
				}
				serverText += "Server Accepting join session request from " + joiner + 
					" (opts.proximity=" + opts.Proximity + ", opts.traffic=" + opts.Traffic + 
					", opts.transports=" + opts.Transports + ")\n";
				Debug.Log("Server Accepting join session request from " + joiner + 
					" (opts.proximity=" + opts.Proximity + ", opts.traffic=" + opts.Traffic + 
					", opts.transports=" + opts.Transports + ")");
				return true;
			}
					
			protected override void SessionJoined(ushort sessionPort, uint sessionId, string joiner)
			{
						Debug.Log("Session Joined!!!!!!");
				theSessionId = sessionId;
			}
		}
				
		private static uint theSessionId = 0;

		public BasicServer()
		{
			serverText = "";
			// Create message bus
			msgBus = new AllJoyn.BusAttachment("myApp", true);

			// Add org.alljoyn.Bus.method_sample interface
			AllJoyn.QStatus status = msgBus.CreateInterface(INTERFACE_NAME, false, out testIntf);
			if(status)
			{
			
				serverText += "Server Interface Created.\n";
				Debug.Log("Server Interface Created.");
				testIntf.AddMember(AllJoyn.Message.Type.MethodCall, "cat", "ss", "s", "inStr1,inStr2,outStr");
				testIntf.AddSignal("chat", "s", "str", 0);
				testIntf.Activate();
			}
			else
			{
				serverText += "Failed to create interface 'org.alljoyn.Bus.method_sample'\n";
				Debug.Log("Failed to create interface 'org.alljoyn.Bus.method_sample'");
			}

			// Create a bus listener
			busListener = new MyBusListener();
			if(status)
			{
			
				msgBus.RegisterBusListener(busListener);
				serverText += "Server BusListener Registered.\n";
				Debug.Log("Server BusListener Registered.");
			}

			// Set up bus object
			testObj = new TestBusObject(msgBus, SERVICE_PATH);
			// Start the msg bus
			if(status)
			{
			
				status = msgBus.Start();
				if(status)
				{
					serverText += "Server BusAttachment started.\n";
					Debug.Log("Server BusAttachment started.");
					msgBus.RegisterBusObject(testObj);

					for (int i = 0; i < connectArgs.Length; ++i)
					{
						status = msgBus.Connect(connectArgs[i]);
						if (status)
						{
							serverText += "BusAttchement.Connect(" + connectArgs[i] + ") SUCCEDED.\n";
							Debug.Log("BusAttchement.Connect(" + connectArgs[i] + ") SUCCEDED.");
							break;
						}
						else
						{
							serverText += "BusAttachment.Connect(" + connectArgs[i] + ") failed.\n";
							Debug.Log("BusAttachment.Connect(" + connectArgs[i] + ") failed.");
						}
					}
					if(!status)
					{
						serverText += "BusAttachment.Connect failed.\n";
						Debug.Log("BusAttachment.Connect failed.");
					}
				}
				else
				{
					serverText += "Server BusAttachment.Start failed.\n";
					Debug.Log("Server BusAttachment.Start failed.");
				}
			}
			
			// Request name
			if(status)
			{
			
				status = msgBus.RequestName(SERVICE_NAME,
					AllJoyn.DBus.NameFlags.ReplaceExisting | AllJoyn.DBus.NameFlags.DoNotQueue);
				if(!status)
				{
					serverText +="Server RequestName(" + SERVICE_NAME + ") failed (status=" + status + ")\n";
					Debug.Log("Server RequestName(" + SERVICE_NAME + ") failed (status=" + status + ")");
				}
			}

			// Create session
			opts = new AllJoyn.SessionOpts(AllJoyn.SessionOpts.TrafficType.Messages, false,
					AllJoyn.SessionOpts.ProximityType.Any, AllJoyn.TransportMask.Any);
			if(status)
			{
			
				ushort sessionPort = SERVICE_PORT;
				sessionPortListener = new MySessionPortListener();
				status = msgBus.BindSessionPort(ref sessionPort, opts, sessionPortListener);
				if(!status || sessionPort != SERVICE_PORT)
				{
					serverText += "Server BindSessionPort failed (" + status + ")\n";
					Debug.Log("Server BindSessionPort failed (" + status + ")");
				}
			}

			// Advertise name
			if(status)
			{
			
				status = msgBus.AdvertiseName(SERVICE_NAME, opts.Transports);
				if(!status)
				{
					serverText += "Server Failed to advertise name " + SERVICE_NAME + " (" + status + ")\n";
					Debug.Log("Server Failed to advertise name " + SERVICE_NAME + " (" + status + ")");
				}
			}
            serverText += "Completed BasicService Constructor\n";
            Debug.Log("Completed BasicService Constructor");
		}

		public bool KeepRunning
		{
			get
			{
				return true;
			}
		}
		
		public void SendTheMsg() {
			if(theSessionId != 0) {
				testObj.SendChatSignal("this is a signal test");
			}
		}
	}
}
