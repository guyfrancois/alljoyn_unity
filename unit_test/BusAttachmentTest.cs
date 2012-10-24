//-----------------------------------------------------------------------
// <copyright file="BusAttachmentTest.cs" company="Qualcomm Innovation Center, Inc.">
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

    public class BusAttachmentTest
    {

        AutoResetEvent notifyEventOne = new AutoResetEvent(false);
        AutoResetEvent notifyEventTwo = new AutoResetEvent(false);

        private bool handledSignalsOne;
        private bool handledSignalsTwo;
        private string signalOneMsg;
        private string signalTwoMsg;

        [Fact]
        public void TestCreateInterface()
        {
            AllJoyn.QStatus status = AllJoyn.QStatus.FAIL;
            AllJoyn.BusAttachment bus = null;
            bus = new AllJoyn.BusAttachment("BusAttachmentTest", true);
            Assert.NotNull(bus);

            AllJoyn.InterfaceDescription testIntf = null;
            status = bus.CreateInterface("org.alljoyn.test.BusAttachment", false, out testIntf);
            Assert.Equal(AllJoyn.QStatus.OK, status);
            Assert.NotNull(testIntf);

            // TODO: move these into a teardown method?
            bus.Dispose();
        }

        [Fact]
        public void TestDeleteInterface()
        {
            AllJoyn.QStatus status = AllJoyn.QStatus.FAIL;
            AllJoyn.BusAttachment bus = null;
            bus = new AllJoyn.BusAttachment("BusAttachmentTest", true);
            Assert.NotNull(bus);

            AllJoyn.InterfaceDescription testIntf = null;
            status = bus.CreateInterface("org.alljoyn.test.BusAttachment", false, out testIntf);
            Assert.Equal(AllJoyn.QStatus.OK, status);
            Assert.NotNull(testIntf);
            status = bus.DeleteInterface(testIntf);
            Assert.Equal(AllJoyn.QStatus.OK, status);

            // TODO: move these into a teardown method?
            bus.Dispose();
        }

        [Fact]
        public void TestStartAndStop()
        {
            AllJoyn.QStatus status = AllJoyn.QStatus.FAIL;
            AllJoyn.BusAttachment bus = null;
            bus = new AllJoyn.BusAttachment("BusAttachmentTest", true);
            Assert.NotNull(bus);

            status = bus.Start();
            Assert.Equal(AllJoyn.QStatus.OK, status);
            status = bus.Stop();
            Assert.Equal(AllJoyn.QStatus.OK, status);

            // TODO: move these into a teardown method?
            bus.Dispose();
        }

        //[Fact]
        public void TestRegisterSignalHandler()
        {
            AllJoyn.QStatus status = AllJoyn.QStatus.FAIL;

            // create bus attachment
            AllJoyn.BusAttachment bus = null;
            bus = new AllJoyn.BusAttachment("BusAttachmentTest", true);
            Assert.NotNull(bus);

            // create the interface description
            AllJoyn.InterfaceDescription testIntf = null;
            status = bus.CreateInterface("org.alljoyn.test.BusAttachment", false, out testIntf);
            Assert.Equal(AllJoyn.QStatus.OK, status);
            Assert.NotNull(testIntf);

            // add the signal member to the interface
            status = testIntf.AddSignal("testSignal", "s", "msg", 0);
            Assert.Equal(AllJoyn.QStatus.OK, status);

            // activate the interface
            testIntf.Activate();

            // start the bus attachment
            status = bus.Start();
            Assert.Equal(AllJoyn.QStatus.OK, status);

            // connect to the bus
            status = bus.Connect(AllJoynTestCommon.GetConnectSpec());

            Assert.Equal(AllJoyn.QStatus.OK, status);

            // create the bus object &
            // add the interface to the bus object
            TestBusObject testBusObject = new TestBusObject(bus, "/test");

            // get the signal member from the interface description
            AllJoyn.InterfaceDescription.Member testSignalMember = testIntf.GetMember("testSignal");

            // register the signal handler
            status = bus.RegisterSignalHandler(this.TestSignalHandlerOne, testSignalMember, null);
            Assert.Equal(AllJoyn.QStatus.OK, status);

            // add match for the signal
            status = bus.AddMatch("type='signal',member='testSignal'");
            Assert.Equal(AllJoyn.QStatus.OK, status);

            handledSignalsOne = false;
            signalOneMsg = null;

            // send a signal
            testBusObject.SendTestSignal("test msg");

            // wait to see if we receive the signal
            WaitEventOne(TimeSpan.FromSeconds(2));

            Assert.Equal(true, handledSignalsOne);
            Assert.Equal("test msg", signalOneMsg);


            // TODO: move these into a teardown method?
            bus.Dispose();
        }

        //[Fact]
        public void TestUnregisterSignalHandler()
        {
            AllJoyn.QStatus status = AllJoyn.QStatus.FAIL;

            // create bus attachment
            AllJoyn.BusAttachment bus = null;
            bus = new AllJoyn.BusAttachment("BusAttachmentTest", true);
            Assert.NotNull(bus);

            // create the interface description
            AllJoyn.InterfaceDescription testIntf = null;
            status = bus.CreateInterface("org.alljoyn.test.BusAttachment", false, out testIntf);
            Assert.Equal(AllJoyn.QStatus.OK, status);
            Assert.NotNull(testIntf);

            // add the signal member to the interface
            status = testIntf.AddSignal("testSignal", "s", "msg", 0);
            Assert.Equal(AllJoyn.QStatus.OK, status);

            // activate the interface
            testIntf.Activate();

            // start the bus attachment
            status = bus.Start();
            Assert.Equal(AllJoyn.QStatus.OK, status);

            // connect to the bus
            status = bus.Connect(AllJoynTestCommon.GetConnectSpec());
            Assert.Equal(AllJoyn.QStatus.OK, status);

            // create the bus object &
            // add the interface to the bus object
            TestBusObject testBusObject = new TestBusObject(bus, "/test");

            // get the signal member from the interface description
            AllJoyn.InterfaceDescription.Member testSignalMember = testIntf.GetMember("testSignal");

            // register both signal handlers
            status = bus.RegisterSignalHandler(this.TestSignalHandlerOne, testSignalMember, null);
            Assert.Equal(AllJoyn.QStatus.OK, status);
            status = bus.RegisterSignalHandler(this.TestSignalHandlerTwo, testSignalMember, null);
            Assert.Equal(AllJoyn.QStatus.OK, status);

            // add match for the signal
            status = bus.AddMatch("type='signal',member='testSignal'");
            Assert.Equal(AllJoyn.QStatus.OK, status);

            handledSignalsOne = false;
            handledSignalsTwo = false;
            signalOneMsg = null;
            signalTwoMsg = null;

            // send a signal
            testBusObject.SendTestSignal("test msg");

            // wait to see if we receive the signal
            WaitEventOne(TimeSpan.FromSeconds(2));
            WaitEventTwo(TimeSpan.FromSeconds(2));

            // make sure that both handlers got the signal
            Assert.Equal(true, handledSignalsOne);
            Assert.Equal("test msg", signalOneMsg);
            Assert.Equal(true, handledSignalsTwo);
            Assert.Equal("test msg", signalTwoMsg);

            // now unregister one handler & make sure it doesn't receive the signal
            handledSignalsOne = false;
            handledSignalsTwo = false;
            signalOneMsg = null;
            signalTwoMsg = null;

            status = bus.UnregisterSignalHandler(this.TestSignalHandlerOne, testSignalMember, null);
            Assert.Equal(AllJoyn.QStatus.OK, status);

            // send another signal
            testBusObject.SendTestSignal("test msg");

            // wait to see if we receive the signal
            WaitEventTwo(TimeSpan.FromSeconds(2));

            // make sure that only the second handler got the signal
            Assert.Equal(false, handledSignalsOne);
            Assert.Null(signalOneMsg);
            Assert.Equal(true, handledSignalsTwo);
            Assert.Equal("test msg", signalTwoMsg);

            // TODO: move these into a teardown method?
            bus.Dispose();
        }

        private void WaitEventOne(TimeSpan timeout)
        {
            notifyEventOne.WaitOne(timeout);
            notifyEventOne.Reset();
        }

        private void WaitEventTwo(TimeSpan timeout)
        {
            notifyEventTwo.WaitOne(timeout);
            notifyEventTwo.Reset();
        }

        private void NotifyEventOne()
        {
            notifyEventOne.Set();
        }

        private void NotifyEventTwo()
        {
            notifyEventTwo.Set();
        }

        public void TestSignalHandlerOne(AllJoyn.InterfaceDescription.Member member, string srcPath, AllJoyn.Message message)
        {
            // mark that the signal was received
            handledSignalsOne = true;
            signalOneMsg = message[0];
            NotifyEventOne();
        }

        public void TestSignalHandlerTwo(AllJoyn.InterfaceDescription.Member member, string srcPath, AllJoyn.Message message)
        {
            // mark that the signal was received
            handledSignalsTwo = true;
            signalTwoMsg = message[0];
            NotifyEventTwo();
        }

        class TestBusObject : AllJoyn.BusObject
        {
            private AllJoyn.InterfaceDescription.Member testSignalMember;

            public TestBusObject(AllJoyn.BusAttachment bus, string path)
                : base(path, false)
            {
                AllJoyn.InterfaceDescription testIntf = bus.GetInterface("org.alljoyn.test.BusAttachment");
                AllJoyn.QStatus status = AddInterface(testIntf);
                Assert.Equal(AllJoyn.QStatus.OK, status);

                testSignalMember = testIntf.GetMember("testSignal");
            }

            public void SendTestSignal(String msg)
            {
                AllJoyn.MsgArgs payload = new AllJoyn.MsgArgs(1);
                payload[0].Set(msg);
                AllJoyn.QStatus status = Signal(null, 0, testSignalMember, payload, 0, 64);
                Assert.Equal(AllJoyn.QStatus.OK, status);
            }
        }
    }
}
