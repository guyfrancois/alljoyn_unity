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
#include <alljoyn_c/Message.h>

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

TEST(InterfaceDescriptionTest, getmember) {
    QStatus status = ER_OK;
    alljoyn_busattachment bus = NULL;
    bus = alljoyn_busattachment_create("InterfaceDescriptionTest", QC_FALSE);
    ASSERT_TRUE(bus != NULL);
    alljoyn_interfacedescription testIntf = NULL;
    status = alljoyn_busattachment_createinterface(bus, "org.alljoyn.test.InterfaceDescription", &testIntf, QC_FALSE);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    status = alljoyn_interfacedescription_addmember(testIntf, ALLJOYN_MESSAGE_METHOD_CALL, "ping", "s", "s", "in,out", 0);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    status = alljoyn_interfacedescription_addmember(testIntf, ALLJOYN_MESSAGE_SIGNAL, "chirp", "s", NULL, "chirp", 0);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);

    alljoyn_interfacedescription_member member;
    EXPECT_TRUE(alljoyn_interfacedescription_getmember(testIntf, "ping", &member));

    EXPECT_EQ(testIntf, member.iface);
    EXPECT_EQ(ALLJOYN_MESSAGE_METHOD_CALL, member.memberType);
    EXPECT_STREQ("ping", member.name);
    EXPECT_STREQ("s", member.signature);
    EXPECT_STREQ("s", member.returnSignature);
    EXPECT_STREQ("in,out", member.argNames);
    EXPECT_EQ(0, member.annotation);

    alljoyn_interfacedescription_member member2;
    EXPECT_TRUE(alljoyn_interfacedescription_getmember(testIntf, "chirp", &member2));

    EXPECT_EQ(testIntf, member2.iface);
    EXPECT_EQ(ALLJOYN_MESSAGE_SIGNAL, member2.memberType);
    EXPECT_STREQ("chirp", member2.name);
    EXPECT_STREQ("s", member2.signature);
    EXPECT_STREQ("", member2.returnSignature);
    EXPECT_STREQ("chirp", member2.argNames);
    EXPECT_EQ(0, member2.annotation);

    alljoyn_busattachment_destroy(bus);
}

TEST(InterfaceDescriptionTest, getmembers) {
    QStatus status = ER_OK;
    alljoyn_busattachment bus = NULL;
    bus = alljoyn_busattachment_create("InterfaceDescriptionTest", QC_FALSE);
    ASSERT_TRUE(bus != NULL);
    alljoyn_interfacedescription testIntf = NULL;
    status = alljoyn_busattachment_createinterface(bus, "org.alljoyn.test.InterfaceDescription", &testIntf, QC_FALSE);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    status = alljoyn_interfacedescription_addmember(testIntf, ALLJOYN_MESSAGE_METHOD_CALL, "ping", "s", "s", "in,out", 0);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    status = alljoyn_interfacedescription_addmember(testIntf, ALLJOYN_MESSAGE_SIGNAL, "chirp", "s", NULL, "chirp", 0);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);

    alljoyn_interfacedescription_member member[6];
    size_t size;
    size = alljoyn_interfacedescription_getmembers(testIntf, member, 6);
    EXPECT_EQ((size_t)2, size);

    /*
     * NOTE there is nothing that specifies the order the members are organized
     * when they are added to the interface.  As can be seen here even though
     * the 'chirp' signal was added to the interface after 'ping' it comes out
     * of the interface before 'ping'. This result is based on actual program
     * behaver.
     */
    EXPECT_EQ(testIntf, member[0].iface);
    EXPECT_EQ(ALLJOYN_MESSAGE_SIGNAL, member[0].memberType);
    EXPECT_STREQ("chirp", member[0].name);
    EXPECT_STREQ("s", member[0].signature);
    EXPECT_STREQ("", member[0].returnSignature);
    EXPECT_STREQ("chirp", member[0].argNames);
    EXPECT_EQ(0, member[0].annotation);

    EXPECT_EQ(testIntf, member[1].iface);
    EXPECT_EQ(ALLJOYN_MESSAGE_METHOD_CALL, member[1].memberType);
    EXPECT_STREQ("ping", member[1].name);
    EXPECT_STREQ("s", member[1].signature);
    EXPECT_STREQ("s", member[1].returnSignature);
    EXPECT_STREQ("in,out", member[1].argNames);
    EXPECT_EQ(0, member[1].annotation);

    alljoyn_busattachment_destroy(bus);
}

