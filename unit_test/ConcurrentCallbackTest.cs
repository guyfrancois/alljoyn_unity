﻿//-----------------------------------------------------------------------
// <copyright file="ConcurrentCallbackTest.cs" company="Qualcomm Innovation Center, Inc.">
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
	public class ConcurrentCallbackTest
	{
		static AllJoyn.BusAttachment mbus;

		public const string ObjectName = "org.alljoyn.test.ConcurrentCallbackTest";
		public TimeSpan MaxWaitTime = TimeSpan.FromSeconds(5);
		public bool listenerRegisteredFlag;
		public bool nameOwnerChangedFlag;
		public AllJoyn.QStatus callbackStatus;

		public AutoResetEvent notifyEvent = new AutoResetEvent(false);

		private void Wait(TimeSpan timeout)
		{
			notifyEvent.WaitOne(timeout);
			notifyEvent.Reset();
		}

		// we need this so that we know when the advertised name has been found
		class BusListenerWithBlockingCall : AllJoyn.BusListener
		{
			ConcurrentCallbackTest _concurrentCallbackTest;

			private void Notify()
			{
				_concurrentCallbackTest.notifyEvent.Set();
			}

			public BusListenerWithBlockingCall(ConcurrentCallbackTest concurrentCallbackTest)
			{
				this._concurrentCallbackTest = concurrentCallbackTest;
			}

			protected override void ListenerRegistered(AllJoyn.BusAttachment busAttachment)
			{
				_concurrentCallbackTest.listenerRegisteredFlag = true;
				Notify();
			}

			protected override void NameOwnerChanged(string busName, string previousOwner, string newOwner)
			{
				AllJoyn.QStatus status = AllJoyn.QStatus.FAIL;
				AllJoyn.ProxyBusObject proxy = new AllJoyn.ProxyBusObject(mbus, "org.alljoyn.Bus", "/org/alljoyn/Bus", 0);
				Assert.NotNull(proxy);
				status = proxy.IntrospectRemoteObject();
				proxy.Dispose();
				_concurrentCallbackTest.callbackStatus = status;
				_concurrentCallbackTest.nameOwnerChangedFlag = true;
				Notify();

			}
		}

		[Fact]
		public void EnableConcurrentCallbacks_Not_Used()
		{
			AllJoyn.QStatus status = AllJoyn.QStatus.FAIL;
			callbackStatus = AllJoyn.QStatus.FAIL;
			listenerRegisteredFlag = false;
			nameOwnerChangedFlag = false;

			mbus = new AllJoyn.BusAttachment("BusListenerTest", true);
			AllJoyn.BusListener busListener = new BusListenerWithBlockingCall(this);

			// start the bus attachment
			status = mbus.Start();
			Assert.Equal(AllJoyn.QStatus.OK, status);

			// connect to the bus
			status = mbus.Connect(AllJoynTestCommon.GetConnectSpec());
			Assert.Equal(AllJoyn.QStatus.OK, status);

			mbus.RegisterBusListener(busListener);
			Wait(MaxWaitTime);
			Assert.True(listenerRegisteredFlag);

			mbus.RequestName(ObjectName, 0);
			Wait(MaxWaitTime);
			Assert.True(nameOwnerChangedFlag);
			/*
			 * Because of the way that callback functions are defered we can still make
			 * what would be a blocking call in alljoyn_core and it is not a blocking
			 * call in Unity.  This is a by product of the alljoyn_c deffered callback class
			 * and its usage.  I am still investigating ways to work around issues caused
			 * by the deffered callback class at some point in the future may start to work
			 * as alljoyn_core.
			 * Assert.Equal(AllJoyn.QStatus.BUS_BLOCKING_CALL_NOT_ALLOWED, callbackStatus);
			 */
			Assert.Equal(AllJoyn.QStatus.OK, callbackStatus);
			mbus.UnregisterBusListener(busListener);
			mbus.Stop();
			mbus.Join();
			mbus.Dispose();
		}

		// we need this so that we know when the advertised name has been found
		class BusListenerEnableConcurrentCallbacks : AllJoyn.BusListener
		{
			ConcurrentCallbackTest _concurrentCallbackTest;

			private void Notify()
			{
				_concurrentCallbackTest.notifyEvent.Set();
			}

			public BusListenerEnableConcurrentCallbacks(ConcurrentCallbackTest concurrentCallbackTest)
			{
				this._concurrentCallbackTest = concurrentCallbackTest;
			}

			protected override void ListenerRegistered(AllJoyn.BusAttachment busAttachment)
			{
				_concurrentCallbackTest.listenerRegisteredFlag = true;
				Notify();
			}

			protected override void NameOwnerChanged(string busName, string previousOwner, string newOwner)
			{
				AllJoyn.QStatus status = AllJoyn.QStatus.FAIL;
				AllJoyn.ProxyBusObject proxy = new AllJoyn.ProxyBusObject(mbus, "org.alljoyn.Bus", "/org/alljoyn/Bus", 0);
				Assert.NotNull(proxy);
				mbus.EnableConcurrentCallbacks();
				status = proxy.IntrospectRemoteObject();
				proxy.Dispose();
				_concurrentCallbackTest.callbackStatus = status;
				_concurrentCallbackTest.nameOwnerChangedFlag = true;
				Notify();

			}
		}

		[Fact]
		public void EnableConcurrentCallbacks_Used()
		{
			AllJoyn.QStatus status = AllJoyn.QStatus.FAIL;
			callbackStatus = AllJoyn.QStatus.FAIL;
			listenerRegisteredFlag = false;
			nameOwnerChangedFlag = false;

			mbus = new AllJoyn.BusAttachment("BusListenerTest", true);
			AllJoyn.BusListener busListener = new BusListenerEnableConcurrentCallbacks(this);

			// start the bus attachment
			status = mbus.Start();
			Assert.Equal(AllJoyn.QStatus.OK, status);

			// connect to the bus
			status = mbus.Connect(AllJoynTestCommon.GetConnectSpec());
			Assert.Equal(AllJoyn.QStatus.OK, status);

			mbus.RegisterBusListener(busListener);
			Wait(MaxWaitTime);
			Assert.True(listenerRegisteredFlag);

			mbus.RequestName(ObjectName, 0);
			Wait(MaxWaitTime);
			Assert.True(nameOwnerChangedFlag);
			Assert.Equal(AllJoyn.QStatus.OK, callbackStatus);

			mbus.UnregisterBusListener(busListener);
			mbus.Stop();
			mbus.Join();
			mbus.Dispose();
		}
	}
}
