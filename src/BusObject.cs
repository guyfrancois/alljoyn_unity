//-----------------------------------------------------------------------
// <copyright file="BusObject.cs" company="Qualcomm Innovation Center, Inc.">
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
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace AllJoynUnity
{
	public partial class AllJoyn
	{
		public class BusObject : IDisposable
		{
			public BusObject(BusAttachment bus, string path, bool isPlaceholder)
			{
			
				// Can't let the GC free these delegates so they must be members
				_propertyGet = new InternalPropertyGetEventHandler(this._PropertyGet);
				_propertySet = new InternalPropertySetEventHandler(this._PropertySet);
				_objectRegistered = new InternalObjectRegisteredEventHandler(this._ObjectRegistered);
				_objectUnregistered = new InternalObjectUnregisteredEventHandler(this._ObjectUnregistered);

				// Ref holder for method handler internal delegates
				_methodHandlerDelegateRefHolder = new List<InternalMethodHandler>();

				callbacks.property_get = Marshal.GetFunctionPointerForDelegate(_propertyGet);
				callbacks.property_set = Marshal.GetFunctionPointerForDelegate(_propertySet);
				callbacks.object_registered = Marshal.GetFunctionPointerForDelegate(_objectRegistered);
				callbacks.object_unregistered = Marshal.GetFunctionPointerForDelegate(_objectUnregistered);

				main = GCHandle.Alloc(callbacks, GCHandleType.Pinned);
				_busObject = alljoyn_busobject_create(bus.UnmanagedPtr, path, isPlaceholder ? 1 : 0, main.AddrOfPinnedObject(), IntPtr.Zero);
			}

            public IntPtr getAddr()
            {
                return _busObject;
            }

			public QStatus AddInterface(InterfaceDescription iface)
			{
			
				return alljoyn_busobject_addinterface(_busObject, iface.UnmanagedPtr);
			}

			public QStatus AddMethodHandler(InterfaceDescription.Member member, MethodHandler handler)
			{
			
				InternalMethodHandler internalMethodHandler = (IntPtr bus, IntPtr m, IntPtr msg) =>
				{
					MethodHandler h = handler;
					h(new InterfaceDescription.Member(m), new Message(msg));
				};
				_methodHandlerDelegateRefHolder.Add(internalMethodHandler);

				GCHandle membGch = GCHandle.Alloc(member._member, GCHandleType.Pinned);

				MethodEntry entry;
				entry.member = membGch.AddrOfPinnedObject();
				entry.method_handler = Marshal.GetFunctionPointerForDelegate(internalMethodHandler);

				GCHandle gch = GCHandle.Alloc(entry, GCHandleType.Pinned);
				QStatus ret = alljoyn_busobject_addmethodhandlers(_busObject, gch.AddrOfPinnedObject(), (UIntPtr)1);
				gch.Free();
				membGch.Free();

				return ret;
			}

			protected QStatus MethodReply(Message message, MsgArgs args)
			{
			
				return alljoyn_busobject_methodreply_args(_busObject, message.UnmanagedPtr, args.UnmanagedPtr,
					(UIntPtr)args.Length);
			}

			protected QStatus MethodReply(Message message, string error, string errorMessage)
			{
			
				return alljoyn_busobject_methodreply_err(_busObject, message.UnmanagedPtr, error,
					errorMessage);
			}

			protected QStatus MethodReply(Message message, QStatus status)
			{
			
				return alljoyn_busobject_methodreply_status(_busObject, message.UnmanagedPtr, status.value);
			}

            protected QStatus Signal(string destination, uint sessionId, InterfaceDescription.Member signal, MsgArgs args)
            {
                return alljoyn_busobject_signal(_busObject, destination, sessionId, signal._member, args.UnmanagedPtr, (UIntPtr)args.Length, 0, 0);
            }

            protected QStatus Signal(string destination, uint sessionId, InterfaceDescription.Member signal,
                MsgArgs args, ushort timeToLife, byte flags)
            {
                return alljoyn_busobject_signal(_busObject, destination, sessionId, signal._member, args.UnmanagedPtr, (UIntPtr)args.Length, timeToLife, flags);
            }

			#region Properties
			public string Path
			{
				get
				{
					return Marshal.PtrToStringAnsi(alljoyn_busobject_getpath(_busObject));
				}
			}

			public string Name
			{
				get
				{
					UIntPtr nameSz = alljoyn_busobject_getname(_busObject, IntPtr.Zero, (UIntPtr)0);
					byte[] sink = new byte[(int)nameSz];

					GCHandle gch = GCHandle.Alloc(sink, GCHandleType.Pinned);
					alljoyn_busobject_getname(_busObject, gch.AddrOfPinnedObject(), nameSz);
					gch.Free();

					return System.Text.ASCIIEncoding.ASCII.GetString(sink);
				}
			}
			#endregion

			#region Delegates
            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			public delegate void MethodHandler(InterfaceDescription.Member member, Message message);
            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			private delegate void InternalMethodHandler(IntPtr bus, IntPtr member, IntPtr message);
            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			private delegate void InternalPropertyGetEventHandler(IntPtr context, IntPtr ifcName, IntPtr propName, IntPtr val);
            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			private delegate void InternalPropertySetEventHandler(IntPtr context, IntPtr ifcName, IntPtr propName, IntPtr val);
            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			private delegate void InternalObjectRegisteredEventHandler(IntPtr context);
            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			private delegate void InternalObjectUnregisteredEventHandler(IntPtr context);
			#endregion

			#region Virtual Methods
			protected virtual void OnPropertyGet(string ifcName, string propName, MsgArg val)
			{
			
			}

			protected virtual void OnPropertySet(string ifcName, string propName, MsgArg val)
			{
			
			}

			protected virtual void OnObjectRegistered()
			{
			
			}

			protected virtual void OnObjectUnregistered()
			{
			
			}
			#endregion

			#region Callbacks
			private void _PropertyGet(IntPtr context, IntPtr ifcName, IntPtr propName, IntPtr val)
			{
			
				OnPropertyGet(Marshal.PtrToStringAnsi(ifcName),
					Marshal.PtrToStringAnsi(propName), new MsgArg(val));
			}

			private void _PropertySet(IntPtr context, IntPtr ifcName, IntPtr propName, IntPtr val)
			{
			
				OnPropertySet(Marshal.PtrToStringAnsi(ifcName),
					Marshal.PtrToStringAnsi(propName), new MsgArg(val));
			}

			private void _ObjectRegistered(IntPtr context)
			{
			
				OnObjectRegistered();
			}

			private void _ObjectUnregistered(IntPtr context)
			{
			
				OnObjectUnregistered();
			}
			#endregion

			#region DLL Imports
			[DllImport(DLL_IMPORT_TARGET)]
			private extern static IntPtr alljoyn_busobject_create(
				IntPtr busAttachment,
				[MarshalAs(UnmanagedType.LPStr)] string path,
				int isPlaceholder,
				IntPtr callbacks_in,
				IntPtr context_in);

			[DllImport(DLL_IMPORT_TARGET)]
			private extern static IntPtr alljoyn_busobject_destroy(IntPtr bus);

			[DllImport(DLL_IMPORT_TARGET)]
			private extern static IntPtr alljoyn_busobject_getpath(IntPtr bus);

			[DllImport(DLL_IMPORT_TARGET)]
			private extern static UIntPtr alljoyn_busobject_getname(IntPtr bus, IntPtr buffer, UIntPtr bufferSz);

			[DllImport(DLL_IMPORT_TARGET)]
			private extern static int alljoyn_busobject_addinterface(IntPtr bus, IntPtr iface);

			[DllImport(DLL_IMPORT_TARGET)]
			private extern static int alljoyn_busobject_addmethodhandlers(IntPtr bus,
				IntPtr entries, UIntPtr numEntries);

			[DllImport(DLL_IMPORT_TARGET)]
			private extern static int alljoyn_busobject_methodreply_args(IntPtr bus,
				IntPtr msg, IntPtr msgArgs, UIntPtr numArgs);

			[DllImport(DLL_IMPORT_TARGET)]
			private extern static int alljoyn_busobject_methodreply_err(IntPtr bus, IntPtr msg,
				[MarshalAs(UnmanagedType.LPStr)] string error,
				[MarshalAs(UnmanagedType.LPStr)] string errorMsg);

			[DllImport(DLL_IMPORT_TARGET)]
			private extern static int alljoyn_busobject_methodreply_status(IntPtr bus, IntPtr msg, int status);

            [DllImport(DLL_IMPORT_TARGET)]
            private extern static int alljoyn_busobject_signal(IntPtr bus,
                [MarshalAs(UnmanagedType.LPStr)] string destination,
                uint sessionId,
                InterfaceDescription._Member signal,
                IntPtr msgArgs, UIntPtr numArgs,
                ushort timeToLive, byte flags);

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
					Thread destroyThread = new Thread((object o) => {
                        alljoyn_busobject_destroy(_busObject); 
                    });
					destroyThread.Start();
					while(destroyThread.IsAlive)
					{
						AllJoyn.TriggerCallbacks();
						Thread.Sleep(0);
					}
					_busObject = IntPtr.Zero;
                    main.Free();
				}
				_isDisposed = true;
			}

			~BusObject()
			{
			
				Dispose(false);
			}
			#endregion

			#region Structs
			[StructLayout(LayoutKind.Sequential)]
			private struct BusObjectCallbacks
			{
				public IntPtr property_get;
				public IntPtr property_set;
				public IntPtr object_registered;
				public IntPtr object_unregistered;
			}

			[StructLayout(LayoutKind.Sequential)]
			private struct MethodEntry
			{
				public IntPtr member;
				public IntPtr method_handler;
			}
			#endregion

			#region Internal Properties
			internal IntPtr UnmanagedPtr
			{
				get
				{
					return _busObject;
				}
			}
			#endregion

			#region Data
			IntPtr _busObject;
			bool _isDisposed = false;

            GCHandle main;
			InternalPropertyGetEventHandler _propertyGet;
			InternalPropertySetEventHandler _propertySet;
			InternalObjectRegisteredEventHandler _objectRegistered;
			InternalObjectUnregisteredEventHandler _objectUnregistered;

            BusObjectCallbacks callbacks;

			List<InternalMethodHandler> _methodHandlerDelegateRefHolder;
			#endregion
		}
	}
}
