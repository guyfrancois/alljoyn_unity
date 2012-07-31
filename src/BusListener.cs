//-----------------------------------------------------------------------
// <copyright file="BusListener.cs" company="Qualcomm Innovation Center, Inc.">
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
using System.Threading;
using System.Runtime.InteropServices;

namespace AllJoynUnity
{
	public partial class AllJoyn
	{
		public class BusListener : IDisposable
		{
			public BusListener()
			{
				// Can't let the GC free these delegates so they must be members
				_listenerRegistered = new InternalListenerRegisteredDelegate(_ListenerRegistered);
				_listenerUnregistered = new InternalListenerUnregisteredDelegate(_ListenerUnregistered);
				_foundAdvertisedName = new InternalFoundAdvertisedNameDelegate(_FoundAdvertisedName);
				_lostAdvertisedName = new InternalLostAdvertisedNameDelegate(_LostAdvertisedName);
				_nameOwnerChanged = new InternalNameOwnerChangedDelegate(_NameOwnerChanged);
				_busStopping = new InternalBusStoppingDelegate(_BusStopping);
				_busDisconnected = new InternalBusDisconnectedDelegate(_BusDisconnected);

				callbacks.listenerRegistered = Marshal.GetFunctionPointerForDelegate(_listenerRegistered);
				callbacks.listenerUnregistered = Marshal.GetFunctionPointerForDelegate(_listenerUnregistered);
                callbacks.foundAdvertisedName =  Marshal.GetFunctionPointerForDelegate(_foundAdvertisedName);
                callbacks.lostAdvertisedName =  Marshal.GetFunctionPointerForDelegate(_lostAdvertisedName);
                callbacks.nameOwnerChanged = Marshal.GetFunctionPointerForDelegate(_nameOwnerChanged);
				callbacks.busStopping = Marshal.GetFunctionPointerForDelegate(_busStopping);
				callbacks.busDisconnected = Marshal.GetFunctionPointerForDelegate(_busDisconnected);

				main = GCHandle.Alloc(callbacks, GCHandleType.Pinned);
				_busListener = alljoyn_buslistener_create(main.AddrOfPinnedObject(), IntPtr.Zero);
			}

			#region Virtual Methods
			protected virtual void ListenerRegistered(BusAttachment busAttachment){}

			protected virtual void ListenerUnregistered() {}

			protected virtual void FoundAdvertisedName(string name, TransportMask transport, string namePrefix) {}

			protected virtual void LostAdvertisedName(string name, TransportMask transport, string namePrefix) {}

			protected virtual void NameOwnerChanged(string busName, string previousOwner, string newOwner) {}

			protected virtual void BusStopping() {}

			protected virtual void BusDisconnected() {}
			#endregion

			#region Delegates
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			private delegate void InternalListenerRegisteredDelegate(IntPtr context, IntPtr bus);
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			private delegate void InternalListenerUnregisteredDelegate(IntPtr context);
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			private delegate void InternalFoundAdvertisedNameDelegate(IntPtr context, IntPtr name, System.UInt16 transport, IntPtr namePrefix);
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			private delegate void InternalLostAdvertisedNameDelegate(IntPtr context, IntPtr name, ushort transport, IntPtr namePrefix);
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			private delegate void InternalNameOwnerChangedDelegate(IntPtr context, IntPtr busName, IntPtr previousOwner, IntPtr newOwner);
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			private delegate void InternalBusStoppingDelegate(IntPtr context);
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			private delegate void InternalBusDisconnectedDelegate(IntPtr context);
			#endregion

			#region Callbacks
			private void _ListenerRegistered(IntPtr context, IntPtr bus)
			{
                IntPtr _bus = bus;
			    System.Threading.Thread callIt = new System.Threading.Thread((object o) =>
                    {
				        _registeredBus = BusAttachment.MapBusAttachment(_bus);
				        ListenerRegistered(_registeredBus);
                    });
                callIt.Start();
			}

			private void _ListenerUnregistered(IntPtr context)
			{
			    System.Threading.Thread callIt = new System.Threading.Thread((object o) =>
                {
				    ListenerUnregistered();
				    _registeredBus = null;
                });
                callIt.Start();
			}