TEST(InterfaceDescriptionTest, hasmembers) {
    QStatus status = ER_OK;
    alljoyn_busattachment bus = NULL;
    bus = alljoyn_busattachment_create("InterfaceDescriptionTest", QC_FALSE);
    ASSERT_TRUE(bus != NULL);
    alljoyn_interfacedescription testIntf = NULL;
    status = alljoyn_busattachment_createinterface(bus, "org.alljoyn.test.InterfaceDescription", &testIntf, QC_FALSE);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    status = alljoyn_interfacedescription_addmember(testIntf, ALLJOYN_MESSAGE_METHOD_CALL, "ping", "s", "s", "in,out", 0);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    status = alljoyn_interfacedescription_addmember(testIntf, ALLJOYN_MESSAGE_SIGNAL, "chirp", "s", NULL, "chirp", 0);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);

    EXPECT_TRUE(alljoyn_interfacedescription_hasmember(testIntf, "ping", "s", "s"));
    EXPECT_TRUE(alljoyn_interfacedescription_hasmember(testIntf, "chirp", "s", NULL));

    /*
     * expected to be false even though the members exist the signatures do not
     * match what is expected.
     */
    EXPECT_FALSE(alljoyn_interfacedescription_hasmember(testIntf, "ping", "i", "s"));
    EXPECT_FALSE(alljoyn_interfacedescription_hasmember(testIntf, "chirp", "b", NULL));

    EXPECT_FALSE(alljoyn_interfacedescription_hasmember(testIntf, "invalid", "s", NULL));

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

    char* introspect;
    size_t buf  = alljoyn_interfacedescription_introspect(testIntf, NULL, 0, 0);
    buf++;
    introspect = (char*)malloc(sizeof(char) * buf);
    alljoyn_interfacedescription_introspect(testIntf, introspect, buf, 0);
    /*
     * NOTE there is nothing that specifies the order the members are organized
     * when they are added to the interface.  As can be seen here even though
     * the 'chirp' signal was added to the interface after 'ping' it is listed
     * before 'ping'. This result is based on actual program behaver.
     */
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

TEST(InterfaceDescriptionTest, addproperty) {
    QStatus status = ER_OK;
    alljoyn_busattachment bus = NULL;
    bus = alljoyn_busattachment_create("InterfaceDescriptionTest", QC_FALSE);
    alljoyn_interfacedescription testIntf = NULL;
    status = alljoyn_busattachment_createinterface(bus, "org.alljoyn.test.InterfaceDescription", &testIntf, QC_FALSE);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    status = alljoyn_interfacedescription_addproperty(testIntf, "prop1", "s", ALLJOYN_PROP_ACCESS_READ);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    status = alljoyn_interfacedescription_addproperty(testIntf, "prop2", "i", ALLJOYN_PROP_ACCESS_WRITE);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    status = alljoyn_interfacedescription_addproperty(testIntf, "prop3", "u", ALLJOYN_PROP_ACCESS_RW);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    char* introspect;
    size_t buf  = alljoyn_interfacedescription_introspect(testIntf, NULL, 0, 0);
    buf++;
    introspect = (char*)malloc(sizeof(char) * buf);
    alljoyn_interfacedescription_introspect(testIntf, introspect, buf, 0);
    const char* expectedIntrospect =
        "<interface name=\"org.alljoyn.test.InterfaceDescription\">\n"
        "  <property name=\"prop1\" type=\"s\" access=\"read\"/>\n"
        "  <property name=\"prop2\" type=\"i\" access=\"write\"/>\n"
        "  <property name=\"prop3\" type=\"u\" access=\"readwrite\"/>\n"
        "</interface>\n";
    EXPECT_STREQ(expectedIntrospect, introspect);
    free(introspect);
    alljoyn_busattachment_destroy(bus);
}

