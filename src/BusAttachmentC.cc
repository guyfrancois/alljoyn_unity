#include "BusAttachmentC.h"
#include <alljoyn/BusAttachment.h>
#include <alljoyn/MessageReceiver.h>
#include <alljoyn_c/InterfaceDescription.h>
#include <qcc/Mutex.h>
#include <stdio.h>

using namespace std;
using namespace qcc;

/*
 * signalCallbackMap used to map a AllJoyn signal (Member*) to a 'C' style signal handler.
 * Since the same signal can be used by multiple signal handlers we require a multimap that
 * can handle have multiple entries with the same key.
 */
std::multimap<const ajn::InterfaceDescription::Member*, alljoyn_messagereceiver_signalhandler_ptr> signalCallbackMap;
/* Lock to prevent two threads from changing the signalCallbackMap at the same time.*/
qcc::Mutex signalCallbackMapLock;

namespace ajn {

QStatus BusAttachmentC::RegisterSignalHandlerC(alljoyn_busobject receiver, alljoyn_messagereceiver_signalhandler_ptr signalHandler, const alljoyn_interfacedescription_member member, const char* srcPath)
{
    QStatus ret = ER_OK;
    const ajn::InterfaceDescription::Member* cpp_member = (const ajn::InterfaceDescription::Member*)(member.internal_member);

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
                                    srcPath);
    }
    if (ret == ER_OK) {
        signalCallbackMapLock.Lock(MUTEX_CONTEXT);
        signalCallbackMap.insert(pair<const ajn::InterfaceDescription::Member*, alljoyn_messagereceiver_signalhandler_ptr>(
                                     cpp_member,
                                     signalHandler));
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
    std::multimap<const ajn::InterfaceDescription::Member*, alljoyn_messagereceiver_signalhandler_ptr>::iterator it;
    pair<std::multimap<const ajn::InterfaceDescription::Member*, alljoyn_messagereceiver_signalhandler_ptr>::iterator,
         std::multimap<const ajn::InterfaceDescription::Member*, alljoyn_messagereceiver_signalhandler_ptr>::iterator> ret;

    signalCallbackMapLock.Lock(MUTEX_CONTEXT);
    ret = signalCallbackMap.equal_range(cpp_member);
    if (ret.first != ret.second) {
        for (it = ret.first; it != ret.second; ++it) {
            if (signalHandler == it->second) {
                signalCallbackMap.erase(it);
                return_status = ER_OK;
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
    std::multimap<const ajn::InterfaceDescription::Member*, alljoyn_messagereceiver_signalhandler_ptr>::iterator it;
    pair<std::multimap<const ajn::InterfaceDescription::Member*, alljoyn_messagereceiver_signalhandler_ptr>::iterator,
         std::multimap<const ajn::InterfaceDescription::Member*, alljoyn_messagereceiver_signalhandler_ptr>::iterator> ret;

    signalCallbackMapLock.Lock(MUTEX_CONTEXT);
    ret = signalCallbackMap.equal_range(member);
    if (ret.first != ret.second) {
        for (it = ret.first; it != ret.second; ++it) {
            alljoyn_messagereceiver_signalhandler_ptr remappedHandler = it->second;
            remappedHandler(&c_member, srcPath, (alljoyn_message) & message);
        }
    }
    signalCallbackMapLock.Unlock(MUTEX_CONTEXT);
}
}
