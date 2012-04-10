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
#include "ajTestCommon.h"
#include <alljoyn/DBusStd.h>
#include <alljoyn_c/Message.h>
#include <alljoyn_c/BusAttachment.h>
#include <alljoyn_c/ProxyBusObject.h>
#include <qcc/Thread.h>

/*constants*/
static const char* INTERFACE_NAME = "org.alljoyn.test.ProxyBusObjectTest";
static const char* OBJECT_NAME =    "org.alljoyn.test.ProxyBusObjectTest";
static const char* OBJECT_PATH =   "/org/alljoyn/test/ProxyObjectTest";

static QC_BOOL chirp_method_flag = QC_FALSE;
static QC_BOOL name_owner_changed_flag = QC_FALSE;

/* Exposed methods */
static void ping_method(alljoyn_busobject bus, const alljoyn_interfacedescription_member* member, alljoyn_message msg)
{
    alljoyn_msgarg outArg = alljoyn_msgarg_create();
    outArg = alljoyn_message_getarg(msg, 0);
    const char* str;
    alljoyn_msgarg_get(outArg, "s", &str);
    QStatus status = alljoyn_busobject_methodreply_args(bus, msg, outArg, 1);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
}

static void chirp_method(alljoyn_busobject bus, const alljoyn_interfacedescription_member* member, alljoyn_message msg)
{
    chirp_method_flag = QC_TRUE;
    alljoyn_msgarg outArg = alljoyn_msgarg_create();
    outArg = alljoyn_message_getarg(msg, 0);
    const char* str;
    alljoyn_msgarg_get(outArg, "s", &str);
    QStatus status = alljoyn_busobject_methodreply_args(bus, msg, NULL, 0);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
}

/* NameOwnerChanged callback */
static void name_owner_changed(const void* context, const char* busName, const char* previousOwner, const char* newOwner)
{
    if (strcmp(busName, OBJECT_NAME) == 0) {
        name_owner_changed_flag = QC_TRUE;
    }
}

class ProxyBusObjectTest : public testing::Test {
  public:
    virtual void SetUp() {
        bus = alljoyn_busattachment_create("ProxyBusObjectTest", false);
        status = alljoyn_busattachment_start(bus);
        EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
        status = alljoyn_busattachment_connect(bus, ajn::getConnectArg().c_str());
        EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    }

    virtual void TearDown() {
        EXPECT_NO_FATAL_FAILURE(alljoyn_busattachment_destroy(bus));
    }

    void SetUpProxyBusObjectTestService()
    {
        /* create/start/connect alljoyn_busattachment */
        servicebus = alljoyn_busattachment_create("ProxyBusObjectTestservice", false);
        status = alljoyn_busattachment_start(servicebus);
        EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
        status = alljoyn_busattachment_connect(servicebus, ajn::getConnectArg().c_str());
        EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);

        /* create/activate alljoyn_interface */
        alljoyn_interfacedescription testIntf = NULL;
        status = alljoyn_busattachment_createinterface(servicebus, INTERFACE_NAME, &testIntf, QC_FALSE);
        EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
        status = alljoyn_interfacedescription_addmember(testIntf, ALLJOYN_MESSAGE_METHOD_CALL, "ping", "s", "s", "in,out", 0);
        EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
        status = alljoyn_interfacedescription_addmember(testIntf, ALLJOYN_MESSAGE_METHOD_CALL, "chirp", "s", "", "chirp", 0);
        EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
        alljoyn_interfacedescription_activate(testIntf);

        /* register bus listener */
        alljoyn_buslistener_callbacks buslistenerCbs = {
            NULL,
            NULL,
            NULL,
            NULL,
            &name_owner_changed,
            NULL,
            NULL
        };
        buslistener = alljoyn_buslistener_create(&buslistenerCbs, NULL);
        alljoyn_busattachment_registerbuslistener(servicebus, buslistener);

        /* Set up bus object */
        alljoyn_busobject_callbacks busObjCbs = {
            NULL,
            NULL,
            NULL,
            NULL
        };
        alljoyn_busobject testObj = alljoyn_busobject_create(servicebus, OBJECT_PATH, QC_FALSE, &busObjCbs, NULL);
        const alljoyn_interfacedescription exampleIntf = alljoyn_busattachment_getinterface(servicebus, INTERFACE_NAME);
        ASSERT_TRUE(exampleIntf);

