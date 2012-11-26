/**
 * @file
 * This file defines a class for parsing and generating message bus messages
 */

/******************************************************************************
 * Copyright 2012-2013, Qualcomm Innovation Center, Inc.
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
using System.Text;
using System.Runtime.InteropServices;

namespace AllJoynUnity
{
	public partial class AllJoyn
	{
		public const int ALLJOYN_MAX_NAME_LEN = 255;  /*!<  The maximum length of certain bus names */
		public const int ALLJOYN_MAX_ARRAY_LEN = 131072;  /*!<  DBus limits array length to 2^26. AllJoyn limits it to 2^17 */
		public const int ALLJOYN_MAX_PACKET_LEN = (ALLJOYN_MAX_ARRAY_LEN + 4096);  /*!<  DBus limits packet length to 2^27. AllJoyn limits it further to 2^17 + 4096 to allow for 2^17 payload */

		/** @name Endianness indicators */
		// @{
		/** indicates the bus is little endian */
		public const byte ALLJOYN_LITTLE_ENDIAN = 0x6c; // hex value for ascii letter 'l'
		/** indicates the bus is big endian */
		public const byte ALLJOYN_BIG_ENDIAN = 0x42; // hex value for ascii letter 'B'
		// @}


		/** @name Flag types */
		// @{
		/** No reply is expected*/
		public const byte ALLJOYN_FLAG_NO_REPLY_EXPECTED = 0x01;
		/** Auto start the service */
		public const byte ALLJOYN_FLAG_AUTO_START = 0x02;
		/** Allow messages from remote hosts (valid only in Hello message) */
		public const byte ALLJOYN_FLAG_ALLOW_REMOTE_MSG = 0x04;
		/** Sessionless message  */
		public const byte ALLJOYN_FLAG_SESSIONLESS = 0x10;
		/** Global (bus-to-bus) broadcast */
		public const byte ALLJOYN_FLAG_GLOBAL_BROADCAST = 0x20;
		/** Header is compressed */
		public const byte ALLJOYN_FLAG_COMPRESSED = 0x40;
		/** Body is encrypted */
		public const byte ALLJOYN_FLAG_ENCRYPTED = 0x80;
		// @}

		/** ALLJOYN protocol version */
		public const byte ALLJOYN_MAJOR_PROTOCOL_VERSION = 1;

		/**
		 * Message is a reference counted (managed) version of _Message
		 */
		public class Message : IDisposable
		{
			/** Message types */
			public enum Type : int
			{
				Invalid = 0, ///< an invalid message type
				MethodCall = 1, ///< a method call message type
				MethodReturn = 2, ///< a method return message type
				Error = 3, ///< an error message type
				Signal = 4 ///< a signal message type
			}

			    /**
			     * Constructor for a message
			     *
			     * @param bus  The bus that this message is sent or received on.
			     */
			public Message(BusAttachment bus)
			{
			
				_message = alljoyn_message_create(bus.UnmanagedPtr);
			}

			    /**
			     * Copy constructor for a message
			     *
			     * @param other  Message to copy from.
			     */
			internal Message(IntPtr message)
			{
			
				_message = message;
				_isDisposed = true;
			}

			/**
			 * Return a specific argument.
			 *
			 * @param index  The index of the argument to get.
			 *
			 * @return
			 *      - The argument
			 *      - NULL if unmarshal failed or there is not such argument.
			 */
			public MsgArg GetArg(int index)
			{
			
				IntPtr msgArgs = alljoyn_message_getarg(_message, (UIntPtr)index);
				return (msgArgs != IntPtr.Zero ? new MsgArg(msgArgs) : null);
			}

			/**
			 * Accessor function to get the sender for this message.
			 *
			 * @return
			 *      - The senders well-known name string stored in the AllJoyn header field.
			 *      - An empty string if the message did not specify a sender.
			 */
		    public string GetSender()
		    {
			IntPtr sender = alljoyn_message_getsender(_message);
			return (sender != IntPtr.Zero ? Marshal.PtrToStringAnsi(sender) : null);
		    }

			/**
			 * Return a specific argument.
			 *
			 * @param index  The index of the argument to get.
			 *
			 * @return
			 *      - The argument
			 *      - NULL if unmarshal failed or there is not such argument.
			 */
			public MsgArg this[int i]
			{
				get
				{
					return GetArg(i);
				}
			}

			#region Properties
			/**
			 * Determine if message is a broadcast signal.
			 *
			 * @return  Return true if this is a broadcast signal.
			 */
			public bool IsBroadcastSignal
			{
				get
				{
					return (alljoyn_message_isbroadcastsignal(_message) == 1 ? true : false);
				}
			}

			/**
			 * Messages broadcast to all devices are global broadcast messages.
			 * 
			 * @return  Return true if this is a global broadcast message.
			 */
			public bool IsGlobalBroadcast
			{
				get
				{
					return (alljoyn_message_isglobalbroadcast(_message) == 1 ? true : false);
				}
			}

			/**
			 * Messages sent without sessions are sessionless.
			 *
			 * @return  Return true if this is a sessionless message.
			 */
			public bool IsSessionless
			{
				get
				{
					return (alljoyn_message_issessionless(_message) == 1 ? true : false);
				}
			}
			#endregion

			#region IDisposable
			/**
			 * Dispose the Message
			 */
			public void Dispose()
			{
				Dispose(true);
				GC.SuppressFinalize(this); 
			}

			/**
			 * Dispose the Message
			 * @param disposing	describes if its activly being disposed
			 */
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

			[DllImport(DLL_IMPORT_TARGET)]
			private extern static int alljoyn_message_isbroadcastsignal(IntPtr msg);

			[DllImport(DLL_IMPORT_TARGET)]
			private extern static int alljoyn_message_isglobalbroadcast(IntPtr msg);

			[DllImport(DLL_IMPORT_TARGET)]
			private extern static int alljoyn_message_issessionless(IntPtr msg);

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
