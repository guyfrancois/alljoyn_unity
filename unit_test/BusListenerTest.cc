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
#include <qcc/Thread.h>
#include <alljoyn_c/BusAttachment.h>

/*constants*/
//static const char* INTERFACE_NAME = "org.alljoyn.test.BusListenerTest";
static const char* OBJECT_NAME = "org.alljoyn.test.BusListenerTest";
//static const char* OBJECT_PATH = "/org/alljoyn/test/BusListenerTest";

/* flags */
static QC_BOOL listener_registered_flag = QC_FALSE;
static QC_BOOL listener_unregistered_flag = QC_FALSE;
static QC_BOOL found_advertised_name_flag = QC_FALSE;
static QC_BOOL lost_advertised_name_flag = QC_FALSE;
static QC_BOOL name_owner_changed_flag = QC_FALSE;
static QC_BOOL bus_stopping_flag = QC_FALSE;
static QC_BOOL bus_disconnected_flag = QC_FALSE;

/* bus listener functions */
static void listener_registered(const void* context, alljoyn_busattachment bus) {
    listener_registered_flag = QC_TRUE;
}
static void listener_unregistered(const void* context) {
    listener_unregistered_flag = QC_TRUE;
}
static void found_advertised_name(const void* context, const char* name, alljoyn_transportmask transport, const char* namePrefix) {
    found_advertised_name_flag = QC_TRUE;
}
static void lost_advertised_name(const void* context, const char* name, alljoyn_transportmask transport, const char* namePrefix) {
    lost_advertised_name_flag = QC_TRUE;
}
static void name_owner_changed(const void* context, const char* busName, const char* previousOwner, const char* newOwner) {
    name_owner_changed_flag = QC_TRUE;
}
static void bus_stopping(const void* context) {
    bus_stopping_flag = QC_TRUE;
}
static void bus_disconnected(const void* context) {
    bus_disconnected_flag = QC_TRUE;
}

class BusListenerTest : public testing::Test {
  public:
    virtual void SetUp() {
        resetFlags();
        /* register bus listener */
        alljoyn_buslistener_callbacks buslistenerCbs = {
            &listener_registered,
            &listener_unregistered,
            &found_advertised_name,
            &lost_advertised_name,
            &name_owner_changed,
            &bus_stopping,
            &bus_disconnected
        };
        buslistener = alljoyn_buslistener_create(&buslistenerCbs, NULL);
        bus = alljoyn_busattachment_create("BusListenerTest", QC_FALSE);
    }

    virtual void TearDown() {
        EXPECT_NO_FATAL_FAILURE(alljoyn_buslistener_destroy(buslistener));
    }

    void resetFlags() {
        listener_registered_flag = QC_FALSE;
        listener_unregistered_flag = QC_FALSE;
        found_advertised_name_flag = QC_FALSE;
        lost_advertised_name_flag = QC_FALSE;
        name_owner_changed_flag = QC_FALSE;
        bus_stopping_flag = QC_FALSE;
        bus_disconnected_flag = QC_FALSE;
    }
    QStatus status;
    alljoyn_busattachment bus;
    alljoyn_buslistener buslistener;
};

TEST_F(BusListenerTest, listner_registered_unregistered) {
    alljoyn_busattachment_registerbuslistener(bus, buslistener);
    for (size_t i = 0; i < 200; ++i) {
        if (listener_registered_flag) {
            break;
        }
        qcc::Sleep(5);
    }
    EXPECT_TRUE(listener_registered_flag);
    alljoyn_busattachment_unregisterbuslistener(bus, buslistener);
    for (size_t i = 0; i < 200; ++i) {
        if (listener_unregistered_flag) {
            break;
        }
        qcc::Sleep(5);
    }
    EXPECT_TRUE(listener_unregistered_flag);
}

TEST_F(BusListenerTest, bus_stopping_disconnected) {

    status = alljoyn_busattachment_start(bus);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    status = alljoyn_busattachment_connect(bus, ajn::getConnectArg().c_str());

    alljoyn_busattachment_registerbuslistener(bus, buslistener);
    for (size_t i = 0; i < 200; ++i) {
        if (listener_registered_flag) {
            break;
        }
        qcc::Sleep(5);
    }
    EXPECT_TRUE(listener_registered_flag);

    alljoyn_busattachment_disconnect(bus, ajn::getConnectArg().c_str());
    for (size_t i = 0; i < 200; ++i) {
        if (bus_disconnected_flag) {
            break;
        }
        qcc::Sleep(5);
    }
    EXPECT_TRUE(bus_disconnected_flag);

    alljoyn_busattachment_stop(bus);
    for (size_t i = 0; i < 200; ++i) {
        if (bus_stopping_flag) {
            break;
        }
        qcc::Sleep(5);
    }
    EXPECT_TRUE(bus_stopping_flag);
    alljoyn_busattachment_join(bus);

    alljoyn_busattachment_unregisterbuslistener(bus, buslistener);
    for (size_t i = 0; i < 200; ++i) {
        if (listener_unregistered_flag) {
            break;
        }
        qcc::Sleep(5);
    }
    EXPECT_TRUE(listener_unregistered_flag);
}

