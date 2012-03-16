/**
 * @file
 * BusAttachment is the top-level object responsible for connecting to and optionally managing a message bus.
 */

/******************************************************************************
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
#include <assert.h>

#include <alljoyn_c/BusAttachment.h>
#include "BusAttachmentC.h"

#define QCC_MODULE "ALLJOYN"

struct _alljoyn_busattachment_handle {
    /* Empty by design, this is just to allow the type restrictions to save coders from themselves */
};

alljoyn_busattachment alljoyn_busattachment_create(const char* applicationName, QC_BOOL allowRemoteMessages)
{
    bool allowRemoteMessagesBool = (allowRemoteMessages == QC_TRUE ? true : false);
    return ((alljoyn_busattachment) new ajn::BusAttachmentC(applicationName, allowRemoteMessagesBool));
}

void alljoyn_busattachment_destroy(alljoyn_busattachment bus)
{
    assert(bus != NULL && "NULL parameter passed to alljoyn_destroy_busattachment.");

    delete (ajn::BusAttachmentC*)bus;
}

QStatus alljoyn_busattachment_start(alljoyn_busattachment bus)
{
    return ((ajn::BusAttachmentC*)bus)->Start();
}

QStatus alljoyn_busattachment_stop(alljoyn_busattachment bus)
{
    return ((ajn::BusAttachmentC*)bus)->Stop();
}

extern AJ_API QStatus alljoyn_busattachment_join(alljoyn_busattachment bus)
{
    return ((ajn::BusAttachmentC*)bus)->Join();
}

QStatus alljoyn_busattachment_createinterface(alljoyn_busattachment bus,
                                              const char* name,
                                              alljoyn_interfacedescription* iface, QC_BOOL secure)
{
    bool secureBool = (secure == QC_TRUE ? true : false);
    ajn::InterfaceDescription* ifaceObj = NULL;
    QStatus ret = ((ajn::BusAttachmentC*)bus)->CreateInterface(name, ifaceObj, secureBool);
    *iface = (alljoyn_interfacedescription)ifaceObj;

    return ret;
}

QStatus alljoyn_busattachment_connect(alljoyn_busattachment bus, const char* connectSpec)
{
    // Because the second parameter to Connect is only used internally it is not exposed to the C interface.
    return ((ajn::BusAttachmentC*)bus)->Connect(connectSpec, NULL);
}

void alljoyn_busattachment_registerbuslistener(alljoyn_busattachment bus, alljoyn_buslistener listener)
{
    ((ajn::BusAttachmentC*)bus)->RegisterBusListener((*(ajn::BusListener*)listener));
}

void alljoyn_busattachment_unregisterbuslistener(alljoyn_busattachment bus, alljoyn_buslistener listener)
{
    ((ajn::BusAttachmentC*)bus)->UnregisterBusListener((*(ajn::BusListener*)listener));
}

QStatus alljoyn_busattachment_findadvertisedname(alljoyn_busattachment bus, const char* namePrefix)
{
    return ((ajn::BusAttachmentC*)bus)->FindAdvertisedName(namePrefix);
}

QStatus alljoyn_busattachment_cancelfindadvertisedname(alljoyn_busattachment bus, const char* namePrefix)
{
    return ((ajn::BusAttachmentC*)bus)->CancelFindAdvertisedName(namePrefix);
}

const alljoyn_interfacedescription alljoyn_busattachment_getinterface(alljoyn_busattachment bus, const char* name)
{
    return (alljoyn_interfacedescription)((ajn::BusAttachmentC*)bus)->GetInterface(name);
}

QStatus alljoyn_busattachment_joinsession(alljoyn_busattachment bus, const char* sessionHost,
                                          alljoyn_sessionport sessionPort, alljoyn_sessionlistener listener,
                                          alljoyn_sessionid* sessionId, alljoyn_sessionopts opts)
{
    return ((ajn::BusAttachmentC*)bus)->JoinSession(sessionHost, (ajn::SessionPort)sessionPort,
                                                    (ajn::SessionListener*)listener, *((ajn::SessionId*)sessionId),
                                                    *((ajn::SessionOpts*)opts));
}

QStatus alljoyn_busattachment_registerbusobject(alljoyn_busattachment bus, alljoyn_busobject obj)
{
    return ((ajn::BusAttachmentC*)bus)->RegisterBusObject(*((ajn::BusObject*)obj));
}

void alljoyn_busattachment_unregisterbusobject(alljoyn_busattachment bus, alljoyn_busobject object)
{
    ((ajn::BusAttachmentC*)bus)->UnregisterBusObject(*((ajn::BusObject*)object));
}

QStatus alljoyn_busattachment_requestname(alljoyn_busattachment bus, const char* requestedName, uint32_t flags)
{
    return ((ajn::BusAttachmentC*)bus)->RequestName(requestedName, flags);
}

