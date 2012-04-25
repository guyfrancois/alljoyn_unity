/**
 * @file
 *
 * This file implements the ProxyBusObject class.
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
#include <alljoyn/ProxyBusObject.h>
#include <alljoyn_c/ProxyBusObject.h>
#include "BusAttachmentC.h"
#include "MessageReceiverC.h"
#include "ProxyBusObjectListenerC.h"

#define QCC_MODULE "ALLJOYN"

struct _alljoyn_proxybusobject_handle {
    /* Empty by design, this is just to allow the type restrictions to save coders from themselves */
};

/* static instances of classes used for Async CallBacks */
static ajn::MessageReceiverC msgReceverC;
static ajn::ProxyBusObjectListenerC proxyObjListener;

alljoyn_proxybusobject alljoyn_proxybusobject_create(alljoyn_busattachment bus, const char* service,
                                                     const char* path, alljoyn_sessionid sessionId)
{
    ajn::ProxyBusObject* ret = new ajn::ProxyBusObject(*((ajn::BusAttachmentC*)bus), service, path, sessionId);
    return (alljoyn_proxybusobject)ret;
}

void alljoyn_proxybusobject_destroy(alljoyn_proxybusobject bus)
{
    assert(bus != NULL && "NULL parameter passed to alljoyn_proxybusobject_destroy.");
    delete (ajn::ProxyBusObject*)bus;
}

QStatus alljoyn_proxybusobject_addinterface(alljoyn_proxybusobject proxyObj, const alljoyn_interfacedescription iface)
{
    return ((ajn::ProxyBusObject*)proxyObj)->AddInterface(*((const ajn::InterfaceDescription*)iface));
}

QStatus alljoyn_proxybusobject_addinterface_by_name(alljoyn_proxybusobject proxyObj, const char* name)
{
    return ((ajn::ProxyBusObject*)proxyObj)->AddInterface(name);
}

alljoyn_proxybusobject alljoyn_proxybusobject_getchild(alljoyn_proxybusobject proxyObj, const char* path)
{
    return (alljoyn_proxybusobject)((ajn::ProxyBusObject*)proxyObj)->GetChild(path);
}

QStatus alljoyn_proxybusobject_addchild(alljoyn_proxybusobject proxyObj, const alljoyn_proxybusobject child)
{
    return ((ajn::ProxyBusObject*)proxyObj)->AddChild(*(ajn::ProxyBusObject*)child);
}

QStatus alljoyn_proxybusobject_removechild(alljoyn_proxybusobject proxyObj, const char* path)
{
    return ((ajn::ProxyBusObject*)proxyObj)->RemoveChild(path);
}

QStatus alljoyn_proxybusobject_introspectremoteobject(alljoyn_proxybusobject proxyObj)
{
    return ((ajn::ProxyBusObject*)proxyObj)->IntrospectRemoteObject();
}

QStatus alljoyn_proxybusobject_introspectremoteobjectasync(alljoyn_proxybusobject proxyObj, alljoyn_proxybusobject_listener_introspectcb_ptr callback, void* context)
{
    return ((ajn::ProxyBusObject*)proxyObj)->IntrospectRemoteObjectAsync(&proxyObjListener,
                                                                         static_cast<ajn::ProxyBusObject::Listener::IntrospectCB>(&ajn::ProxyBusObjectListenerC::IntrospectCB),
                                                                         (void*) new ajn::IntrospectCallbackContext(callback, context));
}

QStatus alljoyn_proxybusobject_getproperty(alljoyn_proxybusobject proxyObj, const char* iface, const char* property, alljoyn_msgarg value)
{
    ajn::MsgArg* reply = (ajn::MsgArg*)&(*value);
    return ((ajn::ProxyBusObject*)proxyObj)->GetProperty(iface, property, *reply);
}

QStatus alljoyn_proxybusobject_getallproperties(alljoyn_proxybusobject proxyObj, const char* iface, alljoyn_msgarg values)
{
    ajn::MsgArg* reply = (ajn::MsgArg*)&(*values);
    return ((ajn::ProxyBusObject*)proxyObj)->GetAllProperties(iface, *reply);
}

QStatus alljoyn_proxybusobject_setproperty(alljoyn_proxybusobject proxyObj, const char* iface, const char* property, alljoyn_msgarg value)
{
    ajn::MsgArg* reply = (ajn::MsgArg*)&(*value);
    return ((ajn::ProxyBusObject*)proxyObj)->SetProperty(iface, property, *reply);
}

QStatus alljoyn_proxybusobject_methodcall(alljoyn_proxybusobject obj,
                                          const char* ifaceName,
                                          const char* methodName,
                                          alljoyn_msgarg args,
                                          size_t numArgs,
                                          alljoyn_message replyMsg,
                                          uint32_t timeout,
                                          uint8_t flags)
{
    ajn::Message* reply = (ajn::Message*)&(*replyMsg);
    return ((ajn::ProxyBusObject*)obj)->MethodCall(ifaceName, methodName, (const ajn::MsgArg*)args,
                                                   numArgs, *reply, timeout, flags);
}

QStatus alljoyn_proxybusobject_methodcall_member(alljoyn_proxybusobject proxyObj,
                                                 const alljoyn_interfacedescription_member method,
                                                 const alljoyn_msgarg args,
                                                 size_t numArgs,
                                                 alljoyn_message replyMsg,
                                                 uint32_t timeout,
                                                 uint8_t flags)
{
    ajn::Message* reply = (ajn::Message*)&(*replyMsg);
    return ((ajn::ProxyBusObject*)proxyObj)->MethodCall(*(const ajn::InterfaceDescription::Member*)(method.internal_member),
                                                        (const ajn::MsgArg*)args, numArgs, *reply, timeout, flags);
}

