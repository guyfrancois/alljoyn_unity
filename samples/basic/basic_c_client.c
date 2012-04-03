/**
 * @file
 * @brief  Sample implementation of an AllJoyn client in C.
 */

/******************************************************************************
 *
 *
 * Copyright 2009-2011, Qualcomm Innovation Center, Inc.
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

#include <qcc/platform.h>

#include <assert.h>
#include <signal.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>

#include <alljoyn/DBusStd.h>
#include <alljoyn_c/BusAttachment.h>
#include <alljoyn_c/BusObject.h>
#include <alljoyn_c/MsgArg.h>
#include <alljoyn_c/InterfaceDescription.h>
#include <alljoyn_c/version.h>

#include <Status.h>

/** Static top level message bus object */
static alljoyn_busattachment g_msgBus = NULL;

/*constants*/
static const char* INTERFACE_NAME = "org.alljoyn.Bus.method_sample";
static const char* OBJECT_NAME = "org.alljoyn.Bus.method_sample";
static const char* OBJECT_PATH = "/method_sample";
static const alljoyn_sessionport SERVICE_PORT = 25;

static QC_BOOL s_joinComplete = QC_FALSE;
static alljoyn_sessionid s_sessionId = 0;

/* Static BusListener */
static alljoyn_buslistener g_busListener;

static volatile sig_atomic_t g_interrupt = QC_FALSE;

static void SigIntHandler(int sig)
{
    g_interrupt = QC_TRUE;
}

/* FoundAdvertisedName callback */
void found_advertised_name(const void* context, const char* name, alljoyn_transportmask transport, const char* namePrefix)
{
    printf("FoundAdvertisedName(name=%s, prefix=%s)\n", name, namePrefix);
    if (0 == strcmp(name, OBJECT_NAME)) {
        /* We found a remote bus that is advertising basic service's  well-known name so connect to it */
        alljoyn_sessionopts opts = alljoyn_sessionopts_create(ALLJOYN_TRAFFIC_TYPE_MESSAGES, QC_FALSE, ALLJOYN_PROXIMITY_ANY, ALLJOYN_TRANSPORT_ANY);
        QStatus status = alljoyn_busattachment_joinsession(g_msgBus, name, SERVICE_PORT, NULL, &s_sessionId, opts);

        if (ER_OK != status) {
            printf("JoinSession failed (status=%s)\n", QCC_StatusText(status));
        } else {
            printf("JoinSession SUCCESS (Session id=%d)\n", s_sessionId);
        }
        alljoyn_sessionopts_destroy(opts);
    }
    s_joinComplete = QC_TRUE;
}

/* NameOwnerChanged callback */
void name_owner_changed(const void* context, const char* busName, const char* previousOwner, const char* newOwner)
{
    if (newOwner && (0 == strcmp(busName, OBJECT_NAME))) {
        printf("NameOwnerChanged: name=%s, oldOwner=%s, newOwner=%s\n",
               busName,
               previousOwner ? previousOwner : "<none>",
               newOwner ? newOwner : "<none>");
    }
}