QStatus alljoyn_busattachment_bindsessionport(alljoyn_busattachment bus, alljoyn_sessionport* sessionPort,
                                              const alljoyn_sessionopts opts, alljoyn_sessionportlistener listener)
{
    return ((ajn::BusAttachmentC*)bus)->BindSessionPort(*((ajn::SessionPort*)sessionPort),
                                                        *((const ajn::SessionOpts*)opts),
                                                        *((ajn::SessionPortListener*)listener));
}

QStatus alljoyn_busattachment_unbindsessionport(alljoyn_busattachment bus, alljoyn_sessionport sessionPort)
{
    return ((ajn::BusAttachmentC*)bus)->UnbindSessionPort(sessionPort);
}

QStatus alljoyn_busattachment_advertisename(alljoyn_busattachment bus, const char* name, alljoyn_transportmask transports)
{
    return ((ajn::BusAttachmentC*)bus)->AdvertiseName(name, transports);
}

QStatus alljoyn_busattachment_canceladvertisedname(alljoyn_busattachment bus, const char* name, alljoyn_transportmask transports)
{
    return ((ajn::BusAttachmentC*)bus)->CancelAdvertiseName(name, transports);
}

QStatus alljoyn_busattachment_enablepeersecurity(alljoyn_busattachment bus, const char* authMechanisms,
                                                 alljoyn_authlistener listener, const char* keyStoreFileName,
                                                 QC_BOOL isShared)
{
    return ((ajn::BusAttachmentC*)bus)->EnablePeerSecurity(authMechanisms, (ajn::AuthListener*)listener, keyStoreFileName,
                                                           (isShared == QC_TRUE ? true : false));
}

QC_BOOL alljoyn_busattachment_ispeersecurityenabled(alljoyn_busattachment bus)
{
    return (((ajn::BusAttachmentC*)bus)->IsPeerSecurityEnabled() == true ? QC_TRUE : QC_FALSE);
}

QStatus alljoyn_busattachment_createinterfacesfromxml(alljoyn_busattachment bus, const char* xml)
{
    return ((ajn::BusAttachmentC*)bus)->CreateInterfacesFromXml(xml);
}

size_t alljoyn_busattachment_getinterfaces(const alljoyn_busattachment bus,
                                           const alljoyn_interfacedescription* ifaces, size_t numIfaces)
{
    return ((ajn::BusAttachmentC*)bus)->GetInterfaces((const ajn::InterfaceDescription**)ifaces, numIfaces);
}

QStatus alljoyn_busattachment_deleteinterface(alljoyn_busattachment bus, alljoyn_interfacedescription iface)
{
    return ((ajn::BusAttachmentC*)bus)->DeleteInterface(*((ajn::InterfaceDescription*)iface));
}

QC_BOOL alljoyn_busattachment_isstarted(alljoyn_busattachment bus)
{
    return (((ajn::BusAttachmentC*)bus)->IsStarted() == true ? QC_TRUE : QC_FALSE);
}

QC_BOOL alljoyn_busattachment_isstopping(alljoyn_busattachment bus)
{
    return (((ajn::BusAttachmentC*)bus)->IsStopping() == true ? QC_TRUE : QC_FALSE);
}

QC_BOOL alljoyn_busattachment_isconnected(const alljoyn_busattachment bus)
{
    return (((const ajn::BusAttachmentC*)bus)->IsConnected() == true ? QC_TRUE : QC_FALSE);
}

QStatus alljoyn_busattachment_disconnect(alljoyn_busattachment bus, const char* connectSpec)
{
    return ((ajn::BusAttachmentC*)bus)->Disconnect(connectSpec);
}

const alljoyn_proxybusobject alljoyn_busattachment_getdbusproxyobj(alljoyn_busattachment bus)
{
    return (const alljoyn_proxybusobject)(&((ajn::BusAttachmentC*)bus)->GetDBusProxyObj());
}

const alljoyn_proxybusobject alljoyn_busattachment_getalljoynproxyobj(alljoyn_busattachment bus)
{
    return (const alljoyn_proxybusobject)(&((ajn::BusAttachmentC*)bus)->GetAllJoynProxyObj());
}

const alljoyn_proxybusobject alljoyn_busattachment_getalljoyndebugobj(alljoyn_busattachment bus)
{
    return (const alljoyn_proxybusobject)(&((ajn::BusAttachmentC*)bus)->GetAllJoynDebugObj());
}

const char* alljoyn_busattachment_getuniquename(const alljoyn_busattachment bus)
{
    return ((const ajn::BusAttachmentC*)bus)->GetUniqueName().c_str();
}

const char* alljoyn_busattachment_getglobalguidstring(const alljoyn_busattachment bus)
{
    return ((const ajn::BusAttachmentC*)bus)->GetGlobalGUIDString().c_str();
}

QStatus alljoyn_busattachment_registerkeystorelistener(alljoyn_busattachment bus, alljoyn_keystorelistener listener)
{
    return ((ajn::BusAttachmentC*)bus)->RegisterKeyStoreListener(*((ajn::KeyStoreListener*)listener));
}

QStatus alljoyn_busattachment_reloadkeystore(alljoyn_busattachment bus)
{
    return ((ajn::BusAttachmentC*)bus)->ReloadKeyStore();
}

