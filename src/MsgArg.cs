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
			 * Gets the byte value of a MsgArg Object
			 *
			 * @param arg	MsgArg used to access value from
			 * @return byte value of the MsgArg Object
			 */
			public static implicit operator byte(MsgArg arg)
			{
				byte y;
				alljoyn_msgarg_get(alljoyn_msgarg_array_element(arg._msgArgs, (UIntPtr)arg._index), "y", out y);
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
				alljoyn_msgarg_get(alljoyn_msgarg_array_element(arg._msgArgs, (UIntPtr)arg._index), "b", out b);
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
				alljoyn_msgarg_get(alljoyn_msgarg_array_element(arg._msgArgs, (UIntPtr)arg._index), "n", out n);
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
				alljoyn_msgarg_get(alljoyn_msgarg_array_element(arg._msgArgs, (UIntPtr)arg._index), "q", out q);
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
				alljoyn_msgarg_get(alljoyn_msgarg_array_element(arg._msgArgs, (UIntPtr)arg._index), "i", out i);
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
				alljoyn_msgarg_get(alljoyn_msgarg_array_element(arg._msgArgs, (UIntPtr)arg._index), "u", out u);
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
				alljoyn_msgarg_get(alljoyn_msgarg_array_element(arg._msgArgs, (UIntPtr)arg._index), "x", out x);
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
				alljoyn_msgarg_get(alljoyn_msgarg_array_element(arg._msgArgs, (UIntPtr)arg._index), "t", out t);
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
				alljoyn_msgarg_get(alljoyn_msgarg_array_element(arg._msgArgs, (UIntPtr)arg._index), "d", out d);
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
				alljoyn_msgarg_get(alljoyn_msgarg_array_element(arg._msgArgs, (UIntPtr)arg._index), "d", out d);
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
				alljoyn_msgarg_get(alljoyn_msgarg_array_element(arg._msgArgs, (UIntPtr)arg._index), "s", out s);
				return Marshal.PtrToStringAnsi(s);
			}

			/** 
			 * Gets the byte array of a MsgArg Object
			 *
			 * @param arg	MsgArg used to access value from
			 * @return byte array of a MsgArg Object
			 */
		    public static implicit operator byte[](MsgArg arg)
		    {
			return alljoyn_msgarg_as_array(arg._msgArgs, (UIntPtr)arg._index);
		    }

			/**
			 * Gets or Sets the value of the ObjectPath
			 */
			public string ObjectPath
			{
				get
				{
					IntPtr o;
					alljoyn_msgarg_get(alljoyn_msgarg_array_element(_msgArgs, (UIntPtr)_index), "o", out o);
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
					alljoyn_msgarg_set(alljoyn_msgarg_array_element(_msgArgs, (UIntPtr)_index), "o", _bytePtr);
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
				string signature = "";
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
					signature = "s";
					_bytePtr = Marshal.StringToCoTaskMemAnsi((string)value);
					return alljoyn_msgarg_set(alljoyn_msgarg_array_element(_msgArgs, (UIntPtr)_index), signature, _bytePtr);
				}
				else if(value.GetType() == typeof(bool))
				{
					signature = "b";
					//int newValue = ((bool)value ? 1 : 0);
					return alljoyn_msgarg_set(alljoyn_msgarg_array_element(_msgArgs, (UIntPtr)_index), signature, (bool)value);
				}
				else if (value.GetType() == typeof(float))
				{
					signature = "d";
					//explicitly cast float to a double.
					double d = Convert.ToDouble((float)value);
					//TODO figure out why call to alljoy_msgarg_set does not work for doubles
					//return alljoyn_msgarg_set(alljoyn_msgarg_array_element(_msgArgs, (UIntPtr)_index), signature, (double)d);
					return alljoyn_msgarg_array_set_offset(_msgArgs, (UIntPtr)_index, ref numArgs, signature, (double)d);
				}
				else if(value.GetType() == typeof(double))
				{
					signature = "d";
					//TODO figure out why call to alljoy_msgarg_set does not work for doubles
					//return alljoyn_msgarg_set(alljoyn_msgarg_array_element(_msgArgs, (UIntPtr)_index), signature, (double)value);
					return alljoyn_msgarg_array_set_offset(_msgArgs, (UIntPtr)_index, ref numArgs, signature, (double)value);
				}
				else if(value.GetType() == typeof(int))
				{
					signature = "i";
					return alljoyn_msgarg_set(alljoyn_msgarg_array_element(_msgArgs, (UIntPtr)_index), signature, (int)value);
				}
				else if(value.GetType() == typeof(uint))
				{
					signature = "u";
					return alljoyn_msgarg_set(alljoyn_msgarg_array_element(_msgArgs, (UIntPtr)_index), signature, (uint)value);
				}
				else if(value.GetType() == typeof(short))
				{
					signature = "n";
					return alljoyn_msgarg_set(alljoyn_msgarg_array_element(_msgArgs, (UIntPtr)_index), signature, (short)value);
				}
				else if(value.GetType() == typeof(ushort))
				{
					signature = "q";
					return alljoyn_msgarg_set(alljoyn_msgarg_array_element(_msgArgs, (UIntPtr)_index), signature, (ushort)value);
				}
				else if(value.GetType() == typeof(long))
				{
					signature = "x";
					return alljoyn_msgarg_set(alljoyn_msgarg_array_element(_msgArgs, (UIntPtr)_index), signature, (long)value);
				}
				else if(value.GetType() == typeof(ulong))
				{
					signature = "t";
					return alljoyn_msgarg_set(alljoyn_msgarg_array_element(_msgArgs, (UIntPtr)_index), signature, (ulong)value);
				}
				else if(value.GetType() == typeof(byte))
				{
					signature = "y";
					return alljoyn_msgarg_set(alljoyn_msgarg_array_element(_msgArgs, (UIntPtr)_index), signature, (byte)value);
				}
				return QStatus.WRITE_ERROR;
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
			private static extern int alljoyn_msgarg_set(IntPtr arg, [MarshalAs(UnmanagedType.LPStr)] string signature, byte y);
			[DllImport(DLL_IMPORT_TARGET)]
			private static extern int alljoyn_msgarg_set(IntPtr arg, [MarshalAs(UnmanagedType.LPStr)] string signature, bool b);
			[DllImport(DLL_IMPORT_TARGET)]
			private static extern int alljoyn_msgarg_set(IntPtr arg, [MarshalAs(UnmanagedType.LPStr)] string signature, short n);
			[DllImport(DLL_IMPORT_TARGET)]
			private static extern int alljoyn_msgarg_set(IntPtr arg, [MarshalAs(UnmanagedType.LPStr)] string signature, ushort q);
			[DllImport(DLL_IMPORT_TARGET)]
			private static extern int alljoyn_msgarg_set(IntPtr arg, [MarshalAs(UnmanagedType.LPStr)] string signature, int i);
			[DllImport(DLL_IMPORT_TARGET)]
			private static extern int alljoyn_msgarg_set(IntPtr arg, [MarshalAs(UnmanagedType.LPStr)] string signature, uint u);
			[DllImport(DLL_IMPORT_TARGET)]
			private static extern int alljoyn_msgarg_set(IntPtr arg, [MarshalAs(UnmanagedType.LPStr)] string signature, long x);
			[DllImport(DLL_IMPORT_TARGET)]
			private static extern int alljoyn_msgarg_set(IntPtr arg, [MarshalAs(UnmanagedType.LPStr)] string signature, ulong t);
			[DllImport(DLL_IMPORT_TARGET)]
			private static extern int alljoyn_msgarg_set(IntPtr arg, [MarshalAs(UnmanagedType.LPStr)] string signature, double d);
			/* char* string arrays are passed as an IntPtr */
			[DllImport(DLL_IMPORT_TARGET)]
			private static extern int alljoyn_msgarg_set(IntPtr arg, [MarshalAs(UnmanagedType.LPStr)] string signature, IntPtr s);
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
			private static extern int alljoyn_msgarg_get(IntPtr arg, [MarshalAs(UnmanagedType.LPStr)] string signature, out byte y);
			[DllImport(DLL_IMPORT_TARGET)]
			private static extern int alljoyn_msgarg_get(IntPtr arg, [MarshalAs(UnmanagedType.LPStr)] string signature, out bool b);
			[DllImport(DLL_IMPORT_TARGET)]
			private static extern int alljoyn_msgarg_get(IntPtr arg, [MarshalAs(UnmanagedType.LPStr)] string signature, out short n);
			[DllImport(DLL_IMPORT_TARGET)]
			private static extern int alljoyn_msgarg_get(IntPtr arg, [MarshalAs(UnmanagedType.LPStr)] string signature, out ushort q);
			[DllImport(DLL_IMPORT_TARGET)]
			private static extern int alljoyn_msgarg_get(IntPtr arg, [MarshalAs(UnmanagedType.LPStr)] string signature, out int i);
			[DllImport(DLL_IMPORT_TARGET)]
			private static extern int alljoyn_msgarg_get(IntPtr arg, [MarshalAs(UnmanagedType.LPStr)] string signature, out uint i);
			[DllImport(DLL_IMPORT_TARGET)]
			private static extern int alljoyn_msgarg_get(IntPtr arg, [MarshalAs(UnmanagedType.LPStr)] string signature, out long x);
			[DllImport(DLL_IMPORT_TARGET)]
			private static extern int alljoyn_msgarg_get(IntPtr arg, [MarshalAs(UnmanagedType.LPStr)] string signature, out ulong t);
			[DllImport(DLL_IMPORT_TARGET)]
			private static extern int alljoyn_msgarg_get(IntPtr arg, [MarshalAs(UnmanagedType.LPStr)] string signature, out double d);
			/* char* string arrays are passed as an IntPtr */
			[DllImport(DLL_IMPORT_TARGET)]
			private static extern int alljoyn_msgarg_get(IntPtr arg, [MarshalAs(UnmanagedType.LPStr)] string signature, out IntPtr s);

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
