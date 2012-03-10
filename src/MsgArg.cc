/**
 * @file
 *
 * This file implements the MsgArg class
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

#include <cstdarg>
#include <assert.h>

#include <alljoyn/Message.h>
#include <alljoyn/MsgArg.h>
#include <alljoyn_c/Message.h>
#include <alljoyn_c/MsgArg.h>
#include "MsgArgC.h"

#define QCC_MODULE "ALLJOYN"

struct _alljoyn_msgarg_handle {
    /* Empty by design, this is just to allow the type restrictions to save coders from themselves */
};

alljoyn_msgarg alljoyn_msgarg_create() {
    ajn::MsgArgC* arg = new ajn::MsgArgC;
    return (alljoyn_msgarg)arg;
}

alljoyn_msgarg alljoyn_msgarg_create_and_set(const char* signature, ...) {
    ajn::MsgArgC* arg = new ajn::MsgArgC(ajn::ALLJOYN_INVALID);
    va_list argp;
    va_start(argp, signature);
    QStatus status = ER_OK;

    ((ajn::MsgArgC*)arg)->Clear();
    size_t sigLen = (signature ? strlen(signature) : 0);
    if ((sigLen < 1) || (sigLen > 255)) {
        status = ER_BUS_BAD_SIGNATURE;
    } else {
        status = ((ajn::MsgArgC*)arg)->VBuildArgsC(signature, sigLen, ((ajn::MsgArgC*)arg), 1, &argp);
        if ((status == ER_OK) && (*signature != 0)) {
            status = ER_BUS_NOT_A_COMPLETE_TYPE;
        }
    }
    va_end(argp);
    return (alljoyn_msgarg)arg;
}

void alljoyn_msgarg_destroy(alljoyn_msgarg arg) {
    if (arg != NULL) {
        delete (ajn::MsgArgC*)arg;
    }
}

QStatus alljoyn_msgarg_set(alljoyn_msgarg arg, const char* signature, ...) {
    if (arg != NULL) {
        va_list argp;
        va_start(argp, signature);
        QStatus status = ER_OK;

        ((ajn::MsgArgC*)arg)->Clear();
        size_t sigLen = (signature ? strlen(signature) : 0);
        if ((sigLen < 1) || (sigLen > 255)) {
            status = ER_BUS_BAD_SIGNATURE;
        } else {
            status = ((ajn::MsgArgC*)arg)->VBuildArgsC(signature, sigLen, ((ajn::MsgArgC*)arg), 1, &argp);
            if ((status == ER_OK) && (*signature != 0)) {
                status = ER_BUS_NOT_A_COMPLETE_TYPE;
            }
        }
        va_end(argp);
        return status;
    } else {
        return ER_BAD_ARG_1;
    }
}

QStatus alljoyn_msgarg_get(alljoyn_msgarg arg, const char* signature, ...) {
    if (arg != NULL) {
        size_t sigLen = (signature ? strlen(signature) : 0);
        if (sigLen == 0) {
            return ER_BAD_ARG_2;
        }
        va_list argp;
        va_start(argp, signature);
        QStatus status = ((ajn::MsgArgC*)arg)->VParseArgsC(signature, sigLen, ((ajn::MsgArgC*)arg), 1, &argp);
        va_end(argp);
        return status;
    } else {
        return ER_BAD_ARG_1;
    }
}


QStatus alljoyn_msgarg_set_array(alljoyn_msgarg* args, size_t* numArgs, const char* signature, ...) {
    if (!args) {
        return ER_BAD_ARG_1;
    }
    va_list argp;
    va_start(argp, signature);
    QStatus status = ((ajn::MsgArgC*)args)->MsgArgUtilsSetVC(((ajn::MsgArgC*)args), *numArgs, signature, &argp);
    va_end(argp);
    return status;
}

QStatus alljoyn_msgarg_get_array(const alljoyn_msgarg* args, size_t numArgs, const char* signature, ...) {
    if (!args) {
        return ER_BAD_ARG_1;
    }
    if (!numArgs) {
        return ER_BAD_ARG_2;
    }
    size_t sigLen = (signature ? strlen(signature) : 0);
    if (sigLen == 0) {
        return ER_BAD_ARG_3;
    }
    va_list argp;
    va_start(argp, signature);
    QStatus status = ((ajn::MsgArgC*)args)->VParseArgsC(signature, sigLen, ((ajn::MsgArgC*)args), numArgs, &argp);
    va_end(argp);
    return status;
}

const char* alljoyn_msgarg_tostring(alljoyn_msgarg arg, size_t indent)
{
    return ((ajn::MsgArgC*)arg)->ToString(indent).c_str();
}

/*******************************************************************************
 * work with the alljoyn_msgargs (NOTE plural MsgArg).  This set of functions
 * were designed to work with an array of MsgArgs not a single MsgArg for this
 * reason it does not properly map with the C++ MsgArg Class.  These calls are
 * being left in here till proper mapping between 'C' and the 'C++' can be
 * completed. And can be verified that the code continues to work with other
 * existing code bindings.
 ******************************************************************************/

struct _alljoyn_msgargs_handle {
    /* Empty by design, this is just to allow the type restrictions to save coders from themselves */
};

