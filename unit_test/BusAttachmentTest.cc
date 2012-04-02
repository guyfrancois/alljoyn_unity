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
#include <qcc/Thread.h>
#include "ajTestCommon.h"

TEST(BusAttachmentTest, createinterface) {
    QStatus status = ER_OK;
    alljoyn_busattachment bus = NULL;
    bus = alljoyn_busattachment_create("BusAttachmentTest", QC_FALSE);
    ASSERT_TRUE(bus != NULL);
    alljoyn_interfacedescription testIntf = NULL;
    status = alljoyn_busattachment_createinterface(bus, "org.alljoyn.test.BusAttachment", &testIntf, QC_FALSE);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    alljoyn_busattachment_destroy(bus);
}

TEST(BusAttachmentTest, deleteinterface) {
    QStatus status = ER_OK;
    alljoyn_busattachment bus = NULL;
    bus = alljoyn_busattachment_create("BusAttachmentTest", QC_FALSE);
    ASSERT_TRUE(bus != NULL);
    alljoyn_interfacedescription testIntf = NULL;
    status = alljoyn_busattachment_createinterface(bus, "org.alljoyn.test.BusAttachment", &testIntf, QC_FALSE);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    status = alljoyn_busattachment_deleteinterface(bus, testIntf);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    alljoyn_busattachment_destroy(bus);
}

TEST(BusAttachmentTest, start_stop_join) {
    QStatus status = ER_FAIL;
    alljoyn_busattachment bus = NULL;
    bus = alljoyn_busattachment_create("BusAttachmentTest", QC_FALSE);
    status = alljoyn_busattachment_start(bus);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    status = alljoyn_busattachment_stop(bus);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    status = alljoyn_busattachment_join(bus);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    alljoyn_busattachment_destroy(bus);
}

TEST(BusAttachmentTest, isstarted_isstopping) {
    QStatus status = ER_FAIL;
    alljoyn_busattachment bus = NULL;
    bus = alljoyn_busattachment_create("BusAttachmentTest", QC_FALSE);
    EXPECT_EQ(QC_FALSE, alljoyn_busattachment_isstarted(bus));
    status = alljoyn_busattachment_start(bus);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    EXPECT_EQ(QC_TRUE, alljoyn_busattachment_isstarted(bus));
    status = alljoyn_busattachment_stop(bus);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    /*
     * Assumption made that the isstopping function will be called before all of
     * the BusAttachement threads have completed so it will return QC_TRUE it is
     * possible, but unlikely, that this could return QC_FALSE.
     */

    EXPECT_EQ(QC_TRUE, alljoyn_busattachment_isstopping(bus));
    status = alljoyn_busattachment_join(bus);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    EXPECT_EQ(QC_FALSE, alljoyn_busattachment_isstarted(bus));
    alljoyn_busattachment_destroy(bus);
}

/*
 * values used for the registersignalhandler test
 */
bool registersignalhandler_flag;
bool registersignalhandler_flag2;
char sourcePath1[256];
char sourcePath2[256];

void registersignalHandler_Handler(const alljoyn_interfacedescription_member* member,
                                   const char* srcPath,
                                   alljoyn_message message) {
    EXPECT_STREQ(sourcePath1, srcPath);
    EXPECT_STREQ("AllJoyn", alljoyn_msgarg_as_string(alljoyn_message_getarg(message, 0), 0));
    registersignalhandler_flag = true;
}

void registersignalHandler_Handler2(const alljoyn_interfacedescription_member* member,
                                    const char* srcPath,
                                    alljoyn_message message) {
    EXPECT_STREQ(sourcePath2, srcPath);
    EXPECT_STREQ("AllJoyn", alljoyn_msgarg_as_string(alljoyn_message_getarg(message, 0), 0));
    registersignalhandler_flag2 = true;
}