        status = alljoyn_busobject_addinterface(testObj, exampleIntf);
        EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);

        /* register method handlers */
        alljoyn_interfacedescription_member ping_member;
        QC_BOOL foundMember = alljoyn_interfacedescription_getmember(exampleIntf, "ping", &ping_member);
        EXPECT_TRUE(foundMember);

        alljoyn_interfacedescription_member chirp_member;
        foundMember = alljoyn_interfacedescription_getmember(exampleIntf, "chirp", &chirp_member);
        EXPECT_TRUE(foundMember);

        /* add methodhandlers */
        alljoyn_busobject_methodentry methodEntries[] = {
            { &chirp_member, chirp_method },
            { &ping_member, ping_method },
        };
        status = alljoyn_busobject_addmethodhandlers(testObj, methodEntries, sizeof(methodEntries) / sizeof(methodEntries[0]));
        EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);

        status = alljoyn_busattachment_registerbusobject(servicebus, testObj);
        EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);

        name_owner_changed_flag = QC_FALSE;

        /* request name */
        uint32_t flags = DBUS_NAME_FLAG_REPLACE_EXISTING | DBUS_NAME_FLAG_DO_NOT_QUEUE;
        status = alljoyn_busattachment_requestname(servicebus, OBJECT_NAME, flags);
        EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
        for (size_t i = 0; i < 200; ++i) {
            if (name_owner_changed_flag) {
                break;
            }
            qcc::Sleep(5);
        }
        EXPECT_TRUE(name_owner_changed_flag);
    }

    void TearDownProxyBusObjectTestService()
    {
        alljoyn_busattachment_unregisterbuslistener(servicebus, buslistener);
        /*
         * must destroy the busattachment before destroying the buslistener or
         * the code will segfault when the code tries to call the bus_stopping
         * callback.
         */
        alljoyn_busattachment_destroy(servicebus);
        alljoyn_buslistener_destroy(buslistener);
    }

    QStatus status;
    alljoyn_busattachment bus;

    alljoyn_busattachment servicebus;
    alljoyn_buslistener buslistener;
};

TEST_F(ProxyBusObjectTest, create_destroy) {
    alljoyn_proxybusobject proxyObj = alljoyn_proxybusobject_create(bus, "org.alljoyn.Bus", "/org/alljoyn/Bus", 0);
    EXPECT_TRUE(proxyObj);
    EXPECT_NO_FATAL_FAILURE(alljoyn_proxybusobject_destroy(proxyObj));
}

TEST_F(ProxyBusObjectTest, introspectremoteobject) {
    alljoyn_proxybusobject proxyObj = alljoyn_proxybusobject_create(bus, "org.alljoyn.Bus", "/org/alljoyn/Bus", 0);
    EXPECT_TRUE(proxyObj);
    status = alljoyn_proxybusobject_introspectremoteobject(proxyObj);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    alljoyn_interfacedescription intf = alljoyn_proxybusobject_getinterface(proxyObj, "org.freedesktop.DBus.Introspectable");
    char* str = alljoyn_interfacedescription_introspect(intf, 0);
    EXPECT_STREQ(
        "<interface name=\"org.freedesktop.DBus.Introspectable\">\n"
        "  <method name=\"Introspect\">\n"
        "    <arg name=\"data\" type=\"s\" direction=\"out\"/>\n"
        "  </method>\n"
        "</interface>\n",
        str);
    free(str);
    EXPECT_NO_FATAL_FAILURE(alljoyn_proxybusobject_destroy(proxyObj));
}

QC_BOOL introspect_callback_flag = QC_FALSE;

void introspect_callback(QStatus status, alljoyn_proxybusobject obj, void* context)
{
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    alljoyn_interfacedescription intf = alljoyn_proxybusobject_getinterface(obj, "org.freedesktop.DBus.Introspectable");
    char* str = alljoyn_interfacedescription_introspect(intf, 0);
    EXPECT_STREQ(
        "<interface name=\"org.freedesktop.DBus.Introspectable\">\n"
        "  <method name=\"Introspect\">\n"
        "    <arg name=\"data\" type=\"s\" direction=\"out\"/>\n"
        "  </method>\n"
        "</interface>\n",
        str);
    free(str);
    introspect_callback_flag = QC_TRUE;
}

