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
#include <gtest/gtest.h>
#include <alljoyn_c/BusAttachment.h>
#include <alljoyn_c/InterfaceDescription.h>

TEST(InterfaceDescriptionTest, addmember) {
    QStatus status = ER_OK;
    alljoyn_busattachment bus = NULL;
    bus = alljoyn_busattachment_create("InterfaceDescriptionTest", QC_FALSE);
    ASSERT_TRUE(bus != NULL);
    alljoyn_interfacedescription testIntf = NULL;
    status = alljoyn_busattachment_createinterface(bus, "org.alljoyn.test.InterfaceDescription", &testIntf, QC_FALSE);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    status = alljoyn_interfacedescription_addmember(testIntf, ALLJOYN_MESSAGE_METHOD_CALL, "ping", "s", "s", "in,out", 0);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    status = alljoyn_interfacedescription_addmember(testIntf, ALLJOYN_MESSAGE_SIGNAL, "chirp", "", "s", "chirp", 0);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    alljoyn_busattachment_destroy(bus);
}

TEST(InterfaceDescriptionTest, activate) {
    QStatus status = ER_OK;
    alljoyn_busattachment bus = NULL;
    bus = alljoyn_busattachment_create("InterfaceDescriptionTest", QC_FALSE);
    alljoyn_interfacedescription testIntf = NULL;
    status = alljoyn_busattachment_createinterface(bus, "org.alljoyn.test.InterfaceDescription", &testIntf, QC_FALSE);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    status = alljoyn_interfacedescription_addmember(testIntf, ALLJOYN_MESSAGE_METHOD_CALL, "ping", "s", "s", "in,out", 0);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    status = alljoyn_interfacedescription_addmember(testIntf, ALLJOYN_MESSAGE_SIGNAL, "chirp", "", "s", "chirp", 0);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    alljoyn_interfacedescription_activate(testIntf);
    /* once the interface has been activated we should not be able to add new members */
    status = alljoyn_interfacedescription_addmember(testIntf, ALLJOYN_MESSAGE_METHOD_CALL, "pong", "s", "s", "in,out", 0);
    EXPECT_EQ(ER_BUS_INTERFACE_ACTIVATED, status) << "  Actual Status: " << QCC_StatusText(status);
    alljoyn_busattachment_destroy(bus);
}

TEST(InterfaceDescriptionTest, introspect) {
    QStatus status = ER_OK;
    alljoyn_busattachment bus = NULL;
    bus = alljoyn_busattachment_create("InterfaceDescriptionTest", QC_FALSE);
    alljoyn_interfacedescription testIntf = NULL;
    status = alljoyn_busattachment_createinterface(bus, "org.alljoyn.test.InterfaceDescription", &testIntf, QC_FALSE);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    status = alljoyn_interfacedescription_addmember(testIntf, ALLJOYN_MESSAGE_METHOD_CALL, "ping", "s", "s", "in,out", 0);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    status = alljoyn_interfacedescription_addmember(testIntf, ALLJOYN_MESSAGE_SIGNAL, "chirp", "", "s", "chirp", 0);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    char* introspect = alljoyn_interfacedescription_introspect(testIntf, 0);
    const char* expectedIntrospect =
        "<interface name=\"org.alljoyn.test.InterfaceDescription\">\n"
        "  <signal name=\"chirp\">\n"
        "    <arg name=\"chirp\" type=\"s\" direction=\"out\"/>\n"
        "  </signal>\n"
        "  <method name=\"ping\">\n"
        "    <arg name=\"in\" type=\"s\" direction=\"in\"/>\n"
        "    <arg name=\"out\" type=\"s\" direction=\"out\"/>\n"
        "  </method>\n"
        "</interface>\n";
    EXPECT_STREQ(expectedIntrospect, introspect);
    free(introspect);
    alljoyn_busattachment_destroy(bus);
}

TEST(InterfaceDescriptionTest, issecure) {
    QStatus status = ER_OK;
    alljoyn_busattachment bus = NULL;
    bus = alljoyn_busattachment_create("InterfaceDescriptionTest", QC_FALSE);
    alljoyn_interfacedescription testIntf = NULL;
    status = alljoyn_busattachment_createinterface(bus, "org.alljoyn.test.InterfaceDescription", &testIntf, QC_TRUE);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    EXPECT_EQ(QC_TRUE, alljoyn_interfacedescription_issecure(testIntf));
    status = alljoyn_busattachment_deleteinterface(bus, testIntf);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    status = alljoyn_busattachment_createinterface(bus, "org.alljoyn.test.InterfaceDescription", &testIntf, QC_FALSE);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    EXPECT_EQ(QC_FALSE, alljoyn_interfacedescription_issecure(testIntf));
    alljoyn_busattachment_destroy(bus);
}
