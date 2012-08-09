//-----------------------------------------------------------------------
// <copyright file="MsgArgs.cs" company="Qualcomm Innovation Center, Inc.">
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
		public class MsgArgs : IDisposable
		{
			public MsgArgs(uint numArgs)
			{
			
				_msgArgs = alljoyn_msgarg_array_create((UIntPtr)numArgs);
				_argArray = new MsgArg[numArgs];
				for(uint i = 0; i < numArgs; i++)
				{
					_argArray[i] = new MsgArg(this, i);
				}
			}

			public int Length
			{
				get
				{
					return _argArray.Length;
				}
			}

			public MsgArg this[int i]
			{
				get
				{
					return _argArray[i];
				}
				set
				{
					MsgArg arg = value as MsgArg;
					if(arg != null)
					{
						if(arg._setValue != null)
						{
							_argArray[i].Set(arg._setValue);
						}
					}
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
					alljoyn_msgarg_destroy(_msgArgs);
					_msgArgs = IntPtr.Zero;
				}
				_isDisposed = true;
			}

			~MsgArgs()
			{
			
				Dispose(false);
			}
			#endregion

			#region DLL Imports
			[DllImport(DLL_IMPORT_TARGET)]
			private static extern IntPtr alljoyn_msgarg_array_create(UIntPtr numArgs); // UIntPtr must map to the same size as size_t, not a typo

			[DllImport(DLL_IMPORT_TARGET)]
			private static extern void alljoyn_msgarg_destroy(IntPtr arg);
			#endregion

			#region Internal Properties
			internal IntPtr UnmanagedPtr
			{
				get
				{
					return _msgArgs;
				}
			}
			#endregion

			#region Data
			IntPtr _msgArgs;
			MsgArg[] _argArray;
			bool _isDisposed = false;
			#endregion
		}
	}
}