TEST_F(ProxyBusObjectTest, introspectremoteobjectasync) {
    alljoyn_proxybusobject proxyObj = alljoyn_proxybusobject_create(bus, "org.alljoyn.Bus", "/org/alljoyn/Bus", 0);
    EXPECT_TRUE(proxyObj);
    introspect_callback_flag = QC_FALSE;
    status = alljoyn_proxybusobject_introspectremoteobjectasync(proxyObj, &introspect_callback, NULL);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    for (size_t i = 0; i < 200; ++i) {
        if (introspect_callback_flag) {
            break;
        }
        qcc::Sleep(5);
    }
    EXPECT_TRUE(introspect_callback_flag);
    alljoyn_interfacedescription intf = alljoyn_proxybusobject_getinterface(proxyObj, "org.freedesktop.DBus.Introspectable");
    char* str = alljoyn_interfacedescription_introspect(intf, 0);
    EXPECT_STREQ(
        "<interface name=\"org.freedesktop.DBus.Introspectable\">\n"
        "  <method name=\"Introspect\">\n"
        "    <arg name=\"data\" type=\"s\" direction=\"out\"/>\n"
        "  </method>\n"
        "</interface>\n",
        str);
    free(str);
    EXPECT_NO_FATAL_FAILURE(alljoyn_proxybusobject_destroy(proxyObj));
}


TEST_F(ProxyBusObjectTest, getinterface_getinterfaces) {
    alljoyn_proxybusobject proxyObj = alljoyn_proxybusobject_create(bus, "org.alljoyn.Bus", "/org/alljoyn/Bus", 0);
    EXPECT_TRUE(proxyObj);
    status = alljoyn_proxybusobject_introspectremoteobject(proxyObj);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    alljoyn_interfacedescription intf = alljoyn_proxybusobject_getinterface(proxyObj, "org.freedesktop.DBus.Introspectable");
    char* intf_introspect = alljoyn_interfacedescription_introspect(intf, 0);
    EXPECT_STREQ(
        "<interface name=\"org.freedesktop.DBus.Introspectable\">\n"
        "  <method name=\"Introspect\">\n"
        "    <arg name=\"data\" type=\"s\" direction=\"out\"/>\n"
        "  </method>\n"
        "</interface>\n",
        intf_introspect);
    free(intf_introspect);

    alljoyn_interfacedescription intf_array[6];
    size_t count = alljoyn_proxybusobject_getinterfaces(proxyObj, intf_array, 6);
    /*
     * the org.alljoyn.Bus object should contain 4 interfaces
     * org.alljoyn.Bus
     * org.alljoyn.Deamon
     * org.freedesktop.DBus.Introspectable
     * org.freedesktop.DBus.Peer
     */
    EXPECT_EQ((size_t)4, count);
    EXPECT_STREQ("org.alljoyn.Bus", alljoyn_interfacedescription_getname(intf_array[0]));
    EXPECT_STREQ("org.alljoyn.Daemon", alljoyn_interfacedescription_getname(intf_array[1]));
    EXPECT_STREQ("org.freedesktop.DBus.Introspectable", alljoyn_interfacedescription_getname(intf_array[2]));
    EXPECT_STREQ("org.freedesktop.DBus.Peer", alljoyn_interfacedescription_getname(intf_array[3]));

    EXPECT_NO_FATAL_FAILURE(alljoyn_proxybusobject_destroy(proxyObj));
}

TEST_F(ProxyBusObjectTest, getpath) {
    alljoyn_proxybusobject proxyObj = alljoyn_proxybusobject_create(bus, "org.alljoyn.Bus", "/org/alljoyn/Bus", 0);
    EXPECT_TRUE(proxyObj);
    EXPECT_STREQ("/org/alljoyn/Bus", alljoyn_proxybusobject_getpath(proxyObj));

    EXPECT_NO_FATAL_FAILURE(alljoyn_proxybusobject_destroy(proxyObj));
}

TEST_F(ProxyBusObjectTest, getservicename) {
    alljoyn_proxybusobject proxyObj = alljoyn_proxybusobject_create(bus, "org.alljoyn.Bus", "/org/alljoyn/Bus", 0);
    EXPECT_TRUE(proxyObj);
    EXPECT_STREQ("org.alljoyn.Bus", alljoyn_proxybusobject_getservicename(proxyObj));

    EXPECT_NO_FATAL_FAILURE(alljoyn_proxybusobject_destroy(proxyObj));
}

