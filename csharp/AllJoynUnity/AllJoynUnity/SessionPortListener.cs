using System;
using System.Runtime.InteropServices;

namespace AllJoynUnity
{
	public partial class AllJoyn
	{
		public class SessionPortListener : IDisposable
		{
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
			protected virtual bool AcceptSessionJoiner(ushort sessionPort, string joiner, SessionOpts opts)
			{
			
				return false;
			}

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
			public void Dispose()
			{
				Dispose(true);
				GC.SuppressFinalize(this); 
			}

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
