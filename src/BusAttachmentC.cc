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

#include "BusAttachmentC.h"
#include <alljoyn/BusAttachment.h>
#include <alljoyn/MessageReceiver.h>
#include <alljoyn_c/InterfaceDescription.h>
#include <qcc/Mutex.h>
#include <stdio.h>

using namespace std;
using namespace qcc;


typedef struct {
    alljoyn_messagereceiver_signalhandler_ptr handler;
    const char* sourcePath;
    alljoyn_busobject busObject;
    ajn::BusAttachmentC* bus;
}signalCallbackMapEntry;

/*
 * signalCallbackMap used to map a AllJoyn signal (Member*) to a 'C' style signal handler.
 * Since the same signal can be used by multiple signal handlers we require a multimap that
 * can handle have multiple entries with the same key.
 */
std::multimap<const ajn::InterfaceDescription::Member*, signalCallbackMapEntry> signalCallbackMap;
/* Lock to prevent two threads from changing the signalCallbackMap at the same time.*/
qcc::Mutex signalCallbackMapLock;

namespace ajn {

QStatus BusAttachmentC::RegisterSignalHandlerC(alljoyn_busobject receiver, alljoyn_messagereceiver_signalhandler_ptr signalHandler, const alljoyn_interfacedescription_member member, const char* srcPath)
{
    QStatus ret = ER_OK;
    const ajn::InterfaceDescription::Member* cpp_member = (const ajn::InterfaceDescription::Member*)(member.internal_member);
    signalCallbackMapEntry entry = { signalHandler, srcPath, receiver, this };
    /*
     * a local multimap connecting the signal to each possible 'C' signal handler is being maintained
     * we only need to Register a new SignalHandler if the signal is a new signal
     * (i.e. InterfaceDescription::Member*).
     *
     */
    if (signalCallbackMap.find(cpp_member) == signalCallbackMap.end()) {
        ret = RegisterSignalHandler((ajn::BusObject*)receiver,
                                    static_cast<ajn::MessageReceiver::SignalHandler>(&BusAttachmentC::SignalHandlerRemap),
                                    cpp_member,
                                    NULL);
    }
    if (ret == ER_OK) {
        signalCallbackMapLock.Lock(MUTEX_CONTEXT);
        signalCallbackMap.insert(pair<const ajn::InterfaceDescription::Member*, signalCallbackMapEntry>(
                                     cpp_member,
                                     entry));

        signalCallbackMapLock.Unlock(MUTEX_CONTEXT);
    }
    return ret;
}

QStatus BusAttachmentC::UnregisterSignalHandlerC(alljoyn_busobject receiver, alljoyn_messagereceiver_signalhandler_ptr signalHandler, const alljoyn_interfacedescription_member member, const char* srcPath)
{
    QStatus return_status = ER_FAIL;
    const ajn::InterfaceDescription::Member* cpp_member = (const ajn::InterfaceDescription::Member*)(member.internal_member);

    /*
     * a local multimap is connecting the signal to each possible 'C' signal handler.
     * The map is being maintained locally the SignalHandler only needs to be unregistered
     * if it is the last signalhandler_ptr using that signal
     * (i.e. InterfaceDescription::Member*).
     */
    /*look up the C callback via map and remove */
    std::multimap<const ajn::InterfaceDescription::Member*, signalCallbackMapEntry>::iterator it;
    pair<std::multimap<const ajn::InterfaceDescription::Member*, signalCallbackMapEntry>::iterator,
         std::multimap<const ajn::InterfaceDescription::Member*, signalCallbackMapEntry>::iterator> ret;

    signalCallbackMapLock.Lock(MUTEX_CONTEXT);
    ret = signalCallbackMap.equal_range(cpp_member);
    if (ret.first != ret.second) {
        for (it = ret.first; it != ret.second; ++it) {
            if (signalHandler == it->second.handler) {
                /*
                 * remove the signalHandler only if the busObject and srcPath match
                 * the busObject and srcPath that were used to register the original
                 * signal.  Always accept a srcPath of NULL as a correct source path.
                 */

                if (receiver == it->second.busObject) {
                    if (srcPath == NULL || strcmp(it->second.sourcePath, srcPath) == 0) {
                        signalCallbackMap.erase(it);
                        return_status = ER_OK;
                    }
                }
            }
        }
    }

    if (signalCallbackMap.find(cpp_member) == signalCallbackMap.end()) {
        return_status = UnregisterSignalHandler((ajn::BusObject*)receiver,
                                                static_cast<ajn::MessageReceiver::SignalHandler>(&BusAttachmentC::SignalHandlerRemap),
                                                cpp_member,
                                                srcPath);
    }
    signalCallbackMapLock.Unlock(MUTEX_CONTEXT);
    return return_status;
}

QStatus BusAttachmentC::UnregisterAllHandlersC(alljoyn_busobject receiver) {
    std::multimap<const ajn::InterfaceDescription::Member*, signalCallbackMapEntry>::iterator it;
    it = signalCallbackMap.begin();
    signalCallbackMapLock.Lock(MUTEX_CONTEXT);
    for (it = signalCallbackMap.begin(); it != signalCallbackMap.end(); ++it) {
        if (receiver == it->second.busObject) {
            signalCallbackMap.erase(it);
        }
    }

    signalCallbackMapLock.Unlock(MUTEX_CONTEXT);
    return UnregisterAllHandlers((ajn::BusObject*)receiver);
}

void BusAttachmentC::UnregisterAllHandlersC() {
    std::multimap<const ajn::InterfaceDescription::Member*, signalCallbackMapEntry>::iterator it;
    it = signalCallbackMap.begin();
    signalCallbackMapLock.Lock(MUTEX_CONTEXT);
    for (it = signalCallbackMap.begin(); it != signalCallbackMap.end(); ++it) {
        if (this == it->second.bus) {
            signalCallbackMap.erase(it);
        }
    }

    signalCallbackMapLock.Unlock(MUTEX_CONTEXT);
}

void BusAttachmentC::SignalHandlerRemap(const InterfaceDescription::Member* member, const char* srcPath, Message& message)
{
    alljoyn_interfacedescription_member c_member;

    c_member.iface = (alljoyn_interfacedescription)member->iface;
    c_member.memberType = (alljoyn_messagetype)member->memberType;
    c_member.name = member->name.c_str();
    c_member.signature = member->signature.c_str();
    c_member.returnSignature = member->returnSignature.c_str();
    c_member.argNames = member->argNames.c_str();
    c_member.annotation = member->annotation;
    c_member.internal_member = member;

    /*look up the C callback via map and invoke */
    std::multimap<const ajn::InterfaceDescription::Member*, signalCallbackMapEntry>::iterator it;
    pair<std::multimap<const ajn::InterfaceDescription::Member*, signalCallbackMapEntry>::iterator,
         std::multimap<const ajn::InterfaceDescription::Member*, signalCallbackMapEntry>::iterator> ret;

    signalCallbackMapLock.Lock(MUTEX_CONTEXT);
    ret = signalCallbackMap.equal_range(member);
    if (ret.first != ret.second) {
        for (it = ret.first; it != ret.second; ++it) {
            /*
             * only remap the recived signal if the sourcePath received matches the
             * the sourcePath specified when the signal was registered,
             * or the sourcePath is not specified (i.e. NULL)
             */
            if (it->second.sourcePath == NULL || strcmp(it->second.sourcePath, srcPath) == 0) {
                alljoyn_messagereceiver_signalhandler_ptr remappedHandler = it->second.handler;
                remappedHandler(&c_member, srcPath, (alljoyn_message) & message);
            }
        }
    }
    signalCallbackMapLock.Unlock(MUTEX_CONTEXT);
}
}
