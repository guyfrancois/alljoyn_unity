/**
 * @file
 *
 * This file implements the _Message class
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

#include <qcc/platform.h>

#include <assert.h>
#include <ctype.h>

#include <alljoyn/Message.h>
#include "BusAttachmentC.h"
#include "MsgArgC.h"
#include <alljoyn_c/Message.h>
#include <alljoyn_c/BusAttachment.h>

#define QCC_MODULE "ALLJOYN"

struct _alljoyn_message_handle {
    _alljoyn_message_handle(ajn::BusAttachmentC& bus) : msg(bus) { }
    _alljoyn_message_handle(const ajn::_Message& other) : msg(other) { }

    ajn::Message msg;
};

alljoyn_message alljoyn_message_create(alljoyn_busattachment bus)
{
    return new struct _alljoyn_message_handle (*((ajn::BusAttachmentC*)bus));
}

void alljoyn_message_destroy(alljoyn_message msg)
{
    delete msg;
}

QC_BOOL alljoyn_message_isbroadcastsignal(alljoyn_message msg)
{
    return msg->msg->IsBroadcastSignal();
}

QC_BOOL alljoyn_message_isglobalbroadcast(alljoyn_message msg)
{
    return msg->msg->IsGlobalBroadcast();
}

uint8_t alljoyn_message_getflags(alljoyn_message msg)
{
    return msg->msg->GetFlags();
}

QC_BOOL alljoyn_message_isexpired(alljoyn_message msg, uint32_t* tillExpireMS)
{
    return msg->msg->IsExpired(tillExpireMS);
}

QC_BOOL alljoyn_message_isunreliable(alljoyn_message msg)
{
    return msg->msg->IsUnreliable();
}

QC_BOOL alljoyn_message_isencrypted(alljoyn_message msg)
{
    return msg->msg->IsEncrypted();
}

const char* alljoyn_message_getauthmechanism(alljoyn_message msg)
{
    return msg->msg->GetAuthMechanism().c_str();
}

alljoyn_messagetype alljoyn_message_gettype(alljoyn_message msg)
{
    return (alljoyn_messagetype)msg->msg->GetType();
}

void alljoyn_message_getargs(alljoyn_message msg, size_t* numArgs, alljoyn_msgarg* args)
{
    const ajn::MsgArg* tmpArgs;
    msg->msg->GetArgs(*numArgs, tmpArgs);
    *args = (alljoyn_msgarg)tmpArgs;
}

const alljoyn_msgarg alljoyn_message_getarg(alljoyn_message msg, size_t argN)
{
    return (alljoyn_msgarg)msg->msg->GetArg(argN);
}

QStatus alljoyn_message_parseargs(alljoyn_message msg, const char* signature, ...)
{

    size_t sigLen = (signature ? strlen(signature) : 0);
    if (sigLen == 0) {
        return ER_BAD_ARG_2;
    }
    const ajn::MsgArg* msgArgs;
    size_t numMsgArgs;

    msg->msg->GetArgs(numMsgArgs, msgArgs);

    va_list argp;
    va_start(argp, signature);
    QStatus status = ajn::MsgArgC::VParseArgsC(signature, sigLen, msgArgs, numMsgArgs, &argp);
    va_end(argp);
    return status;
}

uint32_t alljoyn_message_getcallserial(alljoyn_message msg)
{
    return msg->msg->GetCallSerial();
}

const char* alljoyn_message_getsignature(alljoyn_message msg)
{
    return msg->msg->GetSignature();
}

const char* alljoyn_message_getobjectpath(alljoyn_message msg)
{
    return msg->msg->GetObjectPath();
}

const char* alljoyn_message_getinterface(alljoyn_message msg)
{
    return msg->msg->GetInterface();
}

const char* alljoyn_message_getmembername(alljoyn_message msg)
{
    return msg->msg->GetMemberName();
}

uint32_t alljoyn_message_getreplyserial(alljoyn_message msg)
{
    return msg->msg->GetReplySerial();
}

const char* alljoyn_message_getsender(alljoyn_message msg)
{
    return msg->msg->GetSender();
}

const char* alljoyn_message_getreceiveendpointname(alljoyn_message msg)
{
    return msg->msg->GetRcvEndpointName();
}

const char* alljoyn_message_getdestination(alljoyn_message msg)
{
    return msg->msg->GetDestination();
}

uint32_t alljoyn_message_getcompressiontoken(alljoyn_message msg)
{
    return msg->msg->GetCompressionToken();
}

alljoyn_sessionid alljoyn_message_getsessionid(alljoyn_message msg)
{
    return (alljoyn_sessionid)msg->msg->GetSessionId();
}

const char* alljoyn_message_geterrorname(alljoyn_message msg, const char* errorMessage)
{
    const char* ret;
    qcc::String* str;
    ret = msg->msg->GetErrorName(str);
    errorMessage = str->c_str();
    return ret;
}

size_t alljoyn_message_tostring(alljoyn_message msg, char* str, size_t buf) {
    if (!msg) {
        return (size_t)0;
    }
    qcc::String s = msg->msg->ToString();
    /*
     * it is ok to send in NULL for str when the user is only interested in the
     * size of the resulting string.
     */
    if (str) {
        strncpy(str, s.c_str(), buf);
        str[buf] = '\0'; //prevent sting not being null terminated.
    }
    return s.size();
}

size_t alljoyn_message_description(alljoyn_message msg, char* str, size_t buf)
{
    if (!msg) {
        return (size_t)0;
    }
    qcc::String s = msg->msg->Description();
    /*
     * it is ok to send in NULL for str when the user is only interested in the
     * size of the resulting string.
     */
    if (str) {
        strncpy(str, s.c_str(), buf);
        str[buf] = '\0'; //prevent sting not being null terminated.
    }
    return s.size();
}

uint32_t alljoyn_message_gettimestamp(alljoyn_message msg)
{
    return msg->msg->GetTimeStamp();
}

QC_BOOL alljoyn_message_eql(const alljoyn_message one, const alljoyn_message other)
{
    return (one->msg == other->msg);
}

void alljoyn_message_setendianess(const char endian)
{
    ajn::_Message::SetEndianess(endian);
}
