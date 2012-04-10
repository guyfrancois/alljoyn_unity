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
#include <qcc/String.h>

/*constants*/
static const char* INTERFACE_NAME = "org.alljoyn.test.MessageTest";
static const char* OBJECT_NAME =    "org.alljoyn.test.MessageTest";
static const char* OBJECT_PATH =   "/org/alljoyn/test/MessageTest";

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

/* NameOwnerChanged callback */
static void name_owner_changed(const void* context, const char* busName, const char* previousOwner, const char* newOwner)
{
    if (strcmp(busName, OBJECT_NAME) == 0) {
        name_owner_changed_flag = QC_TRUE;
    }
}

class MessageTest : public testing::Test {
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

    void SetUpMessageTestService()
    {
        /* create/start/connect alljoyn_busattachment */
        servicebus = alljoyn_busattachment_create("MessageTestservice", false);
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

        /* add methodhandlers */
//        alljoyn_busobject_methodentry methodEntries[] = {,
//            { &ping_member, ping_method },
//        };
//        status = alljoyn_busobject_addmethodhandlers(testObj, methodEntries, sizeof(methodEntries) / sizeof(methodEntries[0]));
        status = alljoyn_busobject_addmethodhandler(testObj, ping_member, &ping_method, NULL);
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

    void TearDownMessageTestService()
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

TEST_F(MessageTest, getarg__getargs_parseargs) {
    SetUpMessageTestService();

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

    str = NULL;
    alljoyn_msgarg output;
    size_t numArgs;
    alljoyn_message_getargs(reply, &numArgs, &output);
    EXPECT_EQ((size_t)1, numArgs);
    EXPECT_STREQ("s", alljoyn_msgarg_signature(alljoyn_msgarg_array_element(output, 0)));
    status = alljoyn_msgarg_get(alljoyn_msgarg_array_element(output, 0), "s", &str);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    EXPECT_STREQ("AllJoyn", str);

    str = NULL;
    status = alljoyn_message_parseargs(reply, "s", &str);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    EXPECT_STREQ("AllJoyn", str);

    alljoyn_message_destroy(reply);
    alljoyn_msgarg_destroy(input);
    alljoyn_proxybusobject_destroy(proxyObj);

    TearDownMessageTestService();
}

TEST_F(MessageTest, message_properties) {
    SetUpMessageTestService();

    alljoyn_proxybusobject proxyObj = alljoyn_proxybusobject_create(bus, OBJECT_NAME, OBJECT_PATH, 0);
    EXPECT_TRUE(proxyObj);
    status = alljoyn_proxybusobject_introspectremoteobject(proxyObj);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);

    alljoyn_message reply = alljoyn_message_create(bus);
    alljoyn_msgarg input = alljoyn_msgarg_create_and_set("s", "AllJoyn");
    status = alljoyn_proxybusobject_methodcall(proxyObj, INTERFACE_NAME, "ping", input, 1, reply, ALLJOYN_MESSAGE_DEFAULT_TIMEOUT, 0);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);

    EXPECT_FALSE(alljoyn_message_isbroadcastsignal(reply));
    EXPECT_FALSE(alljoyn_message_isglobalbroadcast(reply));
    uint32_t timeLeft;
    EXPECT_FALSE(alljoyn_message_isexpired(reply, &timeLeft));
    EXPECT_NE((uint32_t)0, timeLeft);
    EXPECT_FALSE(alljoyn_message_isunreliable(reply));
    EXPECT_FALSE(alljoyn_message_isencrypted(reply));

    /* we don't expect andy of the flags to be set */
    EXPECT_EQ(0, alljoyn_message_getflags(reply));

    /* no security is being used so there should not be an auth mechanism specified */
    EXPECT_STREQ("", alljoyn_message_getauthmechanism(reply));

    EXPECT_EQ(ALLJOYN_MESSAGE_METHOD_RET, alljoyn_message_gettype(reply));

    /* The serial is unknown but it should not be zero */
    EXPECT_NE((uint32_t)0, alljoyn_message_getcallserial(reply));
    EXPECT_NE((uint32_t)0, alljoyn_message_getreplyserial(reply));

    EXPECT_STREQ("s", alljoyn_message_getsignature(reply));
    /* in this instance we can not find objectpathm interface name , or member name from the message*/
    EXPECT_STREQ("", alljoyn_message_getobjectpath(reply));
    EXPECT_STREQ("", alljoyn_message_getinterface(reply));
    EXPECT_STREQ("", alljoyn_message_getmembername(reply));

    const char* destination_uniqueName;
    destination_uniqueName = alljoyn_busattachment_getuniquename(bus);
    EXPECT_STREQ(destination_uniqueName, alljoyn_message_getdestination(reply));
    EXPECT_STREQ(destination_uniqueName, alljoyn_message_getreceiveendpointname(reply));

    const char* sender_uniqueName;
    sender_uniqueName = alljoyn_busattachment_getuniquename(servicebus);
    EXPECT_STREQ(sender_uniqueName, alljoyn_message_getsender(reply));

    EXPECT_EQ((uint32_t)0, alljoyn_message_getcompressiontoken(reply));
    EXPECT_EQ((alljoyn_sessionid)0, alljoyn_message_getsessionid(reply));

    char* str;
    size_t buf;
    buf = alljoyn_message_tostring(reply, NULL, 0);
    buf++;
    str = (char*)malloc(sizeof(char) * buf);
    alljoyn_message_tostring(reply, str, buf);
    qcc::String strTest = str;
    /* all messages should start by stating the endianness */
    EXPECT_EQ((size_t)0, strTest.find_first_of("<message endianness="));
    free(str);

    buf = alljoyn_message_description(reply, NULL, 0);
    buf++;
    str = (char*)malloc(sizeof(char) * buf);
    alljoyn_message_description(reply, str, buf);
    strTest = str;
    /* this call to description should return 'METHID_RET[<reply serial>](s)' */
    EXPECT_EQ((size_t)0, strTest.find_first_of("METHOD_RET["));
    free(str);

    TearDownMessageTestService();
}
