using UnityEngine;
using AllJoynUnity;

namespace basic_clientserver
{
	class BasicClient
	{
		private const string INTERFACE_NAME = "org.alljoyn.Bus.method_sample";
		private const string SERVICE_NAME = "org.alljoyn.Bus.method_sample";
		private const string SERVICE_PATH = "/method_sample";
		private const ushort SERVICE_PORT = 25;
		
		private static readonly string[] connectArgs = {"unix:abstract=alljoyn",
														"tcp:addr=127.0.0.1,port=9955",
														"launchd:"};

		private static bool sJoinComplete = false;
		private bool sJoinCalled = false;
		private static AllJoyn.BusAttachment sMsgBus;
		private static MyBusListener sBusListener;
		private static uint sSessionId;
		public static string clientText;
        private AllJoyn.SessionOpts opts;
		
		private static string sFoundName = null;

		class MyBusListener : AllJoyn.BusListener
		{
			protected override void FoundAdvertisedName(string name, AllJoyn.TransportMask transport, string namePrefix)
			{
				clientText += "Client FoundAdvertisedName(name=" + name + ", prefix=" + namePrefix + ")\n";
				Debug.Log("Client FoundAdvertisedName(name=" + name + ", prefix=" + namePrefix + ")");
				if(string.Compare(SERVICE_NAME, name) == 0)
				{
					sFoundName = name;
				}
			}

			protected override void NameOwnerChanged(string busName, string previousOwner, string newOwner)
			{
			
				if(string.Compare(SERVICE_NAME, busName) == 0)
				{
					clientText += "Client NameOwnerChanged: name=" + busName + ", oldOwner=" +
						previousOwner + ", newOwner=" + newOwner + "\n";
					Debug.Log("Client NameOwnerChanged: name=" + busName + ", oldOwner=" +
						previousOwner + ", newOwner=" + newOwner);
				}
			}
		}

		public BasicClient()
		{
			clientText = "";
			// Create message bus
			sMsgBus = new AllJoyn.BusAttachment("myApp", true);

			// Add org.alljoyn.Bus.method_sample interface
			AllJoyn.InterfaceDescription testIntf;
			AllJoyn.QStatus status = sMsgBus.CreateInterface(INTERFACE_NAME, false, out testIntf);
			if(status)
			{
			
				clientText += "Client Interface Created.\n";
				Debug.Log("Client Interface Created.");
				testIntf.AddMember(AllJoyn.Message.Type.MethodCall, "cat", "ss", "s", "inStr1,inStr2,outStr");
				testIntf.AddSignal("chat", "s", "str", 0);
				testIntf.Activate();
			}
			else
			{
				clientText += "Client Failed to create interface 'org.alljoyn.Bus.method_sample'\n";
				Debug.Log("Client Failed to create interface 'org.alljoyn.Bus.method_sample'");
			}

			// Start the msg bus
			if(status)
			{
			
				status = sMsgBus.Start();
				if(status)
				{
					clientText += "Client BusAttachment started.\n";
					Debug.Log("Client BusAttachment started.");
				}
				else
				{
					clientText += "Client BusAttachment.Start failed.\n";
					Debug.Log("Client BusAttachment.Start failed.");
				}
			}

			// Connect to the bus
			if(status)
			{
			
				for (int i = 0; i < connectArgs.Length; ++i)
				{
					status = sMsgBus.Connect(connectArgs[i]);
					if (status)
					{
						clientText += "BusAttchement.Connect(" + connectArgs[i] + ") SUCCEDED.\n";
						Debug.Log("BusAttchement.Connect(" + connectArgs[i] + ") SUCCEDED.");
						break;
					}
					else
					{
						clientText += "BusAttachment.Connect(" + connectArgs[i] + ") failed.\n";
						Debug.Log("BusAttachment.Connect(" + connectArgs[i] + ") failed.");
					}
				}
				if(!status)
				{
					clientText += "BusAttachment.Connect failed.\n";
					Debug.Log("BusAttachment.Connect failed.");
				}
			}

			// Create a bus listener
			sBusListener = new MyBusListener();

			if(status)
			{
			
				sMsgBus.RegisterBusListener(sBusListener);
				clientText += "Client BusListener Registered.\n";
				Debug.Log("Client BusListener Registered.");
			}
			
			// Begin discovery on the well-known name of the service to be called
			status = sMsgBus.FindAdvertisedName(SERVICE_NAME);
			if(!status)
			{
			
				clientText += "Client org.alljoyn.Bus.FindAdvertisedName failed.\n";
				Debug.Log("Client org.alljoyn.Bus.FindAdvertisedName failed.");
			}
			
			AllJoyn.InterfaceDescription.Member chatMember = testIntf.GetMember("chat");
			status = sMsgBus.RegisterSignalHandler(this.ChatSignalHandler, chatMember, null);
			if(!status)
			{
				clientText +="Client Failed to add signal handler " + status + "\n";
				Debug.Log("Client Failed to add signal handler " + status);
			}
			else {			
				clientText +="Client add signal handler " + status + "\n";
				Debug.Log("Client add signal handler " + status);
			}
			
			status = sMsgBus.AddMatch("type='signal',member='chat'");
			if(!status)
			{
				clientText +="Client Failed to add Match " + status.ToString() + "\n";
				Debug.Log("Client Failed to add Match " + status.ToString());
			}
			else {			
				clientText +="Client add Match " + status.ToString() + "\n";
				Debug.Log("Client add Match " + status.ToString());
			}
		}
		