alljoyn_msgargs alljoyn_msgargs_create(size_t numArgs)
{
    ajn::MsgArgC* args = new ajn::MsgArgC[numArgs];
    for (size_t i = 0; i < numArgs; i++) {
        args[i].Clear();
    }
    return (alljoyn_msgargs)args;
}

void alljoyn_msgargs_destroy(alljoyn_msgargs arg)
{
    assert(arg != NULL && "NULL argument passed to alljoyn_msgarg_destroy.");
    delete [] (ajn::MsgArgC*)arg;
}

QStatus alljoyn_msgargs_set(alljoyn_msgargs args, size_t argOffset, size_t* numArgs, const char* signature, ...)
{
    va_list argp;
    va_start(argp, signature);
    QStatus status = ((ajn::MsgArgC*)args)->MsgArgUtilsSetVC(((ajn::MsgArgC*)args) + argOffset, *numArgs, signature, &argp);
    va_end(argp);
    return status;
}

#define _IMPLEMENT_MSGARG_TYPE_ACCESSOR(rt, nt, mt) \
    rt alljoyn_msgargs_as_ ## nt(const alljoyn_msgargs args, size_t idx) \
    { \
        return ((ajn::MsgArgC*)args)[idx].mt; \
    }
#define _IMPLEMENT_MSGARG_TYPE_ACCESSOR_S(t) _IMPLEMENT_MSGARG_TYPE_ACCESSOR(t ## _t, t, v_ ## t)

_IMPLEMENT_MSGARG_TYPE_ACCESSOR_S(int16);
_IMPLEMENT_MSGARG_TYPE_ACCESSOR_S(uint16);
_IMPLEMENT_MSGARG_TYPE_ACCESSOR_S(int32);
_IMPLEMENT_MSGARG_TYPE_ACCESSOR_S(uint32);
_IMPLEMENT_MSGARG_TYPE_ACCESSOR_S(int64);
_IMPLEMENT_MSGARG_TYPE_ACCESSOR_S(uint64);

_IMPLEMENT_MSGARG_TYPE_ACCESSOR(uint8_t, uint8_t, v_byte);
_IMPLEMENT_MSGARG_TYPE_ACCESSOR(QC_BOOL, bool, v_bool);
_IMPLEMENT_MSGARG_TYPE_ACCESSOR(double, double, v_double);

#undef _IMPLEMENT_MSGARG_TYPE_ACCESSOR
#undef _IMPLEMENT_MSGARG_TYPE_ACCESSOR_S

const char* alljoyn_msgargs_as_string(const alljoyn_msgargs args, size_t idx)
{
    return ((ajn::MsgArgC*)args)[idx].v_string.str;
}

const char* alljoyn_msgargs_as_objpath(const alljoyn_msgargs args, size_t idx)
{
    return ((ajn::MsgArgC*)args)[idx].v_objPath.str;
}

void alljoyn_msgargs_as_signature(const alljoyn_msgargs args, size_t idx,
                                  uint8_t* out_len, const char** out_sig)
{
    *out_len = ((ajn::MsgArgC*)args)[idx].v_signature.len;
    *out_sig = ((ajn::MsgArgC*)args)[idx].v_signature.sig;
}

void alljoyn_msgargs_as_handle(const alljoyn_msgargs args, size_t idx, void** out_socketFd)
{
    *out_socketFd = &((ajn::MsgArgC*)args)[idx].v_handle.fd;
}

const alljoyn_msgargs alljoyn_msgargs_as_array(const alljoyn_msgargs args, size_t idx,
                                               size_t* out_len, const char** out_sig)
{
    *out_len = ((ajn::MsgArgC*)args)[idx].v_array.GetNumElements();
    *out_sig = ((ajn::MsgArgC*)args)[idx].v_array.GetElemSig();
    return (const alljoyn_msgargs)(((ajn::MsgArgC*)args)[idx].v_array.GetElements());
}

alljoyn_msgargs alljoyn_msgargs_as_struct(const alljoyn_msgargs args, size_t idx,
                                          size_t* out_numMembers)
{
    *out_numMembers = ((ajn::MsgArgC*)args)[idx].v_struct.numMembers;
    return (alljoyn_msgargs)(((ajn::MsgArgC*)args)[idx].v_struct.members);
}

void alljoyn_msgargs_as_dictentry(const alljoyn_msgargs args, size_t idx,
                                  alljoyn_msgargs* out_key, alljoyn_msgargs* out_val)
{
    *out_key = (alljoyn_msgargs)((ajn::MsgArgC*)args)[idx].v_dictEntry.key;
    *out_val = (alljoyn_msgargs)((ajn::MsgArgC*)args)[idx].v_dictEntry.val;
}

alljoyn_msgargs alljoyn_msgargs_as_variant(const alljoyn_msgargs args, size_t idx)
{
    return (alljoyn_msgargs)((ajn::MsgArgC*)args)[idx].v_variant.val;
}

void alljoyn_msgargs_as_scalararray(const alljoyn_msgargs args, size_t idx,
                                    size_t* out_numElements, const void** out_elements)
{
    *out_numElements = ((ajn::MsgArgC*)args)[idx].v_scalarArray.numElements;
    *out_elements = ((ajn::MsgArgC*)args)[idx].v_scalarArray.v_byte;
}
