/**
 * @file
 *
 * This file implements the BusAttachmentC class.
 */

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
#ifndef _ALLJOYN_C_BUSATTACHMENTC_H
#define _ALLJOYN_C_BUSATTACHMENTC_H

#include <qcc/platform.h>
#include <alljoyn/BusAttachment.h>
#include <alljoyn_c/AjAPI.h>
#include <alljoyn_c/KeyStoreListener.h>
#include <alljoyn_c/AuthListener.h>
#include <alljoyn_c/BusAttachment.h>
#include <alljoyn_c/BusListener.h>
#include <alljoyn_c/BusObject.h>
#include <alljoyn_c/ProxyBusObject.h>
#include <alljoyn_c/InterfaceDescription.h>
#include <alljoyn_c/Session.h>
#include <alljoyn_c/SessionListener.h>
#include <alljoyn_c/SessionPortListener.h>

#include <stdio.h>
#include <Status.h>
#include <map>

namespace ajn {

class JoinsessionCallbackContext {
  public:
    JoinsessionCallbackContext(alljoyn_busattachment_joinsessioncb_ptr joinsessioncb_ptr, void* context) :
        joinsessioncb_ptr(joinsessioncb_ptr), context(context)
    { }

    alljoyn_busattachment_joinsessioncb_ptr joinsessioncb_ptr;
    void* context;
};

/*
 * This class is a child of BusAttachment.  This class contains extra methods needed
 * to map 'C++' callback methods to 'C' callback functions used for signals and
 * other async method calls.
 */
class BusAttachmentC : public BusAttachment, public BusAttachment::JoinSessionAsyncCB {
  public:
    BusAttachmentC(const char* applicationName, bool allowRemoteMessages = false) :
        BusAttachment(applicationName, allowRemoteMessages) { }

    /** Destructor */
    virtual ~BusAttachmentC() {
        /* remove all signal handers associated with this BusAttachment */
        UnregisterAllHandlersC();
    }
    /**
     * Take a 'C' style SignalHandler and map it to a 'C++' style SignalHandler
     * Register the 'C++' SignalHandler with the 'C++' code.
     *
     * @param receiver       The object receiving the signal.
     * @param signalHandler  The signal handler method.
     * @param member         The interface/member of the signal.
     * @param srcPath        The object path of the emitter of the signal or NULL for all paths.
     * @return #ER_OK
     */
    QStatus RegisterSignalHandlerC(alljoyn_busobject receiver, alljoyn_messagereceiver_signalhandler_ptr signalHandler, const alljoyn_interfacedescription_member member, const char* srcPath);

    /**
     * remove the 'C' style SignalHandler from the map and Unregister the 'C++' SignalHandler.
     *
     * @param receiver       The object receiving the signal.
     * @param signalHandler  The signal handler method.
     * @param member         The interface/member of the signal.
     * @param srcPath        The object path of the emitter of the signal or NULL for all paths.
     * @return #ER_OK
     */
    QStatus UnregisterSignalHandlerC(alljoyn_busobject receiver, alljoyn_messagereceiver_signalhandler_ptr signalHandler, const alljoyn_interfacedescription_member member, const char* srcPath);

    /**
     * remove all SignalHandlers associated with the receiver alljoyn_busobject
     *
     * @param receiver the object the signals will no longer be registered with.
     */
    QStatus UnregisterAllHandlersC(alljoyn_busobject receiver);

    void JoinSessionCB(QStatus status, SessionId sessionId, const SessionOpts& opts, void* context) {
        JoinsessionCallbackContext* in = (JoinsessionCallbackContext*)context;
        (in->joinsessioncb_ptr)(status, (alljoyn_sessionid)sessionId, (const alljoyn_sessionopts)&opts, in->context);
    }

  private:
    /**
     * remove all SignalHandlers associated with this BusAttachment
     *
     * This method is called by the C function alljoyn_busattachment_destroy
     * this will ensure that all signal handlers mapped to the signalHandlerMap
     * that are associated with this BusAttachment are removed before the
     * BusAttachment is destroyed.
     */
    void UnregisterAllHandlersC();

    /**
     * Convert the 'C++' SignalHandler callback to a 'C' callback function
     */
    void SignalHandlerRemap(const InterfaceDescription::Member* member, const char* srcPath, Message& message);
};
}
#endif