TEST(BusAttachmentTest, registersignalhandler_basic) {
    QStatus status = ER_OK;
    registersignalhandler_flag = false;
    registersignalhandler_flag2 = false;

    snprintf(sourcePath1, 256, "/org/alljoyn/test/signal");

    alljoyn_busattachment bus = NULL;
    bus = alljoyn_busattachment_create("BusAttachmentTest", QC_TRUE);

    status = alljoyn_busattachment_start(bus);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    status = alljoyn_busattachment_connect(bus, ajn::getConnectArg().c_str());
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);


    alljoyn_interfacedescription testIntf = NULL;

    status = alljoyn_busattachment_createinterface(bus, "org.alljoyn.test.BusAttachment", &testIntf, QC_FALSE);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    if (status == ER_OK) {
        //alljoyn_interfacedescription_addmember(testIntf, ALLJOYN_MESSAGE_SIGNAL, "my_signal", "a{ys}", NULL, NULL, 0);
        status = alljoyn_interfacedescription_addmember(testIntf, ALLJOYN_MESSAGE_SIGNAL, "testSignal", "s", NULL, "newName", 0);
        EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
        alljoyn_interfacedescription_activate(testIntf);
    }
    /* Set up bus object */
    alljoyn_busobject_callbacks busObjCbs = {
        NULL,
        NULL,
        NULL,
        NULL
    };

    alljoyn_busobject testObj = alljoyn_busobject_create(bus, "/org/alljoyn/test/signal", QC_FALSE, &busObjCbs, NULL);

    status = alljoyn_busobject_addinterface(testObj, testIntf);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);

    alljoyn_interfacedescription_member my_signal_member;
    QC_BOOL foundMember = alljoyn_interfacedescription_getmember(testIntf, "testSignal", &my_signal_member);
    EXPECT_EQ(QC_TRUE, foundMember);


    status = alljoyn_busattachment_addmatch(bus, "type='signal',interface='org.alljoyn.test.BusAttachment',member='testSignal'");
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);

    status = alljoyn_busattachment_registersignalhandler(bus, testObj, &registersignalHandler_Handler, my_signal_member, NULL);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);

    alljoyn_msgarg arg = alljoyn_msgarg_array_create(1);
    size_t numArgs = 1;
    status = alljoyn_msgarg_array_set(arg, &numArgs, "s", "AllJoyn");
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    status = alljoyn_busobject_signal(testObj, NULL, 0, my_signal_member, arg, 1, 0, 0);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);

    //Wait upto 2 seconds for the signal to complete.
    for (int i = 0; i < 200; ++i) {

        if (registersignalhandler_flag) {
            break;
        }
        qcc::Sleep(10);
    }
    EXPECT_TRUE(registersignalhandler_flag);

    status = alljoyn_busattachment_stop(bus);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    status = alljoyn_busattachment_join(bus);
    alljoyn_busattachment_destroy(bus);
}

