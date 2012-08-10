﻿/******************************************************************************
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

using AllJoynUnity;
using Xunit;

namespace AllJoynUnityTest
{
    public class InterfaceDescriptionTest
    {
        const string INTERFACE_NAME = "org.alljoyn.test.InterfaceDescriptionTest";

        [Fact]
        public void TestAddMember()
        {
            AllJoyn.QStatus status = AllJoyn.QStatus.FAIL;

            AllJoyn.BusAttachment bus = null;
            bus = new AllJoyn.BusAttachment("InterfaceDescriptionTest", true);
            Assert.NotNull(bus);

            // create the interface
            AllJoyn.InterfaceDescription testIntf = null;
            status = bus.CreateInterface(INTERFACE_NAME, false, out testIntf);
            Assert.Equal(AllJoyn.QStatus.OK, status);
            Assert.NotNull(testIntf);

            // Test adding a MethodCall
            status = testIntf.AddMember(AllJoyn.Message.Type.MethodCall, "ping", "s", "s", "in,out", AllJoyn.InterfaceDescription.AnnotationFlags.Default);
            Assert.Equal(AllJoyn.QStatus.OK, status);

            // Test adding a Signal
            status = testIntf.AddMember(AllJoyn.Message.Type.Signal, "chirp", "", "s", "chirp", AllJoyn.InterfaceDescription.AnnotationFlags.Default);
            Assert.Equal(AllJoyn.QStatus.OK, status);

            bus.Dispose();
        }

        [Fact]
        public void TestGetMember()
        {
            AllJoyn.QStatus status = AllJoyn.QStatus.FAIL;

            AllJoyn.BusAttachment bus = null;
            bus = new AllJoyn.BusAttachment("InterfaceDescriptionTest", true);
            Assert.NotNull(bus);

            // create the interface
            AllJoyn.InterfaceDescription testIntf = null;
            status = bus.CreateInterface(INTERFACE_NAME, false, out testIntf);
            Assert.Equal(AllJoyn.QStatus.OK, status);
            Assert.NotNull(testIntf);

            // Test adding a MethodCall
            status = testIntf.AddMember(AllJoyn.Message.Type.MethodCall, "ping", "s", "s", "in,out", AllJoyn.InterfaceDescription.AnnotationFlags.Default);
            Assert.Equal(AllJoyn.QStatus.OK, status);

            // Test adding a Signal
            status = testIntf.AddMember(AllJoyn.Message.Type.Signal, "chirp", "", "s", "chirp", AllJoyn.InterfaceDescription.AnnotationFlags.Default);
            Assert.Equal(AllJoyn.QStatus.OK, status);

            // Verify the ping member
            AllJoyn.InterfaceDescription.Member pingMember = null;
            pingMember = testIntf.GetMember("ping");
            Assert.NotNull(pingMember);

            Assert.Equal(testIntf, pingMember.Iface);
            Assert.Equal(AllJoyn.Message.Type.MethodCall, pingMember.MemberType);
            Assert.Equal("ping", pingMember.Name);
            Assert.Equal("s", pingMember.Signature);
            Assert.Equal("s", pingMember.ReturnSignature);
            Assert.Equal("in,out", pingMember.ArgNames);
            Assert.Equal(AllJoyn.InterfaceDescription.AnnotationFlags.Default, pingMember.Annotation);

            // Verify the chirp member
            AllJoyn.InterfaceDescription.Member chirpMember = null;
            chirpMember = testIntf.GetMember("chirp");
            Assert.NotNull(chirpMember);

            Assert.Equal(testIntf, chirpMember.Iface);
            Assert.Equal(AllJoyn.Message.Type.Signal, chirpMember.MemberType);
            Assert.Equal("chirp", chirpMember.Name);
            Assert.Equal("", chirpMember.Signature);
            Assert.Equal("s", chirpMember.ReturnSignature);
            Assert.Equal("chirp", chirpMember.ArgNames);
            Assert.Equal(AllJoyn.InterfaceDescription.AnnotationFlags.Default, chirpMember.Annotation);

            bus.Dispose();
        }

        
        public void TestGetMembers()
        {
            //TODO: implement this...
        }

        [Fact]
        public void TestHasMember()
        {
            AllJoyn.QStatus status = AllJoyn.QStatus.FAIL;

            AllJoyn.BusAttachment bus = null;
            bus = new AllJoyn.BusAttachment("InterfaceDescriptionTest", true);
            Assert.NotNull(bus);

            // create the interface
            AllJoyn.InterfaceDescription testIntf = null;
            status = bus.CreateInterface(INTERFACE_NAME, false, out testIntf);
            Assert.Equal(AllJoyn.QStatus.OK, status);
            Assert.NotNull(testIntf);

            // Test adding a MethodCall
            status = testIntf.AddMember(AllJoyn.Message.Type.MethodCall, "ping", "s", "s", "in,out", AllJoyn.InterfaceDescription.AnnotationFlags.Default);
            Assert.Equal(AllJoyn.QStatus.OK, status);

            // Test adding a Signal
            status = testIntf.AddMember(AllJoyn.Message.Type.Signal, "chirp", "", "s", "chirp", AllJoyn.InterfaceDescription.AnnotationFlags.Default);
            Assert.Equal(AllJoyn.QStatus.OK, status);

            Assert.Equal(true, testIntf.HasMember("ping", "s", "s"));
            Assert.Equal(true, testIntf.HasMember("chirp", "", "s"));

            /*
             * expected to be false even though the members exist the signatures do not
             * match what is expected.
             */
            Assert.Equal(false, testIntf.HasMember("ping", "i", "s"));
            Assert.Equal(false, testIntf.HasMember("chirp", "b", null));
            Assert.Equal(false, testIntf.HasMember("invalid", "s", null));

            bus.Dispose();
        }

        [Fact]
        public void TestActivate()
        {
            AllJoyn.QStatus status = AllJoyn.QStatus.FAIL;

            AllJoyn.BusAttachment bus = null;
            bus = new AllJoyn.BusAttachment("InterfaceDescriptionTest", true);
            Assert.NotNull(bus);

            // create the interface
            AllJoyn.InterfaceDescription testIntf = null;
            status = bus.CreateInterface(INTERFACE_NAME, false, out testIntf);
            Assert.Equal(AllJoyn.QStatus.OK, status);
            Assert.NotNull(testIntf);

            // Test adding a MethodCall
            status = testIntf.AddMember(AllJoyn.Message.Type.MethodCall, "ping", "s", "s", "in,out", AllJoyn.InterfaceDescription.AnnotationFlags.Default);
            Assert.Equal(AllJoyn.QStatus.OK, status);

            // Test adding a Signal
            status = testIntf.AddMember(AllJoyn.Message.Type.Signal, "chirp", "", "s", "chirp", AllJoyn.InterfaceDescription.AnnotationFlags.Default);
            Assert.Equal(AllJoyn.QStatus.OK, status);

            // activate the interface
            testIntf.Activate();

            /* once the interface has been activated we should not be able to add new members */
            status = testIntf.AddMember(AllJoyn.Message.Type.MethodCall, "pong", "s", "s", "in,out", AllJoyn.InterfaceDescription.AnnotationFlags.Default);
            Assert.Equal(AllJoyn.QStatus.BUS_INTERFACE_ACTIVATED, status);

            bus.Dispose();
        }

        [Fact]
        public void TestIsSecure()
        {
            AllJoyn.QStatus status = AllJoyn.QStatus.FAIL;

            AllJoyn.BusAttachment bus = null;
            bus = new AllJoyn.BusAttachment("InterfaceDescriptionTest", true);
            Assert.NotNull(bus);

            // create an insecure interface
            AllJoyn.InterfaceDescription testIntf = null;
            status = bus.CreateInterface(INTERFACE_NAME, false, out testIntf);
            Assert.Equal(AllJoyn.QStatus.OK, status);
            Assert.NotNull(testIntf);

            Assert.Equal(false, testIntf.IsSecure);

            bus.DeleteInterface(testIntf);

            // create a secure interface
            status = bus.CreateInterface(INTERFACE_NAME, true, out testIntf);
            Assert.Equal(AllJoyn.QStatus.OK, status);
            Assert.NotNull(testIntf);

            Assert.Equal(true, testIntf.IsSecure);

            bus.Dispose();
        }

        [Fact]
        public void TestAddProperty()
        {
            AllJoyn.QStatus status = AllJoyn.QStatus.FAIL;

            AllJoyn.BusAttachment bus = null;
            bus = new AllJoyn.BusAttachment("InterfaceDescriptionTest", true);
            Assert.NotNull(bus);

            // create the interface
            AllJoyn.InterfaceDescription testIntf = null;
            status = bus.CreateInterface(INTERFACE_NAME, false, out testIntf);
            Assert.Equal(AllJoyn.QStatus.OK, status);
            Assert.NotNull(testIntf);

            status = testIntf.AddProperty("prop1", "s", AllJoyn.InterfaceDescription.AccessFlags.Read);
            Assert.Equal(AllJoyn.QStatus.OK, status);
            status = testIntf.AddProperty("prop2", "i", AllJoyn.InterfaceDescription.AccessFlags.Write);
            Assert.Equal(AllJoyn.QStatus.OK, status);
            status = testIntf.AddProperty("prop3", "u", AllJoyn.InterfaceDescription.AccessFlags.Read);
            Assert.Equal(AllJoyn.QStatus.OK, status);

            bus.Dispose();
        }

        [Fact]
        public void TestHasProperty()
        {
            AllJoyn.QStatus status = AllJoyn.QStatus.FAIL;

            AllJoyn.BusAttachment bus = null;
            bus = new AllJoyn.BusAttachment("InterfaceDescriptionTest", true);
            Assert.NotNull(bus);

            // create the interface
            AllJoyn.InterfaceDescription testIntf = null;
            status = bus.CreateInterface(INTERFACE_NAME, false, out testIntf);
            Assert.Equal(AllJoyn.QStatus.OK, status);
            Assert.NotNull(testIntf);

            status = testIntf.AddProperty("prop1", "s", AllJoyn.InterfaceDescription.AccessFlags.Read);
            Assert.Equal(AllJoyn.QStatus.OK, status);
            status = testIntf.AddProperty("prop2", "i", AllJoyn.InterfaceDescription.AccessFlags.Write);
            Assert.Equal(AllJoyn.QStatus.OK, status);
            status = testIntf.AddProperty("prop3", "u", AllJoyn.InterfaceDescription.AccessFlags.Read);
            Assert.Equal(AllJoyn.QStatus.OK, status);

            // check for the properties
            Assert.Equal(true, testIntf.HasProperty("prop1"));
            Assert.Equal(true, testIntf.HasProperty("prop2"));
            Assert.Equal(true, testIntf.HasProperty("prop3"));
            Assert.Equal(false, testIntf.HasProperty("invalid_prop"));

            bus.Dispose();
        }

        [Fact]
        public void TestHasProperties()
        {
            AllJoyn.QStatus status = AllJoyn.QStatus.FAIL;

            AllJoyn.BusAttachment bus = null;
            bus = new AllJoyn.BusAttachment("InterfaceDescriptionTest", true);
            Assert.NotNull(bus);

            // create the interface
            AllJoyn.InterfaceDescription testIntf = null;
            status = bus.CreateInterface(INTERFACE_NAME, false, out testIntf);
            Assert.Equal(AllJoyn.QStatus.OK, status);
            Assert.NotNull(testIntf);

            /*
             * At this point this is an empty interface the call to hasproperties should
             * return false.
             */
            Assert.False(testIntf.HasProperties);
            status = testIntf.AddMember(AllJoyn.Message.Type.MethodCall, "ping", "s", "s", "in,out", AllJoyn.InterfaceDescription.AnnotationFlags.Default);
            Assert.Equal(AllJoyn.QStatus.OK, status);

            /*
             * At this point the interface only contains a method call the call to
             * hasproperties should return false.
             */
            Assert.False(testIntf.HasProperties);
            status = testIntf.AddProperty("prop1", "s", AllJoyn.InterfaceDescription.AccessFlags.Read);
            Assert.Equal(AllJoyn.QStatus.OK, status);

            /*
             * At this point the interface only contains a property the call to
             * hasproperties should return true.
             */
            Assert.True(testIntf.HasProperties);
            status = testIntf.AddProperty("prop2", "i", AllJoyn.InterfaceDescription.AccessFlags.Write);
            Assert.Equal(AllJoyn.QStatus.OK, status);
            status = testIntf.AddProperty("prop3", "u", AllJoyn.InterfaceDescription.AccessFlags.Read);
            Assert.Equal(AllJoyn.QStatus.OK, status);

            /*
             * At this point the interface only contains multiple properties the call to
             * hasproperties should return true.
             */
            Assert.True(testIntf.HasProperties);

            bus.Dispose();
        }

        [Fact]
        public void TestGetProperty()
        {
            AllJoyn.QStatus status = AllJoyn.QStatus.FAIL;

            AllJoyn.BusAttachment bus = null;
            bus = new AllJoyn.BusAttachment("InterfaceDescriptionTest", true);
            Assert.NotNull(bus);

            // create the interface
            AllJoyn.InterfaceDescription testIntf = null;
            status = bus.CreateInterface(INTERFACE_NAME, false, out testIntf);
            Assert.Equal(AllJoyn.QStatus.OK, status);
            Assert.NotNull(testIntf);

            status = testIntf.AddProperty("prop1", "s", AllJoyn.InterfaceDescription.AccessFlags.Read);
            Assert.Equal(AllJoyn.QStatus.OK, status);
            status = testIntf.AddProperty("prop2", "i", AllJoyn.InterfaceDescription.AccessFlags.Write);
            Assert.Equal(AllJoyn.QStatus.OK, status);
            status = testIntf.AddProperty("prop3", "u", AllJoyn.InterfaceDescription.AccessFlags.Read);
            Assert.Equal(AllJoyn.QStatus.OK, status);

            AllJoyn.InterfaceDescription.Property prop1 = testIntf.GetProperty("prop1");
            Assert.Equal("prop1", prop1.Name);
            Assert.Equal("s", prop1.Signature);
            Assert.Equal(AllJoyn.InterfaceDescription.AccessFlags.Read, prop1.Access);

            AllJoyn.InterfaceDescription.Property prop2 = testIntf.GetProperty("prop2");
            Assert.Equal("prop2", prop2.Name);
            Assert.Equal("i", prop2.Signature);
            Assert.Equal(AllJoyn.InterfaceDescription.AccessFlags.Write, prop2.Access);

            AllJoyn.InterfaceDescription.Property prop3 = testIntf.GetProperty("prop3");
            Assert.Equal("prop3", prop3.Name);
            Assert.Equal("u", prop3.Signature);
            Assert.Equal(AllJoyn.InterfaceDescription.AccessFlags.Read, prop3.Access);

            bus.Dispose();
        }

        public void TestGetProperties()
        {
            //TODO: implement this...
        }

        [Fact]
        public void TestGetName()
        {
            AllJoyn.QStatus status = AllJoyn.QStatus.FAIL;

            AllJoyn.BusAttachment bus = null;
            bus = new AllJoyn.BusAttachment("InterfaceDescriptionTest", true);
            Assert.NotNull(bus);

            // create the interface
            AllJoyn.InterfaceDescription testIntf = null;
            status = bus.CreateInterface(INTERFACE_NAME, false, out testIntf);
            Assert.Equal(AllJoyn.QStatus.OK, status);
            Assert.NotNull(testIntf);

            Assert.Equal(INTERFACE_NAME, testIntf.Name);

            bus.Dispose();
        }

        [Fact]
        public void TestAddSignal()
        {
            AllJoyn.QStatus status = AllJoyn.QStatus.FAIL;

            AllJoyn.BusAttachment bus = null;
            bus = new AllJoyn.BusAttachment("InterfaceDescriptionTest", true);
            Assert.NotNull(bus);

            // create the interface
            AllJoyn.InterfaceDescription testIntf = null;
            status = bus.CreateInterface(INTERFACE_NAME, false, out testIntf);
            Assert.Equal(AllJoyn.QStatus.OK, status);
            Assert.NotNull(testIntf);

            status = testIntf.AddSignal("signal1", "s", "data", AllJoyn.InterfaceDescription.AnnotationFlags.Default);
            Assert.Equal(AllJoyn.QStatus.OK, status);

            // Verify the signal
            AllJoyn.InterfaceDescription.Member signalMember = null;
            signalMember = testIntf.GetMember("signal1");
            Assert.NotNull(signalMember);

            Assert.Equal(testIntf, signalMember.Iface);
            Assert.Equal(AllJoyn.Message.Type.Signal, signalMember.MemberType);
            Assert.Equal("signal1", signalMember.Name);
            Assert.Equal("s", signalMember.Signature);
            Assert.Equal("", signalMember.ReturnSignature);
            Assert.Equal("data", signalMember.ArgNames);
            Assert.Equal(AllJoyn.InterfaceDescription.AnnotationFlags.Default, signalMember.Annotation);

            bus.Dispose();
        }

    }
}
