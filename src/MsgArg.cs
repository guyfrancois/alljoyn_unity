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
			
				return alljoyn_msgarg_as_uint8(arg._msgArgs, (UIntPtr)arg._index);
			}

			/** 
			 * Gets the bool value of a MsgArg Object
			 *
			 * @param arg	MsgArg used to access value from
			 * @return bool value of a MsgArg Object
			 */
			public static implicit operator bool(MsgArg arg)
			{
			
				return (alljoyn_msgarg_as_bool(arg._msgArgs, (UIntPtr)arg._index) == 1 ? true : false);
			}

			/** 
			 * Gets the short value of a MsgArg Object
			 *
			 * @param arg	MsgArg used to access value from
			 * @return short value of a MsgArg Object
			 */
			public static implicit operator short(MsgArg arg)
			{
			
				return alljoyn_msgarg_as_int16(arg._msgArgs, (UIntPtr)arg._index);
			}

			/** 
			 * Gets the ushort value of a MsgArg Object
			 *
			 * @param arg	MsgArg used to access value from
			 * @return ushort value of a MsgArg Object
			 */
			public static implicit operator ushort(MsgArg arg)
			{
			
				return alljoyn_msgarg_as_uint16(arg._msgArgs, (UIntPtr)arg._index);
			}

			/** 
			 * Gets the int value of a MsgArg Object
			 *
			 * @param arg	MsgArg used to access value from
			 * @return int value of a MsgArg Object
			 */
			public static implicit operator int(MsgArg arg)
			{
			
				return alljoyn_msgarg_as_int32(arg._msgArgs, (UIntPtr)arg._index);
			}

			/** 
			 * Gets the uint value of a MsgArg Object
			 *
			 * @param arg	MsgArg used to access value from
			 * @return uint value of a MsgArg Object
			 */
			public static implicit operator uint(MsgArg arg)
			{
			
				return alljoyn_msgarg_as_uint32(arg._msgArgs, (UIntPtr)arg._index);
			}

			/** 
			 * Gets the long value of a MsgArg Object
			 *
			 * @param arg	MsgArg used to access value from
			 * @return long value of a MsgArg Object
			 */
			public static implicit operator long(MsgArg arg)
			{
			
				return alljoyn_msgarg_as_int64(arg._msgArgs, (UIntPtr)arg._index);
			}

			/** 
			 * Gets the ulong value of a MsgArg Object
			 *
			 * @param arg	MsgArg used to access value from
			 * @return ulong value of a MsgArg Object
			 */
			public static implicit operator ulong(MsgArg arg)
			{
			
				return alljoyn_msgarg_as_uint64(arg._msgArgs, (UIntPtr)arg._index);
			}

			/** 
			 * Gets the double value of a MsgArg Object
			 *
			 * @param arg	MsgArg used to access value from
			 * @return double value of a MsgArg Object
			 */
			public static implicit operator double(MsgArg arg)
			{
			
				return alljoyn_msgarg_as_double(arg._msgArgs, (UIntPtr)arg._index);
			}

			/** 
			 * Gets the string value of a MsgArg Object
			 *
			 * @param arg	MsgArg used to access value from
			 * @return string value of a MsgArg Object
			 */
			public static implicit operator string(MsgArg arg)
			{
			
				return Marshal.PtrToStringAnsi(alljoyn_msgarg_as_string(arg._msgArgs, (UIntPtr)arg._index));
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
					return Marshal.PtrToStringAnsi(alljoyn_msgarg_as_objpath(_msgArgs, (UIntPtr)_index));
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
					alljoyn_msgarg_array_set_offset(_msgArgs, (UIntPtr)_index, ref numArgs, "o", _bytePtr);
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
			     *              an array of const qcc::String.
			     *            - If the element type is an ALLJOYN_ARRAY "ARRAY", ALLJOYN_STRUCT "STRUCT",
			     *              ALLJOYN_DICT_ENTRY "DICT_ENTRY" or ALLJOYN_VARIANT "VARIANT" a pointer to an
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
			     *     string fruits[3] =  { "apple", "banana", "orange" };
			     *     MsgArg bowl;
			     *     bowl.Set(fruits);
			     *     @endcode
			     *
			     * @param value   The value for MsgArg value
			     *
			     * @return
			     *      - QStatus#OK if the MsgArg was successfully set
			     *      - An error status otherwise
			     */
			public void Set(object value)
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
					alljoyn_msgarg_array_set_offset(_msgArgs, (UIntPtr)_index, ref numArgs, signature, _bytePtr);
				}
				else if(value.GetType() == typeof(bool))
				{
					signature = "b";
					int newValue = ((bool)value ? 1 : 0);
					alljoyn_msgarg_array_set_offset(_msgArgs, (UIntPtr)_index, ref numArgs, signature, newValue);
				}
				else if(value.GetType() == typeof(double) || value.GetType() == typeof(float))
				{
					signature = "d";
					alljoyn_msgarg_array_set_offset(_msgArgs, (UIntPtr)_index, ref numArgs, signature, (double)value);
				}
				else if(value.GetType() == typeof(int))
				{
					signature = "i";
					alljoyn_msgarg_array_set_offset(_msgArgs, (UIntPtr)_index, ref numArgs, signature, (int)value);
				}
				else if(value.GetType() == typeof(uint))
				{
					signature = "u";
					alljoyn_msgarg_array_set_offset(_msgArgs, (UIntPtr)_index, ref numArgs, signature, (uint)value);
				}
				else if(value.GetType() == typeof(short))
				{
					signature = "n";
					alljoyn_msgarg_array_set_offset(_msgArgs, (UIntPtr)_index, ref numArgs, signature, (short)value);
				}
				else if(value.GetType() == typeof(ushort))
				{
					signature = "q";
					alljoyn_msgarg_array_set_offset(_msgArgs, (UIntPtr)_index, ref numArgs, signature, (ushort)value);
				}
				else if(value.GetType() == typeof(long))
				{
					signature = "x";
					alljoyn_msgarg_array_set_offset(_msgArgs, (UIntPtr)_index, ref numArgs, signature, (long)value);
				}
				else if(value.GetType() == typeof(ulong))
				{
					signature = "t";
					alljoyn_msgarg_array_set_offset(_msgArgs, (UIntPtr)_index, ref numArgs, signature, (ulong)value);
				}
				else if(value.GetType() == typeof(byte))
				{
					signature = "y";
					alljoyn_msgarg_array_set_offset(_msgArgs, (UIntPtr)_index, ref numArgs, signature, (byte)value);
				}
			}

			#region DLL Imports
			[DllImport(DLL_IMPORT_TARGET)]
			private static extern byte alljoyn_msgarg_as_uint8(IntPtr args, UIntPtr idx);

			[DllImport(DLL_IMPORT_TARGET)]
			private static extern int alljoyn_msgarg_as_bool(IntPtr args, UIntPtr idx);

			[DllImport(DLL_IMPORT_TARGET)]
			private static extern short alljoyn_msgarg_as_int16(IntPtr args, UIntPtr idx);

			[DllImport(DLL_IMPORT_TARGET)]
			private static extern ushort alljoyn_msgarg_as_uint16(IntPtr args, UIntPtr idx);

			[DllImport(DLL_IMPORT_TARGET)]
			private static extern int alljoyn_msgarg_as_int32(IntPtr args, UIntPtr idx);

			[DllImport(DLL_IMPORT_TARGET)]
			private static extern uint alljoyn_msgarg_as_uint32(IntPtr args, UIntPtr idx);

			[DllImport(DLL_IMPORT_TARGET)]
			private static extern long alljoyn_msgarg_as_int64(IntPtr args, UIntPtr idx);

			[DllImport(DLL_IMPORT_TARGET)]
			private static extern ulong alljoyn_msgarg_as_uint64(IntPtr args, UIntPtr idx);

			[DllImport(DLL_IMPORT_TARGET)]
			private static extern double alljoyn_msgarg_as_double(IntPtr args, UIntPtr idx);

			[DllImport(DLL_IMPORT_TARGET)]
			private static extern IntPtr alljoyn_msgarg_as_string(IntPtr args, UIntPtr idx);

			[DllImport(DLL_IMPORT_TARGET)]
			private static extern IntPtr alljoyn_msgarg_as_objpath(IntPtr args, UIntPtr idx);

			[DllImport(DLL_IMPORT_TARGET)]
			private static extern IntPtr alljoyn_msgarg_as_variant(IntPtr args, UIntPtr idx);

            [DllImport(DLL_IMPORT_TARGET)]
            private static extern byte[] alljoyn_msgarg_as_array(IntPtr args, UIntPtr idx);

			[DllImport(DLL_IMPORT_TARGET)]
			private static extern int alljoyn_msgarg_array_set_offset(IntPtr args, UIntPtr argOffset, ref UIntPtr numArgs, 
				[MarshalAs(UnmanagedType.LPStr)] string signature, byte arg);

			[DllImport(DLL_IMPORT_TARGET)]
			private static extern int alljoyn_msgarg_array_set_offset(IntPtr args, UIntPtr argOffset, ref UIntPtr numArgs, 
				[MarshalAs(UnmanagedType.LPStr)] string signature, short arg);

			[DllImport(DLL_IMPORT_TARGET)]
			private static extern int alljoyn_msgarg_array_set_offset(IntPtr args, UIntPtr argOffset, ref UIntPtr numArgs, 
				[MarshalAs(UnmanagedType.LPStr)] string signature, int arg);

			[DllImport(DLL_IMPORT_TARGET)]
			private static extern int alljoyn_msgarg_array_set_offset(IntPtr args, UIntPtr argOffset, ref UIntPtr numArgs, 
				[MarshalAs(UnmanagedType.LPStr)] string signature, uint arg);

			[DllImport(DLL_IMPORT_TARGET)]
			private static extern int alljoyn_msgarg_array_set_offset(IntPtr args, UIntPtr argOffset, ref UIntPtr numArgs, 
				[MarshalAs(UnmanagedType.LPStr)] string signature, long arg);

			[DllImport(DLL_IMPORT_TARGET)]
			private static extern int alljoyn_msgarg_array_set_offset(IntPtr args, UIntPtr argOffset, ref UIntPtr numArgs, 
				[MarshalAs(UnmanagedType.LPStr)] string signature, ulong arg);

			[DllImport(DLL_IMPORT_TARGET)]
			private static extern int alljoyn_msgarg_array_set_offset(IntPtr args, UIntPtr argOffset, ref UIntPtr numArgs, 
				[MarshalAs(UnmanagedType.LPStr)] string signature, double arg);

			[DllImport(DLL_IMPORT_TARGET)]
			private static extern int alljoyn_msgarg_array_set_offset(IntPtr args, UIntPtr argOffset, ref UIntPtr numArgs, 
				[MarshalAs(UnmanagedType.LPStr)] string signature, IntPtr arg);
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
