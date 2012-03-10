#ifndef _ALLJOYN_UNITY_MSGARG_H
#define _ALLJOYN_UNITY_MSGARG_H
/**
 * @file
 * This file defines a class for message bus data types and values
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
#include <alljoyn_c/AjAPI.h>
#include <stdarg.h>
#include <Status.h>

#ifdef __cplusplus
extern "C" {
#endif
typedef struct _alljoyn_msgarg_handle* alljoyn_msgarg;

/**
 * Create a new message argument.
 *
 * @return the created message argument
 */
extern AJ_API alljoyn_msgarg alljoyn_msgarg_create();

/**
 * Build a message argument that has its value already set. If the setting the
 * message argument value fails for any reason the message argument type will be
 * set to #ALLJOYN_INVALID. See the description of the #alljoyn_msgarg_set() function
 * for information about the signature and parameters. For initializing complex
 * values it is recommended to use the alljoyn_msgarg_create and alljoyn_msgarg_set
 * functions so the success of setting the value can be explicitly checked.
 *
 * @param signature   The signature for MsgArg value.
 * @param ...         One or more values to initialize the MsgArg.
 */
extern AJ_API alljoyn_msgarg alljoyn_msgarg_create_and_set(const char* signature, ...);

/**
 * Destroy a message argument.
 *
 * @param arg The message argument to destroy.
 */
extern AJ_API void alljoyn_msgarg_destroy(alljoyn_msgarg arg);


/**
 * Set value of a message arg from a signature and a list of values. Note that any values or
 * MsgArg pointers passed in must remain valid until this MsgArg is freed.
 *
 *  - @c 'a'  The array length followed by:
 *            - If the element type is a basic type a pointer to an array of values of that type.
 *            - If the element type is string a pointer to array of const char*, if array length is
 *              non-zero, and the char* pointer is NULL, the NULL must be followed by a pointer to
 *              an array of const qcc::String.
 *            - If the element type is an @ref ALLJOYN_ARRAY "ARRAY", @ref ALLJOYN_STRUCT "STRUCT",
 *              @ref ALLJOYN_DICT_ENTRY "DICT_ENTRY" or @ref ALLJOYN_VARIANT "VARIANT" a pointer to an
 *              array of MsgArgs where each MsgArg has the signature specified by the element type.
 *            - If the element type is specified using the wildcard character '*', a pointer to
 *              an  array of MsgArgs. The array element type is determined from the type of the
 *              first MsgArg in the array, all the elements must have the same type.
 *  - @c 'b'  A bool value
 *  - @c 'd'  A double (64 bits)
 *  - @c 'g'  A pointer to a NUL terminated string (pointer must remain valid for lifetime of the MsgArg)
 *  - @c 'h'  A qcc::SocketFd
 *  - @c 'i'  An int (32 bits)
 *  - @c 'n'  An int (16 bits)
 *  - @c 'o'  A pointer to a NUL terminated string (pointer must remain valid for lifetime of the MsgArg)
 *  - @c 'q'  A uint (16 bits)
 *  - @c 's'  A pointer to a NUL terminated string (pointer must remain valid for lifetime of the MsgArg)
 *  - @c 't'  A uint (64 bits)
 *  - @c 'u'  A uint (32 bits)
 *  - @c 'v'  Not allowed, the actual type must be provided.
 *  - @c 'x'  An int (64 bits)
 *  - @c 'y'  A byte (8 bits)
 *
 *  - @c '(' and @c ')'  The list of values that appear between the parentheses using the notation above
 *  - @c '{' and @c '}'  A pair values using the notation above.
 *
 *  - @c '*'  A pointer to a MsgArg.
 *
 * Examples:
 *
 * An array of strings
 *
 *     @code
 *     char* fruits[3] =  { "apple", "banana", "orange" };
 *     alljoyn_msgarg bowl;
 *     bowl = alljoyn_msgarg_create();
 *     alljoyn_msgarg_set(bowl, "as", 3, fruits);
 *     @endcode
 *
 * A struct with a uint and two string elements.
 *
 *     @code alljoyn_msgarg_set(arg, "(uss)", 1024, "hello", "world"); @endcode
 *
 * An array of 3 dictionary entries where each entry has an integer key and string value.
 *
 *     @code
 *     MsgArg dict[3];
 *     dict[0].Set("{is}", 1, "red");
 *     dict[1].Set("{is}", 2, "green");
 *     dict[2].Set("{is}", 3, "blue");
 *     arg.Set("a{is}", 3, dict);
 *     @endcode
 *
 * An array of uint_16's
 *
 *     @code
 *     uint16_t aq[] = { 1, 2, 3, 5, 6, 7 };
 *     arg.Set("aq", sizeof(aq) / sizeof(uint16_t), aq);
 *     @endcode
 *
 * @param arg         The alljoyn_msgarg being set
 * @param signature   The signature for MsgArg value
 * @param ...         One or more values to initialize the MsgArg.
 *
 * @return
 *      - #ER_OK if the MsgArg was successfully set
 *      - An error status otherwise
 */