/** Main entry point */
/** TODO: Make this C89 compatible. */
int main(int argc, char** argv, char** envArg)
{
    QStatus status = ER_OK;
    char connectArgs[][64] = { "tcp:addr=127.0.0.1,port=9955", "unix:abstract=alljoyn" };
    alljoyn_interfacedescription testIntf = NULL;
    size_t i;
    /* Create a bus listener */
    alljoyn_buslistener_callbacks callbacks = {
        NULL,
        NULL,
        &found_advertised_name,
        NULL,
        &name_owner_changed,
        NULL,
        NULL
    };

    printf("AllJoyn Library version: %s\n", alljoyn_getversion());
    printf("AllJoyn Library build info: %s\n", alljoyn_getbuildinfo());

    /* Install SIGINT handler */
    signal(SIGINT, SigIntHandler);

    /* Create message bus */
    g_msgBus = alljoyn_busattachment_create("myApp", QC_TRUE);

    /* Add org.alljoyn.Bus.method_sample interface */
    status = alljoyn_busattachment_createinterface(g_msgBus, INTERFACE_NAME, &testIntf, QC_FALSE);
    if (status == ER_OK) {
        printf("Interface Created.\n");
        alljoyn_interfacedescription_addmember(testIntf, ALLJOYN_MESSAGE_METHOD_CALL, "cat", "ss",  "s", "inStr1,inStr2,outStr", 0);
        alljoyn_interfacedescription_activate(testIntf);
    } else {
        printf("Failed to create interface 'org.alljoyn.Bus.method_sample'\n");
    }


    /* Start the msg bus */
    if (ER_OK == status) {
        status = alljoyn_busattachment_start(g_msgBus);
        if (ER_OK != status) {
            printf("BusAttachment::Start failed\n");
        } else {
            printf("BusAttachment started.\n");
        }
    }

    /* Connect to the bus */
    if (ER_OK == status) {
        for (i = 0; i < sizeof(connectArgs) / sizeof(connectArgs[0]); ++i) {
            status = alljoyn_busattachment_connect(g_msgBus, connectArgs[i]);
            if (ER_OK != status) {
                printf("BusAttachment::Connect(\"%s\") failed\n", connectArgs[i]);
            } else {
                printf("BusAttchement connected to %s\n", connectArgs[i]);
                break;
            }
        }
        if (ER_OK != status) {
            printf("Multiple BusAttachment::Connect attempts failed\n");
        }
    }

    g_busListener = alljoyn_buslistener_create(&callbacks, NULL);

    /* Register a bus listener in order to get discovery indications */
    if (ER_OK == status) {
        alljoyn_busattachment_registerbuslistener(g_msgBus, g_busListener);
        printf("BusListener Registered.\n");
    }

    /* Begin discovery on the well-known name of the service to be called */
    if (ER_OK == status) {
        status = alljoyn_busattachment_findadvertisedname(g_msgBus, OBJECT_NAME);
        if (status != ER_OK) {
            printf("org.alljoyn.Bus.FindAdvertisedName failed (%s))\n", QCC_StatusText(status));
        }
    }

    /* Wait for join session to complete */
    while (s_joinComplete == QC_FALSE && g_interrupt == QC_FALSE) {
#ifdef _WIN32
        Sleep(10);
#else
        usleep(100 * 1000);
#endif
    }

    if (status == ER_OK && g_interrupt == QC_FALSE) {
        alljoyn_message reply;
        alljoyn_msgarg inputs;
        size_t numArgs;

        alljoyn_proxybusobject remoteObj = alljoyn_proxybusobject_create(g_msgBus, OBJECT_NAME, OBJECT_PATH, s_sessionId);
        const alljoyn_interfacedescription alljoynTestIntf = alljoyn_busattachment_getinterface(g_msgBus, INTERFACE_NAME);
        assert(alljoynTestIntf);
        alljoyn_proxybusobject_addinterface(remoteObj, alljoynTestIntf);

        reply = alljoyn_message_create(g_msgBus);
        inputs = alljoyn_msgarg_array_create(2);
        numArgs = 2;
        status = alljoyn_msgarg_array_set(inputs, &numArgs, "ss", "Hello ", "World!");
        if (ER_OK != status) {
            printf("Arg assignment failed: %s\n", QCC_StatusText(status));
        }
        status = alljoyn_proxybusobject_methodcall(remoteObj, INTERFACE_NAME, "cat", inputs, 2, reply, 5000, 0);
        if (ER_OK == status) {
            printf("%s.%s ( path=%s) returned \"%s\"\n", INTERFACE_NAME, "cat",
                   OBJECT_PATH, alljoyn_msgarg_as_string(alljoyn_message_getarg(reply, 0), 0));
        } else {
            printf("MethodCall on %s.%s failed\n", INTERFACE_NAME, "cat");
        }

        alljoyn_proxybusobject_destroy(remoteObj);
        alljoyn_message_destroy(reply);
        alljoyn_msgarg_destroy(inputs);
    }

    /* Deallocate bus */
    if (g_msgBus) {
        alljoyn_busattachment deleteMe = g_msgBus;
        g_msgBus = NULL;
        alljoyn_busattachment_destroy(deleteMe);
    }

    /* Deallocate bus listener */
    alljoyn_buslistener_destroy(g_busListener);

    printf("basic client exiting with status %d (%s)\n", status, QCC_StatusText(status));

    return (int) status;
}
