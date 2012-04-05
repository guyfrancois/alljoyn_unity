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
#include <alljoyn_c/BusAttachment.h>
#include <qcc/Thread.h>

/*constants*/
static const char* INTERFACE_NAME = "org.alljoyn.test.SessionTest";
static const char* OBJECT_NAME = "org.alljoyn.test.SessionTest";
static const char* OBJECT_PATH = "/org/alljoyn/test/SessionTest";
static const alljoyn_sessionport SESSION_PORT = 42;

QC_BOOL joincompleted_flag;
QC_BOOL joinsessionhandler_flag;
QC_BOOL foundadvertisedname_flag;
QC_BOOL sessionjoined_flag;
alljoyn_sessionid joinsessionid;
alljoyn_sessionid joinsessionid_alt;

/* AcceptSessionJoiner callback */
static QC_BOOL accept_session_joiner(const void* context, alljoyn_sessionport sessionPort,
                                     const char* joiner,  const alljoyn_sessionopts opts)
{
    //printf("accept_session_joiner\n");
    QC_BOOL ret = QC_FALSE;
    if (sessionPort != SESSION_PORT) {
        //printf("Rejecting join attempt on unexpected session port %d\n", sessionPort);
    } else {
        //printf("Accepting join session request from %s (opts.proximity=%x, opts.traffic=%x, opts.transports=%x)\n",
        //       joiner, alljoyn_sessionopts_proximity(opts), alljoyn_sessionopts_traffic(opts), alljoyn_sessionopts_transports(opts));
        ret = QC_TRUE;
    }
    return ret;
}

static void session_joined(const void* context, alljoyn_sessionport sessionPort, alljoyn_sessionid id, const char* joiner)
{
    //printf("session_joined\n");
    EXPECT_EQ(SESSION_PORT, sessionPort);
    joinsessionid = id;
    sessionjoined_flag = true;
}

static void joinsessionhandler(QStatus status, alljoyn_sessionid sessionId, const alljoyn_sessionopts opts, void* context)
{
    EXPECT_STREQ("A test string to send as the context void*", (char*)context);
    joinsessionid_alt = sessionId;
    joinsessionhandler_flag = true;
}


/* FoundAdvertisedName callback */
void found_advertised_name(const void* context, const char* name, alljoyn_transportmask transport, const char* namePrefix)
{
    //printf("FoundAdvertisedName(name=%s, prefix=%s)\n", name, namePrefix);
    EXPECT_STREQ(OBJECT_NAME, name);
    if (0 == strcmp(name, OBJECT_NAME)) {
        foundadvertisedname_flag = QC_TRUE;
    }
}
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

class SessionTest : public testing::Test {
  public:
    virtual void SetUp() {
        bus = alljoyn_busattachment_create("SessionTest", false);
        status = alljoyn_busattachment_start(bus);
        EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
        status = alljoyn_busattachment_connect(bus, ajn::getConnectArg().c_str());
        EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    }

    virtual void TearDown() {
        EXPECT_NO_FATAL_FAILURE(alljoyn_busattachment_destroy(bus));
    }

    void SetUpSessionTestService()
    {
        /* create/start/connect alljoyn_busattachment */
        servicebus = alljoyn_busattachment_create("SessionTestservice", false);
        status = alljoyn_busattachment_start(servicebus);
        EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
        status = alljoyn_busattachment_connect(servicebus, ajn::getConnectArg().c_str());
        EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);

        alljoyn_interfacedescription testIntf = NULL;
        status = alljoyn_busattachment_createinterface(servicebus, INTERFACE_NAME, &testIntf, QC_FALSE);
        EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
        status = alljoyn_interfacedescription_addmethod(testIntf, "ping", "s", "s", "in,out", 0, 0);
        EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
        alljoyn_interfacedescription_activate(testIntf);

//        /* register bus listener */
//        alljoyn_buslistener_callbacks buslistenerCbs = {
//            NULL, /* bus listener registered CB*/
//            NULL, /* bus listener unregistered CB */
//            NULL, /* found advertised name CB*/
//            NULL, /* lost advertised name CB */
//            NULL, /* name owner changed CB*/
//            NULL, /* Bus Stopping CB */
//            NULL /* Bus Disconnected CB*/
//        };
//        buslistener = alljoyn_buslistener_create(&buslistenerCbs, NULL);
//        alljoyn_busattachment_registerbuslistener(servicebus, buslistener);

        /* Set up bus object */
        alljoyn_busobject_callbacks busObjCbs = {
            NULL, /* Get alljoyn property CB */
            NULL, /* Set alljoyn property CB */
            NULL, /* BusObject Registered CB */
            NULL  /* BusObject Unregistered CB */
        };
        alljoyn_busobject testObj = alljoyn_busobject_create(servicebus, OBJECT_PATH, QC_FALSE, &busObjCbs, NULL);

