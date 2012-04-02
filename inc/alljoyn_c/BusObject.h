/**
 * @file
 *
 * This file defines the base class for message bus objects that
 * are implemented and registered locally.
 *
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
#ifndef _ALLJOYN_C_LOCALBUSOBJECT_H
#define _ALLJOYN_C_LOCALBUSOBJECT_H

#include <qcc/platform.h>
#include <alljoyn_c/AjAPI.h>
#include <alljoyn_c/InterfaceDescription.h>
#include <alljoyn_c/MsgArg.h>
#include <alljoyn_c/MessageReceiver.h>
#include <alljoyn_c/Session.h>
#include <Status.h>

#ifdef __cplusplus
extern "C" {
#endif /* #ifdef __cplusplus */

#ifndef _ALLJOYN_OPAQUE_BUSOBJECT_
#define _ALLJOYN_OPAQUE_BUSOBJECT_
typedef struct _alljoyn_busobject_handle*                   alljoyn_busobject;
#endif

/*
 * Callback for property get method.
 */
typedef QStatus (*alljoyn_busobject_prop_get_ptr)(const void* context, const char* ifcName, const char* propName, alljoyn_msgarg val);

/*
 * Callback for property set method.
 */
typedef QStatus (*alljoyn_busobject_prop_set_ptr)(const void* context, const char* ifcName, const char* propName, alljoyn_msgarg val);

/*
 * Callback for ObjectRegistered and ObjectUnregistered
 */
typedef void (*alljoyn_busobject_object_registration_ptr)(const void* context);

typedef struct {
    alljoyn_busobject_prop_get_ptr property_get;
    alljoyn_busobject_prop_set_ptr property_set;
    alljoyn_busobject_object_registration_ptr object_registered;
    alljoyn_busobject_object_registration_ptr object_unregistered;
} alljoyn_busobject_callbacks;

/**
 * Type used to add mulitple methods at one time.
 * @see AddMethodHandlers()
 */
typedef struct {
    const alljoyn_interfacedescription_member* member;          /**< Pointer to method's member */
    alljoyn_messagereceiver_methodhandler_ptr method_handler;   /**< Method implementation */
} alljoyn_busobject_methodentry;

/**
 * Create a %BusObject.
 *
 * @param bus            Bus that this object exists on.
 * @param path           Object path for object.
 * @param isPlaceholder  Place-holder objects are created by the bus itself and serve only
 *                       as parent objects (in the object path sense) to other objects.
 */
extern AJ_API alljoyn_busobject alljoyn_busobject_create(alljoyn_busattachment bus, const char* path, QC_BOOL isPlaceholder,
                                                         const alljoyn_busobject_callbacks* callbacks_in, const void* context_in);

/**
 * Destroy a BusObject
 *
 * @param bus Bus to destroy.
 */
extern AJ_API void alljoyn_busobject_destroy(alljoyn_busobject bus);

/**
 * Return the path for the object
 *
 * @param bus BusObject on which to get the path.
 * @return Object path
 */
extern AJ_API const char* alljoyn_busobject_getpath(alljoyn_busobject bus);

/**
 * Get the name of this object.
 * The name is the last component of the path.
 *
 * If the buffer is NULL or the bufferSz is 0, the length of the name will be returned.
 *
 * @param bus       BusObject on which to get the name.
 * @param buffer    A buffer into which to copy the name.
 * @param bufferSz  The size of the buffer provided.
 * @return The size of the name string, if the returned value is > bufferSz, the entire name was not copied into buffer.
 */
extern AJ_API size_t alljoyn_busobject_getname(alljoyn_busobject bus, char* buffer, size_t bufferSz);

/**
 * Add an interface to this object. If the interface has properties this will also add the
 * standard property access interface. An interface must be added before its method handlers can be
 * added. Note that the Peer interface (org.freedesktop.DBus.peer) is implicit on all objects and
 * cannot be explicitly added, and the Properties interface (org.freedesktop,DBus.Properties) is
 * automatically added when needed and cannot be explicitly added.
 *
 * Once an object is registered, it should not add any additional interfaces. Doing so would
 * confuse remote objects that may have already introspected this object.
 *
 * @param bus    The bus on which to add the interface
 * @param iface  The interface to add
 *
 * @return
 *      - #ER_OK if the interface was successfully added.
 *      - #ER_BUS_IFACE_ALREADY_EXISTS if the interface already exists.
 *      - An error status otherwise
 */
extern AJ_API QStatus alljoyn_busobject_addinterface(alljoyn_busobject bus, const alljoyn_interfacedescription iface);

/**
 * Add a set of method handers at once.
 *
 * @param bus          The bus on which to add method handlers.
 * @param entries      Array of alljoyn_busobject_methodentry.
 * @param numEntries   Number of entries in array.
 *
 * @return
 *      - #ER_OK if all the methods were added
 *      - #ER_BUS_NO_SUCH_INTERFACE is method can not be added because interface does not exist.
 */