TEST(InterfaceDescriptionTest, hasproperty) {
    QStatus status = ER_OK;
    alljoyn_busattachment bus = NULL;
    bus = alljoyn_busattachment_create("InterfaceDescriptionTest", QC_FALSE);
    alljoyn_interfacedescription testIntf = NULL;
    status = alljoyn_busattachment_createinterface(bus, "org.alljoyn.test.InterfaceDescription", &testIntf, QC_FALSE);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    status = alljoyn_interfacedescription_addproperty(testIntf, "prop1", "s", ALLJOYN_PROP_ACCESS_READ);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    status = alljoyn_interfacedescription_addproperty(testIntf, "prop2", "i", ALLJOYN_PROP_ACCESS_WRITE);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    status = alljoyn_interfacedescription_addproperty(testIntf, "prop3", "u", ALLJOYN_PROP_ACCESS_RW);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);

    EXPECT_TRUE(alljoyn_interfacedescription_hasproperty(testIntf, "prop1"));
    EXPECT_TRUE(alljoyn_interfacedescription_hasproperty(testIntf, "prop2"));
    EXPECT_TRUE(alljoyn_interfacedescription_hasproperty(testIntf, "prop3"));
    EXPECT_FALSE(alljoyn_interfacedescription_hasproperty(testIntf, "invalid_prop"));
    alljoyn_busattachment_destroy(bus);
}

TEST(InterfaceDescriptionTest, hasproperties) {
    QStatus status = ER_OK;
    alljoyn_busattachment bus = NULL;
    bus = alljoyn_busattachment_create("InterfaceDescriptionTest", QC_FALSE);
    alljoyn_interfacedescription testIntf = NULL;
    status = alljoyn_busattachment_createinterface(bus, "org.alljoyn.test.InterfaceDescription", &testIntf, QC_FALSE);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    /*
     * At this point this is an empty interface the call to hasproperties should
     * return false.
     */
    EXPECT_FALSE(alljoyn_interfacedescription_hasproperties(testIntf));
    status = alljoyn_interfacedescription_addmember(testIntf, ALLJOYN_MESSAGE_METHOD_CALL, "ping", "s", "s", "in,out", 0);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    /*
     * At this point the interface only contains a method call the call to
     * hasproperties should return false.
     */
    EXPECT_FALSE(alljoyn_interfacedescription_hasproperties(testIntf));
    status = alljoyn_interfacedescription_addproperty(testIntf, "prop1", "s", ALLJOYN_PROP_ACCESS_READ);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);

    /*
     * At this point the interface only contains a property the call to
     * hasproperties should return true.
     */
    EXPECT_TRUE(alljoyn_interfacedescription_hasproperties(testIntf));
    status = alljoyn_interfacedescription_addproperty(testIntf, "prop2", "i", ALLJOYN_PROP_ACCESS_WRITE);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    status = alljoyn_interfacedescription_addproperty(testIntf, "prop3", "u", ALLJOYN_PROP_ACCESS_RW);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    /*
     * At this point the interface only contains multiple properties the call to
     * hasproperties should return true.
     */
    EXPECT_TRUE(alljoyn_interfacedescription_hasproperties(testIntf));

    alljoyn_busattachment_destroy(bus);
}

TEST(InterfaceDescriptionTest, getname) {
    QStatus status = ER_OK;
    alljoyn_busattachment bus = NULL;
    bus = alljoyn_busattachment_create("InterfaceDescriptionTest", QC_FALSE);
    alljoyn_interfacedescription testIntf = NULL;
    status = alljoyn_busattachment_createinterface(bus, "org.alljoyn.test.InterfaceDescription", &testIntf, QC_FALSE);

    EXPECT_STREQ("org.alljoyn.test.InterfaceDescription", alljoyn_interfacedescription_getname(testIntf));

    alljoyn_busattachment_destroy(bus);
}

TEST(InterfaceDescriptionTest, addmethod) {
    QStatus status = ER_OK;
    alljoyn_busattachment bus = NULL;
    bus = alljoyn_busattachment_create("InterfaceDescriptionTest", QC_FALSE);
    alljoyn_interfacedescription testIntf = NULL;
    status = alljoyn_busattachment_createinterface(bus, "org.alljoyn.test.InterfaceDescription", &testIntf, QC_FALSE);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    status = alljoyn_interfacedescription_addmethod(testIntf, "method1", "ss", "b", "string1,string2,bool", 0, 0);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    char* introspect;
    size_t buf  = alljoyn_interfacedescription_introspect(testIntf, NULL, 0, 0);
    buf++;
    introspect = (char*)malloc(sizeof(char) * buf);
    alljoyn_interfacedescription_introspect(testIntf, introspect, buf, 0);
    const char* expectedIntrospect =
        "<interface name=\"org.alljoyn.test.InterfaceDescription\">\n"
        "  <method name=\"method1\">\n"
        "    <arg name=\"string1\" type=\"s\" direction=\"in\"/>\n"
        "    <arg name=\"string2\" type=\"s\" direction=\"in\"/>\n"
        "    <arg name=\"bool\" type=\"b\" direction=\"out\"/>\n"
        "  </method>\n"
        "</interface>\n";
    EXPECT_STREQ(expectedIntrospect, introspect);
    free(introspect);
    alljoyn_busattachment_destroy(bus);
}

