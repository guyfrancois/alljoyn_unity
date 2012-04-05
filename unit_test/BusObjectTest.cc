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

#include <qcc/Thread.h>

#include <string.h>
#include <alljoyn/DBusStd.h>
#include <alljoyn_c/BusAttachment.h>
#include <alljoyn_c/BusObject.h>
#include <alljoyn_c/MsgArg.h>

#include "ajTestCommon.h"

/*constants*/
static const char* INTERFACE_NAME = "org.alljoyn.test.BusObjectTest";
static const char* OBJECT_NAME = "org.alljoyn.test.BusObjectTest";
static const char* OBJECT_PATH = "/org/alljoyn/test/BusObjectTest";
static const alljoyn_sessionport SERVICE_PORT = 25;

/*********** BusObject callback functions **************/
static QC_BOOL object_registered_flag = QC_FALSE;
static QC_BOOL object_unregistered_flag = QC_FALSE;;
static QC_BOOL name_owner_changed_flag = QC_FALSE;

static const char* prop1 = "AllJoyn BusObject Test"; //read only property
static int32_t prop2; //write only property
static uint32_t prop3; //RW property
static QStatus get_property(const void* context, const char* ifcName, const char* propName, alljoyn_msgarg val)
{
    EXPECT_STREQ(INTERFACE_NAME, ifcName);
    QStatus status = ER_OK;
    if (0 == strcmp("prop1", propName)) {
        alljoyn_msgarg_set(val, "s", prop1);
    } else if (0 == strcmp("prop2", propName)) {
        alljoyn_msgarg_set(val, "i", prop2);
    } else if (0 == strcmp("prop3", propName)) {
        alljoyn_msgarg_set(val, "u", prop3);
    } else {
        status = ER_BUS_NO_SUCH_PROPERTY;
    }
    return status;
}

static QStatus set_property(const void* context, const char* ifcName, const char* propName, alljoyn_msgarg val)
{
    EXPECT_STREQ(INTERFACE_NAME, ifcName);
    QStatus status = ER_OK;
    if (0 == strcmp("prop1", propName)) {
        alljoyn_msgarg_get(val, "s", &prop1);
    } else if (0 == strcmp("prop2", propName)) {
        alljoyn_msgarg_get(val, "i", &prop2);
    } else if (0 == strcmp("prop3", propName)) {
        alljoyn_msgarg_get(val, "u", &prop3);
    } else {
        status = ER_BUS_NO_SUCH_PROPERTY;
    }
    return status;
}

static void busobject_registered(const void* context)
{
    object_registered_flag = QC_TRUE;
}

static void busobject_unregistered(const void* context)
{
    object_unregistered_flag = QC_TRUE;
}

/* NameOwnerChanged callback */
static void name_owner_changed(const void* context, const char* busName, const char* previousOwner, const char* newOwner)
{
    if (strcmp(busName, OBJECT_NAME) == 0) {
        name_owner_changed_flag = QC_TRUE;
    }
}

/************* Method handlers *************/
static QC_BOOL chirp_method_flag = QC_FALSE;

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

class BusObjectTest : public testing::Test {
  public:
    virtual void SetUp() {
        object_registered_flag = QC_FALSE;
        object_unregistered_flag = QC_TRUE;

        bus = alljoyn_busattachment_create("ProxyBusObjectTest", false);
        status = alljoyn_busattachment_start(bus);
        EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
        status = alljoyn_busattachment_connect(bus, ajn::getConnectArg().c_str());
        EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    }

    virtual void TearDown() {
        EXPECT_NO_FATAL_FAILURE(alljoyn_busattachment_destroy(bus));
    }

    void SetUpBusObjectTestService()
    {
        /* create/start/connect alljoyn_busattachment */
        servicebus = alljoyn_busattachment_create("ProxyBusObjectTestservice", false);
        status = alljoyn_busattachment_start(servicebus);
        EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
        status = alljoyn_busattachment_connect(servicebus, ajn::getConnectArg().c_str());
        EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);

