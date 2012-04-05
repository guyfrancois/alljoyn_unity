/**
 * @file
 * MessageReceiverC is an implementation of the MessagReceiver base class responsible
 * for mapping a C++ style MessageReceiver to a C style alljoyn_messagereceiver
 * function pointer
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
#ifndef _ALLJOYN_C_MESSAGERECEIVERC_H
#define _ALLJOYN_C_MESSAGERECEIVERC_H

#include <alljoyn/MessageReceiver.h>

namespace ajn {
/*
 * When setting up a asynchronous method call a  callback handler for C
 * alljoyn_messagereceiver_replyhandler_ptr function pointer will be passed in as
 * the callback handler.  AllJoyn expects a MessageReceiver::ReplyHandler method
 * handler.  The function handler will be passed as the part of the void*
 * context that is passed to the internal callback handler and will then be used
 * to map the internal callback handler to the user defined
 * messagereceived_replyhandler callback function pointer.
 */
class MessageReceiverReplyHandlerCallbackContext {
  public:
    MessageReceiverReplyHandlerCallbackContext(alljoyn_messagereceiver_replyhandler_ptr replyhandler_ptr, void* context) :
        replyhandler_ptr(replyhandler_ptr), context(context) { }

    alljoyn_messagereceiver_replyhandler_ptr replyhandler_ptr;
    void* context;
};

class MessageReceiverC : public MessageReceiver {
  public:
    void ReplyHandler(ajn::Message& message, void* context)
    {
        MessageReceiverReplyHandlerCallbackContext* in = (MessageReceiverReplyHandlerCallbackContext*)context;
        (in->replyhandler_ptr)((alljoyn_message) & message, in->context);

    }
};
}

#endif