TEST(BusAttachmentTest, registersignalhandler_multiple_signals) {
    QStatus status = ER_OK;
    registersignalhandler_flag = false;

    snprintf(sourcePath1, 256, "/org/alljoyn/test/signal");
    snprintf(sourcePath2, 256, "/org/alljoyn/test/signal");

    alljoyn_busattachment bus = NULL;
    bus = alljoyn_busattachment_create("BusAttachmentTest", QC_TRUE);

    status = alljoyn_busattachment_start(bus);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    status = alljoyn_busattachment_connect(bus, ajn::getConnectArg().c_str());
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);


    alljoyn_interfacedescription testIntf = NULL;

    status = alljoyn_busattachment_createinterface(bus, "org.alljoyn.test.BusAttachment", &testIntf, QC_FALSE);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    if (status == ER_OK) {
        //alljoyn_interfacedescription_addmember(testIntf, ALLJOYN_MESSAGE_SIGNAL, "my_signal", "a{ys}", NULL, NULL, 0);
        status = alljoyn_interfacedescription_addmember(testIntf, ALLJOYN_MESSAGE_SIGNAL, "testSignal", "s", NULL, "newName", 0);
        EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
        alljoyn_interfacedescription_activate(testIntf);
    }
    /* Set up bus object */
    alljoyn_busobject_callbacks busObjCbs = {
        NULL,
        NULL,
        NULL,
        NULL
    };

    alljoyn_busobject testObj = alljoyn_busobject_create(bus, sourcePath1, QC_FALSE, &busObjCbs, NULL);

    status = alljoyn_busobject_addinterface(testObj, testIntf);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);

    alljoyn_interfacedescription_member my_signal_member;
    QC_BOOL foundMember = alljoyn_interfacedescription_getmember(testIntf, "testSignal", &my_signal_member);
    EXPECT_EQ(QC_TRUE, foundMember);


    status = alljoyn_busattachment_addmatch(bus, "type='signal',interface='org.alljoyn.test.BusAttachment',member='testSignal'");
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);

    status = alljoyn_busattachment_registersignalhandler(bus, testObj, &registersignalHandler_Handler, my_signal_member, NULL);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    status = alljoyn_busattachment_registersignalhandler(bus, testObj, &registersignalHandler_Handler2, my_signal_member, NULL);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);

    alljoyn_msgarg arg = alljoyn_msgarg_array_create(1);
    size_t numArgs = 1;
    status = alljoyn_msgarg_array_set(arg, &numArgs, "s", "AllJoyn");
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    status = alljoyn_busobject_signal(testObj, NULL, 0, my_signal_member, arg, 1, 0, 0);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);

    //Wait upto 2 seconds for the signal to complete.
    for (int i = 0; i < 200; ++i) {

        if (registersignalhandler_flag && registersignalhandler_flag2) {
            break;
        }
        qcc::Sleep(10);
    }
    EXPECT_TRUE(registersignalhandler_flag);
    EXPECT_TRUE(registersignalhandler_flag2);

    status = alljoyn_busattachment_stop(bus);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    status = alljoyn_busattachment_join(bus);
    alljoyn_busattachment_destroy(bus);
}

TEST(BusAttachmentTest, unregistersignalhandler) {
    QStatus status = ER_OK;
    registersignalhandler_flag = false;
    registersignalhandler_flag2 = false;

    snprintf(sourcePath1, 256, "/org/alljoyn/test/signal");
    snprintf(sourcePath2, 256, "/org/alljoyn/test/signal");

    alljoyn_busattachment bus = NULL;
    bus = alljoyn_busattachment_create("BusAttachmentTest", QC_TRUE);

    status = alljoyn_busattachment_start(bus);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    status = alljoyn_busattachment_connect(bus, ajn::getConnectArg().c_str());
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);


    alljoyn_interfacedescription testIntf = NULL;

    status = alljoyn_busattachment_createinterface(bus, "org.alljoyn.test.BusAttachment", &testIntf, QC_FALSE);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    if (status == ER_OK) {
        //alljoyn_interfacedescription_addmember(testIntf, ALLJOYN_MESSAGE_SIGNAL, "my_signal", "a{ys}", NULL, NULL, 0);
        status = alljoyn_interfacedescription_addmember(testIntf, ALLJOYN_MESSAGE_SIGNAL, "testSignal", "s", NULL, "newName", 0);
        EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
        alljoyn_interfacedescription_activate(testIntf);
    }
    /* Set up bus object */
    alljoyn_busobject_callbacks busObjCbs = {
        NULL,
        NULL,
        NULL,
        NULL
    };

    alljoyn_busobject testObj = alljoyn_busobject_create(bus, sourcePath1, QC_FALSE, &busObjCbs, NULL);

    status = alljoyn_busobject_addinterface(testObj, testIntf);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);

    alljoyn_interfacedescription_member my_signal_member;
    QC_BOOL foundMember = alljoyn_interfacedescription_getmember(testIntf, "testSignal", &my_signal_member);
    ASSERT_EQ(QC_TRUE, foundMember);


    status = alljoyn_busattachment_addmatch(bus, "type='signal',interface='org.alljoyn.test.BusAttachment',member='testSignal'");
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);

    status = alljoyn_busattachment_registersignalhandler(bus, testObj, &registersignalHandler_Handler, my_signal_member, NULL);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    status = alljoyn_busattachment_registersignalhandler(bus, testObj, &registersignalHandler_Handler2, my_signal_member, NULL);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);

    alljoyn_msgarg arg = alljoyn_msgarg_array_create(1);
    size_t numArgs = 1;
    status = alljoyn_msgarg_array_set(arg, &numArgs, "s", "AllJoyn");
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    status = alljoyn_busobject_signal(testObj, NULL, 0, my_signal_member, arg, 1, 0, 0);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);

    //Wait upto 2 seconds for the signal to complete.
    for (int i = 0; i < 200; ++i) {

        if (registersignalhandler_flag && registersignalhandler_flag2) {
            break;
        }
        qcc::Sleep(10);
    }
    EXPECT_TRUE(registersignalhandler_flag);
    EXPECT_TRUE(registersignalhandler_flag2);

    registersignalhandler_flag = false;
    registersignalhandler_flag2 = false;

    status = alljoyn_busattachment_unregistersignalhandler(bus, testObj, &registersignalHandler_Handler2, my_signal_member, NULL);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);

    status = alljoyn_busobject_signal(testObj, NULL, 0, my_signal_member, arg, 1, 0, 0);
    //Wait upto 2 seconds for the signal to complete.
    for (int i = 0; i < 200; ++i) {

        if (registersignalhandler_flag) {
            break;
        }
        qcc::Sleep(10);
    }

    //wait a little longer to make sure the signal still did not come through
    qcc::Sleep(50);
    EXPECT_TRUE(registersignalhandler_flag);
    EXPECT_FALSE(registersignalhandler_flag2);

    status = alljoyn_busattachment_stop(bus);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    status = alljoyn_busattachment_join(bus);
    alljoyn_busattachment_destroy(bus);
}