        alljoyn_interfacedescription testIntf = NULL;
        status = alljoyn_busattachment_createinterface(servicebus, INTERFACE_NAME, &testIntf, QC_FALSE);
        EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
        status = alljoyn_interfacedescription_addproperty(testIntf, "prop1", "s", ALLJOYN_PROP_ACCESS_READ);
        EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
        status = alljoyn_interfacedescription_addproperty(testIntf, "prop2", "i", ALLJOYN_PROP_ACCESS_WRITE);
        EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
        status = alljoyn_interfacedescription_addproperty(testIntf, "prop3", "u", ALLJOYN_PROP_ACCESS_RW);
        EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
        alljoyn_interfacedescription_activate(testIntf);
        /* Initialize properties to a known value*/
        prop2 = -32;
        prop3 =  42;
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
            &get_property,
            &set_property,
            &busobject_registered,
            &busobject_unregistered
        };
        alljoyn_busobject testObj = alljoyn_busobject_create(servicebus, OBJECT_PATH, QC_FALSE, &busObjCbs, NULL);

        status = alljoyn_busobject_addinterface(testObj, testIntf);
        EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);

        status = alljoyn_busattachment_registerbusobject(servicebus, testObj);
        EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
        for (size_t i = 0; i < 200; ++i) {
            if (object_registered_flag) {
                break;
            }
            qcc::Sleep(5);
        }
        EXPECT_TRUE(object_registered_flag);

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

    void TearDownBusObjectTestService()
    {
        //alljoyn_busattachment_unregisterbuslistener(servicebus, buslistener);
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

TEST_F(BusObjectTest, object_registered_unregistered)
{
    /* Set up bus object */
    alljoyn_busobject_callbacks busObjCbs = {
        &get_property,
        &set_property,
        &busobject_registered,
        &busobject_unregistered
    };
    alljoyn_busobject testObj = alljoyn_busobject_create(bus, OBJECT_PATH, QC_FALSE, &busObjCbs, NULL);
    status = alljoyn_busattachment_registerbusobject(bus, testObj);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    for (size_t i = 0; i < 200; ++i) {
        if (object_registered_flag) {
            break;
        }
        qcc::Sleep(5);
    }
    EXPECT_TRUE(object_registered_flag);

    alljoyn_busattachment_unregisterbusobject(bus, testObj);
    for (size_t i = 0; i < 200; ++i) {
        if (object_unregistered_flag) {
            break;
        }
        qcc::Sleep(5);
    }
    EXPECT_TRUE(object_unregistered_flag);

    alljoyn_busobject_destroy(testObj);
    alljoyn_busattachment_stop(bus);
    alljoyn_busattachment_join(bus);
}

TEST_F(BusObjectTest, get_property_handler)
{
    SetUpBusObjectTestService();

    alljoyn_proxybusobject proxyObj = alljoyn_proxybusobject_create(bus, OBJECT_NAME, OBJECT_PATH, 0);
    EXPECT_TRUE(proxyObj);
    status = alljoyn_proxybusobject_introspectremoteobject(proxyObj);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);

    alljoyn_msgarg value = alljoyn_msgarg_create();
    status = alljoyn_proxybusobject_getproperty(proxyObj, INTERFACE_NAME, "prop1", value);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    const char*str;
    status = alljoyn_msgarg_get(value, "s", &str);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    EXPECT_STREQ(prop1, str);
    alljoyn_msgarg_destroy(value);

    /* should fail to read a write only property*/
    value = alljoyn_msgarg_create();
    status = alljoyn_proxybusobject_getproperty(proxyObj, INTERFACE_NAME, "prop2", value);
    EXPECT_EQ(ER_BUS_REPLY_IS_ERROR_MESSAGE, status) << "  Actual Status: " << QCC_StatusText(status);
    alljoyn_msgarg_destroy(value);

    value = alljoyn_msgarg_create();
    status = alljoyn_proxybusobject_getproperty(proxyObj, INTERFACE_NAME, "prop3", value);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    uint32_t return_value;
    status = alljoyn_msgarg_get(value, "u", &return_value);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    EXPECT_EQ((uint32_t)42, return_value);

    TearDownBusObjectTestService();
}

TEST_F(BusObjectTest, set_property_handler)
{
    SetUpBusObjectTestService();

    alljoyn_proxybusobject proxyObj = alljoyn_proxybusobject_create(bus, OBJECT_NAME, OBJECT_PATH, 0);
    EXPECT_TRUE(proxyObj);
    status = alljoyn_proxybusobject_introspectremoteobject(proxyObj);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);

    /* should fail to write a read only property*/
    alljoyn_msgarg value = alljoyn_msgarg_create_and_set("s", "This should not work.");
    status = alljoyn_proxybusobject_setproperty(proxyObj, INTERFACE_NAME, "prop1", value);
    EXPECT_EQ(ER_BUS_REPLY_IS_ERROR_MESSAGE, status) << "  Actual Status: " << QCC_StatusText(status);
    alljoyn_msgarg_destroy(value);

    value = alljoyn_msgarg_create_and_set("i", -888);
    status = alljoyn_proxybusobject_setproperty(proxyObj, INTERFACE_NAME, "prop2", value);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    EXPECT_EQ(-888, prop2);
    alljoyn_msgarg_destroy(value);

    value = alljoyn_msgarg_create_and_set("u", 98);
    status = alljoyn_proxybusobject_setproperty(proxyObj, INTERFACE_NAME, "prop3", value);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    EXPECT_EQ((uint32_t)98, prop3);
    alljoyn_msgarg_destroy(value);

    TearDownBusObjectTestService();
}

