/**
 * @file
 * This file defines the class ProxyBusObject.
 * The ProxyBusObject represents a single object registered registered on the bus.
 * ProxyBusObjects are used to make method calls on these remotely located DBus objects.
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
		 * Each %ProxyBusObject instance represents a single DBus/AllJoyn object registered
		 * somewhere on the bus. ProxyBusObjects are used to make method calls on these
		 * remotely located DBus objects.
		 */
		public class ProxyBusObject : IDisposable
		{
			/**
			     * Create an empty proxy object that refers to an object at given remote service name. Note
			     * that the created proxy object does not contain information about the interfaces that the
			     * actual remote object implements with the exception that org.freedesktop.DBus.Peer
			     * interface is special-cased (per the DBus spec) and can always be called on any object. Nor
			     * does it contain information about the child objects that the actual remote object might
			     * contain.
			     *
			     * To fill in this object with the interfaces and child object names that the actual remote
			     * object describes in its introspection data, call IntrospectRemoteObject() or
			     * IntrospectRemoteObjectAsync().
			     *
			     * @param bus        The bus.
			     * @param service    The remote service name (well-known or unique).
			     * @param path       The absolute (non-relative) object path for the remote object.
			     * @param sessionId  The session id the be used for communicating with remote object.
			     */
			public ProxyBusObject(BusAttachment bus, string service, string path, uint sessionId)
			{
			
				_proxyBusObject = alljoyn_proxybusobject_create(bus.UnmanagedPtr, service, path, sessionId);
			}

			internal ProxyBusObject(IntPtr busObject)
			{
			
				_proxyBusObject = busObject;
				_isDisposed = true;
			}

			/**
			     * Add an interface to this ProxyBusObject.
			     *
			     * Occasionally, AllJoyn library user may wish to call a method on
			     * a %ProxyBusObject that was not reported during introspection of the remote object.
			     * When this happens, the InterfaceDescription will have to be registered with the
			     * Bus manually and the interface will have to be added to the %ProxyBusObject using this method.
			     * @remark
			     * The interface added via this call must have been previously registered with the
			     * Bus. (i.e. it must have come from a call to Bus::GetInterface()).
			     *
			     * @param iface    The interface to add to this object. Must come from Bus::GetInterface().
			     * @return
			     *      - QStatus#OK if successful.
			     *      - An error status otherwise
			     */
			public QStatus AddInterface(InterfaceDescription iface)
			{
			
				return alljoyn_proxybusobject_addinterface(_proxyBusObject, iface.UnmanagedPtr);
			}

			/**
			     * Make a synchronous method call from this object
			     *
				 * @param ifaceName		Name of the interface being used.
			     * @param methodName       Method being invoked.
			     * @param args         The arguments for the method call (can be NULL)
			     * @param replyMsg     The reply message received for the method call
			     * @param timeout      Timeout specified in milliseconds to wait for a reply
			     * @param flags        Logical OR of the message flags for this method call. The following flags apply to method calls:
			     *                     - If ALLJOYN_FLAG_ENCRYPTED is set the message is authenticated and the payload if any is encrypted.
			     *                     - If ALLJOYN_FLAG_COMPRESSED is set the header is compressed for destinations that can handle header compression.
			     *                     - If ALLJOYN_FLAG_AUTO_START is set the bus will attempt to start a service if it is not running.
			     *
			     *
			     * @return
			     *      - QStatus#OK if the method call succeeded and the reply message type is MESSAGE_METHOD_RET
			     *      - QStatus#BUS_REPLY_IS_ERROR_MESSAGE if the reply message type is MESSAGE_ERROR
			     */
			public QStatus MethodCallSynch(string ifaceName, string methodName, MsgArgs args, Message replyMsg,
				uint timeout, byte flags)
			{
			
				return alljoyn_proxybusobject_methodcall(_proxyBusObject, ifaceName, methodName, args.UnmanagedPtr,
					(UIntPtr)args.Length, replyMsg.UnmanagedPtr, timeout, flags);
			}

            #region DLL Imports
            [DllImport(DLL_IMPORT_TARGET)]
			private static extern IntPtr alljoyn_proxybusobject_create(IntPtr bus,
				[MarshalAs(UnmanagedType.LPStr)] string service,
				[MarshalAs(UnmanagedType.LPStr)] string path,
				uint sessionId);

			[DllImport(DLL_IMPORT_TARGET)]
			private static extern void alljoyn_proxybusobject_destroy(IntPtr bus);

			[DllImport(DLL_IMPORT_TARGET)]
			private static extern int alljoyn_proxybusobject_addinterface(IntPtr bus, IntPtr iface);

			[DllImport(DLL_IMPORT_TARGET)]
			private static extern int alljoyn_proxybusobject_methodcall(IntPtr obj,
				[MarshalAs(UnmanagedType.LPStr)] string ifaceName,
				[MarshalAs(UnmanagedType.LPStr)] string methodName,
				IntPtr args,
				UIntPtr numArgs,
				IntPtr replyMsg,
				uint timeout,
				byte flags);
			#endregion

			#region IDisposable
			/**
			 * Dispose the ProxyBusObject
			 */
			public void Dispose()
            {
                Dispose(true);
				GC.SuppressFinalize(this); 
			}

			/**
			 * Dispose the ProxyBusObject
			 * @param disposing	describes if its activly being disposed
			 */
			protected virtual void Dispose(bool disposing)
			{
			
				if(!_isDisposed)
				{
					alljoyn_proxybusobject_destroy(_proxyBusObject);
					_proxyBusObject = IntPtr.Zero;
				}
				_isDisposed = true;
			}

			~ProxyBusObject()
			{
			
				Dispose(false);
			}
			#endregion

			#region Data
			IntPtr _proxyBusObject;
			bool _isDisposed = false;
			#endregion
		}
	}
}