TEST(BusAttachmentTest, register_unregister_signalhandler_with_sourcePath) {
    QStatus status = ER_OK;
    registersignalhandler_flag = false;
    registersignalhandler_flag2 = false;

    snprintf(sourcePath1, 256, "/org/alljoyn/test/signal/A");
    snprintf(sourcePath2, 256, "/org/alljoyn/test/signal/B");

    alljoyn_busattachment bus = NULL;
    bus = alljoyn_busattachment_create("BusAttachmentTest", QC_TRUE);

    status = alljoyn_busattachment_start(bus);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    status = alljoyn_busattachment_connect(bus, ajn::getConnectArg().c_str());
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);


    alljoyn_interfacedescription testIntf = NULL;

    status = alljoyn_busattachment_createinterface(bus, "org.alljoyn.test.BusAttachment", &testIntf, QC_FALSE);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    if (status == ER_OK) {
        //alljoyn_interfacedescription_addmember(testIntf, ALLJOYN_MESSAGE_SIGNAL, "my_signal", "a{ys}", NULL, NULL, 0);
        status = alljoyn_interfacedescription_addmember(testIntf, ALLJOYN_MESSAGE_SIGNAL, "testSignal", "s", NULL, "newName", 0);
        EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
        alljoyn_interfacedescription_activate(testIntf);
    }
    /* Set up bus object */
    alljoyn_busobject_callbacks busObjCbs = {
        NULL,
        NULL,
        NULL,
        NULL
    };

    alljoyn_busobject testObjA = alljoyn_busobject_create(bus, sourcePath1, QC_FALSE, &busObjCbs, NULL);
    alljoyn_busobject testObjB = alljoyn_busobject_create(bus, sourcePath2, QC_FALSE, &busObjCbs, NULL);

    status = alljoyn_busobject_addinterface(testObjA, testIntf);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);

    status = alljoyn_busobject_addinterface(testObjB, testIntf);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);

    alljoyn_interfacedescription_member my_signal_member;
    QC_BOOL foundMember = alljoyn_interfacedescription_getmember(testIntf, "testSignal", &my_signal_member);
    ASSERT_EQ(QC_TRUE, foundMember);


    status = alljoyn_busattachment_addmatch(bus, "type='signal',interface='org.alljoyn.test.BusAttachment',member='testSignal'");
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);

    //register signal handler with corresponding sourcePath
    status = alljoyn_busattachment_registersignalhandler(bus, testObjA, &registersignalHandler_Handler, my_signal_member, sourcePath1);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    status = alljoyn_busattachment_registersignalhandler(bus, testObjB, &registersignalHandler_Handler2, my_signal_member, sourcePath2);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);

    alljoyn_msgarg arg = alljoyn_msgarg_array_create(1);
    size_t numArgs = 1;
    status = alljoyn_msgarg_array_set(arg, &numArgs, "s", "AllJoyn");
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);

    //send Two signals one for each path
    status = alljoyn_busobject_signal(testObjA, NULL, 0, my_signal_member, arg, 1, 0, 0);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);

    status = alljoyn_busobject_signal(testObjB, NULL, 0, my_signal_member, arg, 1, 0, 0);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);

    //Wait upto 2 seconds for the signal to complete.
    for (int i = 0; i < 200; ++i) {

        if (registersignalhandler_flag && registersignalhandler_flag2) {
            break;
        }
        qcc::Sleep(10);
    }
    EXPECT_TRUE(registersignalhandler_flag);
    EXPECT_TRUE(registersignalhandler_flag2);

    //Test sending only the signal with the first sourcePath
    registersignalhandler_flag = false;
    registersignalhandler_flag2 = false;

    //send only one signal for sourcePath1
    status = alljoyn_busobject_signal(testObjA, NULL, 0, my_signal_member, arg, 1, 0, 0);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);

    //Wait upto 2 seconds for the signal to complete.
    for (int i = 0; i < 200; ++i) {

        if (registersignalhandler_flag) {
            break;
        }
        qcc::Sleep(10);
    }

    //wait a little longer to make sure the signal still did not come through
    qcc::Sleep(50);
    EXPECT_TRUE(registersignalhandler_flag);
    EXPECT_FALSE(registersignalhandler_flag2);

    //test sending only the signal with the second sourcePath
    registersignalhandler_flag = false;
    registersignalhandler_flag2 = false;

    //send only one signal for sourcePath2
    status = alljoyn_busobject_signal(testObjB, NULL, 0, my_signal_member, arg, 1, 0, 0);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);

    //Wait upto 2 seconds for the signal to complete.
    for (int i = 0; i < 200; ++i) {

        if (registersignalhandler_flag2) {
            break;
        }
        qcc::Sleep(10);
    }

    //wait a little longer to make sure the signal still did not come through
    qcc::Sleep(50);
    EXPECT_FALSE(registersignalhandler_flag);
    EXPECT_TRUE(registersignalhandler_flag2);

    //unregister signal using sourcePath
    registersignalhandler_flag = false;
    registersignalhandler_flag2 = false;

    //unregistersignalhandler using wronge sourcePath
    status = alljoyn_busattachment_unregistersignalhandler(bus, testObjB, &registersignalHandler_Handler2, my_signal_member, sourcePath1);
    EXPECT_EQ(ER_FAIL, status) << "  Actual Status: " << QCC_StatusText(status);

    //unregistersignalhandler using right sourcePath
    status = alljoyn_busattachment_unregistersignalhandler(bus, testObjB, &registersignalHandler_Handler2, my_signal_member, sourcePath2);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);

    status = alljoyn_busobject_signal(testObjA, NULL, 0, my_signal_member, arg, 1, 0, 0);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);

    status = alljoyn_busobject_signal(testObjB, NULL, 0, my_signal_member, arg, 1, 0, 0);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);

    //Wait upto 2 seconds for the signal to complete.
    for (int i = 0; i < 200; ++i) {

        if (registersignalhandler_flag) {
            break;
        }
        qcc::Sleep(10);
    }

    //wait a little longer to make sure the signal still did not come through
    qcc::Sleep(50);
    EXPECT_TRUE(registersignalhandler_flag);
    EXPECT_FALSE(registersignalhandler_flag2);

    //unregistersignalhandler that has already been unregistered using sourcePath
    status = alljoyn_busattachment_unregistersignalhandler(bus, testObjB, &registersignalHandler_Handler2, my_signal_member, sourcePath2);
    EXPECT_EQ(ER_FAIL, status) << "  Actual Status: " << QCC_StatusText(status);

    status = alljoyn_busattachment_stop(bus);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    status = alljoyn_busattachment_join(bus);
    alljoyn_busattachment_destroy(bus);
}