void alljoyn_busattachment_clearkeystore(alljoyn_busattachment bus)
{
    ((ajn::BusAttachmentC*)bus)->ClearKeyStore();
}

QStatus alljoyn_busattachment_clearkeys(alljoyn_busattachment bus, const char* guid)
{
    return ((ajn::BusAttachmentC*)bus)->ClearKeys(guid);
}

QStatus alljoyn_busattachment_setkeyexpiration(alljoyn_busattachment bus, const char* guid, uint32_t timeout)
{
    return ((ajn::BusAttachmentC*)bus)->SetKeyExpiration(guid, timeout);
}

QStatus alljoyn_busattachment_getkeyexpiration(alljoyn_busattachment bus, const char* guid, uint32_t* timeout)
{
    return ((ajn::BusAttachmentC*)bus)->GetKeyExpiration(guid, *timeout);
}

QStatus alljoyn_busattachment_addlogonentry(alljoyn_busattachment bus, const char* authMechanism,
                                            const char* userName, const char* password)
{
    return ((ajn::BusAttachmentC*)bus)->AddLogonEntry(authMechanism, userName, password);
}

QStatus alljoyn_busattachment_releasename(alljoyn_busattachment bus, const char* name)
{
    return ((ajn::BusAttachmentC*)bus)->ReleaseName(name);
}

QStatus alljoyn_busattachment_addmatch(alljoyn_busattachment bus, const char* rule)
{
    return ((ajn::BusAttachmentC*)bus)->AddMatch(rule);
}

QStatus alljoyn_busattachment_removematch(alljoyn_busattachment bus, const char* rule)
{
    return ((ajn::BusAttachmentC*)bus)->RemoveMatch(rule);
}

QStatus alljoyn_busattachment_setsessionlistener(alljoyn_busattachment bus, alljoyn_sessionid sessionId,
                                                 alljoyn_sessionlistener listener)
{
    return ((ajn::BusAttachmentC*)bus)->SetSessionListener(sessionId, (ajn::SessionListener*)listener);
}

QStatus alljoyn_busattachment_leavesession(alljoyn_busattachment bus, alljoyn_sessionid sessionId)
{
    return ((ajn::BusAttachmentC*)bus)->LeaveSession(sessionId);
}

QStatus alljoyn_busattachment_setlinktimeout(alljoyn_busattachment bus, alljoyn_sessionid sessionid, uint32_t* linkTimeout)
{
    return ((ajn::BusAttachmentC*)bus)->SetLinkTimeout(sessionid, *linkTimeout);
}

QStatus alljoyn_busattachment_namehasowner(alljoyn_busattachment bus, const char* name, QC_BOOL* hasOwner)
{
    bool result;
    QStatus ret = ((ajn::BusAttachmentC*)bus)->NameHasOwner(name, result);
    *hasOwner = (result == true ? QC_TRUE : QC_FALSE);
    return ret;
}

QStatus alljoyn_busattachment_getpeerguid(alljoyn_busattachment bus, const char* name, char* guid, size_t* guidSz)
{
    qcc::String guidStr;
    QStatus ret = ((ajn::BusAttachmentC*)bus)->GetPeerGUID(name, guidStr);
    if (guid != NULL) {
        strncpy(guid, guidStr.c_str(), *guidSz);
    }
    *guidSz = guidStr.length();
    return ret;
}

QStatus alljoyn_busattachment_registersignalhandler(alljoyn_busattachment bus,
                                                    alljoyn_busobject receiver,
                                                    alljoyn_messagereceiver_signalhandler_ptr signal_handler,
                                                    const alljoyn_interfacedescription_member member,
                                                    const char* srcPath)
{
    return ((ajn::BusAttachmentC*)bus)->RegisterSignalHandlerC(receiver,
                                                               signal_handler,
                                                               member,
                                                               srcPath);
}

QStatus alljoyn_busattachment_unregistersignalhandler(alljoyn_busattachment bus,
                                                      alljoyn_busobject receiver,
                                                      alljoyn_messagereceiver_signalhandler_ptr signal_handler,
                                                      const alljoyn_interfacedescription_member member,
                                                      const char* srcPath)
{
    return ((ajn::BusAttachmentC*)bus)->UnregisterSignalHandlerC(receiver,
                                                                 signal_handler,
                                                                 member,
                                                                 srcPath);
}

QStatus alljoyn_busattachment_unregisterallhandlers(alljoyn_busattachment bus, alljoyn_busobject receiver)
{
    return ((ajn::BusAttachmentC*)bus)->UnregisterAllHandlersC(receiver);
}

QStatus alljoyn_busattachment_setdaemondebug(alljoyn_busattachment bus, const char* module, uint32_t level)
{
    return ((ajn::BusAttachmentC*)bus)->SetDaemonDebug(module, level);
}

uint32_t alljoyn_busattachment_gettimestamp()
{
    return ajn::BusAttachmentC::GetTimestamp();
}