        status = alljoyn_busobject_addinterface(testObj, testIntf);
        EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);

        /* register method handlers */
        alljoyn_interfacedescription_member ping_member;
        EXPECT_TRUE(alljoyn_interfacedescription_getmember(testIntf, "ping", &ping_member));

        status = alljoyn_busobject_addmethodhandler(testObj, ping_member, &ping_method, NULL);
        EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);

        status = alljoyn_busattachment_registerbusobject(servicebus, testObj);
        EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);

        /* request name */
        uint32_t flags = DBUS_NAME_FLAG_REPLACE_EXISTING | DBUS_NAME_FLAG_DO_NOT_QUEUE;
        status = alljoyn_busattachment_requestname(servicebus, OBJECT_NAME, flags);
        EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);

        /* Create session port listener */
        alljoyn_sessionportlistener_callbacks spl_cbs = {
            &accept_session_joiner, /* accept session joiner CB */
            &session_joined /* session joined CB */
        };
        sessionPortListener = alljoyn_sessionportlistener_create(&spl_cbs, NULL);

        /* Bind SessionPort */
        alljoyn_sessionopts opts = alljoyn_sessionopts_create(ALLJOYN_TRAFFIC_TYPE_MESSAGES, QC_FALSE, ALLJOYN_PROXIMITY_ANY, ALLJOYN_TRANSPORT_ANY);
        alljoyn_sessionport sp = SESSION_PORT;
        status = alljoyn_busattachment_bindsessionport(servicebus, &sp, opts, sessionPortListener);
        EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);

        /* Advertise Name */
        status = alljoyn_busattachment_advertisename(servicebus, OBJECT_NAME, alljoyn_sessionopts_transports(opts));
        EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    }

    void TearDownSessionTestService()
    {
        //alljoyn_busattachment_unregisterbuslistener(servicebus, buslistener);
        /*
         * must destroy the busattachment before destroying the buslistener or
         * the code will segfault when the code tries to call the bus_stopping
         * callback.
         */
        alljoyn_busattachment_destroy(servicebus);
//        alljoyn_buslistener_destroy(buslistener);
    }

    QStatus status;
    alljoyn_busattachment bus;

    alljoyn_busattachment servicebus;
    alljoyn_buslistener buslistener;
    alljoyn_sessionportlistener sessionPortListener;
};


TEST_F(SessionTest, joinsession) {
    SetUpSessionTestService();

    /* Create a bus listener */
    alljoyn_buslistener_callbacks callbacks = {
        NULL,     /* listener registered CB */
        NULL,     /* listener unregistered CB */
        &found_advertised_name,     /* found advertised name CB */
        NULL,     /* lost advertised name CB */
        NULL,     /* name owner changed CB */
        NULL,     /* bus stopping CB*/
        NULL     /* bus disconnected CB */
    };

    buslistener = alljoyn_buslistener_create(&callbacks, NULL);
    alljoyn_busattachment_registerbuslistener(bus, buslistener);

    foundadvertisedname_flag = QC_FALSE;
    sessionjoined_flag = QC_FALSE;
    /* Begin discover of the well-known name */
    status = alljoyn_busattachment_findadvertisedname(bus, OBJECT_NAME);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    for (size_t i = 0; i < 200; ++i) {
        if (foundadvertisedname_flag) {
            break;
        }
        qcc::Sleep(5);
    }
    EXPECT_TRUE(foundadvertisedname_flag);

    /* We found a remote bus that is advertising basic service's  well-known name so connect to it */
    alljoyn_sessionopts opts = alljoyn_sessionopts_create(ALLJOYN_TRAFFIC_TYPE_MESSAGES, QC_FALSE, ALLJOYN_PROXIMITY_ANY, ALLJOYN_TRANSPORT_ANY);
    alljoyn_sessionid sid;
    status = alljoyn_busattachment_joinsession(bus, OBJECT_NAME, SESSION_PORT, NULL, &sid, opts);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    for (size_t i = 0; i < 200; ++i) {
        if (sessionjoined_flag) {
            break;
        }
        qcc::Sleep(5);
    }
    EXPECT_TRUE(sessionjoined_flag);
    EXPECT_EQ(sid, joinsessionid);

    alljoyn_sessionopts_destroy(opts);
    //joincompleted_flag = QC_TRUE;

    alljoyn_busattachment_unregisterbuslistener(bus, buslistener);
    TearDownSessionTestService();
}

TEST_F(SessionTest, joinsessionasync) {
    SetUpSessionTestService();

    /* Create a bus listener */
    alljoyn_buslistener_callbacks callbacks = {
        NULL,     /* listener registered CB */
        NULL,     /* listener unregistered CB */
        &found_advertised_name,     /* found advertised name CB */
        NULL,     /* lost advertised name CB */
        NULL,     /* name owner changed CB */
        NULL,     /* bus stopping CB*/
        NULL     /* bus disconnected CB */
    };

    buslistener = alljoyn_buslistener_create(&callbacks, NULL);
    alljoyn_busattachment_registerbuslistener(bus, buslistener);

    foundadvertisedname_flag = QC_FALSE;
    sessionjoined_flag = QC_FALSE;
    /* Begin discover of the well-known name */
    status = alljoyn_busattachment_findadvertisedname(bus, OBJECT_NAME);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    for (size_t i = 0; i < 200; ++i) {
        if (foundadvertisedname_flag) {
            break;
        }
        qcc::Sleep(5);
    }
    EXPECT_TRUE(foundadvertisedname_flag);

    /* We found a remote bus that is advertising basic service's  well-known name so connect to it */
    alljoyn_sessionopts opts = alljoyn_sessionopts_create(ALLJOYN_TRAFFIC_TYPE_MESSAGES, QC_FALSE, ALLJOYN_PROXIMITY_ANY, ALLJOYN_TRANSPORT_ANY);

    char dave[64] = "A test string to send as the context void*";
    status = alljoyn_busattachment_joinsessionasync(bus, OBJECT_NAME, SESSION_PORT, NULL, opts, &joinsessionhandler, (void*)dave);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    for (size_t i = 0; i < 200; ++i) {
        if (sessionjoined_flag) {
            break;
        }
        qcc::Sleep(5);
    }
    EXPECT_TRUE(sessionjoined_flag);
    EXPECT_EQ(joinsessionid_alt, joinsessionid);

    alljoyn_sessionopts_destroy(opts);

    alljoyn_busattachment_unregisterbuslistener(bus, buslistener);
    TearDownSessionTestService();
}

