//-----------------------------------------------------------------------
// <copyright file="BasicServer.cs" company="Qualcomm Innovation Center, Inc.">
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

		class TestBusObject : AllJoyn.BusObject
		{
			public TestBusObject(AllJoyn.BusAttachment bus, string path) : base(bus, path, false)
			{
				AllJoyn.InterfaceDescription exampleIntf = bus.GetInterface(INTERFACE_NAME);
				AllJoyn.QStatus status = AddInterface(exampleIntf);
				if(!status)
				{
					Console.WriteLine("Server Failed to add interface {0}", status);
				}

				AllJoyn.InterfaceDescription.Member catMember = exampleIntf.GetMember("cat");
				status = AddMethodHandler(catMember, this.Cat);
				if(!status)
				{
					Console.WriteLine("Server Failed to add method handler {0}", status);
				}
			}

			protected override void OnObjectRegistered()
			{
				Console.WriteLine("Server ObjectRegistered has been called");
			}

			protected void Cat(AllJoyn.InterfaceDescription.Member member, AllJoyn.Message message)
			{
				string outStr = (string)message[0] + (string)message[1];
				AllJoyn.MsgArgs outArgs = new AllJoyn.MsgArgs(1);
				outArgs[0] = outStr;

				AllJoyn.QStatus status = MethodReply(message, outArgs);
				if(!status)
				{
					Console.WriteLine("Server Ping: Error sending reply");
				}
			}
		}

		class MyBusListener : AllJoyn.BusListener
		{
			protected override void NameOwnerChanged(string busName, string previousOwner, string newOwner)
			{
				//if(string.Compare(SERVICE_NAME, busName) == 0)
				{
					Console.WriteLine("Server NameOwnerChanged: name=" + busName + ", oldOwner=" +
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
					Console.WriteLine("Server Rejecting join attempt on unexpected session port {0}", sessionPort);
					return false;
				}
				Console.WriteLine("Server Accepting join session request from {0} (opts.proximity={1}, opts.traffic={2}, opts.transports={3})",
					joiner, opts.Proximity, opts.Traffic, opts.Transports);
				return true;
			}
		}

		public BasicServer()
		{
			// Create message bus
			msgBus = new AllJoyn.BusAttachment("myApp", true);

			// Add org.alljoyn.Bus.method_sample interface
			AllJoyn.QStatus status = msgBus.CreateInterface(INTERFACE_NAME, false, out testIntf);
			if(status)
			{
				Console.WriteLine("Server Interface Created.");
				testIntf.AddMember(AllJoyn.Message.Type.MethodCall, "cat", "ss", "s", "inStr1,inStr2,outStr");
				testIntf.Activate();
			}
			else
			{
				Console.WriteLine("Failed to create interface 'org.alljoyn.Bus.method_sample'");
			}

			// Create a bus listener
			busListener = new MyBusListener();
			if(status)
			{
				msgBus.RegisterBusListener(busListener);
				Console.WriteLine("Server BusListener Registered.");
			}

			// Set up bus object
			testObj = new TestBusObject(msgBus, SERVICE_PATH);

			// Start the msg bus
			if(status)
			{
				status = msgBus.Start();
				if(status)
				{
					Console.WriteLine("Server BusAttachment started.");
					msgBus.RegisterBusObject(testObj);

					for (int i = 0; i < connectArgs.Length; ++i)
					{
						status = msgBus.Connect(connectArgs[i]);
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
					Console.WriteLine("Server BusAttachment.Start failed.");
				}
			}

			// Request name
			if(status)
			{
				status = msgBus.RequestName(SERVICE_NAME,
					AllJoyn.DBus.NameFlags.ReplaceExisting | AllJoyn.DBus.NameFlags.DoNotQueue);
				if(!status)
				{
					Console.WriteLine("Server RequestName({0}) failed (status={1})", SERVICE_NAME, status);
				}
			}

			// Create session
			AllJoyn.SessionOpts opts = new AllJoyn.SessionOpts(AllJoyn.SessionOpts.TrafficType.Messages, false,
					AllJoyn.SessionOpts.ProximityType.Any, AllJoyn.TransportMask.Any);
			if(status)
			{
				ushort sessionPort = SERVICE_PORT;
				sessionPortListener = new MySessionPortListener();
				status = msgBus.BindSessionPort(ref sessionPort, opts, sessionPortListener);
				if(!status || sessionPort != SERVICE_PORT)
				{
					Console.WriteLine("Server BindSessionPort failed ({0})", status);
				}
			}

			// Advertise name
			if(status)
			{
				status = msgBus.AdvertiseName(SERVICE_NAME, opts.Transports);
				if(!status)
				{
					Console.WriteLine("Server Failed to advertise name {0} ({1})", SERVICE_NAME, status);
				}
			}
            Console.WriteLine("Should have setup server");
		}

		public bool KeepRunning
		{
			get
			{
				return true;
			}
		}
	}
}