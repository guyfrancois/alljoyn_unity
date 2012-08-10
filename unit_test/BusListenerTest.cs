/******************************************************************************
 * Copyright 2012, Qualcomm Innovation Center, Inc.
 *
 *    Licensed under the Apache License, Version 2.0 (the "License");
 *    you may not use this file except in compliance with the License.
 *    You may obtain a copy of the License at
 *
 *        http://www.apache.org/licenses/LICENSE-2.0
 *
 *    Unless required by applicable law or agreed to in writing, software
 *    distributed under the License is distributed on an "AS IS" BASIS,
 *    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *    See the License for the specific language governing permissions and
 *    limitations under the License.
 ******************************************************************************/

using System;
using System.Threading;
using AllJoynUnity;
using Xunit;

namespace AllJoynUnityTest
{
    public class BusListenerTest
    {
        public const string ObjectName = "org.alljoyn.test.BusListenerTest";
        public TimeSpan MaxWaitTime = TimeSpan.FromSeconds(5);

        AutoResetEvent notifyEvent = new AutoResetEvent(false);

        bool listenerRegistered;
        bool listenerUnregistered;
        bool foundAdvertisedName;
        bool lostAdvertisedName;
        bool nameOwnerChanged;
        bool busDisconnected;
        bool busStopping;

        [Fact]
        public void TestListenerRegisteredUnregistered()
        {
            AllJoyn.BusAttachment bus = new AllJoyn.BusAttachment("BusListenerTest", true);
            AllJoyn.BusListener busListener = new TestBusListener(this);
            listenerRegistered = false;
            listenerUnregistered = false;
            AllJoyn.QStatus status = AllJoyn.QStatus.FAIL;

            // start the bus attachment
            status = bus.Start();
            Assert.Equal(AllJoyn.QStatus.OK, status);

            // connect to the bus
            status = bus.Connect(AllJoynTestCommon.GetConnectSpec());
            Assert.Equal(AllJoyn.QStatus.OK, status);

            bus.RegisterBusListener(busListener);
            Wait(MaxWaitTime);
            Assert.Equal(true, listenerRegistered);

            bus.UnregisterBusListener(busListener);
            Wait(MaxWaitTime);
            Assert.Equal(true, listenerUnregistered);

            // TODO: move these into a teardown method?
            busListener.Dispose();
            bus.Dispose();
        }

        [Fact]
        public void TestFoundLostAdvertisedName()
        {
            // create bus attachment
            AllJoyn.BusAttachment bus = new AllJoyn.BusAttachment("BusListenerTest", true);
            AllJoyn.QStatus status = AllJoyn.QStatus.FAIL;

            // start the bus attachment
            status = bus.Start();
            Assert.Equal(AllJoyn.QStatus.OK, status);

            // connect to the bus
            status = bus.Connect(AllJoynTestCommon.GetConnectSpec());
            Assert.Equal(AllJoyn.QStatus.OK, status);

            listenerRegistered = false;
            foundAdvertisedName = false;
            lostAdvertisedName = false;

            // register the bus listener
            AllJoyn.BusListener busListener = new TestBusListener(this);
            bus.RegisterBusListener(busListener);
            Wait(MaxWaitTime);
            Assert.Equal(true, listenerRegistered);

            // advertise the name, & see if we find it
            status = bus.FindAdvertisedName(ObjectName);
            Assert.Equal(AllJoyn.QStatus.OK, status);

            AllJoyn.SessionOpts sessionOpts = new AllJoyn.SessionOpts(
                AllJoyn.SessionOpts.TrafficType.Messages, false,
                AllJoyn.SessionOpts.ProximityType.Any, AllJoyn.TransportMask.Any);

            status = bus.AdvertiseName(ObjectName, sessionOpts.Transports);
            Assert.Equal(AllJoyn.QStatus.OK, status);

            Wait(MaxWaitTime);
            Assert.Equal(true, foundAdvertisedName);

            // stop advertising the name, & see if we lose it
            status = bus.CancelAdvertisedName(ObjectName, sessionOpts.Transports);
            Assert.Equal(AllJoyn.QStatus.OK, status);

            Wait(MaxWaitTime);
            Assert.Equal(true, lostAdvertisedName);

            // TODO: move these into a teardown method?
            busListener.Dispose();
            bus.Dispose();

        }

