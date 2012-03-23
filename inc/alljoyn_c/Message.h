/**
 * @file
 * This file defines a class for parsing and generating message bus messages
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
#ifndef _ALLJOYN_C_MESSAGE_H
#define _ALLJOYN_C_MESSAGE_H

#include <qcc/platform.h>
#include <alljoyn_c/AjAPI.h>
#include <alljoyn_c/MsgArg.h>
#include <alljoyn_c/Session.h>
#include <Status.h>

#ifdef __cplusplus
extern "C" {
#endif

typedef struct _alljoyn_message_handle*                     alljoyn_message;
#ifndef _ALLJOYN_OPAQUE_BUSATTACHMENT_
#define _ALLJOYN_OPAQUE_BUSATTACHMENT_
typedef struct _alljoyn_busattachment_handle*               alljoyn_busattachment;
#endif

/** @name Flag types */
/* @{ */
/** No reply is expected */
static const uint8_t ALLJOYN_MESSAGE_FLAG_NO_REPLY_EXPECTED  = 0x01;
/** Auto start the service */
static const uint8_t ALLJOYN_MESSAGE_FLAG_AUTO_START         = 0x02;
/** Allow messages from remote hosts (valid only in Hello message) */
static const uint8_t ALLJOYN_MESSAGE_FLAG_ALLOW_REMOTE_MSG   = 0x04;
/** Global (bus-to-bus) broadcast */
static const uint8_t ALLJOYN_MESSAGE_FLAG_GLOBAL_BROADCAST   = 0x20;
/** Header is compressed */
static const uint8_t ALLJOYN_MESSAGE_FLAG_COMPRESSED         = 0x40;
/** Body is encrypted */
static const uint8_t ALLJOYN_MESSAGE_FLAG_ENCRYPTED          = 0x80;
/* @} */

/**
 * The default timeout for method calls
 */
static const uint32_t ALLJOYN_MESSAGE_DEFAULT_TIMEOUT       = 25000;

/** Message types */
typedef enum {
    ALLJOYN_MESSAGE_INVALID     = 0, ///< an invalid message type
    ALLJOYN_MESSAGE_METHOD_CALL = 1, ///< a method call message type
    ALLJOYN_MESSAGE_METHOD_RET  = 2, ///< a method return message type
    ALLJOYN_MESSAGE_ERROR       = 3, ///< an error message type
    ALLJOYN_MESSAGE_SIGNAL      = 4  ///< a signal message type
} alljoyn_messagetype;

/**
 * Create a message object.
 *
 * @param bus  The bus that this message is sent or received on.
 */
extern AJ_API alljoyn_message alljoyn_message_create(alljoyn_busattachment bus);

/**
 * Destroy a message object.
 *
 * @param msg The message to destroy
 */
extern AJ_API void alljoyn_message_destroy(alljoyn_message msg);

/**
 * Return a specific argument.
 *
 * @param msg   The message from which to extract an argument.
 * @param argN  The index of the argument to get.
 *
 * @return
 *      - The argument
 *      - NULL if unmarshal failed or there is not such argument.
 */
extern AJ_API const alljoyn_msgarg alljoyn_message_getarg(alljoyn_message msg, size_t argN);

#ifdef __cplusplus
} /* extern "C" */
#endif

#endif
