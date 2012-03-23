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
#include <alljoyn_c/BusListener.h>
#include <alljoyn_c/BusObject.h>
#include <alljoyn_c/ProxyBusObject.h>
#include <alljoyn_c/InterfaceDescription.h>
#include <alljoyn_c/Session.h>
#include <alljoyn_c/SessionListener.h>
#include <alljoyn_c/SessionPortListener.h>
#include <Status.h>
#include <map>

namespace ajn {
/*
 * This class is a child of BusAttachment.  This class contains extra methods needed
 * to map 'C++' callback methods to 'C' callback functions used for signals and
 * other async method calls.
 */
class BusAttachmentC : public BusAttachment {
  public:
    BusAttachmentC(const char* applicationName, bool allowRemoteMessages = false) :
        BusAttachment(applicationName, allowRemoteMessages) { }

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
  private:
    /**
     * Convert the 'C++' SignalHandler callback to a 'C' callback function
     */
    void SignalHandlerRemap(const InterfaceDescription::Member* member, const char* srcPath, Message& message);

};
}
