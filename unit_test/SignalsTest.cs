//-----------------------------------------------------------------------
// <copyright file="SignalsTest.cs" company="Qualcomm Innovation Center, Inc.">
// Copyright 2013, Qualcomm Innovation Center, Inc.
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
using System.Threading;
using AllJoynUnity;
using Xunit;

namespace AllJoynUnityTest
{
	public class SignalsTest
	{
		class TestBusObject : AllJoyn.BusObject
		{
			public TestBusObject(string path): base(path)
			{

			}

			public AllJoyn.QStatus SendTestSignal(string destination, uint sessionId,
												AllJoyn.InterfaceDescription.Member member,
												AllJoyn.MsgArg args, ushort timeToLife, byte flags,
												AllJoyn.Message msg) {
				return Signal(destination, sessionId, member, args, timeToLife, flags, msg);
			}
		}
		[Fact]
		public void RegisterUnregisterSessionlessSignals()
		{
			AllJoyn.QStatus status = AllJoyn.QStatus.FAIL;

			// create+start+connect bus attachment
			AllJoyn.BusAttachment bus = null;
			bus = new AllJoyn.BusAttachment("SignalsTest", true);
			Assert.NotNull(bus);

			status = bus.Start();
			Assert.Equal(AllJoyn.QStatus.OK, status);

			status = bus.Connect(AllJoynTestCommon.GetConnectSpec());
			Assert.Equal(AllJoyn.QStatus.OK, status);

			AllJoyn.InterfaceDescription testIntf;
			Assert.Equal(AllJoyn.QStatus.OK, bus.CreateInterface("org.alljoyn.test.signalstest", out testIntf));
			Assert.Equal(AllJoyn.QStatus.OK, testIntf.AddSignal("testSignal", "s", "newName"));
			testIntf.Activate();

			TestBusObject testObj = new TestBusObject("/org/alljoyn/test/signal");
			Assert.Equal(AllJoyn.QStatus.OK, testObj.AddInterface(testIntf));
			Assert.Equal(AllJoyn.QStatus.OK, bus.RegisterBusObject(testObj));

			AllJoyn.InterfaceDescription.Member mySignalMember = testIntf.GetMember("testSignal");

			Assert.Equal(AllJoyn.QStatus.OK, bus.AddMatch("type='signal',sessionless='t',interface='org.alljoyn.test.signalstest,member='testSignal'"));

			AllJoyn.Message msg = new AllJoyn.Message(bus);
			AllJoyn.MsgArg arg = new AllJoyn.MsgArg();

			Assert.Equal(AllJoyn.QStatus.OK, arg.Set("s", "AllJoyn"));
			Assert.Equal(AllJoyn.QStatus.OK, testObj.SendTestSignal("", 0, mySignalMember, arg, 0, AllJoyn.ALLJOYN_FLAG_SESSIONLESS, msg));

			Assert.Equal(AllJoyn.QStatus.OK, testObj.CancelSessionlessMessage(msg.CallSerial));

			Assert.Equal(AllJoyn.QStatus.OK, testObj.SendTestSignal("", 0, mySignalMember, arg, 0, AllJoyn.ALLJOYN_FLAG_SESSIONLESS, msg));
			Assert.Equal(AllJoyn.QStatus.OK, testObj.CancelSessionlessMessage(msg));
		}
	};
}