TEST(InterfaceDescriptionTest, getmethod) {
    QStatus status = ER_OK;
    alljoyn_busattachment bus = NULL;
    bus = alljoyn_busattachment_create("InterfaceDescriptionTest", QC_FALSE);
    alljoyn_interfacedescription testIntf = NULL;
    status = alljoyn_busattachment_createinterface(bus, "org.alljoyn.test.InterfaceDescription", &testIntf, QC_FALSE);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    status = alljoyn_interfacedescription_addmethod(testIntf, "method1", "ss", "b", "string1,string2,bool", 0, 0);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    alljoyn_interfacedescription_member member;
    EXPECT_TRUE(alljoyn_interfacedescription_getmethod(testIntf, "method1", &member));

    EXPECT_EQ(testIntf, member.iface);
    EXPECT_EQ(ALLJOYN_MESSAGE_METHOD_CALL, member.memberType);
    EXPECT_STREQ("method1", member.name);
    EXPECT_STREQ("ss", member.signature);
    EXPECT_STREQ("b", member.returnSignature);
    EXPECT_STREQ("string1,string2,bool", member.argNames);
    EXPECT_EQ(0, member.annotation);

    EXPECT_FALSE(alljoyn_interfacedescription_getmethod(testIntf, "invalid", &member));

    /*
     * since we have not called alljoyn_interfacedescription_activate it is
     * possible to continue to add new members to the interface.
     */
    status = alljoyn_interfacedescription_addsignal(testIntf, "signal1", "s", "string", 0, 0);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    /*
     * get method should return false when trying to get a signal
     */
    EXPECT_FALSE(alljoyn_interfacedescription_getmethod(testIntf, "signal1", &member));

    alljoyn_busattachment_destroy(bus);
}

TEST(InterfaceDescriptionTest, addsignal) {
    QStatus status = ER_OK;
    alljoyn_busattachment bus = NULL;
    bus = alljoyn_busattachment_create("InterfaceDescriptionTest", QC_FALSE);
    alljoyn_interfacedescription testIntf = NULL;
    status = alljoyn_busattachment_createinterface(bus, "org.alljoyn.test.InterfaceDescription", &testIntf, QC_FALSE);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    status = alljoyn_interfacedescription_addsignal(testIntf, "signal1", "s", "string", 0, 0);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    char* introspect;
    size_t buf  = alljoyn_interfacedescription_introspect(testIntf, NULL, 0, 0);
    buf++;
    introspect = (char*)malloc(sizeof(char) * buf);
    alljoyn_interfacedescription_introspect(testIntf, introspect, buf, 0);
    const char* expectedIntrospect =
        "<interface name=\"org.alljoyn.test.InterfaceDescription\">\n"
        "  <signal name=\"signal1\">\n"
        "    <arg name=\"string\" type=\"s\" direction=\"in\"/>\n"
        "  </signal>\n"
        "</interface>\n";
    EXPECT_STREQ(expectedIntrospect, introspect);
    free(introspect);
    alljoyn_busattachment_destroy(bus);
}

