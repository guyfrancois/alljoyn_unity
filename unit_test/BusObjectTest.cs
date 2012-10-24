//-----------------------------------------------------------------------
// <copyright file="BusObjectTest.cs" company="Qualcomm Innovation Center, Inc.">
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
using System.Threading;
using AllJoynUnity;
using Xunit;

namespace AllJoynUnityTest
{
    public class BusObjectTest
    {
        const string INTERFACE_NAME = "org.alljoyn.test.BusObjectTest";
        const string OBJECT_NAME = "org.alljoyn.test.BusObjectTest";
        const string OBJECT_PATH = "/org/alljoyn/test/BusObjectTest";

        public TimeSpan MaxWaitTime = TimeSpan.FromSeconds(5);

        AutoResetEvent notifyEvent = new AutoResetEvent(false);

        bool objectRegistered;
        bool objectUnregistered;
        bool nameOwnerChangedFlag;

        [Fact]
        public void TestObjectRegisteredUnregistered()
        {
            AllJoyn.QStatus status = AllJoyn.QStatus.FAIL;

            // create+start+connect bus attachment
            AllJoyn.BusAttachment bus = null;
            bus = new AllJoyn.BusAttachment("BusObjectTest", true);
            Assert.NotNull(bus);

            status = bus.Start();
            Assert.Equal(AllJoyn.QStatus.OK, status);

            status = bus.Connect(AllJoynTestCommon.GetConnectSpec());
            Assert.Equal(AllJoyn.QStatus.OK, status);

            // create the bus object
            TestBusObject testBusObject = new TestBusObject(bus, OBJECT_PATH, this);
            objectRegistered = false;
            objectUnregistered = false;

            // test registering the bus object
            status = bus.RegisterBusObject(testBusObject);
            Assert.Equal(AllJoyn.QStatus.OK, status);
            Wait(MaxWaitTime);
            Assert.Equal(true, objectRegistered);

            // test unregistering the bus object
            bus.UnregisterBusObject(testBusObject);
            Wait(MaxWaitTime);
            Assert.Equal(true, objectUnregistered);

            bus.Dispose();

        }

        [Fact]
        public void TestAddMethodHandler()
        {
            AllJoyn.QStatus status = AllJoyn.QStatus.FAIL;

            // create+start+connect bus attachment
            AllJoyn.BusAttachment servicebus = null;
            servicebus = new AllJoyn.BusAttachment("BusObjectTestService", true);
            Assert.NotNull(servicebus);

            status = servicebus.Start();
            Assert.Equal(AllJoyn.QStatus.OK, status);

            status = servicebus.Connect(AllJoynTestCommon.GetConnectSpec());
            Assert.Equal(AllJoyn.QStatus.OK, status);

            // create+activate the interface
            AllJoyn.InterfaceDescription testIntf = null;
            status = servicebus.CreateInterface(INTERFACE_NAME, false, out testIntf);
            Assert.Equal(AllJoyn.QStatus.OK, status);
            Assert.NotNull(testIntf);

            status = testIntf.AddMember(AllJoyn.Message.Type.MethodCall, "ping", "s", "s", "in,out");
            Assert.Equal(AllJoyn.QStatus.OK, status);

            testIntf.Activate();

            // register bus listener
            AllJoyn.BusListener testBusListener = new TestBusListener(this);
            servicebus.RegisterBusListener(testBusListener);

            // create the bus object
            // the MethodTestBusObject constructor adds the interface & a handler for the ping method
            MethodTestBusObject methodTestBusObject = new MethodTestBusObject(servicebus, OBJECT_PATH);

            // register the bus object
            status = servicebus.RegisterBusObject(methodTestBusObject);
            Assert.Equal(AllJoyn.QStatus.OK, status);

            // request name
            nameOwnerChangedFlag = false;
            status = servicebus.RequestName(OBJECT_NAME, AllJoyn.DBus.NameFlags.ReplaceExisting | AllJoyn.DBus.NameFlags.DoNotQueue);
            Assert.Equal(AllJoyn.QStatus.OK, status);
            Wait(MaxWaitTime);
            Assert.Equal(true, nameOwnerChangedFlag);

            ///////////////////////////////////////////////////////////

            // create the proxy bus object & call methods
            AllJoyn.BusAttachment bus = null;
            bus = new AllJoyn.BusAttachment("BusObjectTest", true);
            Assert.NotNull(bus);

            status = bus.Start();
            Assert.Equal(AllJoyn.QStatus.OK, status);

            status = bus.Connect(AllJoynTestCommon.GetConnectSpec());
            Assert.Equal(AllJoyn.QStatus.OK, status);

            // create+activate the interface
            AllJoyn.InterfaceDescription iFace = null;
            status = bus.CreateInterface(INTERFACE_NAME, false, out iFace);
            Assert.Equal(AllJoyn.QStatus.OK, status);
            Assert.NotNull(iFace);

            status = iFace.AddMember(AllJoyn.Message.Type.MethodCall, "ping", "s", "s", "in,out");
            Assert.Equal(AllJoyn.QStatus.OK, status);

            iFace.Activate();

            AllJoyn.ProxyBusObject proxyBusObject = new AllJoyn.ProxyBusObject(bus, OBJECT_NAME, OBJECT_PATH, 0);
            status = proxyBusObject.AddInterface(iFace);
            Assert.Equal(AllJoyn.QStatus.OK, status);

            AllJoyn.MsgArgs input = new AllJoyn.MsgArgs(1);
            input[0].Set("AllJoyn");
            AllJoyn.Message replyMsg = new AllJoyn.Message(bus);
            status = proxyBusObject.MethodCallSynch(INTERFACE_NAME, "ping", input, replyMsg, 5000, 0);
            Assert.Equal(AllJoyn.QStatus.OK, status);
            Assert.Equal("AllJoyn", (string)replyMsg[0]);

            methodTestBusObject.Dispose();
            servicebus.Dispose();

            // TODO: need to call dispose on proxyBusObject first otherwise you get an AccessViolation???
            proxyBusObject.Dispose();
            bus.Dispose();
        }