TEST(BusAttachmentTest, unregisterallhandlers) {
    QStatus status = ER_OK;
    registersignalhandler_flag = false;
    registersignalhandler_flag2 = false;

    snprintf(sourcePath1, 256, "/org/alljoyn/test/signal");
    snprintf(sourcePath2, 256, "/org/alljoyn/test/signal");

    alljoyn_busattachment bus = NULL;
    bus = alljoyn_busattachment_create("BusAttachmentTest", QC_TRUE);

    status = alljoyn_busattachment_start(bus);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    status = alljoyn_busattachment_connect(bus, ajn::getConnectArg().c_str());
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);


    alljoyn_interfacedescription testIntf = NULL;

    status = alljoyn_busattachment_createinterface(bus, "org.alljoyn.test.BusAttachment", &testIntf, QC_FALSE);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    if (status == ER_OK) {
        //alljoyn_interfacedescription_addmember(testIntf, ALLJOYN_MESSAGE_SIGNAL, "my_signal", "a{ys}", NULL, NULL, 0);
        status = alljoyn_interfacedescription_addmember(testIntf, ALLJOYN_MESSAGE_SIGNAL, "testSignal", "s", NULL, "newName", 0);
        EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
        alljoyn_interfacedescription_activate(testIntf);
    }
    /* Set up bus object */
    alljoyn_busobject_callbacks busObjCbs = {
        NULL,
        NULL,
        NULL,
        NULL
    };

    alljoyn_busobject testObj = alljoyn_busobject_create(bus, sourcePath1, QC_FALSE, &busObjCbs, NULL);

    status = alljoyn_busobject_addinterface(testObj, testIntf);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);

    alljoyn_interfacedescription_member my_signal_member;
    QC_BOOL foundMember = alljoyn_interfacedescription_getmember(testIntf, "testSignal", &my_signal_member);
    ASSERT_EQ(QC_TRUE, foundMember);


    status = alljoyn_busattachment_addmatch(bus, "type='signal',interface='org.alljoyn.test.BusAttachment',member='testSignal'");
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);

    status = alljoyn_busattachment_registersignalhandler(bus, testObj, &registersignalHandler_Handler, my_signal_member, NULL);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    status = alljoyn_busattachment_registersignalhandler(bus, testObj, &registersignalHandler_Handler2, my_signal_member, NULL);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);

    alljoyn_msgarg arg = alljoyn_msgarg_array_create(1);
    size_t numArgs = 1;
    status = alljoyn_msgarg_array_set(arg, &numArgs, "s", "AllJoyn");
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    status = alljoyn_busobject_signal(testObj, NULL, 0, my_signal_member, arg, 1, 0, 0);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);

    //Wait upto 2 seconds for the signal to complete.
    for (int i = 0; i < 200; ++i) {

        if (registersignalhandler_flag && registersignalhandler_flag2) {
            break;
        }
        qcc::Sleep(10);
    }
    EXPECT_TRUE(registersignalhandler_flag);
    EXPECT_TRUE(registersignalhandler_flag2);

    registersignalhandler_flag = false;
    registersignalhandler_flag2 = false;

    status = alljoyn_busattachment_unregisterallhandlers(bus, testObj);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);

    status = alljoyn_busobject_signal(testObj, NULL, 0, my_signal_member, arg, 1, 0, 0);

    //wait a little while to make sure the signal still did not come through
    qcc::Sleep(100);
    EXPECT_FALSE(registersignalhandler_flag);
    EXPECT_FALSE(registersignalhandler_flag2);

    status = alljoyn_busattachment_stop(bus);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    status = alljoyn_busattachment_join(bus);
    alljoyn_busattachment_destroy(bus);
}