TEST_F(ProxyBusObjectTest, getsessionid) {
    alljoyn_proxybusobject proxyObj = alljoyn_proxybusobject_create(bus, "org.alljoyn.Bus", "/org/alljoyn/Bus", 0);
    EXPECT_TRUE(proxyObj);
    EXPECT_EQ((alljoyn_sessionid)0, alljoyn_proxybusobject_getsessionid(proxyObj));

    EXPECT_NO_FATAL_FAILURE(alljoyn_proxybusobject_destroy(proxyObj));
    /*
     * TODO set up a session with a real session and make sure that proxyObj has
     * will return the proper sessionid.
     */

}

TEST_F(ProxyBusObjectTest, implementsinterface) {
    alljoyn_proxybusobject proxyObj = alljoyn_proxybusobject_create(bus, "org.alljoyn.Bus", "/org/alljoyn/Bus", 0);
    EXPECT_TRUE(proxyObj);
    status = alljoyn_proxybusobject_introspectremoteobject(proxyObj);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);

    EXPECT_TRUE(alljoyn_proxybusobject_implementsinterface(proxyObj, "org.alljoyn.Bus"));
    EXPECT_TRUE(alljoyn_proxybusobject_implementsinterface(proxyObj, "org.alljoyn.Daemon"));
    EXPECT_FALSE(alljoyn_proxybusobject_implementsinterface(proxyObj, "org.alljoyn.Invalid"));
    EXPECT_NO_FATAL_FAILURE(alljoyn_proxybusobject_destroy(proxyObj));
}

TEST_F(ProxyBusObjectTest, addinterface_by_name) {
    alljoyn_proxybusobject proxyObj = alljoyn_proxybusobject_create(bus, "org.alljoyn.Bus", "/org/alljoyn/Bus", 0);
    EXPECT_TRUE(proxyObj);

    status = alljoyn_proxybusobject_addinterface_by_name(proxyObj, "org.freedesktop.DBus.Introspectable");
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);

    alljoyn_interfacedescription intf = alljoyn_proxybusobject_getinterface(proxyObj, "org.freedesktop.DBus.Introspectable");
    char* intf_introspect = alljoyn_interfacedescription_introspect(intf, 0);
    EXPECT_STREQ(
        "<interface name=\"org.freedesktop.DBus.Introspectable\">\n"
        "  <method name=\"Introspect\">\n"
        "    <arg name=\"data\" type=\"s\" direction=\"out\"/>\n"
        "  </method>\n"
        "</interface>\n",
        intf_introspect);
    free(intf_introspect);

    EXPECT_FALSE(alljoyn_proxybusobject_implementsinterface(proxyObj, "org.alljoyn.Bus"));
    EXPECT_TRUE(alljoyn_proxybusobject_implementsinterface(proxyObj, "org.freedesktop.DBus.Introspectable"));
    EXPECT_NO_FATAL_FAILURE(alljoyn_proxybusobject_destroy(proxyObj));
}

TEST_F(ProxyBusObjectTest, addinterface) {
    alljoyn_interfacedescription testIntf = NULL;
    status = alljoyn_busattachment_createinterface(bus, "org.alljoyn.test.ProxyBusObjectTest", &testIntf, QC_FALSE);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    status = alljoyn_interfacedescription_addmember(testIntf, ALLJOYN_MESSAGE_METHOD_CALL, "ping", "s", "s", "in,out", 0);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    status = alljoyn_interfacedescription_addmember(testIntf, ALLJOYN_MESSAGE_SIGNAL, "chirp", "", "s", "chirp", 0);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);

    alljoyn_proxybusobject proxyObj = alljoyn_proxybusobject_create(bus, "org.alljoyn.test.ProxyBusObjectTest", "/org/alljoyn/test/ProxyObjectTest", 0);
    EXPECT_TRUE(proxyObj);

    status = alljoyn_proxybusobject_addinterface(proxyObj, testIntf);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);

    EXPECT_TRUE(alljoyn_proxybusobject_implementsinterface(proxyObj, "org.alljoyn.test.ProxyBusObjectTest"));

    char* introspect = alljoyn_interfacedescription_introspect(testIntf, 0);
    const char* expectedIntrospect =
        "<interface name=\"org.alljoyn.test.ProxyBusObjectTest\">\n"
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
}

