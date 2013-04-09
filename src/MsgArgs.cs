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
		 * Class definition for message args.
		 * This class deals with the message bus types and the operations on them
		 *
		 * MsgArg's are designed to be light-weight. A MsgArg will normally hold references to the data
		 * (strings etc.) it wraps and will only copy that data if the MsgArg is assigned. For example no
		 * additional memory is allocated for an ALLJOYN_STRING that references an existing const char*.
		 * If a MsgArg is assigned the destination receives a copy of the contents of the source. The
		 * Stabilize() methods can also be called to explicitly force contents of the MsgArg to be copied.
		 */
		[Obsolete("Usage of this class has been depricated.  Please use a MsgArg.")]
		public class MsgArgs : IDisposable
		{
			/**
			 * Constructor for MsgArgs.
			 */
			public MsgArgs(uint numArgs)
			{
				_msgArg = new MsgArg(numArgs);
			}

			/**
			 * Gets the number of MsgArg that are contained in this MsgArgs Object
			 */
			public int Length
			{
				get
				{
					return _msgArg.Length;
				}
			}

			/**
			 * Gets or Sets the value of a specific MsgArg at index i
			 */
			public MsgArg this[int i]
			{
				get
				{
					return _msgArg[i];
				}
				set
				{
					alljoyn_msgarg_clone(alljoyn_msgarg_array_element(_msgArg.UnmanagedPtr, (UIntPtr)i), value.UnmanagedPtr);
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
				_msgArg.Dispose();
			}

			~MsgArgs()
			{
				Dispose(false);
			}
			#endregion

			#region DLL Imports
			[DllImport(DLL_IMPORT_TARGET)]
			private static extern IntPtr alljoyn_msgarg_array_element(IntPtr args, UIntPtr index);
			[DllImport(DLL_IMPORT_TARGET)]
			private static extern void alljoyn_msgarg_clone(IntPtr destination, IntPtr source);
			#endregion

			#region Internal Properties
			internal IntPtr UnmanagedPtr
			{
				get
				{
					return _msgArg.UnmanagedPtr;
				}
			}
			#endregion

			#region Data
			MsgArg _msgArg;
			#endregion
		}
	}
}