bool registersignalhandler_flag3;
bool registersignalhandler_flag4;
char sourcePath3[256];
char sourcePath4[256];

void registersignalHandler_Handler3(const alljoyn_interfacedescription_member* member,
                                    const char* srcPath,
                                    alljoyn_message message) {
    EXPECT_STREQ(sourcePath3, srcPath);
    EXPECT_STREQ("AllJoyn", alljoyn_msgarg_as_string(alljoyn_message_getarg(message, 0), 0));
    registersignalhandler_flag3 = true;
}

void registersignalHandler_Handler4(const alljoyn_interfacedescription_member* member,
                                    const char* srcPath,
                                    alljoyn_message message) {
    EXPECT_STREQ(sourcePath4, srcPath);
    EXPECT_STREQ("AllJoyn", alljoyn_msgarg_as_string(alljoyn_message_getarg(message, 0), 0));
    registersignalhandler_flag4 = true;
}
/*
 * this test is designed to verify that on unregistering a signal for one busobject
 * will not unregister the signal for other objects that have registered for
 * that same signal.
 */
TEST(BusAttachmentTest, unregistersignalhandler_muliple_busobjects) {
    QStatus status = ER_OK;
    registersignalhandler_flag = false;
    registersignalhandler_flag2 = false;
    registersignalhandler_flag3 = false;
    registersignalhandler_flag4 = false;

    snprintf(sourcePath1, 256, "/org/alljoyn/test/signal");
    snprintf(sourcePath2, 256, "/org/alljoyn/test/signal");
    snprintf(sourcePath3, 256, "/org/alljoyn/test/signal");
    snprintf(sourcePath4, 256, "/org/alljoyn/test/signal");

    alljoyn_busattachment bus = NULL;
    bus = alljoyn_busattachment_create("BusAttachmentTest", QC_TRUE);

    status = alljoyn_busattachment_start(bus);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    status = alljoyn_busattachment_connect(bus, ajn::getConnectArg().c_str());
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);


    alljoyn_interfacedescription testIntf = NULL;

    status = alljoyn_busattachment_createinterface(bus, "org.alljoyn.test.BusAttachment", &testIntf, QC_FALSE);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    if (status == ER_OK) {
        //alljoyn_interfacedescription_addmember(testIntf, ALLJOYN_MESSAGE_SIGNAL, "my_signal", "a{ys}", NULL, NULL, 0);
        status = alljoyn_interfacedescription_addmember(testIntf, ALLJOYN_MESSAGE_SIGNAL, "testSignal", "s", NULL, "newName", 0);
        EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
        alljoyn_interfacedescription_activate(testIntf);
    }
    /* Set up bus object */
    alljoyn_busobject_callbacks busObjCbs = {
        NULL,
        NULL,
        NULL,
        NULL
    };

    alljoyn_busobject testObj = alljoyn_busobject_create(bus, sourcePath1, QC_FALSE, &busObjCbs, NULL);
    alljoyn_busobject testObj2 = alljoyn_busobject_create(bus, sourcePath1, QC_FALSE, &busObjCbs, NULL);

    status = alljoyn_busobject_addinterface(testObj, testIntf);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);

    status = alljoyn_busobject_addinterface(testObj2, testIntf);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);

    alljoyn_interfacedescription_member my_signal_member;
    QC_BOOL foundMember = alljoyn_interfacedescription_getmember(testIntf, "testSignal", &my_signal_member);
    ASSERT_EQ(QC_TRUE, foundMember);

    alljoyn_interfacedescription_member my_signal_member2;
    foundMember = alljoyn_interfacedescription_getmember(testIntf, "testSignal", &my_signal_member2);
    ASSERT_EQ(QC_TRUE, foundMember);

    status = alljoyn_busattachment_addmatch(bus, "type='signal',interface='org.alljoyn.test.BusAttachment',member='testSignal'");
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);

    status = alljoyn_busattachment_registersignalhandler(bus, testObj, &registersignalHandler_Handler, my_signal_member, NULL);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    status = alljoyn_busattachment_registersignalhandler(bus, testObj, &registersignalHandler_Handler2, my_signal_member, NULL);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);

    status = alljoyn_busattachment_registersignalhandler(bus, testObj2, &registersignalHandler_Handler3, my_signal_member2, NULL);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    status = alljoyn_busattachment_registersignalhandler(bus, testObj2, &registersignalHandler_Handler4, my_signal_member2, NULL);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);

    alljoyn_msgarg arg = alljoyn_msgarg_array_create(1);
    size_t numArgs = 1;
    status = alljoyn_msgarg_array_set(arg, &numArgs, "s", "AllJoyn");
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    status = alljoyn_busobject_signal(testObj, NULL, 0, my_signal_member, arg, 1, 0, 0);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);

    //Wait upto 2 seconds for the signal to complete.
    for (int i = 0; i < 200; ++i) {

        if (registersignalhandler_flag && registersignalhandler_flag2
            && registersignalhandler_flag3 && registersignalhandler_flag4) {
            break;
        }
        qcc::Sleep(10);
    }
    EXPECT_TRUE(registersignalhandler_flag);
    EXPECT_TRUE(registersignalhandler_flag2);
    EXPECT_TRUE(registersignalhandler_flag3);
    EXPECT_TRUE(registersignalhandler_flag4);

    registersignalhandler_flag = false;
    registersignalhandler_flag2 = false;
    registersignalhandler_flag3 = false;
    registersignalhandler_flag4 = false;

    status = alljoyn_busattachment_unregistersignalhandler(bus, testObj, &registersignalHandler_Handler2, my_signal_member, NULL);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);

    status = alljoyn_busobject_signal(testObj, NULL, 0, my_signal_member, arg, 1, 0, 0);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    //Wait upto 2 seconds for the signal to complete.
    for (int i = 0; i < 200; ++i) {

        if (registersignalhandler_flag
            && registersignalhandler_flag3 && registersignalhandler_flag4) {
            break;
        }
        qcc::Sleep(10);
    }

    //wait a little longer to make sure the signal still did not come through
    qcc::Sleep(50);
    EXPECT_TRUE(registersignalhandler_flag);
    EXPECT_FALSE(registersignalhandler_flag2);
    EXPECT_TRUE(registersignalhandler_flag3);
    EXPECT_TRUE(registersignalhandler_flag4);

    registersignalhandler_flag = false;
    registersignalhandler_flag2 = false;
    registersignalhandler_flag3 = false;
    registersignalhandler_flag4 = false;

    status = alljoyn_busattachment_unregisterallhandlers(bus, testObj2);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);


    status = alljoyn_busobject_signal(testObj, NULL, 0, my_signal_member, arg, 1, 0, 0);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    //Wait upto 2 seconds for the signal to complete.
    for (int i = 0; i < 200; ++i) {

        if (registersignalhandler_flag) {
            break;
        }
        qcc::Sleep(10);
    }

    //wait a little longer to make sure the signal still did not come through
    qcc::Sleep(50);
    EXPECT_TRUE(registersignalhandler_flag);
    EXPECT_FALSE(registersignalhandler_flag2);
    EXPECT_FALSE(registersignalhandler_flag3);
    EXPECT_FALSE(registersignalhandler_flag4);

    status = alljoyn_busattachment_stop(bus);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    status = alljoyn_busattachment_join(bus);
    EXPECT_EQ(ER_OK, status) << "  Actual Status: " << QCC_StatusText(status);
    alljoyn_busattachment_destroy(bus);
}
