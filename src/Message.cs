//-----------------------------------------------------------------------
// <copyright file="Message.cs" company="Qualcomm Innovation Center, Inc.">
// Copyright 2012, Qualcomm Innovation Center, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Runtime.InteropServices;

namespace AllJoynUnity
{
	public partial class AllJoyn
	{
		public class Message : IDisposable
		{
			public enum Type : int
			{
				Invalid = 0,
				MethodCall = 1,
				MethodReturn = 2,
				Error = 3,
				Signal = 4
			}

			public Message(BusAttachment bus)
			{
			
				_message = alljoyn_message_create(bus.UnmanagedPtr);
			}

			internal Message(IntPtr message)
			{
			
				_message = message;
				_isDisposed = true;
			}

			public MsgArg GetArg(int index)
			{
			
				IntPtr msgArgs = alljoyn_message_getarg(_message, (UIntPtr)index);
				return (msgArgs != IntPtr.Zero ? new MsgArg(msgArgs) : null);
			}

            public string GetSender()
            {
                IntPtr sender = alljoyn_message_getsender(_message);
                return (sender != IntPtr.Zero ? Marshal.PtrToStringAnsi(sender) : null);
            }

			public MsgArg this[int i]
			{
				get
				{
					return GetArg(i);
				}
			}

			#region IDisposable
			public void Dispose()
			{
				Dispose(true);
				GC.SuppressFinalize(this); 
			}

			protected virtual void Dispose(bool disposing)
			{
			
				if(!_isDisposed)
				{
					alljoyn_message_destroy(_message);
					_message = IntPtr.Zero;
				}
				_isDisposed = true;
			}

			~Message()
			{
			
				Dispose(false);
			}
			#endregion

			#region DLL Imports
			[DllImport(DLL_IMPORT_TARGET)]
			private static extern IntPtr alljoyn_message_create(IntPtr bus);

			[DllImport(DLL_IMPORT_TARGET)]
			private static extern void alljoyn_message_destroy(IntPtr msg);

			[DllImport(DLL_IMPORT_TARGET)]
			private static extern IntPtr alljoyn_message_getarg(IntPtr msg, UIntPtr argN);

            [DllImport(DLL_IMPORT_TARGET)]
            private static extern IntPtr alljoyn_message_getsender(IntPtr msg);
			#endregion

			#region Internal Properties
			internal IntPtr UnmanagedPtr
			{
				get
				{
					return _message;
				}
			}
			#endregion

			#region Data
			IntPtr _message;
			bool _isDisposed = false;
			#endregion
		}
	}
}