        [Fact]
        public void TestStopDisconnected()
        {
            // create bus attachment
            AllJoyn.BusAttachment bus = new AllJoyn.BusAttachment("BusListenerTest", true);
            AllJoyn.QStatus status = AllJoyn.QStatus.FAIL;

            // start the bus attachment
            status = bus.Start();
            Assert.Equal(AllJoyn.QStatus.OK, status);

            // connect to the bus
            status = bus.Connect(AllJoynTestCommon.GetConnectSpec());
            Assert.Equal(AllJoyn.QStatus.OK, status);

            listenerRegistered = false;
            busDisconnected = false;
            busStopping = false;

            // register the bus listener
            AllJoyn.BusListener busListener = new TestBusListener(this);
            bus.RegisterBusListener(busListener);
            Wait(MaxWaitTime);
            Assert.Equal(true, listenerRegistered);

            // test disconnecting from the bus
            status = bus.Disconnect(AllJoynTestCommon.GetConnectSpec());
            Assert.Equal(AllJoyn.QStatus.OK, status);
            Wait(MaxWaitTime);
            Assert.Equal(true, busDisconnected);

            // test stopping the bus
            status = bus.Stop();
            Assert.Equal(AllJoyn.QStatus.OK, status);
            Wait(MaxWaitTime);
            Assert.Equal(true, busStopping);

            busListener.Dispose();
            bus.Dispose();
        }

        [Fact]
        public void TestNameOwnerChanged()
        {
            // create bus attachment
            AllJoyn.BusAttachment bus = new AllJoyn.BusAttachment("BusListenerTest", true);
            AllJoyn.QStatus status = AllJoyn.QStatus.FAIL;

            // start the bus attachment
            status = bus.Start();
            Assert.Equal(AllJoyn.QStatus.OK, status);

            // connect to the bus
            status = bus.Connect(AllJoynTestCommon.GetConnectSpec());
            Assert.Equal(AllJoyn.QStatus.OK, status);

            listenerRegistered = false;
            nameOwnerChanged = false;

            // register the bus listener
            AllJoyn.BusListener busListener = new TestBusListener(this);
            bus.RegisterBusListener(busListener);
            Wait(MaxWaitTime);
            Assert.Equal(true, listenerRegistered);

            // test name owner changed
            status = bus.RequestName(ObjectName, 0);
            Assert.Equal(AllJoyn.QStatus.OK, status);
            Wait(MaxWaitTime);
            Assert.Equal(true, nameOwnerChanged);

            busListener.Dispose();
            bus.Dispose();
        }

        private void Wait(TimeSpan timeout)
        {
            notifyEvent.WaitOne(timeout);
            notifyEvent.Reset();
        }

        public class TestBusListener : AllJoyn.BusListener
        {
            BusListenerTest _busListenerTest;

            public TestBusListener(BusListenerTest busListenerTest)
            {
                this._busListenerTest = busListenerTest;
            }

            protected override void ListenerRegistered(AllJoyn.BusAttachment busAttachment)
            {
                _busListenerTest.listenerRegistered = true;
                Notify();
            }

            protected override void ListenerUnregistered()
            {
                _busListenerTest.listenerUnregistered = true;
                Notify();
            }

            protected override void FoundAdvertisedName(string name, AllJoyn.TransportMask transport, string namePrefix)
            {
                _busListenerTest.foundAdvertisedName = true;
                Notify();
            }

            protected override void LostAdvertisedName(string name, AllJoyn.TransportMask transport, string namePrefix)
            {
                _busListenerTest.lostAdvertisedName = true;
                Notify();
            }

            protected override void NameOwnerChanged(string busName, string previousOwner, string newOwner)
            {
                _busListenerTest.nameOwnerChanged = true;
                Notify();
            }

            protected override void BusDisconnected()
            {
                _busListenerTest.busDisconnected = true;
                Notify();
            }

            protected override void BusStopping()
            {
                _busListenerTest.busStopping = true;
                Notify();
            }

            private void Notify()
            {
                _busListenerTest.notifyEvent.Set();
            }

        }
    }
}
