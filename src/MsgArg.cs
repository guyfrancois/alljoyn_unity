/**
 * @file
 * This file defines a class for message bus data types and values
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

using System;
using System.Runtime.InteropServices;

namespace AllJoynUnity
{
	public partial class AllJoyn
	{
		/**
		 * Class definition for a message arg.
		 * This class deals with the message bus types and the operations on them
		 *
		 * MsgArg's are designed to be light-weight. A MsgArg will normally hold references to the data
		 * (strings etc.) it wraps and will only copy that data if the MsgArg is assigned. For example no
		 * additional memory is allocated for an ALLJOYN_STRING that references an existing const char*.
		 * If a MsgArg is assigned the destination receives a copy of the contents of the source. The
		 * Stabilize() methods can also be called to explicitly force contents of the MsgArg to be copied.
		 */
		public class MsgArg : IDisposable
		{
			internal MsgArg(MsgArgs owner, uint index)
			{
			
				_msgArgs = owner.UnmanagedPtr;
				_index = index;
			}

			internal MsgArg(IntPtr msgArgs)
			{
			
				_msgArgs = msgArgs;
				_index = 0;
			}

			internal MsgArg(object val)
			{
			
				_msgArgs = IntPtr.Zero;
				_index = 0;
				_setValue = val;
			}

			/** 
			 * Gets a new MsgArg containing the byte arg value
			 *
			 * @param y byte to assign to the MsgArg object
			 * @return a new MsgArg object
			 */
			public static implicit operator MsgArg(byte y)
			{
				return new MsgArg(y);
			}

			/** 
			 * Gets a new MsgArg containing the short arg value
			 *
			 * @param n short to assign to the MsgArg object
			 * @return a new MsgArg object
			 */
			public static implicit operator MsgArg(short n)
			{
				return new MsgArg(n);
			}

			/** 
			 * Gets a new MsgArg containing the ushort arg value
			 *
			 * @param q ushort to assign to the MsgArg object
			 * @return a new MsgArg object
			 */
			public static implicit operator MsgArg(ushort q)
			{
				return new MsgArg(q);
			}

			/** 
			 * Gets a new MsgArg containing the int arg value
			 *
			 * @param i int to assign to the MsgArg object
			 * @return a new MsgArg object
			 */
			public static implicit operator MsgArg(int i)
			{
				return new MsgArg(i);
			}

			/** 
			 * Gets a new MsgArg containing the uint arg value
			 *
			 * @param u uint to assign to the MsgArg object
			 * @return a new MsgArg object
			 */
			public static implicit operator MsgArg(uint u)
			{
				return new MsgArg(u);
			}


			/** 
			 * Gets a new MsgArg containing the bool arg value
			 *
			 * @param b bool to assign to the MsgArg object
			 * @return a new MsgArg object
			 */
			public static implicit operator MsgArg(bool b)
			{
				return new MsgArg(b);
			}


			/** 
			 * Gets a new MsgArg containing the long arg value
			 *
			 * @param x long to assign to the MsgArg object
			 * @return a new MsgArg object
			 */
			public static implicit operator MsgArg(long x)
			{
				return new MsgArg(x);
			}

			/** 
			 * Gets a new MsgArg containing the ulong arg value
			 *
			 * @param t ulong to assign to the MsgArg object
			 * @return a new MsgArg object
			 */
			public static implicit operator MsgArg(ulong t)
			{
				return new MsgArg(t);
			}

			/** 
			 * Gets a new MsgArg containing the double arg value
			 *
			 * @param d double to assign to the MsgArg object
			 * @return a new MsgArg object
			 */
			public static implicit operator MsgArg(double d)
			{
				return new MsgArg(d);
			}

			/** 
			 * Gets a new MsgArg containing the string arg value
			 *
			 * @param arg string to assign to the MsgArg object
			 * @return a new MsgArg object
			 */
			public static implicit operator MsgArg(string arg)
			{
				return new MsgArg(arg);
			}

			/** 
			 * Gets a new MsgArg containing the byte[] arg value
			 *
			 * @param ay byte array to assign to the MsgArg object
			 * @return a new MsgArg object
			 */
			public static implicit operator MsgArg(byte[] ay)
			{
				return new MsgArg(ay);
			}

			/** 
			 * Gets a new MsgArg containing the bool[] arg value
			 *
			 * @param ab bool array to assign to the MsgArg object
			 * @return a new MsgArg object
			 */
			public static implicit operator MsgArg(bool[] ab)
			{
				return new MsgArg(ab);
			}

			/** 
			 * Gets a new MsgArg containing the short[] arg value
			 *
			 * @param an short array to assign to the MsgArg object
			 * @return a new MsgArg object
			 */
			public static implicit operator MsgArg(short[] an)
			{
				return new MsgArg(an);
			}

			/** 
			 * Gets a new MsgArg containing the ushort[] arg value
			 *
			 * @param aq ushort to assign to the MsgArg object
			 * @return a new MsgArg object
			 */
			public static implicit operator MsgArg(ushort[] aq)
			{
				return new MsgArg(aq);
			}

			/** 
			 * Gets a new MsgArg containing the int[] arg value
			 *
			 * @param ai int to assign to the MsgArg object
			 * @return a new MsgArg object
			 */
			public static implicit operator MsgArg(int[] ai)
			{
				return new MsgArg(ai);
			}

			/** 
			 * Gets a new MsgArg containing the uint[] arg value
			 *
			 * @param au uint array  to assign to the MsgArg object
			 * @return a new MsgArg object
			 */
			public static implicit operator MsgArg(uint[] au)
			{
				return new MsgArg(au);
			}

			/** 
			 * Gets a new MsgArg containing the long[] arg value
			 *
			 * @param ax long array to assign to the MsgArg object
			 * @return a new MsgArg object
			 */
			public static implicit operator MsgArg(long[] ax)
			{
				return new MsgArg(ax);
			}

			/** 
			 * Gets a new MsgArg containing the ulong[] arg value
			 *
			 * @param at ulong array to assign to the MsgArg object
			 * @return a new MsgArg object
			 */
			public static implicit operator MsgArg(ulong[] at)
			{
				return new MsgArg(at);
			}

			/** 
			 * Gets a new MsgArg containing the double[] arg value
			 *
			 * @param ad double array to assign to the MsgArg object
			 * @return a new MsgArg object
			 */
			public static implicit operator MsgArg(double[] ad)
			{
				return new MsgArg(ad);
			}

			/** 
			 * Gets a new MsgArg containing the string[] arg value
			 *
			 * @param sa string array to assign to the MsgArg object
			 * @return a new MsgArg object
			 */
			public static implicit operator MsgArg(string[] sa)
			{
				return new MsgArg(sa);
			}

			/** 
			 * Gets the byte value of a MsgArg Object
			 *
			 * @param arg	MsgArg used to access value from
			 * @return byte value of the MsgArg Object
			 */
			public static implicit operator byte(MsgArg arg)
			{
				byte y;
				alljoyn_msgarg_get_uint8(alljoyn_msgarg_array_element(arg._msgArgs, (UIntPtr)arg._index), out y);
				return y;
			}

			/** 
			 * Gets the bool value of a MsgArg Object
			 *
			 * @param arg	MsgArg used to access value from
			 * @return bool value of a MsgArg Object
			 */
			public static implicit operator bool(MsgArg arg)
			{
				bool b;
				alljoyn_msgarg_get_bool(alljoyn_msgarg_array_element(arg._msgArgs, (UIntPtr)arg._index), out b);
				return b;
			}

			/** 
			 * Gets the short value of a MsgArg Object
			 *
			 * @param arg	MsgArg used to access value from
			 * @return short value of a MsgArg Object
			 */
			public static implicit operator short(MsgArg arg)
			{

				short n;
				alljoyn_msgarg_get_int16(alljoyn_msgarg_array_element(arg._msgArgs, (UIntPtr)arg._index), out n);
				return n;
			}

			/** 
			 * Gets the ushort value of a MsgArg Object
			 *
			 * @param arg	MsgArg used to access value from
			 * @return ushort value of a MsgArg Object
			 */
			public static implicit operator ushort(MsgArg arg)
			{
				ushort q;
				alljoyn_msgarg_get_uint16(alljoyn_msgarg_array_element(arg._msgArgs, (UIntPtr)arg._index), out q);
				return q;
			}

			/** 
			 * Gets the int value of a MsgArg Object
			 *
			 * @param arg	MsgArg used to access value from
			 * @return int value of a MsgArg Object
			 */
			public static implicit operator int(MsgArg arg)
			{
				int i;
				alljoyn_msgarg_get_int32(alljoyn_msgarg_array_element(arg._msgArgs, (UIntPtr)arg._index), out i);
				return i;
			}

			/** 
			 * Gets the uint value of a MsgArg Object
			 *
			 * @param arg	MsgArg used to access value from
			 * @return uint value of a MsgArg Object
			 */
			public static implicit operator uint(MsgArg arg)
			{
				uint u;
				alljoyn_msgarg_get_uint32(alljoyn_msgarg_array_element(arg._msgArgs, (UIntPtr)arg._index), out u);
				return u;
			}

			/** 
			 * Gets the long value of a MsgArg Object
			 *
			 * @param arg	MsgArg used to access value from
			 * @return long value of a MsgArg Object
			 */
			public static implicit operator long(MsgArg arg)
			{
				long x;
				alljoyn_msgarg_get_int64(alljoyn_msgarg_array_element(arg._msgArgs, (UIntPtr)arg._index), out x);
				return x;
			}

			/** 
			 * Gets the ulong value of a MsgArg Object
			 *
			 * @param arg	MsgArg used to access value from
			 * @return ulong value of a MsgArg Object
			 */
			public static implicit operator ulong(MsgArg arg)
			{
				ulong t;
				alljoyn_msgarg_get_uint64(alljoyn_msgarg_array_element(arg._msgArgs, (UIntPtr)arg._index), out t);
				return t;
			}

			/** 
			 * Gets the float value of a MsgArg Object. 
			 * 
			 * AllJoyn can not marshal data of type float. Any float obtained from 
			 * AllJoyn is cast from a double so using type fload may result in data
			 * loss through truncation.
			 *
			 * @param arg	MsgArg used to access value from
			 * @return float value of a MsgArg Object
			 */
			public static implicit operator float(MsgArg arg)
			{
				double d;
				alljoyn_msgarg_get_double(alljoyn_msgarg_array_element(arg._msgArgs, (UIntPtr)arg._index), out d);
				// convert double to a float.
				float f = (float)d;
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
				double d;
				alljoyn_msgarg_get_double(alljoyn_msgarg_array_element(arg._msgArgs, (UIntPtr)arg._index), out d);
				return d;
			}

			/** 
			 * Gets the string value of a MsgArg Object
			 *
			 * @param arg	MsgArg used to access value from
			 * @return string value of a MsgArg Object
			 */
			public static implicit operator string(MsgArg arg)
			{
				IntPtr s;
				alljoyn_msgarg_get_string(alljoyn_msgarg_array_element(arg._msgArgs, (UIntPtr)arg._index), out s);
				return Marshal.PtrToStringAnsi(s);
			}

			/** 
			 * Gets the byte array of a MsgArg Object
			 *
			 * @param arg	MsgArg used to access value from
			 * @return byte array of a MsgArg Object
			 */
		    //public static implicit operator byte[](MsgArg arg)
		    //{
			//return alljoyn_msgarg_as_array(arg._msgArgs, (UIntPtr)arg._index);
		    //}

			/** 
			 * Gets byte array value from a MsgArg Object
			 *
			 * @param arg	MsgArg used to access value from
			 * @return byte array value from the MsgArg Object
			 */
			public static implicit operator byte[](MsgArg arg)
			{
				int length;
				IntPtr ay;
				alljoyn_msgarg_get_uint8_array(alljoyn_msgarg_array_element(arg._msgArgs, (UIntPtr)arg._index), out length, out ay);
				byte[] result = new byte[length];
				Marshal.Copy(ay, result, 0, length);
				return result;
			}

			/** 
			 * Gets bool array value from a MsgArg Object
			 *
			 * @param arg	MsgArg used to access value from
			 * @return bool array value from the MsgArg Object
			 */
			public static implicit operator bool[](MsgArg arg)
			{
				int length;
				IntPtr ab;
				alljoyn_msgarg_get_bool_array(alljoyn_msgarg_array_element(arg._msgArgs, (UIntPtr)arg._index), out length, out ab);
				Int32[] result = new Int32[length];
				Marshal.Copy(ab, result, 0, length);
				bool[] retValue = new bool[length];
				for (int i = 0; i < length; i++)
				{
					if (result[i] == 0)
					{
						retValue[i] = false;
					}
					else
					{
						retValue[i] = true;
					}
				}
				return retValue;
			}

			/** 
			 * Gets short array value from a MsgArg Object
			 *
			 * @param arg	MsgArg used to access value from
			 * @return short array value from the MsgArg Object
			 */
			public static implicit operator short[](MsgArg arg)
			{
				int length;
				IntPtr an;
				alljoyn_msgarg_get_int16_array(alljoyn_msgarg_array_element(arg._msgArgs, (UIntPtr)arg._index), out length, out an);
				short[] result = new short[length];
				Marshal.Copy(an, result, 0, length);
				return result;
			}

			/** 
			 * Gets ushort array value from a MsgArg Object
			 *
			 * @param arg	MsgArg used to access value from
			 * @return ushort array value from the MsgArg Object
			 */
			public static implicit operator ushort[](MsgArg arg)
			{
				int length;
				IntPtr aq;
				alljoyn_msgarg_get_uint16_array(alljoyn_msgarg_array_element(arg._msgArgs, (UIntPtr)arg._index), out length, out aq);
				short[] tmp = new short[length];
				Marshal.Copy(aq, tmp, 0, length);

				ShortConverter converter = new ShortConverter();
				converter.Shorts = tmp;
				return converter.UShorts;
			}

			/** 
			 * Gets int array value from a MsgArg Object
			 *
			 * @param arg	MsgArg used to access value from
			 * @return int array value from the MsgArg Object
			 */
			public static implicit operator int[](MsgArg arg)
			{
				int length;
				IntPtr ai;
				alljoyn_msgarg_get_int32_array(alljoyn_msgarg_array_element(arg._msgArgs, (UIntPtr)arg._index), out length, out ai);
				int[] result = new int[length];
				Marshal.Copy(ai, result, 0, length);
				return result;
			}

			/** 
			 * Gets uint array value from a MsgArg Object
			 *
			 * @param arg	MsgArg used to access value from
			 * @return uint array value from the MsgArg Object
			 */
			public static implicit operator uint[](MsgArg arg)
			{
				int length;
				IntPtr au;
				alljoyn_msgarg_get_uint32_array(alljoyn_msgarg_array_element(arg._msgArgs, (UIntPtr)arg._index), out length, out au);
				int[] result = new int[length];
				Marshal.Copy(au, result, 0, length);

				IntConverter converter = new IntConverter();
				converter.Ints = result;
				return converter.UInts;
			}

			/** 
			 * Gets a long array value from a MsgArg Object
			 *
			 * @param arg	MsgArg used to access value from
			 * @return long array value from the MsgArg Object
			 */
			public static implicit operator long[](MsgArg arg)
			{
				int length;
				IntPtr ax;
				alljoyn_msgarg_get_int64_array(alljoyn_msgarg_array_element(arg._msgArgs, (UIntPtr)arg._index), out length, out ax);
				long[] result = new long[length];
				Marshal.Copy(ax, result, 0, length);
				return result;
			}

			/** 
			 * Gets a ulong array value from a MsgArg Object
			 *
			 * @param arg	MsgArg used to access value from
			 * @return ulong array value from the MsgArg Object
			 */
			public static implicit operator ulong[](MsgArg arg)
			{
				int length;
				IntPtr at;
				alljoyn_msgarg_get_uint64_array(alljoyn_msgarg_array_element(arg._msgArgs, (UIntPtr)arg._index), out length, out at);
				long[] result = new long[length];
				Marshal.Copy(at, result, 0, length);

				LongConverter converter = new LongConverter();
				converter.Longs = result;
				return converter.ULongs;
			}

			/** 
			 * Gets a double array value from a MsgArg Object
			 *
			 * @param arg	MsgArg used to access value from
			 * @return double array value from the MsgArg Object
			 */
			public static implicit operator double[](MsgArg arg)
			{
				int length;
				IntPtr ad;
				alljoyn_msgarg_get_double_array(alljoyn_msgarg_array_element(arg._msgArgs, (UIntPtr)arg._index), out length, out ad);
				double[] result = new double[length];
				Marshal.Copy(ad, result, 0, length);
				return result;
			}

			/** 
			 * Gets string array value from a MsgArg Object
			 *
			 * @param arg	MsgArg used to access value from
			 * @return string array value from the MsgArg Object
			 */
			public static implicit operator string[](MsgArg arg)
			{
				int length;
				IntPtr sa;
				alljoyn_msgarg_get_string_array(alljoyn_msgarg_array_element(arg._msgArgs, (UIntPtr)arg._index), out length, out sa);

				string[] result = new string[length];
				for (int i = 0; i < length; ++i)
				{
					IntPtr s;
					alljoyn_msgarg_get_string(alljoyn_msgarg_array_element(sa, (UIntPtr)i), out s);
					result[i] = Marshal.PtrToStringAnsi(s);
				}
				return result;
			}

			/**
			 * Gets or Sets the value of the ObjectPath
			 */
			public string ObjectPath
			{
				get
				{
					IntPtr o;
					alljoyn_msgarg_get_objectpath(alljoyn_msgarg_array_element(_msgArgs, (UIntPtr)_index), out o);
					return Marshal.PtrToStringAnsi(o);
				}
				set
				{
					if(_bytePtr != IntPtr.Zero)
					{
						Marshal.FreeCoTaskMem(_bytePtr);
						_bytePtr = IntPtr.Zero;
					}
					UIntPtr numArgs = (UIntPtr)1;
					_bytePtr = Marshal.StringToCoTaskMemAnsi((string)value);
					alljoyn_msgarg_set_objectpath(alljoyn_msgarg_array_element(_msgArgs, (UIntPtr)_index), _bytePtr);
				}
			}

			/**
			 * Gets the value of the Variant
			 */
			public MsgArg Variant
			{
				get
				{
					return new MsgArg(alljoyn_msgarg_as_variant(_msgArgs, (UIntPtr)_index));
				}
			}

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
			
				UIntPtr numArgs = (UIntPtr)1;
				_setValue = value;

				if(_bytePtr != IntPtr.Zero)
				{
					Marshal.FreeCoTaskMem(_bytePtr);
					_bytePtr = IntPtr.Zero;
				}

				/*
				ALLJOYN_ARRAY            = 'a',    ///< AllJoyn array container type
				ALLJOYN_DICT_ENTRY       = 'e',    ///< AllJoyn dictionary or map container type - an array of key-value pairs
				ALLJOYN_SIGNATURE        = 'g',    ///< AllJoyn signature basic type
				ALLJOYN_HANDLE           = 'h',    ///< AllJoyn socket handle basic type
				ALLJOYN_STRUCT           = 'r',    ///< AllJoyn struct container type
				*/

				if(value.GetType() == typeof(string))
				{
					//signature = "s";
					_bytePtr = Marshal.StringToCoTaskMemAnsi((string)value);
					return alljoyn_msgarg_set_string(alljoyn_msgarg_array_element(_msgArgs, (UIntPtr)_index),_bytePtr);
				}
				else if(value.GetType() == typeof(bool))
				{
					//signature = "b";
					//int newValue = ((bool)value ? 1 : 0);
					return alljoyn_msgarg_set_bool(alljoyn_msgarg_array_element(_msgArgs, (UIntPtr)_index), (bool)value);
				}
				else if (value.GetType() == typeof(float))
				{
					//signature = "d";
					//explicitly cast float to a double.
					double d = Convert.ToDouble((float)value);
					//TODO figure out why call to alljoy_msgarg_set does not work for doubles
					return alljoyn_msgarg_set_double(alljoyn_msgarg_array_element(_msgArgs, (UIntPtr)_index), (double)d);
					//return alljoyn_msgarg_array_set_offset(_msgArgs, (UIntPtr)_index, ref numArgs, signature, (double)d);
				}
				else if(value.GetType() == typeof(double))
				{
					//signature = "d";
					//TODO figure out why call to alljoy_msgarg_set does not work for doubles
					return alljoyn_msgarg_set_double(alljoyn_msgarg_array_element(_msgArgs, (UIntPtr)_index), (double)value);
					//return alljoyn_msgarg_array_set_offset(_msgArgs, (UIntPtr)_index, ref numArgs, signature, (double)value);
				}
				else if(value.GetType() == typeof(int))
				{
					//signature = "i";
					return alljoyn_msgarg_set_int32(alljoyn_msgarg_array_element(_msgArgs, (UIntPtr)_index),(int)value);
				}
				else if(value.GetType() == typeof(uint))
				{
					//signature = "u";
					return alljoyn_msgarg_set_uint32(alljoyn_msgarg_array_element(_msgArgs, (UIntPtr)_index), (uint)value);
				}
				else if(value.GetType() == typeof(short))
				{
					//signature = "n";
					return alljoyn_msgarg_set_int16(alljoyn_msgarg_array_element(_msgArgs, (UIntPtr)_index), (short)value);
				}
				else if(value.GetType() == typeof(ushort))
				{
					//signature = "q";
					return alljoyn_msgarg_set_uint16(alljoyn_msgarg_array_element(_msgArgs, (UIntPtr)_index), (ushort)value);
				}
				else if(value.GetType() == typeof(long))
				{
					//signature = "x";
					return alljoyn_msgarg_set_int64(alljoyn_msgarg_array_element(_msgArgs, (UIntPtr)_index), (long)value);
				}
				else if(value.GetType() == typeof(ulong))
				{
					//signature = "t";
					return alljoyn_msgarg_set_uint64(alljoyn_msgarg_array_element(_msgArgs, (UIntPtr)_index), (ulong)value);
				}
				else if(value.GetType() == typeof(byte))
				{
					//signature = "y";
					return alljoyn_msgarg_set_uint8(alljoyn_msgarg_array_element(_msgArgs, (UIntPtr)_index), (byte)value);
				}
				else if (value.GetType() == typeof(byte[]))
				{
					//signature = "ay";
					return alljoyn_msgarg_set_uint8_array(alljoyn_msgarg_array_element(_msgArgs, (UIntPtr)_index), ((byte[])value).Length, (byte[])value);
				}
				else if (value.GetType() == typeof(bool[]))
				{
					//signature = "ab";
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
					return alljoyn_msgarg_set_bool_array(alljoyn_msgarg_array_element(_msgArgs, (UIntPtr)_index), ab.Length, ab);
				}
				else if (value.GetType() == typeof(short[]))
				{
					//signature = "an";
					return alljoyn_msgarg_set_int16_array(alljoyn_msgarg_array_element(_msgArgs, (UIntPtr)_index), ((short[])value).Length, (short[])value);
				}
				else if (value.GetType() == typeof(ushort[]))
				{
					//signature = "aq";
					return alljoyn_msgarg_set_uint16_array(alljoyn_msgarg_array_element(_msgArgs, (UIntPtr)_index), ((ushort[])value).Length, (ushort[])value);
				}
				else if (value.GetType() == typeof(int[]))
				{
					//signature = "ai";
					return alljoyn_msgarg_set_int32_array(alljoyn_msgarg_array_element(_msgArgs, (UIntPtr)_index), ((int[])value).Length, (int[])value);
				}
				else if(value.GetType() == typeof(uint[]))
				{
					//signature = "au";
					return alljoyn_msgarg_set_uint32_array(alljoyn_msgarg_array_element(_msgArgs, (UIntPtr)_index), ((uint[])value).Length, (uint[])value);
				}
				else if (value.GetType() == typeof(long[]))
				{
					//signature = "ax";
					return alljoyn_msgarg_set_int64_array(alljoyn_msgarg_array_element(_msgArgs, (UIntPtr)_index), ((long[])value).Length, (long[])value);
				}
				else if (value.GetType() == typeof(ulong[]))
				{
					//signature = "ai";
					return alljoyn_msgarg_set_uint64_array(alljoyn_msgarg_array_element(_msgArgs, (UIntPtr)_index), ((ulong[])value).Length, (ulong[])value);
				}
				else if (value.GetType() == typeof(double[]))
				{
					//signature = "ad";
					return alljoyn_msgarg_set_double_array(alljoyn_msgarg_array_element(_msgArgs, (UIntPtr)_index), ((double[])value).Length, (double[])value);
				}
				else if (value.GetType() == typeof(string[]))
				{
				    //signature = "as";
					//IntPtr a_size = new IntPtr(((string[])value).Length);
					return alljoyn_msgarg_set_string_array(alljoyn_msgarg_array_element(_msgArgs, (UIntPtr)_index), ((string[])value).Length, (string[])value);
				}
				return QStatus.WRITE_ERROR;
			}

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

			#region DLL Imports
			[DllImport(DLL_IMPORT_TARGET)]
			private static extern IntPtr alljoyn_msgarg_as_variant(IntPtr args, UIntPtr idx);

            [DllImport(DLL_IMPORT_TARGET)]
            private static extern byte[] alljoyn_msgarg_as_array(IntPtr args, UIntPtr idx);

			[DllImport(DLL_IMPORT_TARGET)]
			private static extern int alljoyn_msgarg_array_set_offset(IntPtr args, UIntPtr argOffset, ref UIntPtr numArgs,
				[MarshalAs(UnmanagedType.LPStr)] string signature, double arg);

			/* 
			 * allthough __arglist works really well when using microsoft C# 
			 * problems have been encountered when using it with mono for this
			 * reason individual implementations of the call for basic types is
			 * implemented. For the time being this dll import is being left in
			 * untill a way to handle more AllJoyn types than basic data types
			 * is figured out.
			 */
			[DllImport (DLL_IMPORT_TARGET)]
			private static extern int alljoyn_msgarg_set(IntPtr arg, [MarshalAs(UnmanagedType.LPStr)] string signature, __arglist);
			  

			[DllImport(DLL_IMPORT_TARGET)]
			private static extern int alljoyn_msgarg_set_uint8(IntPtr arg, byte y);
			[DllImport(DLL_IMPORT_TARGET)]
			private static extern int alljoyn_msgarg_set_bool(IntPtr arg, bool b);
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
			/* 
			 * allthough __arglist works really well when using microsoft C# 
			 * problems have been encountered when using it with mono for this
			 * reason individual implementations of the call for basic types is
			 * implemented. For the time being this dll import is being left in
			 * untill a way to handle more AllJoyn types than basic data types
			 * is figured out.
			 */
			[DllImport (DLL_IMPORT_TARGET)]
			private static extern int alljoyn_msgarg_get(IntPtr args, [MarshalAs(UnmanagedType.LPStr)] string signature, __arglist);
			 

			[DllImport(DLL_IMPORT_TARGET)]
			private static extern int alljoyn_msgarg_get_uint8(IntPtr arg, out byte y);
			[DllImport(DLL_IMPORT_TARGET)]
			private static extern int alljoyn_msgarg_get_bool(IntPtr arg, out bool b);
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
			[DllImport(DLL_IMPORT_TARGET, CharSet = CharSet.Ansi)]
			private static extern int alljoyn_msgarg_set_string_array(IntPtr arg, int length, [In]string[] sa);
			[DllImport(DLL_IMPORT_TARGET, CharSet = CharSet.Ansi)]
			private static extern int alljoyn_msgarg_set_objectpath_array(IntPtr arg, int length, [In][MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr)] string[] ao);
			[DllImport(DLL_IMPORT_TARGET, CharSet = CharSet.Ansi)]
			private static extern int alljoyn_msgarg_set_signature_array(IntPtr arg, int length, [In][MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr)] string[] ag);

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
			private static extern IntPtr alljoyn_msgarg_array_element(IntPtr args, UIntPtr index);
			#endregion

			#region IDisposable
			/**
			 * Dispose the MsgArg
			 */
			public void Dispose()
			{
				Dispose(true);
				GC.SuppressFinalize(this); 
			}

			/**
			 * Dispose the MsgArg
			 * @param disposing	describes if its activly being disposed
			 */
			protected virtual void Dispose(bool disposing)
			{
			
				if(!_isDisposed)
				{
					if(_bytePtr != IntPtr.Zero)
					{
						Marshal.FreeCoTaskMem(_bytePtr);
						_bytePtr = IntPtr.Zero;
					}
				}
				_isDisposed = true;
			}

			~MsgArg()
			{
			
				Dispose(false);
			}
			#endregion

			#region Data
			IntPtr _msgArgs;
			uint _index;
			internal object _setValue;
			IntPtr _bytePtr;
			bool _isDisposed = false;
			#endregion
		}
	}
}
