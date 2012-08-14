/**
 * @file
 * SessionListener is an abstract base class (interface) implemented by users of the
 * AllJoyn API in order to receive sessions related event information.
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
		public class SessionListener : IDisposable
		{
		    /**
		     * Constructor for a SessionListener.
		     */
			public SessionListener()
			{
			
				_sessionLost = new InternalSessionLost(this._SessionLost);
				_sessionMemberAdded = new InternalSessionMemberAdded(this._SessionMemberAdded);
				_sessionMemberRemoved = new InternalSessionMemberRemoved(this._SessionMemberRemoved);

				callbacks.sessionLost = Marshal.GetFunctionPointerForDelegate(_sessionLost);
				callbacks.sessionMemberAdded = Marshal.GetFunctionPointerForDelegate(_sessionMemberAdded);
				callbacks.sessionMemberRemoved = Marshal.GetFunctionPointerForDelegate(_sessionMemberRemoved);

				main = GCHandle.Alloc(callbacks, GCHandleType.Pinned);
				_sessionListener = alljoyn_sessionlistener_create(main.AddrOfPinnedObject(), IntPtr.Zero);
			}

		    /**
		     * Request the raw pointer of the AllJoyn C SessionListener
		     *
		     * @return the raw pointer of the AllJoyn C SessionListener
		     */ 
		    public IntPtr getAddr()
		    {
			return _sessionListener;
		    }

			#region Virtual Methods
			    /**
			     * Called by the bus when an existing session becomes disconnected.
			     *
			     * @param sessionId     Id of session that was lost.
			     */
			protected virtual void SessionLost(uint sessionId)
			{
			
			}

			/**
			     * Called by the bus when a member of a multipoint session is added.
			     *
			     * @param sessionId     Id of session whose member(s) changed.
			     * @param uniqueName    Unique name of member who was added.
			     */
			protected virtual void SessionMemberAdded(uint sessionId, string uniqueName)
			{
			
			}

			/**
			     * Called by the bus when a member of a multipoint session is removed.
			     *
			     * @param sessionId     Id of session whose member(s) changed.
			     * @param uniqueName    Unique name of member who was removed.
			     */
			protected virtual void SessionMemberRemoved(uint sessionId, string uniqueName)
			{
			
			}
			#endregion

			#region Callbacks
			private void _SessionLost(IntPtr context, uint sessionId)
			{
                uint _sessionId = sessionId;
                System.Threading.Thread callIt = new System.Threading.Thread((object o) =>
                    {
                        SessionLost(_sessionId);
                    });
                callIt.Start();
			}

			private void _SessionMemberAdded(IntPtr context, uint sessionId, IntPtr uniqueName)
			{
                uint _sessionId = sessionId;
                String _uniqueName = Marshal.PtrToStringAnsi(uniqueName);
                System.Threading.Thread callIt = new System.Threading.Thread((object o) =>
                    {
                        SessionMemberAdded(_sessionId, _uniqueName);
                    });
                callIt.Start();
			}

			private void _SessionMemberRemoved(IntPtr context, uint sessionId, IntPtr uniqueName)
            {
                uint _sessionId = sessionId;
                String _uniqueName = Marshal.PtrToStringAnsi(uniqueName);
				System.Threading.Thread callIt = new System.Threading.Thread((object o) =>
                    {
                        SessionMemberRemoved(_sessionId, _uniqueName);
                    });
                 callIt.Start();
			}
			#endregion

			#region Delegates
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			private delegate void InternalSessionLost(IntPtr context, uint sessionId);
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			private delegate void InternalSessionMemberAdded(IntPtr context, uint sessionId, IntPtr uniqueName);
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			private delegate void InternalSessionMemberRemoved(IntPtr context, uint sessionId, IntPtr uniqueName);
			#endregion

			#region DLL Imports
			[DllImport(DLL_IMPORT_TARGET)]
			private extern static IntPtr alljoyn_sessionlistener_create(
				IntPtr callbacks,
				IntPtr context);

			[DllImport(DLL_IMPORT_TARGET)]
			private extern static void alljoyn_sessionlistener_destroy(IntPtr listener);
			#endregion

			#region IDisposable
			/**
			 * Dispose the SessionListener
			 */
			public void Dispose()
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}

			/**
			 * Dispose the SessionListener
			 * @param disposing	describes if its activly being disposed
			 */
			protected virtual void Dispose(bool disposing)
			{
			
				if(!_isDisposed)
				{
					alljoyn_sessionlistener_destroy(_sessionListener);
					_sessionListener = IntPtr.Zero;
                    main.Free();
				}
				_isDisposed = true;
			}

			~SessionListener()
			{
			
				Dispose(false);
			}
			#endregion

			#region Structs
			private struct SessionListenerCallbacks
			{
				public IntPtr sessionLost;
				public IntPtr sessionMemberAdded;
				public IntPtr sessionMemberRemoved;
			}
			#endregion

			#region Internal Properties
			internal IntPtr UnmanagedPtr
			{
				get
				{
					return _sessionListener;
				}
			}
			#endregion

			#region Data
			IntPtr _sessionListener;
			bool _isDisposed = false;

            GCHandle main;
			InternalSessionLost _sessionLost;
			InternalSessionMemberAdded _sessionMemberAdded;
			InternalSessionMemberRemoved _sessionMemberRemoved;
            SessionListenerCallbacks callbacks;
			#endregion
		}
	}
}
