/**
 * @file
 * This file defines a class for message bus data types and values
 */

/******************************************************************************
 * Copyright 2012-2013, Qualcomm Innovation Center, Inc.
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

using System;
using System.Runtime.InteropServices;

namespace AllJoynUnity
{
	public partial class AllJoyn
	{
		public class MsgArg : IDisposable
		{
			/*
			 * The AllJoyn data type IDs.
			 */
			enum AllJoynTypeId{
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
			}

			/**
			 * Constructor for MsgArgs.
			 */
			public MsgArg()
			{
				_msgArg = alljoyn_msgarg_create();
				_length = 1;
			}

			/**
			 * Constructor for MsgArgs.
			 */
			public MsgArg(uint numArgs)
			{ 
				_msgArg = alljoyn_msgarg_array_create((UIntPtr)numArgs);
				_length = numArgs;
			}
			/**
			 * @cond ALLJOYN_DEV
			 * @internal
			 * Constructor for MsgArgs.
			 * This constructor shold only be used internaly 
			 * This will create a C# MsgArg using a pointer to an already existing 
			 * native C MsgArg
			 * 
			 * @param nativeMsgArg a pointer to a native C MsgArg
			 */
			public MsgArg(IntPtr nativeMsgArg)
			{
				_msgArg = nativeMsgArg;
				_length = 1;
				_isDisposed = true;
			}
			/** @endcond */

			/**
			 * Access the indexed element in an array of MsgArgs
			 * 
			 */
			public MsgArg this[int index]
			{
				get 
				{
					MsgArg ret = new MsgArg();
					ret._msgArg = alljoyn_msgarg_array_element(this._msgArg, (UIntPtr)index);
					//Prevent the code calling alljoyn_msgarg_destroy on this variable
					ret._isDisposed = true;
					return ret;
				}
				set
				{
					alljoyn_msgarg_clone(alljoyn_msgarg_array_element(this._msgArg, (UIntPtr)index), value._msgArg);
				}
			}
			/**
			 * obtain the length of an array of MsgArgs
			 */
			public int Length
			{
				get
				{
					return (int)_length;
				}
			}


			#region implicit To MsgArg
			/** 
			 * Gets a new MsgArg containing the byte arg value
			 *
			 * @param y byte to assign to the MsgArg object
			 * @return a new MsgArg object
			 */
			public static implicit operator MsgArg(byte y)
			{
				MsgArg arg = new MsgArg();
				arg.Set("y", y);
				return arg;
			}

			/** 
			 * Gets a new MsgArg containing the short arg value
			 *
			 * @param n short to assign to the MsgArg object
			 * @return a new MsgArg object
			 */
			public static implicit operator MsgArg(short n)
			{
				MsgArg arg = new MsgArg();
				arg.Set("n", n);
				return arg;
			}

			/** 
			 * Gets a new MsgArg containing the ushort arg value
			 *
			 * @param q ushort to assign to the MsgArg object
			 * @return a new MsgArg object
			 */
			public static implicit operator MsgArg(ushort q)
			{
				MsgArg arg = new MsgArg();
				arg.Set("q", q);
				return arg;
			}

			/** 
			 * Gets a new MsgArg containing the int arg value
			 *
			 * @param i int to assign to the MsgArg object
			 * @return a new MsgArg object
			 */
			public static implicit operator MsgArg(int i)
			{
				MsgArg arg = new MsgArg();
				arg.Set("i", i);
				return arg;
			}

			/** 
			 * Gets a new MsgArg containing the uint arg value
			 *
			 * @param u uint to assign to the MsgArg object
			 * @return a new MsgArg object
			 */
			public static implicit operator MsgArg(uint u)
			{
				MsgArg arg = new MsgArg();
				arg.Set("u", u);
				return arg;
			}


			/** 
			 * Gets a new MsgArg containing the bool arg value
			 *
			 * @param b bool to assign to the MsgArg object
			 * @return a new MsgArg object
			 */
			public static implicit operator MsgArg(bool b)
			{
				MsgArg arg = new MsgArg();
				arg.Set("b", b);
				return arg;
			}


			/** 
			 * Gets a new MsgArg containing the long arg value
			 *
			 * @param x long to assign to the MsgArg object
			 * @return a new MsgArg object
			 */
			public static implicit operator MsgArg(long x)
			{
				MsgArg arg = new MsgArg();
				arg.Set("x", x);
				return arg;
			}

			/** 
			 * Gets a new MsgArg containing the ulong arg value
			 *
			 * @param t ulong to assign to the MsgArg object
			 * @return a new MsgArg object
			 */
			public static implicit operator MsgArg(ulong t)
			{
				MsgArg arg = new MsgArg();
				arg.Set("t", t);
				return arg;
			}

			/** 
			 * Gets a new MsgArg containing the double arg value
			 *
			 * @param d double to assign to the MsgArg object
			 * @return a new MsgArg object
			 */
			public static implicit operator MsgArg(double d)
			{
				MsgArg arg = new MsgArg();
				arg.Set("d", d);
				return arg;
			}

			/** 
			 * Gets a new MsgArg containing the string arg value
			 *
			 * @param s string to assign to the MsgArg object
			 * @return a new MsgArg object
			 */
			public static implicit operator MsgArg(string s)
			{
				MsgArg arg = new MsgArg();
				arg.Set("s", s);
				return arg;
			}

			/** 
			 * Gets a new MsgArg containing the byte[] arg value
			 *
			 * @param ay byte array to assign to the MsgArg object
			 * @return a new MsgArg object
			 */
			public static implicit operator MsgArg(byte[] ay)
			{
				MsgArg arg = new MsgArg();
				arg.Set("ay", ay);
				return arg;
			}

			// DISALLOW IMPLICIT CONVERSION from a bool array to a MsgArg
			// this does not work as expected as best as I can tell it has 
			// something to do with the fact that in C bool is 4 bytes and in
			// C++ the size of bool is 1 byte.  
			// to set boolean arrays call arg.Set(bool_array); or 
			// arg.Set("ab", bool_array);
			/*
			 * Gets a new MsgArg containing the bool[] arg value
			 *
			 * @param ab bool array to assign to the MsgArg object
			 * @return a new MsgArg object
			 */
			//public static implicit operator MsgArg(bool[] ab)
			//{
			//    MsgArg arg = new MsgArg();
			//    QStatus status = arg.Set(ab);
			//    return arg;
			//}

			/** 
			 * Gets a new MsgArg containing the short[] arg value
			 *
			 * @param an short array to assign to the MsgArg object
			 * @return a new MsgArg object
			 */
			public static implicit operator MsgArg(short[] an)
			{
				MsgArg arg = new MsgArg();
				arg.Set("an", an);
				return arg;
			}

			/** 
			 * Gets a new MsgArg containing the ushort[] arg value
			 *
			 * @param aq ushort to assign to the MsgArg object
			 * @return a new MsgArg object
			 */
			public static implicit operator MsgArg(ushort[] aq)
			{
				MsgArg arg = new MsgArg();
				arg.Set("aq", aq);
				return arg;
			}

			/** 
			 * Gets a new MsgArg containing the int[] arg value
			 *
			 * @param ai int to assign to the MsgArg object
			 * @return a new MsgArg object
			 */
			public static implicit operator MsgArg(int[] ai)
			{
				MsgArg arg = new MsgArg();
				arg.Set("ai", ai);
				return arg;
			}

			/** 
			 * Gets a new MsgArg containing the uint[] arg value
			 *
			 * @param au uint array  to assign to the MsgArg object
			 * @return a new MsgArg object
			 */
			public static implicit operator MsgArg(uint[] au)
			{
				MsgArg arg = new MsgArg();
				arg.Set("au", au);
				return arg;
			}

			/** 
			 * Gets a new MsgArg containing the long[] arg value
			 *
			 * @param ax long array to assign to the MsgArg object
			 * @return a new MsgArg object
			 */
			public static implicit operator MsgArg(long[] ax)
			{
				MsgArg arg = new MsgArg();
				arg.Set("ax", ax);
				return arg;
			}

			/** 
			 * Gets a new MsgArg containing the ulong[] arg value
			 *
			 * @param at ulong array to assign to the MsgArg object
			 * @return a new MsgArg object
			 */
			public static implicit operator MsgArg(ulong[] at)
			{
				MsgArg arg = new MsgArg();
				arg.Set("at", at);
				return arg;
			}

			/** 
			 * Gets a new MsgArg containing the double[] arg value
			 *
			 * @param ad double array to assign to the MsgArg object
			 * @return a new MsgArg object
			 */
			public static implicit operator MsgArg(double[] ad)
			{
				MsgArg arg = new MsgArg();
				arg.Set("ad", ad);
				return arg;
			}

			/** 
			 * Gets a new MsgArg containing the string[] arg value
			 *
			 * @param sa string array to assign to the MsgArg object
			 * @return a new MsgArg object
			 */
			public static implicit operator MsgArg(string[] sa)
			{
				MsgArg arg = new MsgArg();
				arg.Set("as", sa);
				return arg;
			}
			#endregion 
			#region implicit From MsgArg
			/** 
			 * Gets the byte value of a MsgArg Object
			 *
			 * @param arg	MsgArg used to access value from
			 * @return byte value of the MsgArg Object
			 */
			public static implicit operator byte(MsgArg arg)
			{
				object y;
				arg.Get("y", out y);
				return (byte)y;
			}

			/** 
			 * Gets the bool value of a MsgArg Object
			 *
			 * @param arg	MsgArg used to access value from
			 * @return bool value of a MsgArg Object
			 */
			public static implicit operator bool(MsgArg arg)
			{
				object b;
				arg.Get("b", out b);
				return (bool)b;
			}

			/** 
			 * Gets the short value of a MsgArg Object
			 *
			 * @param arg	MsgArg used to access value from
			 * @return short value of a MsgArg Object
			 */
			public static implicit operator short(MsgArg arg)
			{

				object n;
				arg.Get("n", out n);
				return (short)n;
			}

			/** 
			 * Gets the ushort value of a MsgArg Object
			 *
			 * @param arg	MsgArg used to access value from
			 * @return ushort value of a MsgArg Object
			 */
			public static implicit operator ushort(MsgArg arg)
			{
				object q;
				arg.Get("q", out q);
				return (ushort)q;
			}

			/** 
			 * Gets the int value of a MsgArg Object
			 *
			 * @param arg	MsgArg used to access value from
			 * @return int value of a MsgArg Object
			 */
			public static implicit operator int(MsgArg arg)
			{
				object i;
				arg.Get("i", out i);
				return (int)i;
			}

			/** 
			 * Gets the uint value of a MsgArg Object
			 *
			 * @param arg	MsgArg used to access value from
			 * @return uint value of a MsgArg Object
			 */
			public static implicit operator uint(MsgArg arg)
			{
				object u;
				arg.Get("u", out u);
				return (uint)u;
			}

			/** 
			 * Gets the long value of a MsgArg Object
			 *
			 * @param arg	MsgArg used to access value from
			 * @return long value of a MsgArg Object
			 */
			public static implicit operator long(MsgArg arg)
			{
				object x;
				arg.Get("x", out x);
				return (long)x;
			}

			/** 
			 * Gets the ulong value of a MsgArg Object
			 *
			 * @param arg	MsgArg used to access value from
			 * @return ulong value of a MsgArg Object
			 */
			public static implicit operator ulong(MsgArg arg)
			{
				object t;
				arg.Get("t", out t);
				return (ulong)t;
			}

			/** 
			 * Gets the float value of a MsgArg Object. 
			 * 
			 * AllJoyn can not marshal data of type float. Any float obtained from 
			 * AllJoyn is cast from a double so using type float may result in data
			 * loss through truncation.
			 *
			 * @param arg	MsgArg used to access value from
			 * @return float value of a MsgArg Object
			 */
			public static implicit operator float(MsgArg arg)
			{
				object d;
				arg.Get("d", out d);
				double tmp_d = (double)d;
				// convert double to a float.
				float f = (float)tmp_d;
				return f;
			}

			/** 
			 * Gets the double value of a MsgArg Object
			 *
			 * @param arg	MsgArg used to access value from
			 * @return double value of a MsgArg Object
			 */
			public static implicit operator double(MsgArg arg)
			{
				object d;
				arg.Get("d", out d);
				return (double)d;
			}

			/** 
			 * Gets the string value of a MsgArg Object
			 *
			 * @param arg	MsgArg used to access value from
			 * @return string value of a MsgArg Object
			 */
			public static implicit operator string(MsgArg arg)
			{
				object s;
				arg.Get("s", out s);
				return (string)s;
			}

			/** 
			 * Gets byte array value from a MsgArg Object
			 *
			 * @param arg	MsgArg used to access value from
			 * @return byte array value from the MsgArg Object
			 */
			public static implicit operator byte[](MsgArg arg)
			{
				object ay;
				arg.Get("ay", out ay);
				return (byte[])ay;
			}

			/** 
			 * Gets bool array value from a MsgArg Object
			 *
			 * @param arg	MsgArg used to access value from
			 * @return bool array value from the MsgArg Object
			 */
			public static implicit operator bool[](MsgArg arg)
			{
				object ab;
				arg.Get("ab", out ab);
				return (bool[])ab;
			}

			/** 
			 * Gets short array value from a MsgArg Object
			 *
			 * @param arg	MsgArg used to access value from
			 * @return short array value from the MsgArg Object
			 */
			public static implicit operator short[](MsgArg arg)
			{
				object an;
				arg.Get("an", out an);
				return (short[])an;
			}

			/** 
			 * Gets ushort array value from a MsgArg Object
			 *
			 * @param arg	MsgArg used to access value from
			 * @return ushort array value from the MsgArg Object
			 */
			public static implicit operator ushort[](MsgArg arg)
			{
				object aq;
				arg.Get("aq", out aq);
				return (ushort[])aq;
			}

			/** 
			 * Gets int array value from a MsgArg Object
			 *
			 * @param arg	MsgArg used to access value from
			 * @return int array value from the MsgArg Object
			 */
			public static implicit operator int[](MsgArg arg)
			{
				object ai;
				arg.Get("ai", out ai);
				return (int[])ai;
			}

			/** 
			 * Gets uint array value from a MsgArg Object
			 *
			 * @param arg	MsgArg used to access value from
			 * @return uint array value from the MsgArg Object
			 */
			public static implicit operator uint[](MsgArg arg)
			{
				object au;
				arg.Get("au", out au);
				return (uint[])au;
			}

			/** 
			 * Gets a long array value from a MsgArg Object
			 *
			 * @param arg	MsgArg used to access value from
			 * @return long array value from the MsgArg Object
			 */
			public static implicit operator long[](MsgArg arg)
			{
				object ax;
				arg.Get("ax", out ax);
				return (long[])ax;
			}

			/** 
			 * Gets a ulong array value from a MsgArg Object
			 *
			 * @param arg	MsgArg used to access value from
			 * @return ulong array value from the MsgArg Object
			 */
			public static implicit operator ulong[](MsgArg arg)
			{
				object at;
				arg.Get("at", out at);
				return (ulong[])at;
			}

			/** 
			 * Gets a double array value from a MsgArg Object
			 *
			 * @param arg	MsgArg used to access value from
			 * @return double array value from the MsgArg Object
			 */
			public static implicit operator double[](MsgArg arg)
			{
				object ad;
				arg.Get("ad", out ad);
				return (double[])ad;
			}

			/** 
			 * Gets string array value from a MsgArg Object
			 *
			 * @param arg	MsgArg used to access value from
			 * @return string array value from the MsgArg Object
			 */
			public static implicit operator string[](MsgArg arg)
			{
				object sa;
				arg.Get("as", out sa);
				return (string[])sa;
			}
			#endregion
			#region single param Set
			/**
			 * Set value of a message arg from a value. Note that any values or
			 * MsgArg pointers passed in must remain valid until this MsgArg is freed.
			 *
			 *  - @c 'a'  The array length followed by:
			 *            - If the element type is a basic type a pointer to an array of values of that type.
			 *            - If the element type is string a pointer to array of const char*, if array length is
			 *              non-zero, and the char* pointer is NULL, the NULL must be followed by a pointer to
			 *              an array of const qcc.String.
			 *            - If the element type is an ALLJOYN_ARRAY "ARRAY", ALLJOYN_STRUCT "STRUCT",
			 *              ALLJOYN_DICT_ENTRY "DICT_ENTRY" or ALLJOYN_VARIANT "VARIANT" a pointer to an
			 *              array of MsgArgs where each MsgArg has the signature specified by the element type.
			 *            - If the element type is specified using the wildcard character '*', a pointer to
			 *              an  array of MsgArgs. The array element type is determined from the type of the
			 *              first MsgArg in the array, all the elements must have the same type.
			 *  - @c 'b'  A bool value
			 *  - @c 'd'  A double (64 bits)
			 *  - @c 'g'  A pointer to a NUL terminated string (pointer must remain valid for lifetime of the MsgArg)
			 *  - @c 'h'  A qcc.SocketFd
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
			 *     string fruits[3] =  { "apple", "banana", "orange" };
			 *     MsgArg bowl;
			 *     bowl.Set(fruits);
			 *     @endcode
			 *
			 * @param value   The value for MsgArg value
			 *
			 * @return
			 *      - QStatus.OK if the MsgArg was successfully set
			 *      - An error status otherwise
			 */
			public QStatus Set(object value)
			{
				string signature = "";
				if (value.GetType().Equals(typeof(byte)))
				{
					signature = "y";
				}
				else if (value.GetType().Equals(typeof(bool)))
				{
					signature = "b";
				}
				else if (value.GetType().Equals(typeof(short)))
				{
					signature = "n";
				}
				else if (value.GetType().Equals(typeof(ushort)))
				{
					signature = "q";
				}
				else if (value.GetType().Equals(typeof(int)))
				{
					signature = "i";
				}
				else if (value.GetType().Equals(typeof(uint)))
				{
					signature = "u";
				}
				else if (value.GetType().Equals(typeof(long)))
				{
					signature = "x";
				}
				else if (value.GetType().Equals(typeof(ulong)))
				{
					signature = "t";
				}
				else if (value.GetType().Equals(typeof(float)))
				{
					signature = "d";
					//explicitly cast float to a double.
					double d = Convert.ToDouble((float)value);
					return Set(signature, d);
				}
				else if (value.GetType().Equals(typeof(double)))
				{
					signature = "d";
				}
				else if (value.GetType().Equals(typeof(string)))
				{
					signature = "s";
				}
				else if (value.GetType().Equals(typeof(byte[])))
				{
					signature = "ay";
				}
				else if (value.GetType().Equals(typeof(bool[])))
				{
					signature = "ab";
				}
				else if (value.GetType().Equals(typeof(short[])))
				{
					signature = "an";
				}
				else if (value.GetType().Equals(typeof(ushort[])))
				{
					signature = "aq";
				}
				else if (value.GetType().Equals(typeof(int[])))
				{
					signature = "ai";
				}
				else if (value.GetType().Equals(typeof(uint[])))
				{
					signature = "au";
				}
				else if (value.GetType().Equals(typeof(long[])))
				{
					signature = "ax";
				}
				else if (value.GetType().Equals(typeof(ulong[])))
				{
					signature = "at";
				}
				else if (value.GetType().Equals(typeof(double[])))
				{
					signature = "ad";
				}
				else if (value.GetType().Equals(typeof(string[])))
				{
					signature = "as";
				}
				if (signature.Length != 0)
				{
					return Set(signature, value);
				}
				else
				{
					return QStatus.WRITE_ERROR;
				}
			}
			#endregion
			#region single param Get
			/**
			 * Gets a byte value from a MsgArg
			 * 
			 * This performs the same action as calling 
			 * MsgArg.Get("y", value);
			 * 
			 * @param[out] y the output value that will hold the byte
			 * @return
			 *      - QStatus.OK if the signature matched and MsgArg was successfully unpacked.
			 *      - QStatus.BUS_SIGNATURE_MISMATCH if the signature did not match.
			 *      - An error status otherwise
			 */
			public QStatus Get(out byte y)
			{
				object val;
				QStatus status = Get("y", out val);
				y = (byte)val;
				return status;
			}
			/**
			 * Gets a bool value from a MsgArg
			 * 
			 * Performs the same action as calling 
			 * MsgArg.Get("b", value);
			 * 
			 * @param[out] b the output value that will hold the bool
			 * @return
			 *      - QStatus.OK if the signature matched and MsgArg was successfully unpacked.
			 *      - QStatus.BUS_SIGNATURE_MISMATCH if the signature did not match.
			 *      - An error status otherwise
			 */
			public QStatus Get(out bool b)
			{
				object val;
				QStatus status = Get("b", out val);
				b = (bool)val;
				return status;
			}
			/**
			 * Gets a short value from a MsgArg
			 * 
			 * This performs the same action as calling 
			 * MsgArg.Get("n", value);
			 * 
			 * @param[out] n the output value that will hold the short
			 * @return
			 *      - QStatus.OK if the signature matched and MsgArg was successfully unpacked.
			 *      - QStatus.BUS_SIGNATURE_MISMATCH if the signature did not match.
			 *      - An error status otherwise
			 */
			public QStatus Get(out short n)
			{
				object val;
				QStatus status = Get("n", out val);
				n = (short)val;
				return status;

			}
			/**
			 * Gets a ushort value from a MsgArg
			 * 
			 * This performs the same action as calling 
			 * MsgArg.Get("q", value);
			 * 
			 * @param[out] q the output value that will hold the ushort
			 * @return
			 *      - QStatus.OK if the signature matched and MsgArg was successfully unpacked.
			 *      - QStatus.BUS_SIGNATURE_MISMATCH if the signature did not match.
			 *      - An error status otherwise
			 */
			public QStatus Get(out ushort q)
			{
				object val;
				QStatus status = Get("q", out val);
				q = (ushort)val;
				return status;
			}
			/**
			 * Gets an int value from a MsgArg
			 * 
			 * This performs the same action as calling 
			 * MsgArg.Get("i", value);
			 * 
			 * @param[out] i the output value that will hold the int
			 * @return
			 *      - QStatus.OK if the signature matched and MsgArg was successfully unpacked.
			 *      - QStatus.BUS_SIGNATURE_MISMATCH if the signature did not match.
			 *      - An error status otherwise
			 */
			public QStatus Get(out int i)
			{
				object val;
				QStatus status = Get("i", out val);
				i = (int)val;
				return status;
			}
			/**
			 * Gets a uint value from a MsgArg
			 * 
			 * This performs the same action as calling 
			 * MsgArg.Get("u", value);
			 * 
			 * @param[out] u the output value that will hold the uint
			 * @return
			 *      - QStatus.OK if the signature matched and MsgArg was successfully unpacked.
			 *      - QStatus.BUS_SIGNATURE_MISMATCH if the signature did not match.
			 *      - An error status otherwise
			 */
			public QStatus Get(out uint u)
			{
				object val;
				QStatus status = Get("u", out val);
				u = (uint)val;
				return status;
			}
			/**
			 * Gets a long value from a MsgArg
			 * 
			 * This performs the same action as calling 
			 * MsgArg.Get("x", value);
			 * 
			 * @param[out] x the output value that will hold the long
			 * @return
			 *      - QStatus.OK if the signature matched and MsgArg was successfully unpacked.
			 *      - QStatus.BUS_SIGNATURE_MISMATCH if the signature did not match.
			 *      - An error status otherwise
			 */
			public QStatus Get(out long x)
			{
				object val;
				QStatus status = Get("x", out val);
				x = (long)val;
				return status;
			}
			/**
			 * Gets a ulong value from a MsgArg
			 * 
			 * This performs the same action as calling 
			 * MsgArg.Get("t", value);
			 * 
			 * @param[out] t the output value that will hold the ulong
			 * @return
			 *      - QStatus.OK if the signature matched and MsgArg was successfully unpacked.
			 *      - QStatus.BUS_SIGNATURE_MISMATCH if the signature did not match.
			 *      - An error status otherwise
			 */
			public QStatus Get(out ulong t)
			{
				object val;
				QStatus status = Get("t", out val);
				t = (ulong)val;
				return status;
			}
			/**
			 * Gets a double (64-bit) value from a MsgArg
			 * 
			 * This performs the same action as calling 
			 * MsgArg.Get("d", value);
			 * 
			 * @param[out] d the output value that will hold the double
			 * @return
			 *      - QStatus.OK if the signature matched and MsgArg was successfully unpacked.
			 *      - QStatus.BUS_SIGNATURE_MISMATCH if the signature did not match.
			 *      - An error status otherwise
			 */
			public QStatus Get(out double d)
			{
				object val;
				QStatus status = Get("d", out val);
				d = (double)val;
				return status;
			}

			/**
			 * Gets a string value from a MsgArg
			 * 
			 * This performs the same action as calling 
			 * MsgArg.Get("s", value);
			 * 
			 * To obtain a signature "g" or object "o" value from a MsgArg usage of the
			 * see MsgArg.Get(string, out object);
			 * 
			 * @param[out] s the output value that will hold the string
			 * @return
			 *      - QStatus.OK if the signature matched and MsgArg was successfully unpacked.
			 *      - QStatus.BUS_SIGNATURE_MISMATCH if the signature did not match.
			 *      - An error status otherwis
			 */
			public QStatus Get(out string s)
			{
				object val;
				QStatus status = Get("s", out val);
				s = (string)val;
				return status;
			}
			#endregion
			/**
			 * Matches a signature to the MsArg and if the signature matches unpacks the component values of a MsgArg into an object. 
			 * This function resolved through variants, so if the MsgArg is a variant that references a 32 bit integer is can be unpacked
			 * directly into a 32 bit integer pointer.
			 *
			 *  - @c 'a'  An object containing an array 
			 *  - @c 'b'  An object containing a bool
			 *  - @c 'd'  An object containing a double (64 bits)
			 *  - @c 'g'  An object containing a string that represents an AllJoyn signature
			 *  - @c 'h'  An object containing a Socket file desciptor
			 *  - @c 'i'  An object containing an int
			 *  - @c 'n'  An object containing a short
			 *  - @c 'o'  An object containing a string that represents the name of an AllJoyn object
			 *  - @c 'q'  An object containing a ushort
			 *  - @c 's'  An object containing a string
			 *  - @c 't'  An object containing a ulong
			 *  - @c 'u'  An object containing a uint
			 *  - @c 'v'  An object containing a MsgArg
			 *  - @c 'x'  An object containing a long
			 *  - @c 'y'  An object containing a byte
			 *
			 *  - @c '(' and @c ')'  An object containing a struct
			 *  - @c '{' and @c '}'  An object containing the key and value pair of a dictonary
			 *
			 *  - @c '*' This matches any value type. This should not show up in a signature it is used by AllJoyn for matching
			 *
			 * @param      sig    The signature for MsgArg value
			 * @param[out] value  object containing values to initialize the MsgArg
			 * @return
			 *      - QStatus.OK if the signature matched and MsgArg was successfully unpacked.
			 *      - QStatus.BUS_SIGNATURE_MISMATCH if the signature did not match.
			 *      - An error status otherwise
			 */
			//TODO add in examples into the Get documentation
			public QStatus Get(string sig, out object value)
			{
				QStatus status = QStatus.OK;
				value = null;
				switch((AllJoynTypeId)sig[0])
				{
					case AllJoynTypeId.ALLJOYN_BYTE:
						byte y;
						status = alljoyn_msgarg_get_uint8(_msgArg, out y);
						value = y;
						break;
					case AllJoynTypeId.ALLJOYN_BOOLEAN:
						bool b;
						status = alljoyn_msgarg_get_bool(_msgArg, out b);
						value = b;
						break;
					case AllJoynTypeId.ALLJOYN_INT16:
						short n;
						status = alljoyn_msgarg_get_int16(_msgArg, out n);
						value = n;
						break;
					case AllJoynTypeId.ALLJOYN_UINT16:
						ushort q;
						status = alljoyn_msgarg_get_uint16(_msgArg, out q);
						value = q;
						break;
					case AllJoynTypeId.ALLJOYN_INT32:
						int i;
						status = alljoyn_msgarg_get_int32(_msgArg, out i);
						value = i;
						break;
					case AllJoynTypeId.ALLJOYN_UINT32:
						uint u;
						status = alljoyn_msgarg_get_uint32(_msgArg, out u);
						value = u;
						break;
					case AllJoynTypeId.ALLJOYN_INT64:
						long x;
						status = alljoyn_msgarg_get_int64(_msgArg, out x);
						value = x;
						break;
					case AllJoynTypeId.ALLJOYN_UINT64:
						ulong t;
						status = alljoyn_msgarg_get_uint64(_msgArg, out t);
						value = t;
						break;
					case AllJoynTypeId.ALLJOYN_DOUBLE:
						double d;
						status = alljoyn_msgarg_get_double(_msgArg, out d);
						value = d;
						break;
					case AllJoynTypeId.ALLJOYN_STRING:
						IntPtr s;
						status = alljoyn_msgarg_get_string(_msgArg, out s);
						value = Marshal.PtrToStringAnsi(s);
						break;
					case AllJoynTypeId.ALLJOYN_OBJECT_PATH:
						IntPtr o;
						status = alljoyn_msgarg_get_objectpath(_msgArg, out o);
						value = Marshal.PtrToStringAnsi(o);
						break;	
					case AllJoynTypeId.ALLJOYN_SIGNATURE:
						IntPtr g;
						status = alljoyn_msgarg_get_signature(_msgArg, out g);
						value = Marshal.PtrToStringAnsi(g);
						break;
					case AllJoynTypeId.ALLJOYN_ARRAY:
						int length;
						switch ((AllJoynTypeId)sig[1])
						{
							case AllJoynTypeId.ALLJOYN_BYTE:
								IntPtr ay;
								status = alljoyn_msgarg_get_uint8_array(_msgArg, out length, out ay);
								byte[] ay_result = new byte[length];
								Marshal.Copy(ay, ay_result, 0, length);
								value = ay_result;
								break;
							case AllJoynTypeId.ALLJOYN_BOOLEAN:
								IntPtr ab;
								status = alljoyn_msgarg_get_bool_array(_msgArg, out length, out ab);
								int[] ab_result = new int[length];
								Marshal.Copy(ab, ab_result, 0, length);
								bool[] ab_retValue = new bool[length];
								for (int j = 0; j < length; j++)
								{
									if (ab_result[j] == 0)
									{
										ab_retValue[j] = false;
									}
									else
									{
										ab_retValue[j] = true;
									}
								}
								value = ab_retValue;
								break;
							case AllJoynTypeId.ALLJOYN_INT16:
								IntPtr an;
								status = alljoyn_msgarg_get_int16_array(_msgArg, out length, out an);
								short[] an_result = new short[length];
								Marshal.Copy(an, an_result, 0, length);
								value = an_result;
								break;
							case AllJoynTypeId.ALLJOYN_UINT16:
								IntPtr aq;
								status = alljoyn_msgarg_get_uint16_array(_msgArg, out length, out aq);
								short[] aq_result = new short[length];
								Marshal.Copy(aq, aq_result, 0, length);
								ShortConverter shortConverter = new ShortConverter();
								shortConverter.Shorts = aq_result;
								value = shortConverter.UShorts;
								break;
							case AllJoynTypeId.ALLJOYN_INT32:
								IntPtr ai;
								status = alljoyn_msgarg_get_int32_array(_msgArg, out length, out ai);
								int[] ai_result = new int[length];
								Marshal.Copy(ai, ai_result, 0, length);
								value = ai_result;
								break;
							case AllJoynTypeId.ALLJOYN_UINT32:
								IntPtr au;
								status = alljoyn_msgarg_get_uint32_array(_msgArg, out length, out au);
								int[] au_result = new int[length];
								Marshal.Copy(au, au_result, 0, length);

								IntConverter intConverter = new IntConverter();
								intConverter.Ints = au_result;
								value = intConverter.UInts;
								break;
							case AllJoynTypeId.ALLJOYN_INT64:
								IntPtr ax;
								status = alljoyn_msgarg_get_int64_array(_msgArg, out length, out ax);
								long[] ax_result = new long[length];
								Marshal.Copy(ax, ax_result, 0, length);
								value = ax_result;
								break;
							case AllJoynTypeId.ALLJOYN_UINT64:
								IntPtr at;
								status = alljoyn_msgarg_get_uint64_array(_msgArg, out length, out at);
								long[] at_result = new long[length];
								Marshal.Copy(at, at_result, 0, length);

								LongConverter longConverter = new LongConverter();
								longConverter.Longs = at_result;
								value = longConverter.ULongs;
								break;
							case AllJoynTypeId.ALLJOYN_DOUBLE:
								IntPtr ad;
								status = alljoyn_msgarg_get_double_array(_msgArg, out length, out ad);
								double[] ad_result = new double[length];
								Marshal.Copy(ad, ad_result, 0, length);
								value = ad_result;
								break;
							case AllJoynTypeId.ALLJOYN_STRING:
								IntPtr sa;
								status = alljoyn_msgarg_get_string_array(_msgArg, out length, out sa);
								if (status)
								{
									string[] as_result = new string[length];
									for (int j = 0; j < length; ++j)
									{
										if (status)
										{
											IntPtr inner_s;
											status = alljoyn_msgarg_get_string(alljoyn_msgarg_array_element(sa, (UIntPtr)j), out inner_s);
											as_result[j] = Marshal.PtrToStringAnsi(inner_s);
										}
										else
										{
											break;
										}
									}
									value = as_result;
								}
								break;
							case AllJoynTypeId.ALLJOYN_OBJECT_PATH:
								IntPtr ao;
								status = alljoyn_msgarg_get_objectpath_array(_msgArg, out length, out ao);
								if (status)
								{
									string[] ao_result = new string[length];
									for (int j = 0; j < length; ++j)
									{
										if (status)
										{
											IntPtr inner_o;
											status = alljoyn_msgarg_get_objectpath(alljoyn_msgarg_array_element(ao, (UIntPtr)j), out inner_o);
											ao_result[j] = Marshal.PtrToStringAnsi(inner_o);
										}
										else
										{
											break;
										}
									}
									value = ao_result;
								}
								break;
							case AllJoynTypeId.ALLJOYN_SIGNATURE:
								IntPtr ag;
								status = alljoyn_msgarg_get_signature_array(_msgArg, out length, out ag);
								if (status)
								{
									string[] ag_result = new string[length];
									for (int j = 0; j < length; ++j)
									{
										if (status)
										{
											IntPtr inner_g;
											status = alljoyn_msgarg_get_signature(alljoyn_msgarg_array_element(ag, (UIntPtr)j), out inner_g);
											ag_result[j] = Marshal.PtrToStringAnsi(inner_g);
										}
										else
										{
											break;
										}
									}
									value = ag_result;
								}
								break;
							//TODO handle ALLJOYN_DICT_ENTRY
							case AllJoynTypeId.ALLJOYN_ARRAY:
								int outer_array_size = alljoyn_msgarg_get_array_numberofelements(_msgArg);
								object[] outerArray = new object[outer_array_size];
								for(int j = 0; j < outer_array_size; j++)
								{
									if (status)
									{
										IntPtr inner_data_ptr;
										alljoyn_msgarg_get_array_element(_msgArg, j, out inner_data_ptr);
										MsgArg tmp = new MsgArg(inner_data_ptr);
										string inner_array_sig = Marshal.PtrToStringAnsi(alljoyn_msgarg_get_array_elementsignature(_msgArg, j));
										status = tmp.Get(inner_array_sig, out outerArray[j]);
									}
									else
									{
										break;
									}
								}
								value = outerArray;
								break;
							default:
								status = QStatus.WRITE_ERROR;
								break;
						}
						break;
						//TODO handle ALLJOYN_STRUCT
					case AllJoynTypeId.ALLJOYN_STRUCT_OPEN:
						status = QStatus.WRITE_ERROR;
						break;
						//TODO handle ALLJOYN_DICT
					case AllJoynTypeId.ALLJOYN_DICT_ENTRY_OPEN:
						status = QStatus.WRITE_ERROR;
						break;
					default:
						status = QStatus.WRITE_ERROR;
						break;
				}
				return status;
			}

			/**
			 * Set value of a message arg from a signature and a list of values. Note that any values or
			 * MsgArg pointers passed in must remain valid until this MsgArg is freed.
			 *
			 *  - @c 'a'  The array length followed by:
			 *            - If the element type is a basic type an array of values of that type.
			 *            - If the element type is an "ARRAY", "STRUCT", "DICT_ENTRY" or "VARIANT" an
			 *              array of MsgArgs where each MsgArg has the signature specified by the element type.
			 *            - If the element type is specified using the wildcard character '*', a pointer to
			 *              an  array of MsgArgs. The array element type is determined from the type of the
			 *              first MsgArg in the array, all the elements must have the same type.
			 *  - @c 'b'  A bool value
			 *  - @c 'd'  A double (64 bits)
			 *  - @c 'g'  A string representing an AllJoyn signature
			 *  - @c 'h'  A SocketFd
			 *  - @c 'i'  An int (32 bits)
			 *  - @c 'n'  A short (16 bits)
			 *  - @c 'o'  A string representing an AllJoyn object
			 *  - @c 'q'  A ushort (16 bits)
			 *  - @c 's'  A pointer to a NUL terminated string (pointer must remain valid for lifetime of the MsgArg)
			 *  - @c 't'  A ulong (64 bits)
			 *  - @c 'u'  A uint (32 bits)
			 *  - @c 'v'  A MsgArg.
			 *  - @c 'x'  An long (64 bits)
			 *  - @c 'y'  A byte (8 bits)
			 *
			 *  - @c '(' and @c ')'  The list of values that appear between the parentheses using the notation above
			 *  - @c '{' and @c '}'  A pair values using the notation above.
			 *
			 *  - @c '*'  A pointer to a MsgArg
			 *
			 * @param sig    The signature for MsgArg value
			 * @param value  object containing values to initialize the MsgArg.
			 *
			 * @return
			 *      - QStatus.OK if the MsgArg was successfully set
			 *      - An error status otherwise
			 */
			//TODO add in examples into the Set documentation
			public QStatus Set(string sig, object value)
			{
				QStatus status = QStatus.OK;
				switch ((AllJoynTypeId)sig[0])
				{
					case AllJoynTypeId.ALLJOYN_BYTE:
						status = alljoyn_msgarg_set_uint8(_msgArg, (byte)value);
						break;
					case AllJoynTypeId.ALLJOYN_BOOLEAN:
						status = alljoyn_msgarg_set_bool(_msgArg, (bool)value);
						break;
					case AllJoynTypeId.ALLJOYN_INT16:
						status = alljoyn_msgarg_set_int16(_msgArg, (short)value);
						break;
					case AllJoynTypeId.ALLJOYN_UINT16:
						status = alljoyn_msgarg_set_uint16(_msgArg, (ushort)value);
						break;
					case AllJoynTypeId.ALLJOYN_INT32:
						status = alljoyn_msgarg_set_int32(_msgArg, (int)value);
						break;
					case AllJoynTypeId.ALLJOYN_UINT32:
						status = alljoyn_msgarg_set_uint32(_msgArg, (uint)value);
						break;
					case AllJoynTypeId.ALLJOYN_INT64:
						status = alljoyn_msgarg_set_int64(_msgArg, (long)value);
						break;
					case AllJoynTypeId.ALLJOYN_UINT64:
						status = alljoyn_msgarg_set_uint64(_msgArg, (ulong)value);
						break;
					case AllJoynTypeId.ALLJOYN_DOUBLE:
						status = alljoyn_msgarg_set_double(_msgArg, (double)value);
						break;
					case AllJoynTypeId.ALLJOYN_STRING:
						status = alljoyn_msgarg_set_and_stabilize(_msgArg, sig, (string)value);
						break;
					case AllJoynTypeId.ALLJOYN_OBJECT_PATH:
						goto case AllJoynTypeId.ALLJOYN_STRING;
					case AllJoynTypeId.ALLJOYN_SIGNATURE:
						goto case AllJoynTypeId.ALLJOYN_STRING;
					case AllJoynTypeId.ALLJOYN_ARRAY:
						switch ((AllJoynTypeId)sig[1])
						{
							case AllJoynTypeId.ALLJOYN_BYTE:
								status = alljoyn_msgarg_set_uint8_array(_msgArg, ((byte[])value).Length, (byte[])value);
								break;
							case AllJoynTypeId.ALLJOYN_BOOLEAN:
								Int32[] ab = new Int32[((bool[])value).Length];
								for (int i = 0; i < ab.Length; i++)
								{
									if (((bool[])value)[i])
									{
										ab[i] = 1;
									}
									else
									{
										ab[i] = 0;
									}
								}
								status =  alljoyn_msgarg_set_bool_array(_msgArg, ab.Length, ab);
								break;
							case AllJoynTypeId.ALLJOYN_INT16:
								status = alljoyn_msgarg_set_int16_array(_msgArg, ((short[])value).Length, (short[])value);
								break;
							case AllJoynTypeId.ALLJOYN_UINT16:
								status = alljoyn_msgarg_set_uint16_array(_msgArg, ((ushort[])value).Length, (ushort[])value);
								break;
							case AllJoynTypeId.ALLJOYN_INT32:
								status = alljoyn_msgarg_set_int32_array(_msgArg, ((int[])value).Length, (int[])value);
								break;
							case AllJoynTypeId.ALLJOYN_UINT32:
								status = alljoyn_msgarg_set_uint32_array(_msgArg, ((uint[])value).Length, (uint[])value);
								break;
							case AllJoynTypeId.ALLJOYN_INT64:
								status = alljoyn_msgarg_set_int64_array(_msgArg, ((long[])value).Length, (long[])value);
								break;
							case AllJoynTypeId.ALLJOYN_UINT64:
								status = alljoyn_msgarg_set_uint64_array(_msgArg, ((ulong[])value).Length, (ulong[])value);
								break;
							case AllJoynTypeId.ALLJOYN_DOUBLE:
								status = alljoyn_msgarg_set_double_array(_msgArg, ((double[])value).Length, (double[])value);
								break;
							case AllJoynTypeId.ALLJOYN_STRING:
								status = alljoyn_msgarg_set_and_stabilize(_msgArg, sig, ((string[])value).Length, (string[])value);
								break;
							case AllJoynTypeId.ALLJOYN_SIGNATURE:
								goto case AllJoynTypeId.ALLJOYN_STRING;
							case AllJoynTypeId.ALLJOYN_OBJECT_PATH:
								goto case AllJoynTypeId.ALLJOYN_STRING;
							//TODO handle ALLJOYN_DICT_ENTRY
							// when working with arrays of arrays the user must pass in a jagged array
							// i.e.  int[2,4] will not work but int[2][] will work.
							case AllJoynTypeId.ALLJOYN_ARRAY:
								string inner_sig = sig.Substring(1);
								int array_size = ((System.Array)value).GetLength(0);
								MsgArg args = new MsgArg((uint)array_size);
								for (int j = 0; j < array_size; ++j)
								{
									if (status != QStatus.OK)
									{
										break;
									}
									object inner_array = ((System.Array)value).GetValue(j);
									status = args[j].Set(inner_sig, inner_array);
								}
								if (status = QStatus.OK)
								{
									status = alljoyn_msgarg_set(_msgArg, sig, array_size, args.UnmanagedPtr);
								}
								break;
							default:
								status = QStatus.WRITE_ERROR;
								break;
						}
						break;
					case AllJoynTypeId.ALLJOYN_STRUCT_OPEN:
						status = QStatus.WRITE_ERROR;
						break;
					case AllJoynTypeId.ALLJOYN_DICT_ENTRY_OPEN:
						status = QStatus.WRITE_ERROR;
						break;
					case AllJoynTypeId.ALLJOYN_VARIANT:
						status = alljoyn_msgarg_set(_msgArg, sig, ((MsgArg)value)._msgArg);
						break;
					default:
						status = QStatus.WRITE_ERROR;
						break;
				}
				return status;
			}

			/**
			 * Gets or Sets the value of the ObjectPath
			 */
			[Obsolete("To assigne a MsgArg ObjectPath us arg.Set(\"o\", \"/path/of/object\")")]
			public string ObjectPath
			{
				get
				{
					IntPtr o;
					alljoyn_msgarg_get_objectpath(_msgArg, out o);
					return Marshal.PtrToStringAnsi(o);
				}
				set
				{
					Set("o", value);
				}
			}

			#region IDisposable
			/**
			 * Dispose the MsgArgs
			 */
			public void Dispose()
			{
				Dispose(true);
				GC.SuppressFinalize(this); 
			}

			/**
			 * Dispose the MsgArgs
			 * @param disposing	describes if its activly being disposed
			 */
			protected virtual void Dispose(bool disposing)
			{
				if(!_isDisposed)
				{
					alljoyn_msgarg_destroy(_msgArg);
					_msgArg = IntPtr.Zero;
				}
				_isDisposed = true;
			}

			~MsgArg()
			{
				Dispose(false);
			}
			#endregion

			#region typeConversion
			/*
			 * trick used to map two fields to the same memory location
			 * This allows us to convert a short[] to an ushort[]. This 
			 * allows us to over come the limitation that Marshal.Copy does 
			 * not work for ushort[] data types.
			 */
			[StructLayout(LayoutKind.Explicit)]
			private struct ShortConverter
			{
				[FieldOffset(0)]
				public short[] Shorts;

				[FieldOffset(0)]
				public ushort[] UShorts;
			}

			/*
			 * trick used to map two fields to the same memory location
			 * This allows us to convert a int[] to an uint[]. This 
			 * allows us to over come the limitation that Marshal.Copy does 
			 * not work for uint[] data types.
			 */
			[StructLayout(LayoutKind.Explicit)]
			private struct IntConverter
			{
				[FieldOffset(0)]
				public int[] Ints;

				[FieldOffset(0)]
				public uint[] UInts;
			}

			/*
			 * trick used to map two fields to the same memory location
			 * This allows us to convert a a long[] to an ulong[]. This 
			 * allows us to over come the limitation that Marshal.Copy does 
			 * not work for ulong[] data types.
			 */
			[StructLayout(LayoutKind.Explicit)]
			private struct LongConverter
			{
				[FieldOffset(0)]
				public long[] Longs;

				[FieldOffset(0)]
				public ulong[] ULongs;
			}
			#endregion

			#region DLL Imports
			[DllImport(DLL_IMPORT_TARGET)]
			private static extern IntPtr alljoyn_msgarg_create();
			[DllImport(DLL_IMPORT_TARGET)]
			private static extern IntPtr alljoyn_msgarg_array_create(UIntPtr numArgs); // UIntPtr must map to the same size as size_t, not a typo
			[DllImport(DLL_IMPORT_TARGET)]
			private static extern IntPtr alljoyn_msgarg_array_element(IntPtr args, UIntPtr index);
			[DllImport(DLL_IMPORT_TARGET)]
			private static extern void alljoyn_msgarg_destroy(IntPtr arg);
			[DllImport(DLL_IMPORT_TARGET)]
			private static extern void alljoyn_msgarg_clone(IntPtr destination, IntPtr source);

			[DllImport(DLL_IMPORT_TARGET)]
			private static extern void alljoyn_msgarg_stabilize(IntPtr arg);
			[DllImport(DLL_IMPORT_TARGET)]
			private static extern int alljoyn_msgarg_gettype(IntPtr arg);

			[DllImport(DLL_IMPORT_TARGET)]
			private static extern int alljoyn_msgarg_set_uint8(IntPtr arg, byte y);
			// bool in C# is 4 bytes  to marshal between C and C# we must specify that a 
			// C boolean is a 1-byte signed integer
			// http://msdn.microsoft.com/en-us/library/system.runtime.interopservices.unmanagedtype.aspx
			[DllImport(DLL_IMPORT_TARGET)]
			private static extern int alljoyn_msgarg_set_bool(IntPtr arg, [MarshalAs(UnmanagedType.I1)]bool b);
			[DllImport(DLL_IMPORT_TARGET)]
			private static extern int alljoyn_msgarg_set_int16(IntPtr arg, short n);
			[DllImport(DLL_IMPORT_TARGET)]
			private static extern int alljoyn_msgarg_set_uint16(IntPtr arg, ushort q);
			[DllImport(DLL_IMPORT_TARGET)]
			private static extern int alljoyn_msgarg_set_int32(IntPtr arg, int i);
			[DllImport(DLL_IMPORT_TARGET)]
			private static extern int alljoyn_msgarg_set_uint32(IntPtr arg, uint u);
			[DllImport(DLL_IMPORT_TARGET)]
			private static extern int alljoyn_msgarg_set_int64(IntPtr arg, long x);
			[DllImport(DLL_IMPORT_TARGET)]
			private static extern int alljoyn_msgarg_set_uint64(IntPtr arg, ulong t);
			[DllImport(DLL_IMPORT_TARGET)]
			private static extern int alljoyn_msgarg_set_double(IntPtr arg, double d);
			/* char* string arrays are passed as an IntPtr */
			[DllImport(DLL_IMPORT_TARGET)]
			private static extern int alljoyn_msgarg_set_string(IntPtr arg, IntPtr s);
			[DllImport(DLL_IMPORT_TARGET)]
			private static extern int alljoyn_msgarg_set_objectpath(IntPtr arg, IntPtr o);
			[DllImport(DLL_IMPORT_TARGET)]
			private static extern int alljoyn_msgarg_set_signature(IntPtr arg, IntPtr g);
			[DllImport(DLL_IMPORT_TARGET, CharSet = CharSet.Ansi)]
			private static extern int alljoyn_msgarg_set_and_stabilize(IntPtr arg, string sig, string s);
			/* DllImport for variant data types */
			[DllImport(DLL_IMPORT_TARGET, CharSet = CharSet.Ansi)]
			private static extern int alljoyn_msgarg_set(IntPtr arg, string sig, IntPtr v);

			[DllImport(DLL_IMPORT_TARGET)]
			private static extern int alljoyn_msgarg_get_uint8(IntPtr arg, out byte y);

			// bool in C# is 4 bytes  to marshal between C and C# we must specify that a 
			// C boolean is a 1-byte signed integer
			// http://msdn.microsoft.com/en-us/library/system.runtime.interopservices.unmanagedtype.aspx
			[DllImport(DLL_IMPORT_TARGET)]
			private static extern int alljoyn_msgarg_get_bool(IntPtr arg, [MarshalAs(UnmanagedType.I1)] out bool b);
			[DllImport(DLL_IMPORT_TARGET)]
			private static extern int alljoyn_msgarg_get_int16(IntPtr arg, out short n);
			[DllImport(DLL_IMPORT_TARGET)]
			private static extern int alljoyn_msgarg_get_uint16(IntPtr arg, out ushort q);
			[DllImport(DLL_IMPORT_TARGET)]
			private static extern int alljoyn_msgarg_get_int32(IntPtr arg, out int i);
			[DllImport(DLL_IMPORT_TARGET)]
			private static extern int alljoyn_msgarg_get_uint32(IntPtr arg, out uint i);
			[DllImport(DLL_IMPORT_TARGET)]
			private static extern int alljoyn_msgarg_get_int64(IntPtr arg, out long x);
			[DllImport(DLL_IMPORT_TARGET)]
			private static extern int alljoyn_msgarg_get_uint64(IntPtr arg, out ulong t);
			[DllImport(DLL_IMPORT_TARGET)]
			private static extern int alljoyn_msgarg_get_double(IntPtr arg, out double d);
			/* char* string arrays are passed as an IntPtr */
			[DllImport(DLL_IMPORT_TARGET)]
			private static extern int alljoyn_msgarg_get_string(IntPtr arg, out IntPtr s);
			[DllImport(DLL_IMPORT_TARGET)]
			private static extern int alljoyn_msgarg_get_objectpath(IntPtr arg, out IntPtr o);
			[DllImport(DLL_IMPORT_TARGET)]
			private static extern int alljoyn_msgarg_get_signature(IntPtr arg, out IntPtr g);

			/* set functions for arrays of basic types. */
			[DllImport(DLL_IMPORT_TARGET)]
			private static extern int alljoyn_msgarg_set_uint8_array(IntPtr arg, int length, byte[] ay);
			[DllImport(DLL_IMPORT_TARGET)]
			private static extern int alljoyn_msgarg_set_bool_array(IntPtr arg, int length, Int32[] ab);
			[DllImport(DLL_IMPORT_TARGET)]
			private static extern int alljoyn_msgarg_set_int16_array(IntPtr arg, int length, short[] an);
			[DllImport(DLL_IMPORT_TARGET)]
			private static extern int alljoyn_msgarg_set_uint16_array(IntPtr arg, int length, ushort[] aq);
			[DllImport(DLL_IMPORT_TARGET)]
			private static extern int alljoyn_msgarg_set_int32_array(IntPtr arg, int length, int[] ai);
			[DllImport(DLL_IMPORT_TARGET)]
			private static extern int alljoyn_msgarg_set_uint32_array(IntPtr arg, int length, uint[] au);
			[DllImport(DLL_IMPORT_TARGET)]
			private static extern int alljoyn_msgarg_set_int64_array(IntPtr arg, int length, long[] ax);
			[DllImport(DLL_IMPORT_TARGET)]
			private static extern int alljoyn_msgarg_set_uint64_array(IntPtr arg, int length, ulong[] at);
			[DllImport(DLL_IMPORT_TARGET)]
			private static extern int alljoyn_msgarg_set_double_array(IntPtr arg, int length, double[] ad);
			/* char* string arrays are passed as an IntPtr */
			//TODO investigate if these next 3 imports are still needed.
			[DllImport(DLL_IMPORT_TARGET, CharSet = CharSet.Ansi)]
			private static extern int alljoyn_msgarg_set_string_array(IntPtr arg, int length, [In]string[] sa);
			[DllImport(DLL_IMPORT_TARGET, CharSet = CharSet.Ansi)]
			private static extern int alljoyn_msgarg_set_objectpath_array(IntPtr arg, int length, [In]string[] ao);
			[DllImport(DLL_IMPORT_TARGET, CharSet = CharSet.Ansi)]
			private static extern int alljoyn_msgarg_set_signature_array(IntPtr arg, int length, [In]string[] ag);
			[DllImport(DLL_IMPORT_TARGET, CharSet = CharSet.Ansi)]
			private static extern int alljoyn_msgarg_set_and_stabilize(IntPtr arg, string sig, int length, string[] s);
			/* DllImport for Arrays of Arrays data types */
			[DllImport(DLL_IMPORT_TARGET, CharSet = CharSet.Ansi)]
			private static extern int alljoyn_msgarg_set(IntPtr arg, string sig, int length, IntPtr v);

			/* get functions for arrays of basic types. */
			[DllImport(DLL_IMPORT_TARGET)]
			private static extern int alljoyn_msgarg_get_uint8_array(IntPtr arg, out int length, out IntPtr ay);
			[DllImport(DLL_IMPORT_TARGET)]
			private static extern int alljoyn_msgarg_get_bool_array(IntPtr arg, out int length, out IntPtr ab);
			[DllImport(DLL_IMPORT_TARGET)]
			private static extern int alljoyn_msgarg_get_int16_array(IntPtr arg, out int length, out IntPtr an);
			[DllImport(DLL_IMPORT_TARGET)]
			private static extern int alljoyn_msgarg_get_uint16_array(IntPtr arg, out int length, out IntPtr aq);
			[DllImport(DLL_IMPORT_TARGET)]
			private static extern int alljoyn_msgarg_get_int32_array(IntPtr arg, out int length, out IntPtr ai);
			[DllImport(DLL_IMPORT_TARGET)]
			private static extern int alljoyn_msgarg_get_uint32_array(IntPtr arg, out int length, out IntPtr ai);
			[DllImport(DLL_IMPORT_TARGET)]
			private static extern int alljoyn_msgarg_get_int64_array(IntPtr arg, out int length, out IntPtr ax);
			[DllImport(DLL_IMPORT_TARGET)]
			private static extern int alljoyn_msgarg_get_uint64_array(IntPtr arg, out int length, out IntPtr at);
			[DllImport(DLL_IMPORT_TARGET)]
			private static extern int alljoyn_msgarg_get_double_array(IntPtr arg, out int length, out IntPtr ad);
			/* char* string arrays are passed as an IntPtr */
			[DllImport(DLL_IMPORT_TARGET)]
			private static extern int alljoyn_msgarg_get_string_array(IntPtr arg, out int length, out IntPtr sa);
			[DllImport(DLL_IMPORT_TARGET)]
			private static extern int alljoyn_msgarg_get_objectpath_array(IntPtr arg, out int length, out IntPtr ao);
			[DllImport(DLL_IMPORT_TARGET)]
			private static extern int alljoyn_msgarg_get_signature_array(IntPtr arg, out int length, out IntPtr ag);
			[DllImport(DLL_IMPORT_TARGET)]
			private static extern int alljoyn_msgarg_get_array_numberofelements(IntPtr arg);
			[DllImport(DLL_IMPORT_TARGET)]
			private static extern void alljoyn_msgarg_get_array_element(IntPtr arg, int index, out IntPtr element);
			[DllImport(DLL_IMPORT_TARGET)]
			private static extern IntPtr alljoyn_msgarg_get_array_elementsignature(IntPtr arg, int index);
			#endregion

			#region Internal Properties
			internal IntPtr UnmanagedPtr
			{
				get
				{
					return _msgArg;
				}
			}
			#endregion

			#region Data
			IntPtr _msgArg;
			uint _length;
			bool _isDisposed = false;
			#endregion
		}
	}
}
