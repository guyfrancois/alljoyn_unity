using System;
using System.Threading;
using System.Runtime.InteropServices;

namespace AllJoynUnity
{
	public partial class AllJoyn
	{
		public abstract class AuthListener : IDisposable
		{
			public AuthListener()
			{
			
				_requestCredentials = new InternalRequestCredentials(this._RequestCredentials);
				_verifyCredentials = new InternalVerifyCredentials(this._VerifyCredentials);
				_securityViolation = new InternalSecurityViolation(this._SecurityViolation);
				_authenticationComplete = new InternalAuthenticationComplete(this._AuthenticationComplete);

				AuthListenerCallbacks callbacks;
				callbacks.requestCredentials = Marshal.GetFunctionPointerForDelegate(_requestCredentials);
				callbacks.verifyCredentials = Marshal.GetFunctionPointerForDelegate(_verifyCredentials);
				callbacks.securityViolation = Marshal.GetFunctionPointerForDelegate(_securityViolation);
				callbacks.authenticationComplete = Marshal.GetFunctionPointerForDelegate(_authenticationComplete);

				main = GCHandle.Alloc(callbacks, GCHandleType.Pinned);
				_authListener = alljoyn_authlistener_create(main.AddrOfPinnedObject(), IntPtr.Zero);
			}

			#region Virtual Methods
			protected abstract bool RequestCredentials(string authMechanism, string peerName, ushort authCount,
				string userName, Credentials.CredentialFlags credMask, Credentials credentials);

			protected virtual bool VerifyCredentials(string authMechanism, string peerName, Credentials credentials)
			{
			
				return false;
			}

			protected virtual void SecurityViolation(QStatus status, Message msg)
			{
			
			}

			protected abstract void AuthenticationComplete(string authMechanism, string peerName, bool success);
			#endregion

			#region Callbacks
			private int _RequestCredentials(IntPtr context, IntPtr authMechanism, IntPtr peerName, ushort authCount,
				IntPtr userName, ushort credMask, IntPtr credentials)
			{
			
				return (RequestCredentials(Marshal.PtrToStringAnsi(authMechanism), Marshal.PtrToStringAnsi(peerName),
					authCount, Marshal.PtrToStringAnsi(userName), (Credentials.CredentialFlags)credMask, new Credentials(credentials)) ? 1 : 0);
			}

			private int _VerifyCredentials(IntPtr context, IntPtr authMechanism, IntPtr peerName,
				IntPtr credentials)
			{
			
				return (VerifyCredentials(Marshal.PtrToStringAnsi(authMechanism), Marshal.PtrToStringAnsi(peerName),
					new Credentials(credentials)) ? 1 : 0);
			}

			private void _SecurityViolation(IntPtr context, int status, IntPtr msg)
			{
			
				SecurityViolation(status, new Message(msg));
			}

			private void _AuthenticationComplete(IntPtr context, IntPtr authMechanism, IntPtr peerName, int success)
			{
			
				AuthenticationComplete(Marshal.PtrToStringAnsi(authMechanism), Marshal.PtrToStringAnsi(peerName),
					success == 1 ? true : false);
			}
			#endregion

			#region Delegates
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			private delegate int InternalRequestCredentials(IntPtr context, IntPtr authMechanism, IntPtr peerName, ushort authCount,
				IntPtr userName, ushort credMask, IntPtr credentials);
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			private delegate int InternalVerifyCredentials(IntPtr context, IntPtr authMechanism, IntPtr peerName,
				IntPtr credentials);
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			private delegate void InternalSecurityViolation(IntPtr context, int status, IntPtr msg);
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			private delegate void InternalAuthenticationComplete(IntPtr context, IntPtr authMechanism, IntPtr peerName, int success);
			#endregion

			#region DLL Imports
			[DllImport(DLL_IMPORT_TARGET)]
			private extern static IntPtr alljoyn_authlistener_create(
				IntPtr callbacks,
				IntPtr context);

			[DllImport(DLL_IMPORT_TARGET)]
			private extern static void alljoyn_authlistener_destroy(IntPtr listener);
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
					alljoyn_authlistener_destroy(_authListener);
					_authListener = IntPtr.Zero;
                    main.Free();
				}
				_isDisposed = true;
			}

			~AuthListener()
			{
			
				Dispose(false);
			}
			#endregion

			#region Structs
			private struct AuthListenerCallbacks
			{
				public IntPtr requestCredentials;
				public IntPtr verifyCredentials;
				public IntPtr securityViolation;
				public IntPtr authenticationComplete;
			}
			#endregion

			#region Internal Properties
			internal IntPtr UnmanagedPtr
			{
				get
				{
					return _authListener;
				}
			}
			#endregion

			#region Data
			IntPtr _authListener;
			bool _isDisposed = false;

            GCHandle main;

			InternalRequestCredentials _requestCredentials;
			InternalVerifyCredentials _verifyCredentials;
			InternalSecurityViolation _securityViolation;
			InternalAuthenticationComplete _authenticationComplete;
			#endregion
		}
	}
}