extern AJ_API QStatus alljoyn_msgarg_set(alljoyn_msgarg arg, const char* signature, ...);

/**
 * Matches a signature to the MsArg and if the signature matches unpacks the component values of a MsgArg. Note that the values
 * returned are references into the MsgArg itself so unless copied will become invalid if the MsgArg is freed or goes out of scope.
 * This function resolved through variants, so if the MsgArg is a variant that references a 32 bit integer is can be unpacked
 * directly into a 32 bit integer pointer.
 *
 *  - @c 'a'  A pointer to a length of type size_t that returns the number of elements in the array followed by:
 *            - If the element type is a scalar type a pointer to a pointer of the correct type for the values.
 *            - Otherwise a pointer to a pointer to a MsgArg.
 *
 *  - @c 'b'  A pointer to a bool
 *  - @c 'd'  A pointer to a double (64 bits)
 *  - @c 'g'  A pointer to a char*  (character string is valid for the lifetime of the MsgArg)
 *  - @c 'h'  A pointer to a qcc::SocketFd
 *  - @c 'i'  A pointer to a uint16_t
 *  - @c 'n'  A pointer to an int16_t
 *  - @c 'o'  A pointer to a char*  (character string is valid for the lifetime of the MsgArg)
 *  - @c 'q'  A pointer to a uint16_t
 *  - @c 's'  A pointer to a char*  (character string is valid for the lifetime of the MsgArg)
 *  - @c 't'  A pointer to a uint64_t
 *  - @c 'u'  A pointer to a uint32_t
 *  - @c 'v'  A pointer to a pointer to a MsgArg, matches to a variant but returns a pointer to
 *            the MsgArg of the underlying real type.
 *  - @c 'x'  A pointer to an int64_t
 *  - @c 'y'  A pointer to a uint8_t
 *
 *  - @c '(' and @c ')'  A list of pointers as required for each of the struct members.
 *  - @c '{' and @c '}'  Pointers as required for the key and value members.
 *
 *  - @c '*' A pointer to a pointer to a MsgArg. This matches any value type.
 *
 * Examples:
 *
 * A struct with and uint32 and two string elements.
 *
 *     @code
 *     struct {
 *         uint32_t i;
 *         char *hello;
 *         char *world;
 *     } myStruct;
 *     arg.Get("(uss)", &myStruct.i, &myStruct.hello, &myStruct.world);
 *     @endcode
 *
 * A variant where it is known that the value is a uint32, a string, or double. Note that the
 * variant is resolved away.
 *
 *     @code
 *     uint32_t i;
 *     double d;
 *     char *str;
 *     QStatus status = arg.Get("i", &i);
 *     if (status == ER_BUS_SIGNATURE_MISMATCH) {
 *         status = arg.Get("s", &str);
 *         if (status == ER_BUS_SIGNATURE_MISMATCH) {
 *             status = arg.Get("d", &d);
 *         }
 *     }
 *     @endcode
 *
 * An array of dictionary entries where each entry has an integer key and variant. Find the
 * entries where the variant value is a string or a struct of 2 strings.
 *
 *     @code
 *     MsgArg *entries;
 *     size_t num;
 *     arg.Get("a{iv}", &num, &entries);
 *     for (size_t i = 0; i > num; ++i) {
 *         char *str1;
 *         char *str2;
 *         uint32_t key;
 *         status = entries[i].Get("{is}", &key, &str1);
 *         if (status == ER_BUS_SIGNATURE_MISMATCH) {
 *             status = entries[i].Get("{i(ss)}", &key, &str1, &str2);
 *         }
 *     }
 *     @endcode
 *
 * An array of uint_16's
 *
 *     @code
 *     uint16_t *vals;
 *     size_t numVals;
 *     arg.Get("aq", &numVals, &vals);
 *     @endcode
 *
 * @param get         The alljoyn_msgarg we are reading reading from
 * @param signature   The signature for MsgArg value
 * @param ...         Pointers to return the unpacked values.
 *
 * @return
 *      - #ER_OK if the signature matched and MsgArg was successfully unpacked.
 *      - #ER_BUS_SIGNATURE_MISMATCH if the signature did not match.
 *      - An error status otherwise
 */