extern AJ_API QStatus alljoyn_busobject_addmethodhandlers(alljoyn_busobject bus,
                                                          const alljoyn_busobject_methodentry* entries,
                                                          size_t numEntries);

/**
 * Reply to a method call
 *
 * @param bus      The bus on which to reply.
 * @param msg      The method call message
 * @param args     The reply arguments (can be NULL)
 * @param numArgs  The number of arguments
 * @return
 *      - #ER_OK if successful
 *      - An error status otherwise
 */
extern AJ_API QStatus alljoyn_busobject_methodreply_args(alljoyn_busobject bus, alljoyn_message msg,
                                                         const alljoyn_msgarg args, size_t numArgs);
/**
 * Reply to a method call with an error message
 *
 * @param bus              The bus on which to reply.
 * @param msg              The method call message
 * @param error            The name of the error
 * @param errorMessage     An error message string
 * @return
 *      - #ER_OK if successful
 *      - An error status otherwise
 */
extern AJ_API QStatus alljoyn_busobject_methodreply_err(alljoyn_busobject bus, alljoyn_message msg,
                                                        const char* error, const char* errorMessage);
/**
 * Reply to a method call with an error message
 *
 * @param bus        The bus on which to reply.
 * @param msg        The method call message
 * @param status     The status code for the error
 * @return
 *      - #ER_OK if successful
 *      - An error status otherwise
 */
extern AJ_API QStatus alljoyn_busobject_methodreply_status(alljoyn_busobject bus, alljoyn_message msg, QStatus status);

/**
 * Send a signal.
 *
 * @param bus              The bus used to send the signal
 * @param destination      The unique or well-known bus name or the signal recipient (NULL for broadcast signals)
 * @param sessionId        A unique SessionId for this AllJoyn session instance
 * @param signal           Interface member of signal being emitted.
 * @param args             The arguments for the signal (can be NULL)
 * @param numArgs          The number of arguments
 * @param timeToLive       If non-zero this specifies in milliseconds the useful lifetime for this
 *                         signal. If delivery of the signal is delayed beyond the timeToLive due to
 *                         network congestion or other factors the signal may be discarded. There is
 *                         no guarantee that expired signals will not still be delivered.
 * @param flags            Logical OR of the message flags for this signals. The following flags apply to signals:
 *                         - If ::ALLJOYN_FLAG_GLOBAL_BROADCAST is set broadcast signal (null destination) will be forwarded across bus-to-bus connections.
 *                         - If ::ALLJOYN_FLAG_COMPRESSED is set the header is compressed for destinations that can handle header compression.
 *                         - If ::ALLJOYN_FLAG_ENCRYPTED is set the message is authenticated and the payload if any is encrypted.
 * @return
 *      - #ER_OK if successful
 *      - An error status otherwise
 */
extern AJ_API QStatus alljoyn_busobject_signal(alljoyn_busobject bus,
                                               const char* destination,
                                               alljoyn_sessionid sessionId,
                                               const alljoyn_interfacedescription_member signal,
                                               const alljoyn_msgarg args,
                                               size_t numArgs,
                                               uint16_t timeToLive,
                                               uint8_t flags);
#if 0
/* TODO list of C++ methods that have not be mapped to the C yet */
/**
 * Add a method handler to this object. The interface for the method handler must have already
 * been added by calling AddInterface().
 *
 * @param member   Interface member implemented by handler.
 * @param handler  Method handler.
 * @param context  An optional context. This is mainly intended for implementing language
 *                 bindings and should normally be NULL.
 *
 * @return
 *      - #ER_OK if the method handler was added.
 *      - An error status otherwise
 */
QStatus AddMethodHandler(const InterfaceDescription::Member* member, MessageReceiver::MethodHandler handler, void* context = NULL);

/**
 * Returns a description of the object in the D-Bus introspection XML format.
 * This method can be overridden by derived classes in order to customize the
 * introspection XML presented to remote nodes. Note that to DTD description and
 * the root element are not generated.
 *
 * @param deep     Include XML for all descendants rather than stopping at direct children.
 * @param indent   Number of characters to indent the XML
 * @return Description of the object in D-Bus introspection XML format
 */
virtual qcc::String GenerateIntrospection(bool deep = false, size_t indent = 0) const;

/**
 * Default handler for a bus attempt to read the object's introspection data.
 * @remark
 * A derived class can override this function to provide a custom handler for the GetProp method
 * call. If overridden the custom handler must compose an appropriate reply message.
 *
 * @param member   Identifies the @c org.freedesktop.DBus.Introspectable.Introspect method.
 * @param msg      The Introspectable.Introspect request.
 */
virtual void Introspect(const InterfaceDescription::Member* member, Message& msg);
#endif

#ifdef __cplusplus
} /* extern "C" */
#endif

#endif
