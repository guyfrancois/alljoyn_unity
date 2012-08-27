//-----------------------------------------------------------------------
// <copyright file="SessionTest.cs" company="Qualcomm Innovation Center, Inc.">
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
    public class SessionTest
    {
        const string INTERFACE_NAME = "org.alljoyn.test.SessionTest";
        const string OBJECT_NAME = "org.alljoyn.test.SessionTest";
        const string OBJECT_PATH = "/org/alljoyn/test/SessionTest";
        const ushort SERVICE_PORT = 25;

        public TimeSpan MaxWaitTime = TimeSpan.FromSeconds(5);

        AllJoyn.BusAttachment hostBus;
        AllJoyn.BusAttachment memberOneBus;
        AllJoyn.BusAttachment memberTwoBus;

        bool acceptSessionJoinerFlag;
        bool sessionJoinedFlag;
        bool foundAdvertisedNameFlag;
        bool sessionMemberAddedFlag;
        bool sessionMemberRemovedFlag;
        bool sessionLostFlag;

        [Fact]
        public void TestSessionJoined()
        {
            AllJoyn.QStatus status = AllJoyn.QStatus.FAIL;

            // create+start+connect bus attachment
            AllJoyn.BusAttachment servicebus = null;
            servicebus = new AllJoyn.BusAttachment("SessionTestService", true);
            Assert.NotNull(servicebus);

            status = servicebus.Start();
            Assert.Equal(AllJoyn.QStatus.OK, status);

            status = servicebus.Connect(AllJoynTestCommon.GetConnectSpec());
            Assert.Equal(AllJoyn.QStatus.OK, status);

            // Create session
            AllJoyn.SessionOpts opts = new AllJoyn.SessionOpts(
                AllJoyn.SessionOpts.TrafficType.Messages, false,
                AllJoyn.SessionOpts.ProximityType.Any, AllJoyn.TransportMask.Any);
            ushort sessionPort = SERVICE_PORT;

            // create the session port listener
            AllJoyn.SessionPortListener sessionPortListener = new TestSessionPortListener(this);

            // bind to the session port
            status = servicebus.BindSessionPort(ref sessionPort, opts, sessionPortListener);
            Assert.Equal(AllJoyn.QStatus.OK, status);

            // request name
            status = servicebus.RequestName(OBJECT_NAME, AllJoyn.DBus.NameFlags.ReplaceExisting | AllJoyn.DBus.NameFlags.DoNotQueue);
            Assert.Equal(AllJoyn.QStatus.OK, status);

            // Advertise name
            status = servicebus.AdvertiseName(OBJECT_NAME, opts.Transports);
            Assert.Equal(AllJoyn.QStatus.OK, status);

            ///////////////////////////////////////////////////////////
            foundAdvertisedNameFlag = false;
            acceptSessionJoinerFlag = false;
            sessionJoinedFlag = false;

            // try to join the session & verify callbacks are called
            AllJoyn.BusAttachment bus = null;
            bus = new AllJoyn.BusAttachment("SessionTest", true);
            Assert.NotNull(bus);

            status = bus.Start();
            Assert.Equal(AllJoyn.QStatus.OK, status);

            status = bus.Connect(AllJoynTestCommon.GetConnectSpec());
            Assert.Equal(AllJoyn.QStatus.OK, status);

            // register the bus listener
            AllJoyn.BusListener busListener = new TestBusListener(this);
            bus.RegisterBusListener(busListener);

            // find the advertised name from the "servicebus"
            status = bus.FindAdvertisedName(OBJECT_NAME);
            Assert.Equal(AllJoyn.QStatus.OK, status);
            EventWaitHandle ewh = new EventWaitHandle(false, EventResetMode.AutoReset, "FoundAdvertisedName");
            ewh.WaitOne(MaxWaitTime);
            Assert.Equal(true, foundAdvertisedNameFlag);

            // try to join & verify that the sessionedJoined callback was called
            uint sSessionId;
            status = bus.JoinSession(OBJECT_NAME, SERVICE_PORT, null, out sSessionId, opts);
            Assert.Equal(AllJoyn.QStatus.OK, status);
            ewh = new EventWaitHandle(false, EventResetMode.AutoReset, "SessionJoined");
            ewh.WaitOne(MaxWaitTime);
            Assert.Equal(true, acceptSessionJoinerFlag);
            Assert.Equal(true, sessionJoinedFlag);

            servicebus.Dispose();
            bus.Dispose();

        }

        [Fact]
        public void TestSessionMemberAddRemove()
        {
            AllJoyn.QStatus status = AllJoyn.QStatus.FAIL;
            sessionMemberAddedFlag = false;

            ///////////////////////////////////////////////////////////
            // Setup the session host
            ///////////////////////////////////////////////////////////
            SetupHost();
            // Create session
            AllJoyn.SessionOpts opts = new AllJoyn.SessionOpts(
                AllJoyn.SessionOpts.TrafficType.Messages, true,
                AllJoyn.SessionOpts.ProximityType.Any, AllJoyn.TransportMask.Any);
            ushort sessionPort = SERVICE_PORT;

            // create the session port listener
            AllJoyn.SessionPortListener sessionPortListener = new TestSessionPortListener(this);

            // bind to the session port
            status = hostBus.BindSessionPort(ref sessionPort, opts, sessionPortListener);
            Assert.Equal(AllJoyn.QStatus.OK, status);

            // request name
            status = hostBus.RequestName(OBJECT_NAME, AllJoyn.DBus.NameFlags.ReplaceExisting | AllJoyn.DBus.NameFlags.DoNotQueue);
            Assert.Equal(AllJoyn.QStatus.OK, status);

            // Advertise name
            status = hostBus.AdvertiseName(OBJECT_NAME, opts.Transports);
            Assert.Equal(AllJoyn.QStatus.OK, status);

            ///////////////////////////////////////////////////////////
            // Setup session member one
            ///////////////////////////////////////////////////////////
            SetupMemberOne();
            // register sessionMemberOne's bus listener
            AllJoyn.BusListener busListenerMemberOne = new TestBusListener(this);
            memberOneBus.RegisterBusListener(busListenerMemberOne);
            // create the session listener
            AllJoyn.SessionListener sessionListener = new TestSessionListener(this);

            ///////////////////////////////////////////////////////////
            // Setup session member two
            ///////////////////////////////////////////////////////////
            SetupMemberTwo();
            AllJoyn.BusListener busListenerMemberTwo = new TestBusListener(this);
            memberTwoBus.RegisterBusListener(busListenerMemberTwo);

            ///////////////////////////////////////////////////////////
            // have sessionMemberOne find and join the session  
            foundAdvertisedNameFlag = false;
            status = memberOneBus.FindAdvertisedName(OBJECT_NAME);  // find the advertised name from the "hostbus"
            Assert.Equal(AllJoyn.QStatus.OK, status);
            EventWaitHandle ewh = new EventWaitHandle(false, EventResetMode.AutoReset, "FoundAdvertisedName");
            ewh.WaitOne(MaxWaitTime);
            Assert.Equal(true, foundAdvertisedNameFlag);

            uint sSessionId;
            acceptSessionJoinerFlag = false;
            sessionJoinedFlag = false;
            status = memberOneBus.JoinSession(OBJECT_NAME, SERVICE_PORT, sessionListener, out sSessionId, opts);
            Assert.Equal(AllJoyn.QStatus.OK, status);
            ewh = new EventWaitHandle(false, EventResetMode.AutoReset, "SessionJoined");
            ewh.WaitOne(MaxWaitTime);

            // verify that sessionMemberOne joined by checking that the sessionedJoined callback was called
            Assert.Equal(true, acceptSessionJoinerFlag);
            Assert.Equal(true, sessionJoinedFlag);

            ///////////////////////////////////////////////////////////
            // have session member two find and join the session
            sessionMemberAddedFlag = false;
            foundAdvertisedNameFlag = false;
            status = memberTwoBus.FindAdvertisedName(OBJECT_NAME);  // find the advertised name from the "hostbus"
            Assert.Equal(AllJoyn.QStatus.OK, status);
            ewh = new EventWaitHandle(false, EventResetMode.AutoReset, "FoundAdvertisedName");
            ewh.WaitOne(MaxWaitTime);
            Assert.Equal(true, foundAdvertisedNameFlag);

            acceptSessionJoinerFlag = false;
            sessionJoinedFlag = false;
            status = memberTwoBus.JoinSession(OBJECT_NAME, SERVICE_PORT, null, out sSessionId, opts);
            Assert.Equal(AllJoyn.QStatus.OK, status);
            ewh = new EventWaitHandle(false, EventResetMode.AutoReset, "SessionJoined");
            ewh.WaitOne(MaxWaitTime);

            // verify that sessionMemberTwo joined by checking that the sessionedJoined callback was called
            Assert.Equal(true, acceptSessionJoinerFlag);
            Assert.Equal(true, sessionJoinedFlag);

            ///////////////////////////////////////////////////////////                     
            // Now that sessionMemberTwo has joined, the SessionMemberAdded callback should have been triggered
            ewh = new EventWaitHandle(false, EventResetMode.AutoReset, "SessionMemberAdded");
            ewh.WaitOne(MaxWaitTime);
            Assert.Equal(true, sessionMemberAddedFlag);

            ///////////////////////////////////////////////////////////
            // Now have sessionMemberTwo leave & verify SessionMemberRemoved callback is triggered
            sessionMemberRemovedFlag = false;
            memberTwoBus.LeaveSession(sSessionId);
            ewh = new EventWaitHandle(false, EventResetMode.AutoReset, "SessionMemberRemoved");
            ewh.WaitOne(MaxWaitTime);
            Assert.Equal(true, sessionMemberRemovedFlag);


            memberOneBus.Dispose();
            memberTwoBus.Dispose();
            hostBus.Dispose();

        }

        [Fact]
        public void TestSessionLost()
        {
            AllJoyn.QStatus status = AllJoyn.QStatus.FAIL;

            ///////////////////////////////////////////////////////////
            // Setup the session host
            ///////////////////////////////////////////////////////////
            SetupHost();
            // Create session
            AllJoyn.SessionOpts opts = new AllJoyn.SessionOpts(
                AllJoyn.SessionOpts.TrafficType.Messages, true,
                AllJoyn.SessionOpts.ProximityType.Any, AllJoyn.TransportMask.Any);
            ushort sessionPort = SERVICE_PORT;

            // create the session port listener
            AllJoyn.SessionPortListener sessionPortListener = new TestSessionPortListener(this);

            // bind to the session port
            status = hostBus.BindSessionPort(ref sessionPort, opts, sessionPortListener);
            Assert.Equal(AllJoyn.QStatus.OK, status);

            // request name
            status = hostBus.RequestName(OBJECT_NAME, AllJoyn.DBus.NameFlags.ReplaceExisting | AllJoyn.DBus.NameFlags.DoNotQueue);
            Assert.Equal(AllJoyn.QStatus.OK, status);

            // Advertise name
            status = hostBus.AdvertiseName(OBJECT_NAME, opts.Transports);
            Assert.Equal(AllJoyn.QStatus.OK, status);

            ///////////////////////////////////////////////////////////
            // Setup session member one
            ///////////////////////////////////////////////////////////
            SetupMemberOne();
            // register sessionMemberOne's bus listener
            AllJoyn.BusListener busListenerMemberOne = new TestBusListener(this);
            memberOneBus.RegisterBusListener(busListenerMemberOne);
            // create the session listener
            AllJoyn.SessionListener sessionListener = new TestSessionListener(this);

            ///////////////////////////////////////////////////////////
            // have sessionMemberOne find and join the session  
            foundAdvertisedNameFlag = false;
            status = memberOneBus.FindAdvertisedName(OBJECT_NAME);  // find the advertised name from the "hostbus"
            Assert.Equal(AllJoyn.QStatus.OK, status);
            EventWaitHandle ewh = new EventWaitHandle(false, EventResetMode.AutoReset, "FoundAdvertisedName");
            ewh.WaitOne(MaxWaitTime);
            Assert.Equal(true, foundAdvertisedNameFlag);

            uint sSessionId;
            acceptSessionJoinerFlag = false;
            sessionJoinedFlag = false;
            status = memberOneBus.JoinSession(OBJECT_NAME, SERVICE_PORT, sessionListener, out sSessionId, opts);
            Assert.Equal(AllJoyn.QStatus.OK, status);
            ewh = new EventWaitHandle(false, EventResetMode.AutoReset, "SessionJoined");
            ewh.WaitOne(MaxWaitTime);

            // verify that sessionMemberOne joined by checking that the sessionedJoined callback was called
            Assert.Equal(true, acceptSessionJoinerFlag);
            Assert.Equal(true, sessionJoinedFlag);

            ///////////////////////////////////////////////////////////
            // Now have the host leave & verify SessionLost callback is triggered
            sessionLostFlag = false;
            sessionMemberRemovedFlag = false;
            hostBus.Stop();
            ewh = new EventWaitHandle(false, EventResetMode.AutoReset, "SessionLost");
            ewh.WaitOne(MaxWaitTime);
            Assert.Equal(true, sessionLostFlag);

            // SessionMemberRemoved should also be triggered
            ewh = new EventWaitHandle(false, EventResetMode.AutoReset, "SessionMemberRemoved");
            ewh.WaitOne(MaxWaitTime);
            Assert.Equal(true, sessionMemberRemovedFlag);

            memberOneBus.Dispose();
            hostBus.Dispose();
        }


        private void SetupHost()
        {
            AllJoyn.QStatus status = AllJoyn.QStatus.FAIL;

            // create+start+connect bus attachment
            hostBus = null;
            hostBus = new AllJoyn.BusAttachment("SessionTestHost", true);
            Assert.NotNull(hostBus);

            status = hostBus.Start();
            Assert.Equal(AllJoyn.QStatus.OK, status);

            status = hostBus.Connect(AllJoynTestCommon.GetConnectSpec());
            Assert.Equal(AllJoyn.QStatus.OK, status);


        }

        private void SetupMemberOne()
        {
            AllJoyn.QStatus status = AllJoyn.QStatus.FAIL;

            memberOneBus = null;
            memberOneBus = new AllJoyn.BusAttachment("SessionTestMemberOne", true);
            Assert.NotNull(memberOneBus);

            status = memberOneBus.Start();
            Assert.Equal(AllJoyn.QStatus.OK, status);

            status = memberOneBus.Connect(AllJoynTestCommon.GetConnectSpec());
            Assert.Equal(AllJoyn.QStatus.OK, status);
        }

        private void SetupMemberTwo()
        {
            AllJoyn.QStatus status = AllJoyn.QStatus.FAIL;

            memberTwoBus = null;
            memberTwoBus = new AllJoyn.BusAttachment("SessionTestMemberTwo", true);
            Assert.NotNull(memberTwoBus);

            status = memberTwoBus.Start();
            Assert.Equal(AllJoyn.QStatus.OK, status);

            status = memberTwoBus.Connect(AllJoynTestCommon.GetConnectSpec());
            Assert.Equal(AllJoyn.QStatus.OK, status);
        }

        class TestSessionPortListener : AllJoyn.SessionPortListener
        {
            SessionTest _sessionTest;

            public TestSessionPortListener(SessionTest sessionTest)
            {
                this._sessionTest = sessionTest;
            }

            protected override bool AcceptSessionJoiner(ushort sessionPort, string joiner, AllJoyn.SessionOpts opts)
            {
                Console.WriteLine("AcceptSessionJoiner called");
                _sessionTest.acceptSessionJoinerFlag = true;
                return true;
            }

            protected override void SessionJoined(ushort sessionPort, uint sessionId, string joiner)
            {
                Console.WriteLine("SessionJoined called");
                _sessionTest.sessionJoinedFlag = true;
                EventWaitHandle ewh = new EventWaitHandle(true, EventResetMode.AutoReset, "SessionJoined");
                ewh.Set();
            }

        }

        class TestSessionListener : AllJoyn.SessionListener
        {
            SessionTest _sessionTest;

            public TestSessionListener(SessionTest sessionTest)
            {
                this._sessionTest = sessionTest;
            }

            protected override void SessionMemberAdded(uint sessionId, string uniqueName)
            {
                Console.WriteLine("SessionMemberAdded called");
                _sessionTest.sessionMemberAddedFlag = true;
                EventWaitHandle ewh = new EventWaitHandle(true, EventResetMode.AutoReset, "SessionMemberAdded");
                ewh.Set();
            }

            protected override void SessionMemberRemoved(uint sessionId, string uniqueName)
            {
                Console.WriteLine("SessionMemberRemoved called");
                _sessionTest.sessionMemberRemovedFlag = true;
                EventWaitHandle ewh = new EventWaitHandle(true, EventResetMode.AutoReset, "SessionMemberRemoved");
                ewh.Set();
            }

            protected override void SessionLost(uint sessionId)
            {
                Console.WriteLine("SessionLost called");
                _sessionTest.sessionLostFlag = true;
                EventWaitHandle ewh = new EventWaitHandle(true, EventResetMode.AutoReset, "SessionLost");
                ewh.Set();
            }
        }

        // we need this so that we know when the advertised name has been found
        class TestBusListener : AllJoyn.BusListener
        {
            SessionTest _sessionTest;

            public TestBusListener(SessionTest sessionTest)
            {
                this._sessionTest = sessionTest;
            }

            protected override void FoundAdvertisedName(string name, AllJoyn.TransportMask transport, string namePrefix)
            {
                if (string.Compare(OBJECT_NAME, name) == 0)
                {
                    _sessionTest.foundAdvertisedNameFlag = true;
                    EventWaitHandle ewh = new EventWaitHandle(true, EventResetMode.AutoReset, "FoundAdvertisedName");
                    ewh.Set();
                }

            }

        }
    }


}