QStatus alljoyn_proxybusobject_methodcall_noreply(alljoyn_proxybusobject proxyObj,
                                                  const char* ifaceName,
                                                  const char* methodName,
                                                  const alljoyn_msgarg args,
                                                  size_t numArgs,
                                                  uint8_t flags)
{
    return ((ajn::ProxyBusObject*)proxyObj)->MethodCall(ifaceName, methodName, (const ajn::MsgArg*)args, numArgs, flags);
}

QStatus alljoyn_proxybusobject_methodcall_member_noreply(alljoyn_proxybusobject proxyObj,
                                                         const alljoyn_interfacedescription_member method,
                                                         const alljoyn_msgarg args,
                                                         size_t numArgs,
                                                         uint8_t flags)
{
    return ((ajn::ProxyBusObject*)proxyObj)->MethodCall(*(const ajn::InterfaceDescription::Member*)(method.internal_member),
                                                        (const ajn::MsgArg*)args, numArgs, flags);
}

QStatus alljoyn_proxybusobject_methodcallasync(alljoyn_proxybusobject proxyObj,
                                               const char* ifaceName,
                                               const char* methodName,
                                               alljoyn_messagereceiver_replyhandler_ptr replyFunc,
                                               const alljoyn_msgarg args,
                                               size_t numArgs,
                                               void* context,
                                               uint32_t timeout,
                                               uint8_t flags)
{
    return ((ajn::ProxyBusObject*)proxyObj)->MethodCallAsync(ifaceName,
                                                             methodName,
                                                             &msgReceverC,
                                                             static_cast<ajn::MessageReceiver::ReplyHandler>(&ajn::MessageReceiverC::ReplyHandler),
                                                             (const ajn::MsgArg*)args,
                                                             numArgs,
                                                             new ajn::MessageReceiverReplyHandlerCallbackContext(replyFunc, context),
                                                             timeout,
                                                             flags);
}

QStatus alljoyn_proxybusobject_methodcallasync_member(alljoyn_proxybusobject proxyObj,
                                                      const alljoyn_interfacedescription_member method,
                                                      /* MessageReceiver* receiver,*/
                                                      alljoyn_messagereceiver_replyhandler_ptr replyFunc,
                                                      const alljoyn_msgarg args,
                                                      size_t numArgs,
                                                      void* context,
                                                      uint32_t timeout,
                                                      uint8_t flags)
{
    return ((ajn::ProxyBusObject*)proxyObj)->MethodCallAsync(*(const ajn::InterfaceDescription::Member*)(method.internal_member),
                                                             &msgReceverC,
                                                             static_cast<ajn::MessageReceiver::ReplyHandler>(&ajn::MessageReceiverC::ReplyHandler),
                                                             (const ajn::MsgArg*)args,
                                                             numArgs,
                                                             new ajn::MessageReceiverReplyHandlerCallbackContext(replyFunc, context),
                                                             timeout,
                                                             flags);
}

QStatus alljoyn_proxybusobject_parsexml(alljoyn_proxybusobject proxyObj, const char* xml, const char* identifier)
{
    return ((ajn::ProxyBusObject*)proxyObj)->ParseXml(xml, identifier);
}

QStatus alljoyn_proxybusobject_secureconnection(alljoyn_proxybusobject proxyObj, QC_BOOL forceAuth)
{
    return ((ajn::ProxyBusObject*)proxyObj)->SecureConnection(forceAuth);
}

QStatus alljoyn_proxybusobject_secureconnectionasync(alljoyn_proxybusobject proxyObj, QC_BOOL forceAuth)
{
    return ((ajn::ProxyBusObject*)proxyObj)->SecureConnectionAsync(forceAuth);
}
const alljoyn_interfacedescription alljoyn_proxybusobject_getinterface(alljoyn_proxybusobject proxyObj, const char* iface)
{
    return (const alljoyn_interfacedescription)((ajn::ProxyBusObject*)proxyObj)->GetInterface(iface);
}

size_t alljoyn_proxybusobject_getinterfaces(alljoyn_proxybusobject proxyObj, const alljoyn_interfacedescription* ifaces, size_t numIfaces)
{
    return ((ajn::ProxyBusObject*)proxyObj)->GetInterfaces(((const ajn::InterfaceDescription**)ifaces), numIfaces);
}

const char* alljoyn_proxybusobject_getpath(alljoyn_proxybusobject proxyObj)
{
    return ((ajn::ProxyBusObject*)proxyObj)->GetPath().c_str();
}

const char* alljoyn_proxybusobject_getservicename(alljoyn_proxybusobject proxyObj)
{
    return ((ajn::ProxyBusObject*)proxyObj)->GetServiceName().c_str();
}

alljoyn_sessionid alljoyn_proxybusobject_getsessionid(alljoyn_proxybusobject proxyObj)
{
    return (alljoyn_sessionid)((ajn::ProxyBusObject*)proxyObj)->GetSessionId();
}

QC_BOOL alljoyn_proxybusobject_implementsinterface(alljoyn_proxybusobject proxyObj, const char* iface)
{
    return (QC_BOOL)((ajn::ProxyBusObject*)proxyObj)->ImplementsInterface(iface);
}

alljoyn_proxybusobject alljoyn_proxybusobject_copy(const alljoyn_proxybusobject source)
{
    if (!source) {
        return NULL;
    }
    ajn::ProxyBusObject* ret = new ajn::ProxyBusObject;
    *ret = *(ajn::ProxyBusObject*)source;
    return (alljoyn_proxybusobject) ret;
}

QC_BOOL alljoyn_proxybusobject_isvalid(alljoyn_proxybusobject proxyObj)
{
    return (QC_BOOL)((ajn::ProxyBusObject*)proxyObj)->IsValid();
}
