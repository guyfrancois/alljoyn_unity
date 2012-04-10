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
#ifndef _ALLJOYN_C_MSGARG_H
#define _ALLJOYN_C_MSGARG_H

#include <qcc/platform.h>
#include <alljoyn_c/AjAPI.h>
#include <stdarg.h>
#include <Status.h>

#ifdef __cplusplus
extern "C" {
#endif
typedef struct _alljoyn_msgarg_handle* alljoyn_msgarg;
//typedef struct _alljoyn_msgarg_array_handle* alljoyn_msgarg_array;


typedef enum {
    ALLJOYN_INVALID          =  0,     ///< AllJoyn INVALID typeId
    ALLJOYN_ARRAY            = 'a',    ///< AllJoyn array container type
    ALLJOYN_BOOLEAN          = 'b',    ///< AllJoyn boolean basic type, @c 0 is @c FALSE and @c 1 is @c TRUE - Everything else is invalid
    ALLJOYN_DOUBLE           = 'd',    ///< AllJoyn IEEE 754 double basic type
    ALLJOYN_DICT_ENTRY       = 'e',    ///< AllJoyn dictionary or map container type - an array of key-value pairs
    ALLJOYN_SIGNATURE        = 'g',    ///< AllJoyn signature basic type
    ALLJOYN_HANDLE           = 'h',    ///< AllJoyn socket handle basic type
    ALLJOYN_INT32            = 'i',    ///< AllJoyn 32-bit signed integer basic type
    ALLJOYN_INT16            = 'n',    ///< AllJoyn 16-bit signed integer basic type
    ALLJOYN_OBJECT_PATH      = 'o',    ///< AllJoyn Name of an AllJoyn object instance basic type
    ALLJOYN_UINT16           = 'q',    ///< AllJoyn 16-bit unsigned integer basic type
    ALLJOYN_STRUCT           = 'r',    ///< AllJoyn struct container type
    ALLJOYN_STRING           = 's',    ///< AllJoyn UTF-8 NULL terminated string basic type
    ALLJOYN_UINT64           = 't',    ///< AllJoyn 64-bit unsigned integer basic type
    ALLJOYN_UINT32           = 'u',    ///< AllJoyn 32-bit unsigned integer basic type
    ALLJOYN_VARIANT          = 'v',    ///< AllJoyn variant container type
    ALLJOYN_INT64            = 'x',    ///< AllJoyn 64-bit signed integer basic type
    ALLJOYN_BYTE             = 'y',    ///< AllJoyn 8-bit unsigned integer basic type

    ALLJOYN_STRUCT_OPEN      = '(', /**< Never actually used as a typeId: specified as ALLJOYN_STRUCT */
    ALLJOYN_STRUCT_CLOSE     = ')', /**< Never actually used as a typeId: specified as ALLJOYN_STRUCT */
    ALLJOYN_DICT_ENTRY_OPEN  = '{', /**< Never actually used as a typeId: specified as ALLJOYN_DICT_ENTRY */
    ALLJOYN_DICT_ENTRY_CLOSE = '}', /**< Never actually used as a typeId: specified as ALLJOYN_DICT_ENTRY */

    ALLJOYN_BOOLEAN_ARRAY    = ('b' << 8) | 'a',   ///< AllJoyn array of booleans
    ALLJOYN_DOUBLE_ARRAY     = ('d' << 8) | 'a',   ///< AllJoyn array of IEEE 754 doubles
    ALLJOYN_INT32_ARRAY      = ('i' << 8) | 'a',   ///< AllJoyn array of 32-bit signed integers
    ALLJOYN_INT16_ARRAY      = ('n' << 8) | 'a',   ///< AllJoyn array of 16-bit signed integers
    ALLJOYN_UINT16_ARRAY     = ('q' << 8) | 'a',   ///< AllJoyn array of 16-bit unsigned integers
    ALLJOYN_UINT64_ARRAY     = ('t' << 8) | 'a',   ///< AllJoyn array of 64-bit unsigned integers
    ALLJOYN_UINT32_ARRAY     = ('u' << 8) | 'a',   ///< AllJoyn array of 32-bit unsigned integers
    ALLJOYN_INT64_ARRAY      = ('x' << 8) | 'a',   ///< AllJoyn array of 64-bit signed integers
    ALLJOYN_BYTE_ARRAY       = ('y' << 8) | 'a',   ///< AllJoyn array of 8-bit unsigned integers

    ALLJOYN_WILDCARD         = '*'     ///< This never appears in a signature but is used for matching arbitrary message args

} AllJoynTypeId;

/**
 * Create a new message argument.
 * calling alljoyn_msgarg_create() is the same as calling
 * alljoyn_msgarg_array_create(1).
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
 * Create an array of message arguments
 *
 * This returns and alljoyn_msgarg however the individual array elements can only
 * be accessed using alljoyn_msgarg_array_element() function.
 *
 * the alljoyn_msgarg returned can be used in any functions that states says
 * alljoyn_msgarg_array_*.  and alljoyn_msgarg created using alljoyn_msgarg_create()
 * is an array of message arguments with size of 1.
 *
 * If the function does not specifically say it is for an array it is assumed that
 * the message argument was created using alljoyn_msgarg_create() and will be treated
 * like an array with only one element.
 *
 * For example the following code would only copy the first msgarg in the array.
 * An alljoyn_msgarg of the string "hello". Not both array elements.
 *
 * @code
 * alljoyn_msgarg args = alljoyn_msgarg_array_create(2);
 * alljoyn_msgarg_array_set(args, 2, "ss", "hello", "world");
 * alljoyn_msgarg arg = alljoyn_msgarg_copy(args);
 * @endcode
 *
 * To make a copy of a alljoyn_msgarg that contains an array of elements requires
 * a for loop.
 *
 * @code
 * alljoyn_msgarg args = alljoyn_msgarg_array_create(2);
 * alljoyn_msgarg_array_set(args, 2, "ss", "hello", "world");
 * alljoyn_msgarg cp_args = alljoyn_msgarg_array_create(2);
 * for (size_t i = 0; i < 2; ++i) {
 *     alljoyn_msgarg_array_element(cp_args, i) = alljoyn_msgarg_copy(alljoyn_msgarg_array_element(args, i));
 * }
 * @endcode
 *
 * @return the created array of message arguments
 */
extern AJ_API alljoyn_msgarg alljoyn_msgarg_array_create(size_t size);

/**
 * Destroy a message argument array
 *
 * @param arg the message argument array to destory
 */
//extern AJ_API void  alljoyn_msgarg_array_destroy(alljoyn_msgarg_array arg);

/*
 * when working with an array of message arguments this will return the nth item
 * in the array.
 * @param arg   the alljoyn_msgarg that contains an array of msgargs
 * @param index the index number of the element we wish to access.
 */
extern AJ_API alljoyn_msgarg alljoyn_msgarg_array_element(alljoyn_msgarg arg, size_t index);

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
 *     alljoyn_msgarg dict = alljoyn_msgarg_array_create(3)
 *     alljoyn_msgarg_set(alljoyn_msgarg_array_element(0), "{is}", 1, "red");
 *     alljoyn_msgarg_set(alljoyn_msgarg_array_element(1), "{is}", 2, "green");
 *     alljoyn_msgarg_set(alljoyn_msgarg_array_element(2), "{is}", 3, "blue");
 *     alljoyn_msgarg arg = alljoyn_msgarg_create();
 *     alljoyn_msgarg_set(arg, "a{is}", 3, dict);
 *     @endcode
 *
 * An array of uint_16's
 *
 *     @code
 *     uint16_t aq[] = { 1, 2, 3, 5, 6, 7 };
 *     alljoyn_msgarg_set(arg, "aq", sizeof(aq) / sizeof(uint16_t), aq);
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
 *     alljoyn_msgarg_get(arg, "(uss)", &myStruct.i, &myStruct.hello, &myStruct.world);
 *     @endcode
 *
 * A variant where it is known that the value is a uint32, a string, or double. Note that the
 * variant is resolved away.
 *
 *     @code
 *     uint32_t i;
 *     double d;
 *     char *str;
 *     QStatus status = alljoyn_msgarg_get(arg, "i", &i);
 *     if (status == ER_BUS_SIGNATURE_MISMATCH) {
 *         status = alljoyn_msgarg_get(arg, "s", &str);
 *         if (status == ER_BUS_SIGNATURE_MISMATCH) {
 *             status = alljoyn_msgarg_get(arg, "d", &d);
 *         }
 *     }
 *     @endcode
 *
 * An array of dictionary entries where each entry has an integer key and variant. Find the
 * entries where the variant value is a string or a struct of 2 strings.
 *
 *     @code
 *     alljoyn_msgarg entries;
 *     size_t num;
 *     alljoyn_msgarg_get(arg, "a{iv}", &num, &entries);
 *     for (size_t i = 0; i < num; ++i) {
 *         char *str1;
 *         char *str2;
 *         uint32_t key;
 *         status = alljoyn_msgarg_get(alljoyn_msgarg_array_element(entries, i), "{is}", &key, &str1);
 *         if (status == ER_BUS_SIGNATURE_MISMATCH) {
 *             status = alljoyn_msgarg_get(alljoyn_msgarg_array_element(entries, i), "{i(ss)}", &key, &str1, &str2);
 *         }
 *     }
 *     @endcode
 *
 * An array of uint_16's
 *
 *     @code
 *     uint16_t *vals;
 *     size_t numVals;
 *     alljoyn_msgarg_get(arg, "aq", &numVals, &vals);
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
 * create a copy of a message argument.  This will create a new alljoyn_msgarg and
 * must be cleaned up using alljoyn_msgarg_detroy.
 *
 * @param source      the alljoyn_msgarg to be copied
 *
 * @return copy of the source message argument is returned
 */
extern AJ_API alljoyn_msgarg alljoyn_msgarg_copy(const alljoyn_msgarg source);

/**
 * Equality operator.
 *
 * @param lhv  The alljoyn_msgarg to compare.
 * @param rhv  The other alljoyn_msgarg to compare
 *
 * @return  Returns true if the two message args have the same signatures and values.
 */
extern AJ_API QC_BOOL alljoyn_msgarg_equal(alljoyn_msgarg lhv, alljoyn_msgarg rhv);



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
extern AJ_API QStatus alljoyn_msgarg_array_set(alljoyn_msgarg args, size_t* numArgs, const char* signature, ...);

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
extern AJ_API QStatus alljoyn_msgarg_array_get(const alljoyn_msgarg args, size_t numArgs, const char* signature, ...);

/**
 * Returns an XML string representation of this type
 *
 * @param[in]  arg    The message arg to generate the XML string representation of
 * @param[out] str    The character string that will hold the XML string
 *               representation of the alljoyn_msgarg
 * @param[in]  buf    The size of the char* array that will hold the string
 * @param[in]  indent Number of spaces to indent the generated xml (default value 0)
 *
 * @return  The number of characters (excluding the terminating null byte) which
 *          would have been written to the final string if enough space
 *          available.  Thus returning a value of buf or larger means the output
 *          was truncated.
 */
extern AJ_API size_t alljoyn_msgarg_tostring(alljoyn_msgarg arg, char* str, size_t buf, size_t indent);

/**
 * Returns an XML string representation for an array of message args.
 *
 * @param[in]  args     The message arg array to generate the XML string representation of
 * @param[in]  numArgs  The size of the message arg array.
 * @param[out] str      The character string that will hold the XML string
 *                      representation of the alljoyn_msgarg array
 * @param[in]  buf      The size of the char* array that will hold the string
 * @param[in]  indent   Number of spaces to indent the generated xml (default value 0)
 *
 * @return  The number of characters (excluding the terminating null byte) which
 *          would have been written to the final string if enough space is
 *          available.  Thus returning a value of buf or larger means the output
 *          was truncated.
 */
extern AJ_API size_t alljoyn_msgarg_array_tostring(const alljoyn_msgarg args, size_t numArgs, char* str, size_t buf, size_t indent);

/**
 * Returns a string for the signature of this value
 *
 * @param arg the argument to read the signature from
 *
 * @return The signature string for this MsgArg
 */
extern AJ_API const char* alljoyn_msgarg_signature(alljoyn_msgarg arg);

/**
 * Returns a string representation of the signature of an array of message args.
 *
 * @param values     A pointer to an array of message arg values
 * @param numValues  Length of the array
 *
 * @return The signature string for the message args.
 */
extern AJ_API const char* alljoyn_msgarg_array_signature(alljoyn_msgarg values, size_t numValues);

/**
 * Checks the signature of this arg.
 *
 * @param arg        The message argument we want to check the signature of
 * @param signature  The signature to check
 *
 * @return  true if this arg has the specified signature, otherwise returns false.
 */
extern AJ_API QC_BOOL alljoyn_msgarg_hassignature(alljoyn_msgarg arg, const char* signature);

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
 *     QStatus status = alljoyn_msgarg_getdictelement(arg, "{s(yus)}", "fred", &age, &height,  &address);
 *     @endcode
 *
 * This function is particularly useful for extracting specific properties from the array of property
 * values returned by ProxyBusObject::GetAllProperties.
 *
 * @param arg      a message argument containing an array of dictionary elements
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
extern AJ_API QStatus alljoyn_msgarg_getdictelement(alljoyn_msgarg arg, const char* elemSig, ...);


extern AJ_API AllJoynTypeId alljoyn_msgarg_gettype(alljoyn_msgarg arg);

/**
 * Clear the MsgArg setting the type to ALLJOYN_INVALID and freeing any memory allocated for the
 * MsgArg value.
 *
 * @param arg the message argument to be cleared.
 */
extern AJ_API void alljoyn_msgarg_clear(alljoyn_msgarg arg);

/**
 * Makes a MsgArg stable by completely copying the contents into locally
 * managed memory. After a MsgArg has been stabilized any values used to
 * initialize or set the message arg can be freed.
 *
 * @param arg the message argument to stabilize
 */
extern AJ_API void alljoyn_msgarg_stabilize(alljoyn_msgarg arg);


/*******************************************************************************
 * This set of functions were originally designed for the alljoyn_unity bindings
 * however they did not not properly map with the C++ MsgArg Class.  These calls
 * are being left in here till proper mapping between 'C' and 'unity' can be
 * completed. And can be verified that the code continues to work.
 ******************************************************************************/

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
extern AJ_API QStatus alljoyn_msgarg_array_set_offset(alljoyn_msgarg args, size_t argOffset, size_t* numArgs, const char* signature, ...);

extern AJ_API uint8_t alljoyn_msgarg_as_uint8(const alljoyn_msgarg args, size_t idx);
extern AJ_API QC_BOOL alljoyn_msgarg_as_bool(const alljoyn_msgarg args, size_t idx);
extern AJ_API int16_t alljoyn_msgarg_as_int16(const alljoyn_msgarg args, size_t idx);
extern AJ_API uint16_t alljoyn_msgarg_as_uint16(const alljoyn_msgarg args, size_t idx);
extern AJ_API int32_t alljoyn_msgarg_as_int32(const alljoyn_msgarg args, size_t idx);
extern AJ_API uint32_t alljoyn_msgarg_as_uint32(const alljoyn_msgarg args, size_t idx);
extern AJ_API int64_t alljoyn_msgarg_as_int64(const alljoyn_msgarg args, size_t idx);
extern AJ_API uint64_t alljoyn_msgarg_as_uint64(const alljoyn_msgarg args, size_t idx);
extern AJ_API double alljoyn_msgarg_as_double(const alljoyn_msgarg args, size_t idx);
extern AJ_API const char* alljoyn_msgarg_as_string(const alljoyn_msgarg args, size_t idx);
extern AJ_API const char* alljoyn_msgarg_as_objpath(const alljoyn_msgarg args, size_t idx);
extern AJ_API alljoyn_msgarg alljoyn_msgarg_as_variant(const alljoyn_msgarg args, size_t idx);
extern AJ_API void alljoyn_msgarg_as_signature(const alljoyn_msgarg args, size_t idx,
                                               uint8_t* out_len, const char** out_sig);
extern AJ_API void alljoyn_msgarg_as_handle(const alljoyn_msgarg args, size_t idx,
                                            void** out_socketFd);
extern AJ_API const alljoyn_msgarg alljoyn_msgarg_as_array(const alljoyn_msgarg args, size_t idx,
                                                           size_t* out_len, const char** out_sig);
extern AJ_API alljoyn_msgarg alljoyn_msgarg_as_struct(const alljoyn_msgarg args, size_t idx,
                                                      size_t* out_numMembers);
extern AJ_API void alljoyn_msgarg_as_dictentry(const alljoyn_msgarg args, size_t idx,
                                               alljoyn_msgarg* out_key, alljoyn_msgarg* out_val);
extern AJ_API void alljoyn_msgarg_as_scalararray(const alljoyn_msgarg args, size_t idx,
                                                 size_t* out_numElements, const void** out_elements);
#ifdef __cplusplus
} /* extern "C" */
#endif

#endif