TEST_F(ProxyBusObjectTest, methodcall) {
    SetUpProxyBusObjectTestService();

    alljoyn_proxybusobject proxyObj = alljoyn_proxybusobject_create(bus, OBJECT_NAME, OBJECT_PATH, 0);
    EXPECT_TRUE(proxyObj);
    status = alljoyn_proxybusobject_introspectremoteobject(proxyObj);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);

    alljoyn_message reply = alljoyn_message_create(bus);
    alljoyn_msgarg input = alljoyn_msgarg_create_and_set("s", "AllJoyn");
    status = alljoyn_proxybusobject_methodcall(proxyObj, INTERFACE_NAME, "ping", input, 1, reply, ALLJOYN_MESSAGE_DEFAULT_TIMEOUT, 0);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    const char* str;
    alljoyn_msgarg_get(alljoyn_message_getarg(reply, 0), "s", &str);
    EXPECT_STREQ("AllJoyn", str);

    alljoyn_message_destroy(reply);
    alljoyn_msgarg_destroy(input);
    alljoyn_proxybusobject_destroy(proxyObj);

    TearDownProxyBusObjectTestService();
}

TEST_F(ProxyBusObjectTest, methodcall_member) {
    SetUpProxyBusObjectTestService();

    alljoyn_proxybusobject proxyObj = alljoyn_proxybusobject_create(bus, OBJECT_NAME, OBJECT_PATH, 0);
    EXPECT_TRUE(proxyObj);
    status = alljoyn_proxybusobject_introspectremoteobject(proxyObj);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);

    alljoyn_message reply = alljoyn_message_create(bus);
    alljoyn_msgarg input = alljoyn_msgarg_create_and_set("s", "AllJoyn");

    /* register method handlers */
    alljoyn_interfacedescription_member ping_member_from_proxy;
    QC_BOOL foundMember = alljoyn_interfacedescription_getmember(alljoyn_proxybusobject_getinterface(proxyObj, INTERFACE_NAME), "ping", &ping_member_from_proxy);
    EXPECT_TRUE(foundMember);

    status = alljoyn_proxybusobject_methodcall_member(proxyObj, ping_member_from_proxy, input, 1, reply, ALLJOYN_MESSAGE_DEFAULT_TIMEOUT, 0);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    const char* str;
    alljoyn_msgarg_get(alljoyn_message_getarg(reply, 0), "s", &str);
    EXPECT_STREQ("AllJoyn", str);

    alljoyn_message_destroy(reply);
    alljoyn_msgarg_destroy(input);
    alljoyn_proxybusobject_destroy(proxyObj);

    TearDownProxyBusObjectTestService();
}

TEST_F(ProxyBusObjectTest, methodcall_noreply) {
    SetUpProxyBusObjectTestService();

    alljoyn_proxybusobject proxyObj = alljoyn_proxybusobject_create(bus, OBJECT_NAME, OBJECT_PATH, 0);
    EXPECT_TRUE(proxyObj);
    status = alljoyn_proxybusobject_introspectremoteobject(proxyObj);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);

    chirp_method_flag = QC_FALSE;

    alljoyn_message reply = alljoyn_message_create(bus);
    alljoyn_msgarg input = alljoyn_msgarg_create_and_set("s", "AllJoyn");
    status = alljoyn_proxybusobject_methodcall_noreply(proxyObj, INTERFACE_NAME, "chirp", input, 1, 0);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);

    for (size_t i = 0; i < 200; ++i) {
        if (chirp_method_flag) {
            break;
        }
        qcc::Sleep(5);
    }
    EXPECT_TRUE(chirp_method_flag);

    alljoyn_message_destroy(reply);
    alljoyn_msgarg_destroy(input);
    alljoyn_proxybusobject_destroy(proxyObj);

    TearDownProxyBusObjectTestService();
}