TEST(InterfaceDescriptionTest, getsignal) {
    QStatus status = ER_OK;
    alljoyn_busattachment bus = NULL;
    bus = alljoyn_busattachment_create("InterfaceDescriptionTest", QC_FALSE);
    alljoyn_interfacedescription testIntf = NULL;
    status = alljoyn_busattachment_createinterface(bus, "org.alljoyn.test.InterfaceDescription", &testIntf, QC_FALSE);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    status = alljoyn_interfacedescription_addsignal(testIntf, "signal1", "s", "string", 0, 0);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    alljoyn_interfacedescription_member member;
    EXPECT_TRUE(alljoyn_interfacedescription_getsignal(testIntf, "signal1", &member));

    EXPECT_EQ(testIntf, member.iface);
    EXPECT_EQ(ALLJOYN_MESSAGE_SIGNAL, member.memberType);
    EXPECT_STREQ("signal1", member.name);
    EXPECT_STREQ("s", member.signature);
    EXPECT_STREQ("", member.returnSignature);
    EXPECT_STREQ("string", member.argNames);
    EXPECT_EQ(0, member.annotation);

    EXPECT_FALSE(alljoyn_interfacedescription_getsignal(testIntf, "invalid", &member));

    /*
     * since we have not called alljoyn_interfacedescription_activate it is
     * possible to continue to add new members to the interface.
     */
    status = alljoyn_interfacedescription_addmethod(testIntf, "method1", "ss", "b", "string1,string2,bool", 0, 0);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);

    EXPECT_FALSE(alljoyn_interfacedescription_getsignal(testIntf, "method1", &member));

    alljoyn_busattachment_destroy(bus);
}

TEST(InterfaceDescriptionTest, getproperty) {
    QStatus status = ER_OK;
    alljoyn_busattachment bus = NULL;
    bus = alljoyn_busattachment_create("InterfaceDescriptionTest", QC_FALSE);
    alljoyn_interfacedescription testIntf = NULL;
    status = alljoyn_busattachment_createinterface(bus, "org.alljoyn.test.InterfaceDescription", &testIntf, QC_FALSE);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    status = alljoyn_interfacedescription_addproperty(testIntf, "prop1", "s", ALLJOYN_PROP_ACCESS_READ);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    status = alljoyn_interfacedescription_addproperty(testIntf, "prop2", "i", ALLJOYN_PROP_ACCESS_WRITE);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    status = alljoyn_interfacedescription_addproperty(testIntf, "prop3", "u", ALLJOYN_PROP_ACCESS_RW);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);

    alljoyn_interfacedescription_property propa;
    EXPECT_TRUE(alljoyn_interfacedescription_getproperty(testIntf, "prop1", &propa));
    EXPECT_STREQ("prop1", propa.name);
    EXPECT_STREQ("s", propa.signature);
    EXPECT_EQ(ALLJOYN_PROP_ACCESS_READ, propa.access);
    alljoyn_interfacedescription_property propb;
    EXPECT_TRUE(alljoyn_interfacedescription_getproperty(testIntf, "prop2", &propb));
    EXPECT_STREQ("prop2", propb.name);
    EXPECT_STREQ("i", propb.signature);
    EXPECT_EQ(ALLJOYN_PROP_ACCESS_WRITE, propb.access);
    alljoyn_interfacedescription_property propc;
    EXPECT_TRUE(alljoyn_interfacedescription_getproperty(testIntf, "prop3", &propc));
    EXPECT_STREQ("prop3", propc.name);
    EXPECT_STREQ("u", propc.signature);
    EXPECT_EQ(ALLJOYN_PROP_ACCESS_RW, propc.access);

    alljoyn_busattachment_destroy(bus);
}

TEST(InterfaceDescriptionTest, getproperties) {
    QStatus status = ER_OK;
    alljoyn_busattachment bus = NULL;
    bus = alljoyn_busattachment_create("InterfaceDescriptionTest", QC_FALSE);
    alljoyn_interfacedescription testIntf = NULL;
    status = alljoyn_busattachment_createinterface(bus, "org.alljoyn.test.InterfaceDescription", &testIntf, QC_FALSE);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    status = alljoyn_interfacedescription_addproperty(testIntf, "prop1", "s", ALLJOYN_PROP_ACCESS_READ);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    status = alljoyn_interfacedescription_addproperty(testIntf, "prop2", "i", ALLJOYN_PROP_ACCESS_WRITE);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    status = alljoyn_interfacedescription_addproperty(testIntf, "prop3", "u", ALLJOYN_PROP_ACCESS_RW);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);

    alljoyn_interfacedescription_property prop[6];
    size_t size = alljoyn_interfacedescription_getproperties(testIntf, prop, 6);
    EXPECT_EQ((size_t)3, size);
    EXPECT_STREQ("prop1", prop[0].name);
    EXPECT_STREQ("s", prop[0].signature);
    EXPECT_EQ(ALLJOYN_PROP_ACCESS_READ, prop[0].access);

    EXPECT_STREQ("prop2", prop[1].name);
    EXPECT_STREQ("i", prop[1].signature);
    EXPECT_EQ(ALLJOYN_PROP_ACCESS_WRITE, prop[1].access);

    EXPECT_STREQ("prop3", prop[2].name);
    EXPECT_STREQ("u", prop[2].signature);
    EXPECT_EQ(ALLJOYN_PROP_ACCESS_RW, prop[2].access);

    /*
     * testing to see if it will not cause a problem if the array does not have
     * enough room for all of the properties.
     */
    alljoyn_interfacedescription_property prop2[2];
    size = alljoyn_interfacedescription_getproperties(testIntf, prop2, 2);
    EXPECT_EQ((size_t)2, size);
    EXPECT_STREQ("prop1", prop2[0].name);
    EXPECT_STREQ("s", prop2[0].signature);
    EXPECT_EQ(ALLJOYN_PROP_ACCESS_READ, prop2[0].access);

    EXPECT_STREQ("prop2", prop2[1].name);
    EXPECT_STREQ("i", prop2[1].signature);
    EXPECT_EQ(ALLJOYN_PROP_ACCESS_WRITE, prop2[1].access);

    alljoyn_busattachment_destroy(bus);
}

