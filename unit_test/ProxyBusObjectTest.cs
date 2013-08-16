//-----------------------------------------------------------------------
// <copyright file="ProxyBusObjectTest.cs" company="Qualcomm Innovation Center, Inc.">
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
	public class ProxyBusObjectTest
	{
		//private static string WELLKNOWN_NAME = "org.alljoyn.test.ProxyBusObjectTest";
		//private static string OBJECT_PATH = "/org/alljoyn/test/ProxyBusObjectTest";
		private static string INTERFACE_NAME = "org.alljoyn.test.ProxyBusObjectTest";

		[Fact]
		public void CreateDispose()
		{
			AllJoyn.BusAttachment busAttachment = new AllJoyn.BusAttachment("ProxyBusObjectTest", false);
			Assert.Equal(AllJoyn.QStatus.OK, busAttachment.Start());
			Assert.Equal(AllJoyn.QStatus.OK, busAttachment.Connect(AllJoynTestCommon.GetConnectSpec()));

			AllJoyn.ProxyBusObject proxyBusObject = new AllJoyn.ProxyBusObject(busAttachment, "org.alljoyn.Bus", "/org/alljoyn/Bus", 0);
			Assert.NotNull(proxyBusObject);
			proxyBusObject.Dispose();
			Assert.Equal(AllJoyn.QStatus.OK, busAttachment.Stop());
			Assert.Equal(AllJoyn.QStatus.OK, busAttachment.Join());
			busAttachment.Dispose();
		}

		[Fact]
		public void IntrospectRemoteObject()
		{
			AllJoyn.BusAttachment busAttachment = new AllJoyn.BusAttachment("ProxyBusObjectTest", false);
			Assert.Equal(AllJoyn.QStatus.OK, busAttachment.Start());
			Assert.Equal(AllJoyn.QStatus.OK, busAttachment.Connect(AllJoynTestCommon.GetConnectSpec()));

			AllJoyn.ProxyBusObject proxyBusObject = new AllJoyn.ProxyBusObject(busAttachment, "org.alljoyn.Bus", "/org/alljoyn/Bus", 0);
			Assert.NotNull(proxyBusObject);

			Assert.Equal(AllJoyn.QStatus.OK, proxyBusObject.IntrospectRemoteObject());

			AllJoyn.InterfaceDescription interfaceDescription = proxyBusObject.GetInterface("org.freedesktop.DBus.Introspectable");

			string expectedIntrospect = "<interface name=\"org.freedesktop.DBus.Introspectable\">\n" +
										"  <method name=\"Introspect\">\n" +
										"    <arg name=\"data\" type=\"s\" direction=\"out\"/>\n" +
										"  </method>\n" +
										"</interface>\n";
			Assert.Equal(expectedIntrospect, interfaceDescription.Introspect());


			proxyBusObject.Dispose();
			Assert.Equal(AllJoyn.QStatus.OK, busAttachment.Stop());
			Assert.Equal(AllJoyn.QStatus.OK, busAttachment.Join());
			busAttachment.Dispose();
		}

		[Fact]
		public void Path()
		{
			AllJoyn.BusAttachment busAttachment = new AllJoyn.BusAttachment("ProxyBusObjectTest", false);
			Assert.Equal(AllJoyn.QStatus.OK, busAttachment.Start());
			Assert.Equal(AllJoyn.QStatus.OK, busAttachment.Connect(AllJoynTestCommon.GetConnectSpec()));

			AllJoyn.ProxyBusObject proxyBusObject = new AllJoyn.ProxyBusObject(busAttachment, "org.alljoyn.Bus", "/org/alljoyn/Bus", 0);
			Assert.NotNull(proxyBusObject);

			Assert.Equal("/org/alljoyn/Bus", proxyBusObject.Path);
		}

		[Fact]
		public void ServiceName()
		{
			AllJoyn.BusAttachment busAttachment = new AllJoyn.BusAttachment("ProxyBusObjectTest", false);
			Assert.Equal(AllJoyn.QStatus.OK, busAttachment.Start());
			Assert.Equal(AllJoyn.QStatus.OK, busAttachment.Connect(AllJoynTestCommon.GetConnectSpec()));

			AllJoyn.ProxyBusObject proxyBusObject = new AllJoyn.ProxyBusObject(busAttachment, "org.alljoyn.Bus", "/org/alljoyn/Bus", 0);
			Assert.NotNull(proxyBusObject);

			Assert.Equal("org.alljoyn.Bus", proxyBusObject.ServiceName);
		}

		[Fact]
		public void SessionId()
		{
			AllJoyn.BusAttachment busAttachment = new AllJoyn.BusAttachment("ProxyBusObjectTest", false);
			Assert.Equal(AllJoyn.QStatus.OK, busAttachment.Start());
			Assert.Equal(AllJoyn.QStatus.OK, busAttachment.Connect(AllJoynTestCommon.GetConnectSpec()));

			AllJoyn.ProxyBusObject proxyBusObject = new AllJoyn.ProxyBusObject(busAttachment, "org.alljoyn.Bus", "/org/alljoyn/Bus", 0);
			Assert.NotNull(proxyBusObject);

			Assert.Equal(0u, proxyBusObject.SessionId);
			/*
			 * TODO set up a session with a real session and make sure that proxyObj
			 * has and will return the proper sessionid.
			 */
		}

		[Fact]
		public void ImplementsInterface()
		{
			AllJoyn.BusAttachment busAttachment = new AllJoyn.BusAttachment("ProxyBusObjectTest", false);
			Assert.Equal(AllJoyn.QStatus.OK, busAttachment.Start());
			Assert.Equal(AllJoyn.QStatus.OK, busAttachment.Connect(AllJoynTestCommon.GetConnectSpec()));

			AllJoyn.ProxyBusObject proxyBusObject = new AllJoyn.ProxyBusObject(busAttachment, "org.alljoyn.Bus", "/org/alljoyn/Bus", 0);
			Assert.NotNull(proxyBusObject);

			Assert.Equal(AllJoyn.QStatus.OK, proxyBusObject.IntrospectRemoteObject());

			Assert.True(proxyBusObject.ImplementsInterface("org.alljoyn.Bus"));
			Assert.True(proxyBusObject.ImplementsInterface("org.alljoyn.Daemon"));
			Assert.False(proxyBusObject.ImplementsInterface("org.alljoyn.Invalid"));
		}

		[Fact]
		public void GetInteface()
		{
			AllJoyn.BusAttachment busAttachment = new AllJoyn.BusAttachment("ProxyBusObjectTest", false);
			Assert.Equal(AllJoyn.QStatus.OK, busAttachment.Start());
			Assert.Equal(AllJoyn.QStatus.OK, busAttachment.Connect(AllJoynTestCommon.GetConnectSpec()));

			AllJoyn.ProxyBusObject proxyBusObject = new AllJoyn.ProxyBusObject(busAttachment, "org.alljoyn.Bus", "/org/alljoyn/Bus", 0);
			Assert.NotNull(proxyBusObject);

			Assert.Equal(AllJoyn.QStatus.OK, proxyBusObject.IntrospectRemoteObject());

			AllJoyn.InterfaceDescription iface = proxyBusObject.GetInterface("org.freedesktop.DBus.Introspectable");
			string expectedIntrospect = "<interface name=\"org.freedesktop.DBus.Introspectable\">\n" +
										"  <method name=\"Introspect\">\n" +
										"    <arg name=\"data\" type=\"s\" direction=\"out\"/>\n" +
										"  </method>\n" +
										"</interface>\n";
			Assert.Equal(expectedIntrospect, iface.Introspect());

			Assert.Null(proxyBusObject.GetInterface("org.alljoyn.not.a.real.interface"));
		}

		[Fact]
		public void AddInterface()
		{
			AllJoyn.BusAttachment busAttachment = new AllJoyn.BusAttachment("ProxyBusObjectTest", false);
			Assert.Equal(AllJoyn.QStatus.OK, busAttachment.Start());
			Assert.Equal(AllJoyn.QStatus.OK, busAttachment.Connect(AllJoynTestCommon.GetConnectSpec()));

			AllJoyn.InterfaceDescription testIntf = null;
			Assert.Equal(AllJoyn.QStatus.OK, busAttachment.CreateInterface(INTERFACE_NAME, out testIntf));
			Assert.NotNull(testIntf);
			Assert.Equal(AllJoyn.QStatus.OK, testIntf.AddMember(AllJoyn.Message.Type.MethodCall, "ping", "s", "s", "in,out"));
			testIntf.Activate();

			AllJoyn.ProxyBusObject proxyBusObject = new AllJoyn.ProxyBusObject(busAttachment, "org.alljoyn.Bus", "/org/alljoyn/Bus", 0);
			Assert.NotNull(proxyBusObject);

			Assert.Equal(AllJoyn.QStatus.OK, proxyBusObject.AddInterface(testIntf));

			Assert.True(proxyBusObject.ImplementsInterface(INTERFACE_NAME));
		}

		[Fact]
		public void ParseXml()
		{
			 string busObjectXML = "<node name=\"/org/alljoyn/test/ProxyObjectTest\">" +
									"  <interface name=\"org.alljoyn.test.ProxyBusObjectTest\">\n" +
									"    <signal name=\"chirp\">\n" +
									"      <arg name=\"chirp\" type=\"s\"/>\n" +
									"    </signal>\n" +
									"    <signal name=\"chirp2\">\n" +
									"      <arg name=\"chirp\" type=\"s\" direction=\"out\"/>\n" +
									"    </signal>\n" +
									"    <method name=\"ping\">\n" +
									"      <arg name=\"in\" type=\"s\" direction=\"in\"/>\n" +
									"      <arg name=\"out\" type=\"s\" direction=\"out\"/>\n" +
									"    </method>\n" +
									"  </interface>\n" +
									"</node>\n";
			AllJoyn.BusAttachment busAttachment = new AllJoyn.BusAttachment("ProxyBusObjectTest", false);
			Assert.Equal(AllJoyn.QStatus.OK, busAttachment.Start());
			Assert.Equal(AllJoyn.QStatus.OK, busAttachment.Connect(AllJoynTestCommon.GetConnectSpec()));

			AllJoyn.ProxyBusObject proxyBusObject = new AllJoyn.ProxyBusObject(busAttachment, null, null, 0);
			Assert.NotNull(proxyBusObject);

			Assert.Equal(AllJoyn.QStatus.OK, proxyBusObject.ParseXml(busObjectXML));

			Assert.True(proxyBusObject.ImplementsInterface("org.alljoyn.test.ProxyBusObjectTest"));

			AllJoyn.InterfaceDescription iface = proxyBusObject.GetInterface("org.alljoyn.test.ProxyBusObjectTest");
			Assert.NotNull(iface);

			string expectedIntrospect = "<interface name=\"org.alljoyn.test.ProxyBusObjectTest\">\n" +
										"  <signal name=\"chirp\">\n" +
										"    <arg name=\"chirp\" type=\"s\" direction=\"out\"/>\n" +
										"  </signal>\n" +
										"  <signal name=\"chirp2\">\n" +
										"    <arg name=\"chirp\" type=\"s\" direction=\"out\"/>\n" +
										"  </signal>\n" +
										"  <method name=\"ping\">\n" +
										"    <arg name=\"in\" type=\"s\" direction=\"in\"/>\n" +
										"    <arg name=\"out\" type=\"s\" direction=\"out\"/>\n" +
										"  </method>\n" +
										"</interface>\n";
			Assert.Equal(expectedIntrospect, iface.Introspect());
		}
	}
}