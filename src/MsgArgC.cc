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
#include "MsgArgC.h"
#include <Status.h>
#include <alljoyn/MsgArg.h>

namespace ajn {

QStatus MsgArgC::MsgArgUtilsSetVC(MsgArg* args, size_t& numArgs, const char* signature, va_list* argp) {
    return MsgArgUtils::SetV(args, numArgs, signature, argp);
}

QStatus MsgArgC::VBuildArgsC(const char*& signature, size_t sigLen, MsgArg* arg, size_t maxArgs, va_list* argp, size_t* count) {
    return VBuildArgs(signature, sigLen, arg, maxArgs, argp, count);
}

QStatus MsgArgC::VParseArgsC(const char*& signature, size_t sigLen, const MsgArg* argList, size_t numArgs, va_list* argp) {
    return VParseArgs(signature, sigLen, argList, numArgs, argp);
}
}