			private void _FoundAdvertisedName(IntPtr context, IntPtr name, System.UInt16 transport, IntPtr namePrefix)
			{
                String _name = Marshal.PtrToStringAnsi(name);
                TransportMask _transport = (TransportMask)transport;
                String _namePrefix = Marshal.PtrToStringAnsi(namePrefix);
                System.Threading.Thread callIt = new System.Threading.Thread((object o) =>
                        {
                            FoundAdvertisedName(_name, _transport, _namePrefix);
                        });
                callIt.Start();
			}

			private void _LostAdvertisedName(IntPtr context, IntPtr name, ushort transport, IntPtr namePrefix)
			{
                String _name = Marshal.PtrToStringAnsi(name);
                TransportMask _transport = (TransportMask)transport;
                String _namePrefix = Marshal.PtrToStringAnsi(namePrefix);
			    System.Threading.Thread callIt = new System.Threading.Thread((object o) =>
                        {
				            LostAdvertisedName(_name, _transport, _namePrefix);
                        });
                callIt.Start();
			}

			private void _NameOwnerChanged(IntPtr context, IntPtr busName, IntPtr previousOwner, IntPtr newOwner)
			{
                String _busName = Marshal.PtrToStringAnsi(busName);
                String _previousOwner = Marshal.PtrToStringAnsi(previousOwner);
                String _newOwner = Marshal.PtrToStringAnsi(newOwner);
			    System.Threading.Thread callIt = new System.Threading.Thread((object o) =>
                    {
                        NameOwnerChanged(_busName, _previousOwner, _newOwner);
                    });
                callIt.Start();
			}

			private void _BusStopping(IntPtr context)
			{
                System.Threading.Thread callIt = new System.Threading.Thread((object o) =>
                    {
                        BusStopping();
                    });
                callIt.Start();
			}

			private void _BusDisconnected(IntPtr context)
			{
                System.Threading.Thread callIt = new System.Threading.Thread((object o) =>
                    {
                        BusDisconnected();
                    });
                callIt.Start();
			}
			#endregion

			#region DLL Imports
			[DllImport(DLL_IMPORT_TARGET)]
			private extern static IntPtr alljoyn_buslistener_create(
				IntPtr callbacks,
				IntPtr context);

			[DllImport(DLL_IMPORT_TARGET)]
			private extern static void alljoyn_buslistener_destroy(IntPtr listener);
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
                	// Dispose of BusAttachment before listeners
					if(_registeredBus != null)
					{
						_registeredBus.Dispose();
					}
					
					Thread destroyThread = new Thread((object o) => {
                        alljoyn_buslistener_destroy(_busListener);
                    });
					destroyThread.Start();
					while(destroyThread.IsAlive)
					{
						AllJoyn.TriggerCallbacks();
						Thread.Sleep(0);
					}

                    _busListener = IntPtr.Zero;
                    main.Free();
				}
				_isDisposed = true;
			}

			~BusListener()
			{
			
				Dispose(false);
			}
			#endregion

			#region Structs
			[StructLayout(LayoutKind.Sequential)]
			private struct BusListenerCallbacks
			{
				public IntPtr listenerRegistered;
				public IntPtr listenerUnregistered;
				public IntPtr foundAdvertisedName;
				public IntPtr lostAdvertisedName;
				public IntPtr nameOwnerChanged;
				public IntPtr busStopping;
				public IntPtr busDisconnected;
			}
			#endregion

			#region Internal Properties
			internal IntPtr UnmanagedPtr
			{
				get
				{
					return _busListener;
				}
			}
			#endregion

			#region Data
			IntPtr _busListener;
			bool _isDisposed = false;
			BusAttachment _registeredBus;

            GCHandle main;
            BusListenerCallbacks callbacks;
			InternalListenerRegisteredDelegate _listenerRegistered;
			InternalListenerUnregisteredDelegate _listenerUnregistered;
			InternalFoundAdvertisedNameDelegate _foundAdvertisedName;
			InternalLostAdvertisedNameDelegate _lostAdvertisedName;
			InternalNameOwnerChangedDelegate _nameOwnerChanged;
			InternalBusStoppingDelegate _busStopping;
			InternalBusDisconnectedDelegate _busDisconnected;
			#endregion
		}
	}
}