extern AJ_API QStatus alljoyn_msgarg_get(alljoyn_msgarg arg, const char* signature, ...);

/**
 * Set an array of MsgArgs by applying the alljoyn_msgarg_set() function to each MsgArg in turn.
 *
 * @param args     An array of MsgArgs to set.
 * @param numArgs  [in,out] On input the size of the args array. On output the number of MsgArgs
 *                 that were set. There must be at least enough MsgArgs to completely
 *                 initialize the signature.
 *
 * @param signature   The signature for MsgArg values
 * @param ...         One or more values to initialize the MsgArg list.
 *
 * @return
 *       - #ER_OK if the MsgArgs were successfully set.
 *       - #ER_BUS_TRUNCATED if the signature was longer than expected.
 *       - Other error status codes indicating a failure.
 */
extern AJ_API QStatus alljoyn_msgarg_set_array(alljoyn_msgarg* args, size_t* numArgs, const char* signature, ...);

/**
 * Unpack an array of MsgArgs by applying the alljoyn_msgarg_get() function to each MsgArg in turn.
 *
 * @param args       An array of MsgArgs to unpack.
 * @param numArgs    The size of the MsgArgs array.
 * @param signature  The signature to match against the MsgArg values
 * @param ...         Pointers to return references to the unpacked values.
 *
 * @return
 *      - #ER_OK if the MsgArgs were successfully set.
 *      - #ER_BUS_SIGNATURE_MISMATCH if the signature did not match.
 *      - Other error status codes indicating a failure.
 */
extern AJ_API QStatus alljoyn_msgarg_get_array(const alljoyn_msgarg* args, size_t numArgs, const char* signature, ...);

/**
 * Returns an XML string representation of this type
 *
 * @param indent  Number of spaces to indent the generated xml
 *
 * @return  The XML string
 */
extern AJ_API const char* alljoyn_msgarg_tostring(alljoyn_msgarg arg, size_t indent);


#if 0
/**
 * Returns a string for the signature of this value
 *
 * @return The signature string for this MsgArg
 */
qcc::String Signature() const {
    return Signature(this, 1);
}

/**
 * Returns a string representation of the signature of an array of message args.
 *
 * @param values     A pointer to an array of message arg values
 * @param numValues  Length of the array
 *
 * @return The signature string for the message args.
 */
static qcc::String Signature(const MsgArg* values, size_t numValues);

/**
 * Returns an XML string representation of this type
 *
 * @param indent  Number of spaces to indent the generated xml
 *
 * @return  The XML string
 */
qcc::String ToString(size_t indent = 0) const;

/**
 * Returns an XML string representation for an array of message args.
 *
 * @param args     The message arg array.
 * @param numArgs  The size of the message arg array.
 * @param indent   Number of spaces to indent the generated xml
 *
 * @return The XML string representation of the message args.
 */
static qcc::String ToString(const MsgArg* args, size_t numArgs, size_t indent = 0);

/**
 * Checks the signature of this arg.
 *
 * @param signature  The signature to check
 *
 * @return  true if this arg has the specified signature, otherwise returns false.
 */
bool HasSignature(const char* signature) const;

/**
 * Assignment operator
 *
 * @param other  The source MsgArg for the assignment
 *
 * @return  The assigned MsgArg
 */
MsgArg& operator=(const MsgArg& other) {
    if (this != &other) {
        Clone(*this, other);
    }
    return *this;
}

/**
 * Copy constructor
 *
 * @param other  The source MsgArg for the copy
 */
MsgArg(const MsgArg &other) : typeId(ALLJOYN_INVALID) {
    Clone(*this, other);
}

/**
 * Destructor
 */
~MsgArg() { Clear(); }

/**
 * Constructor
 *
 * @param typeId  The type for the MsgArg
 */
MsgArg(AllJoynTypeId typeId) : typeId(typeId), flags(0) {
    v_invalid.unused[0] = v_invalid.unused[1] = v_invalid.unused[2] = NULL;
}