TEST_F(BusObjectTest, getall_properties)
{
    SetUpBusObjectTestService();

    alljoyn_proxybusobject proxyObj = alljoyn_proxybusobject_create(bus, OBJECT_NAME, OBJECT_PATH, 0);
    EXPECT_TRUE(proxyObj);
    status = alljoyn_proxybusobject_introspectremoteobject(proxyObj);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);

    alljoyn_msgarg value = alljoyn_msgarg_create();
    status = alljoyn_proxybusobject_getallproperties(proxyObj, INTERFACE_NAME, value);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    alljoyn_msgarg variant_arg;
    const char* str;
    status = alljoyn_msgarg_getdictelement(value, "{sv}", "prop1", &variant_arg);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    status = alljoyn_msgarg_get(variant_arg, "s", &str);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    EXPECT_STREQ(prop1, str);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);

    uint32_t num;
    status = alljoyn_msgarg_getdictelement(value, "{sv}", "prop3", &variant_arg);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    status = alljoyn_msgarg_get(variant_arg, "u", &num);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    EXPECT_EQ((uint32_t)42, num);

    TearDownBusObjectTestService();
}

TEST_F(BusObjectTest, addmethodhandler)
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
    status = alljoyn_interfacedescription_addmethod(testIntf, "ping", "s", "s", "in,out", 0, 0);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    status = alljoyn_interfacedescription_addmethod(testIntf, "chirp", "s", "", "chirp", 0, 0);
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

    status = alljoyn_busobject_addmethodhandler(testObj, ping_member, &ping_method, NULL);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);

    status = alljoyn_busobject_addmethodhandler(testObj, chirp_member, &chirp_method, NULL);
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

    /* call methods */
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

    chirp_method_flag = QC_FALSE;

    status = alljoyn_proxybusobject_methodcall(proxyObj, INTERFACE_NAME, "chirp", input, 1, reply, ALLJOYN_MESSAGE_DEFAULT_TIMEOUT, 0);
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

    /* cleanup */
    alljoyn_busattachment_destroy(servicebus);
    alljoyn_buslistener_destroy(buslistener);
}

TEST_F(BusObjectTest, addmethodhandlers)
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
    status = alljoyn_interfacedescription_addmethod(testIntf, "ping", "s", "s", "in,out", 0, 0);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    status = alljoyn_interfacedescription_addmethod(testIntf, "chirp", "s", "", "chirp", 0, 0);
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

    /* call methods */
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

    chirp_method_flag = QC_FALSE;

    status = alljoyn_proxybusobject_methodcall(proxyObj, INTERFACE_NAME, "chirp", input, 1, reply, ALLJOYN_MESSAGE_DEFAULT_TIMEOUT, 0);
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

    /* cleanup */
    alljoyn_busattachment_destroy(servicebus);
    alljoyn_buslistener_destroy(buslistener);
}

TEST_F(BusObjectTest, addmethodhandler_addmethodhandlers_mix)
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
    status = alljoyn_interfacedescription_addmethod(testIntf, "ping", "s", "s", "in,out", 0, 0);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    status = alljoyn_interfacedescription_addmethod(testIntf, "chirp", "s", "", "chirp", 0, 0);
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
    };
    status = alljoyn_busobject_addmethodhandlers(testObj, methodEntries, sizeof(methodEntries) / sizeof(methodEntries[0]));
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);

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

    /* call methods */
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

    chirp_method_flag = QC_FALSE;

    status = alljoyn_proxybusobject_methodcall(proxyObj, INTERFACE_NAME, "chirp", input, 1, reply, ALLJOYN_MESSAGE_DEFAULT_TIMEOUT, 0);
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

    /* cleanup */
    alljoyn_busattachment_destroy(servicebus);
    alljoyn_buslistener_destroy(buslistener);
}