TEST_F(BusListenerTest, found_lost__advertised_name) {
    status = alljoyn_busattachment_start(bus);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    status = alljoyn_busattachment_connect(bus, ajn::getConnectArg().c_str());

    alljoyn_busattachment_registerbuslistener(bus, buslistener);
    for (size_t i = 0; i < 200; ++i) {
        if (listener_registered_flag) {
            break;
        }
        qcc::Sleep(5);
    }
    EXPECT_TRUE(listener_registered_flag);

    alljoyn_sessionopts opts = alljoyn_sessionopts_create(ALLJOYN_TRAFFIC_TYPE_MESSAGES, QC_FALSE, ALLJOYN_PROXIMITY_ANY, ALLJOYN_TRANSPORT_ANY);

    status = alljoyn_busattachment_findadvertisedname(bus, OBJECT_NAME);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);

    status = alljoyn_busattachment_advertisename(bus, OBJECT_NAME, alljoyn_sessionopts_transports(opts));
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);

    for (size_t i = 0; i < 200; ++i) {
        if (found_advertised_name_flag) {
            break;
        }
        qcc::Sleep(5);
    }
    EXPECT_TRUE(found_advertised_name_flag);

    status = alljoyn_busattachment_canceladvertisename(bus, OBJECT_NAME, alljoyn_sessionopts_transports(opts));
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    for (size_t i = 0; i < 200; ++i) {
        if (lost_advertised_name_flag) {
            break;
        }
        qcc::Sleep(5);
    }
    EXPECT_TRUE(lost_advertised_name_flag);

    alljoyn_busattachment_stop(bus);
    for (size_t i = 0; i < 200; ++i) {
        if (bus_stopping_flag) {
            break;
        }
        qcc::Sleep(5);
    }
    EXPECT_TRUE(bus_stopping_flag);
    alljoyn_busattachment_join(bus);
    /* the bus will automatically disconnect when it is stopped */
    for (size_t i = 0; i < 200; ++i) {
        if (bus_disconnected_flag) {
            break;
        }
        qcc::Sleep(5);
    }
    EXPECT_TRUE(bus_disconnected_flag);

    alljoyn_busattachment_unregisterbuslistener(bus, buslistener);
    for (size_t i = 0; i < 200; ++i) {
        if (listener_unregistered_flag) {
            break;
        }
        qcc::Sleep(5);
    }
    EXPECT_TRUE(listener_unregistered_flag);

    status = alljoyn_busattachment_stop(bus);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
}

TEST_F(BusListenerTest, name_owner_changed) {
    status = alljoyn_busattachment_start(bus);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    status = alljoyn_busattachment_connect(bus, ajn::getConnectArg().c_str());

    alljoyn_busattachment_registerbuslistener(bus, buslistener);
    for (size_t i = 0; i < 200; ++i) {
        if (listener_registered_flag) {
            break;
        }
        qcc::Sleep(5);
    }
    EXPECT_TRUE(listener_registered_flag);

    alljoyn_busattachment_requestname(bus, OBJECT_NAME, 0);
    for (size_t i = 0; i < 200; ++i) {
        if (name_owner_changed_flag) {
            break;
        }
        qcc::Sleep(5);
    }
    EXPECT_TRUE(name_owner_changed_flag);

    alljoyn_busattachment_stop(bus);
    for (size_t i = 0; i < 200; ++i) {
        if (bus_stopping_flag) {
            break;
        }
        qcc::Sleep(5);
    }
    EXPECT_TRUE(bus_stopping_flag);
    alljoyn_busattachment_join(bus);
    /* the bus will automatically disconnect when it is stopped */
    for (size_t i = 0; i < 200; ++i) {
        if (bus_disconnected_flag) {
            break;
        }
        qcc::Sleep(5);
    }
    EXPECT_TRUE(bus_disconnected_flag);

    alljoyn_busattachment_unregisterbuslistener(bus, buslistener);
    for (size_t i = 0; i < 200; ++i) {
        if (listener_unregistered_flag) {
            break;
        }
        qcc::Sleep(5);
    }
    EXPECT_TRUE(listener_unregistered_flag);

    status = alljoyn_busattachment_stop(bus);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
}