/**
 * Constructor to build a message arg. If the constructor fails for any reason the type will be
 * set to #ALLJOYN_INVALID. See the description of the #Set() method for information about the
 * signature and parameters. For initializing complex values it is recommended to use the
 * default constructor and the #Set() method so the success of setting the value can be
 * explicitly checked.
 *
 * @param signature   The signature for MsgArg value.
 * @param ...         One or more values to initialize the MsgArg.
 */
MsgArg(const char* signature, ...);

/**
 * Unpack an array of MsgArgs by applying the Get() method to each MsgArg in turn.
 *
 * @param args       An array of MsgArgs to unpack.
 * @param numArgs    The size of the MsgArgs array.
 * @param signature  The signature to match against the MsgArg values
 * @param ...         Pointers to return references to the unpacked values.
 *
 * @return
 *      - #ER_OK if the MsgArgs were successfully set.
 *      - #ER_BUS_SIGNATURE_MISMATCH if the signature did not match.
 *      - Other error status codes indicating a failure.
 */
static QStatus Get(const MsgArg* args, size_t numArgs, const char* signature, ...);

/**
 * Helper function for accessing dictionary elements. The MsgArg must be an array of dictionary
 * elements. The second parameter is the key value, this is expressed according to the rules for
 * MsgArg::Set so is either a scalar, a pointer to a string, or for 64 bit values a pointer to
 * the value. This value is matched against the dictionary array to locate the matching element.
 * The third and subsequent parameters are unpacked according to the rules of MsgArg::Get.
 *
 * For example, where the key is a string and the values are structs:
 *
 *     @code
 *     uint8_t age;
 *     uint32_t height;
 *     const char* address;
 *     QStatus status = arg.GetElement("{s(yus)}", "fred", &age, &height,  &address);
 *     @endcode
 *
 * This function is particularly useful for extracting specific properties from the array of property
 * values returned by ProxyBusObject::GetAllProperties.
 *
 * @param elemSig  The expected signature for the dictionary element, e.g. "{su}"
 * @param ...      Pointers to return unpacked key values.
 *
 * @return
 *      - #ER_OK if the dictionary signature matched and MsgArg was successfully unpacked.
 *      - #ER_BUS_NOT_A_DICTIONARY if this method is called on a MsgArg that is not a dictionary.
 *      - #ER_BUS_SIGNATURE_MISMATCH if the signature did not match.
 *      - #ER_BUS_ELEMENT_NOT_FOUND if the key was not found in the dictionary.
 *      - An error status otherwise
 */
QStatus GetElement(const char* elemSig, ...) const;

/**
 * Equality operator.
 *
 * @param other  The other MsgArg to compare.
 *
 * @return  Returns true if the two message args have the same signatures and values.
 */
bool operator==(const MsgArg& other);

/**
 * Inequality operator.
 *
 * @param other  The other MsgArg to compare.
 *
 * @return  Returns true if the two message args do not have the same signatures and values.
 */
bool operator!=(const MsgArg& other) {
    return !(*this == other);
}

/**
 * Clear the MsgArg setting the type to ALLJOYN_INVALID and freeing any memory allocated for the
 * MsgArg value.
 */
void Clear();

/**
 * Makes a MsgArg stable by completely copying the contents into locally
 * managed memory. After a MsgArg has been stabilized any values used to
 * initialize or set the message arg can be freed.
 */
void Stabilize();

/**
 * This method sets the ownership flags on this MsgArg, and optionally all
 * MsgArgs subordinate to this MsgArg. By setting the ownership flags the
 * caller can transfer responsibility for freeing nested data referenced
 * by this MsgArg to the MsgArg's destructor. The #OwnsArgs flag is
 * particularly useful for managing complex data structures such as arrays
 * of structs, nested structs, and variants where the inner MsgArgs are
 * dynamically allocated. The #OwnsData flag is useful for freeing
 * dynamically allocated strings, byte arrays, etc,.
 *
 * @param flags  A logical or of the applicable ownership flags (OwnsArgs and OwnsData).
 * @param deep   If true recursively sets the ownership flags on all MsgArgs owned by this MsgArg.
 */
void SetOwnershipFlags(uint8_t flags, bool deep = false) {
    this->flags |= (flags & (OwnsData | OwnsArgs)); if (deep) {
        SetOwnershipDeep();
    }
}
#endif