TEST_F(ProxyBusObjectTest, methodcall_member_noreply) {
    SetUpProxyBusObjectTestService();

    alljoyn_proxybusobject proxyObj = alljoyn_proxybusobject_create(bus, OBJECT_NAME, OBJECT_PATH, 0);
    EXPECT_TRUE(proxyObj);
    status = alljoyn_proxybusobject_introspectremoteobject(proxyObj);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);

    alljoyn_message reply = alljoyn_message_create(bus);
    alljoyn_msgarg input = alljoyn_msgarg_create_and_set("s", "AllJoyn");

    /* register method handlers */
    alljoyn_interfacedescription_member chirp_member_from_proxy;
    QC_BOOL foundMember = alljoyn_interfacedescription_getmember(alljoyn_proxybusobject_getinterface(proxyObj, INTERFACE_NAME), "chirp", &chirp_member_from_proxy);
    EXPECT_TRUE(foundMember);

    chirp_method_flag = QC_FALSE;

    status = alljoyn_proxybusobject_methodcall_member_noreply(proxyObj, chirp_member_from_proxy, input, 1, 0);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);

    for (size_t i = 0; i < 200; ++i) {
        if (chirp_method_flag) {
            break;
        }
        qcc::Sleep(5);
    }
    EXPECT_TRUE(chirp_method_flag);

    alljoyn_message_destroy(reply);
    alljoyn_msgarg_destroy(input);
    alljoyn_proxybusobject_destroy(proxyObj);

    TearDownProxyBusObjectTestService();
}
QC_BOOL ping_methodcall_reply_handler_flag = QC_FALSE;
QC_BOOL chirp_methodcall_reply_handler_flag = QC_FALSE;
void ping_methodcall_reply_handler(alljoyn_message message, void* context)
{
    // TODO add alljoyn_message_gettype()
    EXPECT_EQ(ALLJOYN_MESSAGE_METHOD_RET, alljoyn_message_gettype(message));
    EXPECT_STREQ("Input String to test context", (char*)context);
    const char* str;
    alljoyn_msgarg_get(alljoyn_message_getarg(message, 0), "s", &str);
    EXPECT_STREQ("AllJoyn", str);

    alljoyn_message_parseargs(message, "s", &str);
    EXPECT_STREQ("AllJoyn", str);

    ping_methodcall_reply_handler_flag = QC_TRUE;
}

void chirp_methodcall_reply_handler(alljoyn_message message, void* context)
{
    //not yet used in any tests
}

TEST_F(ProxyBusObjectTest, methodcallasync) {
    SetUpProxyBusObjectTestService();

    alljoyn_proxybusobject proxyObj = alljoyn_proxybusobject_create(bus, OBJECT_NAME, OBJECT_PATH, 0);
    EXPECT_TRUE(proxyObj);
    status = alljoyn_proxybusobject_introspectremoteobject(proxyObj);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);

    alljoyn_msgarg input = alljoyn_msgarg_create_and_set("s", "AllJoyn");

    ping_methodcall_reply_handler_flag = QC_FALSE;

    char context_str[64] = "Input String to test context";
    status = alljoyn_proxybusobject_methodcallasync(proxyObj,
                                                    INTERFACE_NAME,
                                                    "ping",
                                                    &ping_methodcall_reply_handler,
                                                    input,
                                                    1,
                                                    context_str,
                                                    ALLJOYN_MESSAGE_DEFAULT_TIMEOUT, 0);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    alljoyn_msgarg_destroy(input);
    alljoyn_proxybusobject_destroy(proxyObj);

    for (size_t i = 0; i < 200; ++i) {
        if (ping_methodcall_reply_handler_flag) {
            break;
        }
        qcc::Sleep(5);
    }
    EXPECT_TRUE(ping_methodcall_reply_handler_flag);

    TearDownProxyBusObjectTestService();
}

TEST_F(ProxyBusObjectTest, methodcallasync_member) {
    SetUpProxyBusObjectTestService();

    alljoyn_proxybusobject proxyObj = alljoyn_proxybusobject_create(bus, OBJECT_NAME, OBJECT_PATH, 0);
    EXPECT_TRUE(proxyObj);
    status = alljoyn_proxybusobject_introspectremoteobject(proxyObj);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);

    alljoyn_msgarg input = alljoyn_msgarg_create_and_set("s", "AllJoyn");

    ping_methodcall_reply_handler_flag = QC_FALSE;

    /* register method handlers */
    alljoyn_interfacedescription_member ping_member_from_proxy;
    QC_BOOL foundMember = alljoyn_interfacedescription_getmember(alljoyn_proxybusobject_getinterface(proxyObj, INTERFACE_NAME), "ping", &ping_member_from_proxy);
    EXPECT_TRUE(foundMember);
    char context_str[64] = "Input String to test context";
    status = alljoyn_proxybusobject_methodcallasync_member(proxyObj,
                                                           ping_member_from_proxy,
                                                           &ping_methodcall_reply_handler,
                                                           input,
                                                           1,
                                                           context_str,
                                                           ALLJOYN_MESSAGE_DEFAULT_TIMEOUT, 0);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    alljoyn_msgarg_destroy(input);
    alljoyn_proxybusobject_destroy(proxyObj);

    for (size_t i = 0; i < 200; ++i) {
        if (ping_methodcall_reply_handler_flag) {
            break;
        }
        qcc::Sleep(5);
    }
    EXPECT_TRUE(ping_methodcall_reply_handler_flag);

    TearDownProxyBusObjectTestService();
}
