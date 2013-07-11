/**
 * @file
 * This file defines the PasswordManager class that provides the interface to
 * set credentials used for the authentication of thin clients.
 */

/******************************************************************************
 * Copyright 2013, Qualcomm Innovation Center, Inc.
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
		 * Class to allow the user or application to set credentials used for the authentication
		 * of thin clients.
		 * Before invoking Connect() to BusAttachment, the application should call SetCredentials
		 * if it expects to be able to communicate to/from thin clients.
		 * The bundled daemon will start advertising the name as soon as it is started and MUST have
		 * the credentials set to be able to authenticate any thin clients that may try to use the
		 * bundled daemon to communicate with the app.
		 */
		public static class PasswordManager
		{
			/**
			 * Set credentials used for the authentication of thin clients.
			 *
			 * @param authMechanism  Mechanism to use for authentication.
			 * @param password       Password to use for authentication.
			 *
			 * @return   Returns QStatus.OK if the credentials was successfully set.
			 */
			public static QStatus SetCredentials(string authMechanism, string password)
			{
				return alljoyn_passwordmanager_setcredentials(authMechanism, password);
			}

			#region DLL Imports
			[DllImport(DLL_IMPORT_TARGET)]
			private static extern int alljoyn_passwordmanager_setcredentials([MarshalAs(UnmanagedType.LPStr)] string authMechanism,
																			[MarshalAs(UnmanagedType.LPStr)] string password);
			#endregion
		}
	}
}