/*******************************************************************************
 * work with the alljoyn_msgargs (NOTE plural MsgArg).  This set of functions
 * were designed to work with an array of MsgArgs not a single MsgArg for this
 * reason it does not properly map with the C++ MsgArg Class.  These calls are
 * being left in here till proper mapping between 'C' and the 'C++' can be
 * completed. And can be verified that the code continues to work with other
 * existing code bindings.
 ******************************************************************************/
typedef struct _alljoyn_msgargs_handle*                     alljoyn_msgargs;

/**
 * Create a new message argument array.
 *
 * @param numArgs Number of arguments to create in the array.
 */
extern AJ_API alljoyn_msgargs alljoyn_msgargs_create(size_t numArgs);

/**
 * Destroy a message argument.
 *
 * @param arg The message argument to destroy.
 */
extern AJ_API void alljoyn_msgargs_destroy(alljoyn_msgargs arg);

/**
 * Set an array of MsgArgs by applying the Set() method to each MsgArg in turn.
 *
 * @param args        An array of MsgArgs to set.
 * @param argOffset   Offset from the start of the MsgArg array.
 * @param numArgs     [in,out] On input the number of args to set. On output the number of MsgArgs
 *                    that were set. There must be at least enought MsgArgs to completely
 *                    initialize the signature.
 *                    there should at least enough.
 * @param signature   The signature for MsgArg values
 * @param ...         One or more values to initialize the MsgArg list.
 *
 * @return
 *       - #ER_OK if the MsgArgs were successfully set.
 *       - #ER_BUS_TRUNCATED if the signature was longer than expected.
 *       - Other error status codes indicating a failure.
 */
extern AJ_API QStatus alljoyn_msgargs_set(alljoyn_msgargs args, size_t argOffset, size_t* numArgs, const char* signature, ...);

extern AJ_API uint8_t alljoyn_msgargs_as_uint8(const alljoyn_msgargs args, size_t idx);
extern AJ_API QC_BOOL alljoyn_msgargs_as_bool(const alljoyn_msgargs args, size_t idx);
extern AJ_API int16_t alljoyn_msgargs_as_int16(const alljoyn_msgargs args, size_t idx);
extern AJ_API uint16_t alljoyn_msgargs_as_uint16(const alljoyn_msgargs args, size_t idx);
extern AJ_API int32_t alljoyn_msgargs_as_int32(const alljoyn_msgargs args, size_t idx);
extern AJ_API uint32_t alljoyn_msgargs_as_uint32(const alljoyn_msgargs args, size_t idx);
extern AJ_API int64_t alljoyn_msgargs_as_int64(const alljoyn_msgargs args, size_t idx);
extern AJ_API uint64_t alljoyn_msgargs_as_uint64(const alljoyn_msgargs args, size_t idx);
extern AJ_API double alljoyn_msgargs_as_double(const alljoyn_msgargs args, size_t idx);
extern AJ_API const char* alljoyn_msgargs_as_string(const alljoyn_msgargs args, size_t idx);
extern AJ_API const char* alljoyn_msgargs_as_objpath(const alljoyn_msgargs args, size_t idx);
extern AJ_API alljoyn_msgargs alljoyn_msgargs_as_variant(const alljoyn_msgargs args, size_t idx);
extern AJ_API void alljoyn_msgargs_as_signature(const alljoyn_msgargs args, size_t idx,
                                                uint8_t* out_len, const char** out_sig);
extern AJ_API void alljoyn_msgargs_as_handle(const alljoyn_msgargs args, size_t idx,
                                             void** out_socketFd);
extern AJ_API const alljoyn_msgargs alljoyn_msgargs_as_array(const alljoyn_msgargs args, size_t idx,
                                                             size_t* out_len, const char** out_sig);
extern AJ_API alljoyn_msgargs alljoyn_msgargs_as_struct(const alljoyn_msgargs args, size_t idx,
                                                        size_t* out_numMembers);
extern AJ_API void alljoyn_msgargs_as_dictentry(const alljoyn_msgargs args, size_t idx,
                                                alljoyn_msgargs* out_key, alljoyn_msgargs* out_val);
extern AJ_API void alljoyn_msgargs_as_scalararray(const alljoyn_msgargs args, size_t idx,
                                                  size_t* out_numElements, const void** out_elements);
#ifdef __cplusplus
} /* extern "C" */
#endif

#endif
