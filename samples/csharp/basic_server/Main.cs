//-----------------------------------------------------------------------
// <copyright file="Main.cs" company="Qualcomm Innovation Center, Inc.">
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

using System;
using AllJoynUnity;

namespace basic_server
{
	class MainClass
	{
		private const string INTERFACE_NAME = "org.alljoyn.Bus.sample";
		private const string SERVICE_NAME = "org.alljoyn.Bus.sample";
		private const string SERVICE_PATH = "/sample";
		private const ushort SERVICE_PORT = 25;

		private static readonly string[] connectArgs = {"null:"};

		private static AllJoyn.BusAttachment sMsgBus;
		private static MyBusListener sBusListener;
		private static MySessionPortListener sSessionPortListener;

		class TestBusObject : AllJoyn.BusObject
		{
			public TestBusObject(AllJoyn.BusAttachment bus, string path) : base(path, false)
			{
				AllJoyn.InterfaceDescription exampleIntf = bus.GetInterface(INTERFACE_NAME);
				AllJoyn.QStatus status = AddInterface(exampleIntf);
				if(!status)
				{
					Console.WriteLine("Failed to add interface {0}", status);
				}

				AllJoyn.InterfaceDescription.Member catMember = exampleIntf.GetMember("cat");
				status = AddMethodHandler(catMember, this.Cat);
				if(!status)
				{
					Console.WriteLine("Failed to add method handler {0}", status);
				}
			}

			protected override void OnObjectRegistered()
			{
				Console.WriteLine("ObjectRegistered has been called");
			}

			protected void Cat(AllJoyn.InterfaceDescription.Member member, AllJoyn.Message message)
			{
				string outStr = (string)message[0] + (string)message[1];
				AllJoyn.MsgArgs outArgs = new AllJoyn.MsgArgs(1);
				outArgs[0] = outStr;

				AllJoyn.QStatus status = MethodReply(message, outArgs);
				if(!status)
				{
					Console.WriteLine("Ping: Error sending reply");
				}
			}
		}

		class MyBusListener : AllJoyn.BusListener
		{
			protected override void NameOwnerChanged(string busName, string previousOwner, string newOwner)
			{
				if(string.Compare(SERVICE_NAME, busName) == 0)
				{
					Console.WriteLine("NameOwnerChanged: name=" + busName + ", oldOwner=" +
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
					Console.WriteLine("Rejecting join attempt on unexpected session port {0}", sessionPort);
					return false;
				}
				Console.WriteLine("Accepting join session request from {0} (opts.proximity={1}, opts.traffic={2}, opts.transports={3})",
					joiner, opts.Proximity, opts.Traffic, opts.Transports);
				return true;
			}
		}

		public static void Main(string[] args)
		{
			Console.WriteLine("AllJoyn Library version: " + AllJoyn.GetVersion());
			Console.WriteLine("AllJoyn Library buildInfo: " + AllJoyn.GetBuildInfo());

			// Create message bus
			sMsgBus = new AllJoyn.BusAttachment("myApp", true);

			// Add org.alljoyn.Bus.method_sample interface
			AllJoyn.InterfaceDescription testIntf;
			AllJoyn.QStatus status = sMsgBus.CreateInterface(INTERFACE_NAME, false, out testIntf);
			if(status)
			{
				Console.WriteLine("Interface Created.");
				testIntf.AddMember(AllJoyn.Message.Type.MethodCall, "cat", "ss", "s", "inStr1,inStr2,outStr");
				testIntf.Activate();
			}
			else
			{
				Console.WriteLine("Failed to create interface 'org.alljoyn.Bus.method_sample'");
			}

			// Create a bus listener
			sBusListener = new MyBusListener();
			if(status)
			{
				sMsgBus.RegisterBusListener(sBusListener);
				Console.WriteLine("BusListener Registered.");
			}

			// Set up bus object
			TestBusObject testObj = new TestBusObject(sMsgBus, SERVICE_PATH);

			// Start the msg bus
			if(status)
			{
				status = sMsgBus.Start();
				if(status)
				{
					Console.WriteLine("BusAttachment started.");
					sMsgBus.RegisterBusObject(testObj);

					for (int i = 0; i < connectArgs.Length; ++i)
					{
						status = sMsgBus.Connect(connectArgs[i]);
						if (status)
						{
							Console.WriteLine("BusAttchement.Connect(" + connectArgs[i] + ") SUCCEDED.");
							break;
						}
						else
						{
							Console.WriteLine("BusAttachment.Connect(" + connectArgs[i] + ") failed.");
						}
					}
					if (!status)
					{
						Console.WriteLine("BusAttachment.Connect failed.");
					}
				}
				else
				{
					Console.WriteLine("BusAttachment.Start failed.");
				}
			}

			// Request name
			if(status)
			{
				status = sMsgBus.RequestName(SERVICE_NAME,
					AllJoyn.DBus.NameFlags.ReplaceExisting | AllJoyn.DBus.NameFlags.DoNotQueue);
				if(!status)
				{
					Console.WriteLine("RequestName({0}) failed (status={1})", SERVICE_NAME, status);
				}
			}

			// Create session
			AllJoyn.SessionOpts opts = new AllJoyn.SessionOpts(AllJoyn.SessionOpts.TrafficType.Messages, false,
					AllJoyn.SessionOpts.ProximityType.Any, AllJoyn.TransportMask.Any);
			if(status)
			{
				ushort sessionPort = SERVICE_PORT;
				sSessionPortListener = new MySessionPortListener();
				status = sMsgBus.BindSessionPort(ref sessionPort, opts, sSessionPortListener);
				if(!status)
				{
					Console.WriteLine("BindSessionPort failed ({0})", status);
				}
			}

			// Advertise name
			if(status)
			{
				status = sMsgBus.AdvertiseName(SERVICE_NAME, opts.Transports);
				if(!status)
				{
					Console.WriteLine("Failed to advertise name {0} ({1})", SERVICE_NAME, status);
				}
			}

			if(status)
			{
				while(true)
				{
					System.Threading.Thread.Sleep(1);
				}
			}

			// Dispose of objects now
			sMsgBus.Dispose();
			sBusListener.Dispose();

			Console.WriteLine("basic server exiting with status {0} ({1})", status, status.ToString());
		}
	}
}