TEST(InterfaceDescriptionTest, alljoyn_interfacedescription_member_eql)
{
    QStatus status = ER_OK;
    alljoyn_busattachment bus = NULL;
    bus = alljoyn_busattachment_create("InterfaceDescriptionTest", QC_FALSE);
    ASSERT_TRUE(bus != NULL);
    alljoyn_interfacedescription testIntf = NULL;
    status = alljoyn_busattachment_createinterface(bus, "org.alljoyn.test.InterfaceDescription", &testIntf, QC_FALSE);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    status = alljoyn_interfacedescription_addmember(testIntf, ALLJOYN_MESSAGE_METHOD_CALL, "ping", "s", "s", "in,out", 0);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    status = alljoyn_interfacedescription_addmember(testIntf, ALLJOYN_MESSAGE_SIGNAL, "chirp", "s", NULL, "chirp", 0);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);

    alljoyn_interfacedescription_member member;
    EXPECT_TRUE(alljoyn_interfacedescription_getmember(testIntf, "ping", &member));

    alljoyn_interfacedescription_member other_member;
    EXPECT_TRUE(alljoyn_interfacedescription_getmember(testIntf, "ping", &other_member));

    alljoyn_interfacedescription_member other_member2;
    EXPECT_TRUE(alljoyn_interfacedescription_getmember(testIntf, "chirp", &other_member2));

    EXPECT_TRUE(alljoyn_interfacedescription_member_eql(member, other_member));

    EXPECT_FALSE(alljoyn_interfacedescription_member_eql(member, other_member2));

    alljoyn_busattachment_destroy(bus);
}

TEST(InterfaceDescriptionTest, alljoyn_interfacedescription_property_eql)
{
    QStatus status = ER_OK;
    alljoyn_busattachment bus = NULL;
    bus = alljoyn_busattachment_create("InterfaceDescriptionTest", QC_FALSE);
    alljoyn_interfacedescription testIntf = NULL;
    status = alljoyn_busattachment_createinterface(bus, "org.alljoyn.test.InterfaceDescription", &testIntf, QC_FALSE);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    status = alljoyn_interfacedescription_addproperty(testIntf, "prop1", "s", ALLJOYN_PROP_ACCESS_READ);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    status = alljoyn_interfacedescription_addproperty(testIntf, "prop2", "i", ALLJOYN_PROP_ACCESS_WRITE);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);

    alljoyn_interfacedescription_property propa;
    EXPECT_TRUE(alljoyn_interfacedescription_getproperty(testIntf, "prop1", &propa));

    alljoyn_interfacedescription_property propa2;
    EXPECT_TRUE(alljoyn_interfacedescription_getproperty(testIntf, "prop1", &propa2));

    alljoyn_interfacedescription_property propb;
    EXPECT_TRUE(alljoyn_interfacedescription_getproperty(testIntf, "prop2", &propb));

    EXPECT_TRUE(alljoyn_interfacedescription_property_eql(propa, propa2));

    EXPECT_FALSE(alljoyn_interfacedescription_property_eql(propa, propb));
}