		public void ConnectToFoundName() {
			sJoinCalled = true;
			// We found a remote bus that is advertising basic service's  well-known name so connect to it
			AllJoyn.SessionOpts opts = new AllJoyn.SessionOpts(AllJoyn.SessionOpts.TrafficType.Messages, false,
				AllJoyn.SessionOpts.ProximityType.Any, AllJoyn.TransportMask.Any);

			AllJoyn.QStatus status = sMsgBus.JoinSession(sFoundName, SERVICE_PORT, null, out sSessionId, opts);
			if(status)
			{
				clientText += "Client JoinSession SUCCESS (Session id=" + sSessionId + ")\n";
				Debug.Log("Client JoinSession SUCCESS (Session id=" + sSessionId + ")");
				sJoinComplete = true;
			}
			else
			{
				clientText += "Client JoinSession failed (status=" + status.ToString() + ")\n";
				Debug.Log("Client JoinSession failed (status=" + status.ToString() + ")");
			}
		}
		
		public void ChatSignalHandler(AllJoyn.InterfaceDescription.Member member, string srcPath, AllJoyn.Message message)
		{
			Debug.Log("Client Chat msg - "+message+": "+message[0]);
			clientText += "Client Chat msg - "+message+": "+message[0]+ "\n";
		}

		public AllJoyn.QStatus Connect()
		{
			// Begin discovery on the well-known name of the service to be called
			AllJoyn.QStatus status = sMsgBus.FindAdvertisedName(SERVICE_NAME);
			if(!status)
			{
			
				clientText += "Client org.alljoyn.Bus.FindAdvertisedName failed.\n";
				Debug.Log("Client org.alljoyn.Bus.FindAdvertisedName failed.");
			}

			return status;
		}

		public bool Connected
		{
			get
			{
				return sJoinComplete;
			}
		}
		
		public bool FoundAdvertisedName
		{
			get
			{
				return sFoundName != null && !sJoinCalled;
			}
		}

		public string CallRemoteMethod()
		{
			using(AllJoyn.ProxyBusObject remoteObj = new AllJoyn.ProxyBusObject(sMsgBus, SERVICE_NAME, SERVICE_PATH, sSessionId))
			{
			
				AllJoyn.InterfaceDescription alljoynTestIntf = sMsgBus.GetInterface(INTERFACE_NAME);
				if(alljoynTestIntf == null)
				{
					//throw new Exception("Client Failed to get test interface.");
					return "";
				}
				else
				{
					remoteObj.AddInterface(alljoynTestIntf);

					AllJoyn.Message reply = new AllJoyn.Message(sMsgBus);
					AllJoyn.MsgArgs inputs = new AllJoyn.MsgArgs(2);
					inputs[0] = "Hello ";
					inputs[1] = "World!";

					AllJoyn.QStatus status = remoteObj.MethodCallSynch(SERVICE_NAME, "cat", inputs, reply, 5000, 0);
					
					if(status)
					{
						clientText += SERVICE_NAME + ".cat(path=" + SERVICE_PATH + ") returned \"" + (string)reply[0] + "\"\n";
						Debug.Log(SERVICE_NAME + ".cat(path=" + SERVICE_PATH + ") returned \"" + (string)reply[0] + "\"");
						return (string)reply[0];
					}
					else
					{
						clientText += "MethodCall on " + SERVICE_NAME + ".cat failed\n";
						Debug.Log("MethodCall on " + SERVICE_NAME + ".cat failed");
						return "";
					}
				}
			}
		}
	}
}
