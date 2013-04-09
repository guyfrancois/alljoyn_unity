/**
 * @file
 * SessionPortListener is an abstract base class (interface) implemented by users of the
 * AllJoyn API in order to receive session port related event information.
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
		 * Abstract base class implemented by AllJoyn users and called by AllJoyn to inform
		 * users of session related events.
		 */
		public class SessionPortListener : IDisposable
		{
			/**
			 * Constructor for SessionPortListener.
			 */
			public SessionPortListener()
			{
				_acceptSessionJoiner = new InternalAcceptSessionJoiner(this._AcceptSessionJoiner);
				_sessionJoined = new InternalSessionJoined(this._SessionJoined);

				callbacks.acceptSessionJoiner = Marshal.GetFunctionPointerForDelegate(_acceptSessionJoiner);
				callbacks.sessionJoined = Marshal.GetFunctionPointerForDelegate(_sessionJoined);

				main = GCHandle.Alloc(callbacks, GCHandleType.Pinned);
				_sessionPortListener = alljoyn_sessionportlistener_create(main.AddrOfPinnedObject(), IntPtr.Zero);
			}

			#region Virtual Methods
			/**
			 * Accept or reject an incoming JoinSession request. The session does not exist until this
			 * after this function returns.
			 *
			 * This callback is only used by session creators. Therefore it is only called on listeners
			 * passed to BusAttachment.BindSessionPort.
			 *
			 * @param sessionPort    Session port that was joined.
			 * @param joiner         Unique name of potential joiner.
			 * @param opts           Session options requested by the joiner.
			 * @return   Return true if JoinSession request is accepted. false if rejected.
			 */
			protected virtual bool AcceptSessionJoiner(ushort sessionPort, string joiner, SessionOpts opts)
			{
				return false;
			}

			/**
			 * Called by the bus when a session has been successfully joined. The session is now fully up.
			 *
			 * This callback is only used by session creators. Therefore it is only called on listeners
			 * passed to BusAttachment.BindSessionPort.
			 *
			 * @param sessionPort    Session port that was joined.
			 * @param sessionId             Id of session.
			 * @param joiner         Unique name of the joiner.
			 */
			protected virtual void SessionJoined(ushort sessionPort, uint sessionId, string joiner)
			{
			
			}
			#endregion

			#region Callbacks
			private int _AcceptSessionJoiner(IntPtr context, ushort sessionPort, IntPtr joiner, IntPtr opts)
			{
				return (AcceptSessionJoiner(sessionPort, Marshal.PtrToStringAnsi(joiner), new SessionOpts(opts)) ? 1 : 0);
			}

			private void _SessionJoined(IntPtr context, ushort sessionPort, uint sessionId, IntPtr joiner)
			{
				ushort _sessionPort = sessionPort;
				uint _sessionId = sessionId;
				String _joiner = Marshal.PtrToStringAnsi(joiner);
				System.Threading.Thread callIt = new System.Threading.Thread((object o) =>
					{
						SessionJoined(_sessionPort, _sessionId, _joiner);
					});
				callIt.Start();
			}
			#endregion

			#region Delegates
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			private delegate int InternalAcceptSessionJoiner(IntPtr context, ushort sessionPort, IntPtr joiner, IntPtr opts);
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			private delegate void InternalSessionJoined(IntPtr context, ushort sessionPort, uint sessionId, IntPtr joiner);
			#endregion

			#region DLL Imports
			[DllImport(DLL_IMPORT_TARGET)]
			private extern static IntPtr alljoyn_sessionportlistener_create(
				IntPtr callbacks,
				IntPtr context);

			[DllImport(DLL_IMPORT_TARGET)]
			private extern static void alljoyn_sessionportlistener_destroy(IntPtr listener);
			#endregion

			#region IDisposable
			/**
			 * Dispose the SessionPortListener
			 */
			public void Dispose()
			{
				Dispose(true);
				GC.SuppressFinalize(this); 
			}

			/**
			 * Dispose the SessionPortListener
			 * @param disposing	describes if its activly being disposed
			 */
			protected virtual void Dispose(bool disposing)
			{
				if(!_isDisposed)
				{
					alljoyn_sessionportlistener_destroy(_sessionPortListener);
					_sessionPortListener = IntPtr.Zero;
					main.Free();
				}
				_isDisposed = true;
			}

			~SessionPortListener()
			{
			
				Dispose(false);
			}
			#endregion

			#region Structs
			private struct SessionPortListenerCallbacks
			{
				public IntPtr acceptSessionJoiner;
				public IntPtr sessionJoined;
			}
			#endregion

			#region Internal Properties
			internal IntPtr UnmanagedPtr
			{
				get
				{
					return _sessionPortListener;
				}
			}
			#endregion

			#region Data
			IntPtr _sessionPortListener;
			bool _isDisposed = false;

			GCHandle main;
			InternalAcceptSessionJoiner _acceptSessionJoiner;
			InternalSessionJoined _sessionJoined;
			SessionPortListenerCallbacks callbacks;
			#endregion
		}
	}
}