        private void Wait(TimeSpan timeout)
        {
            notifyEvent.WaitOne(timeout);
            notifyEvent.Reset();
        }

        class TestBusListener : AllJoyn.BusListener
        {
            BusObjectTest _busObjectTest;

            public TestBusListener(BusObjectTest busObjectTest)
            {
                this._busObjectTest = busObjectTest;
            }

            protected override void NameOwnerChanged(string busName, string previousOwner, string newOwner)
            {
                _busObjectTest.nameOwnerChangedFlag = true;
                Notify();
            }

            private void Notify()
            {
                _busObjectTest.notifyEvent.Set();
            }
        }

        class TestBusObject : AllJoyn.BusObject
        {
            BusObjectTest _busObjectTest;

            public TestBusObject(AllJoyn.BusAttachment bus, string path, BusObjectTest busObjectTest)
                : base(path, false)
            {
                _busObjectTest = busObjectTest;
            }

            protected override void OnObjectRegistered()
            {
                _busObjectTest.objectRegistered = true;
                Notify();
            }

            protected override void OnObjectUnregistered()
            {
                _busObjectTest.objectUnregistered = true;
                Notify();
            }

            private void Notify()
            {
                _busObjectTest.notifyEvent.Set();
            }
        }

        class MethodTestBusObject : AllJoyn.BusObject
        {
            public MethodTestBusObject(AllJoyn.BusAttachment bus, string path)
                : base(path, false)
            {

                // add the interface to the bus object
                AllJoyn.InterfaceDescription testIntf = bus.GetInterface(INTERFACE_NAME);
                AllJoyn.QStatus status = AddInterface(testIntf);
                Assert.Equal(AllJoyn.QStatus.OK, status);

                // register a method handler for the ping method
                AllJoyn.InterfaceDescription.Member pingMember = testIntf.GetMember("ping");
                status = AddMethodHandler(pingMember, this.Ping);
                Assert.Equal(AllJoyn.QStatus.OK, status);
            }

            protected void Ping(AllJoyn.InterfaceDescription.Member member, AllJoyn.Message message)
            {
                string outStr = (string)message[0];
                AllJoyn.MsgArgs outArgs = new AllJoyn.MsgArgs(1);
                outArgs[0] = outStr;

                AllJoyn.QStatus status = MethodReply(message, outArgs);
                Assert.Equal(AllJoyn.QStatus.OK, status);
            }
        }
    }


}
