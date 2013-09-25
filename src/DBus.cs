/**
 * @file
 * This file provides definitions for standard DBus interfaces
 *
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
using System.Runtime.InteropServices;

namespace AllJoynUnity
{
	public partial class AllJoyn
	{
		/**
		 * This file provides definitions for standard DBus interfaces
		 */
		public class DBus
		{
			[Flags]
			/**
			 * DBus RequestName input params.
			 * org.freedesktop.DBus.RequestName input params (see DBus spec)
			 */
			public enum NameFlags : uint
			{
				None = 0x00, /**< Do not change the default RequestName behavor */
				AllowReplacement = 0x01, /**< RequestName input flag: Allow others to take ownership of this name */
				ReplaceExisting = 0x02, /**< RequestName input flag: Attempt to take ownership of name if already taken */
				DoNotQueue = 0x04 /**< RequestName input flag: Fail if name cannot be immediately obtained */
			}

			/**
			 * DBus RequestName return values
			 * org.freedesktop.DBus.RequestName return values (see DBus spec)
			 */
			public enum RequestNameReply : uint
			{
				PrimaryOwner = 1, /**< The caller is now the primary owner of the name, replacing any previous owner. */
				InQueue = 2, /**< The name already had an owner and the name request has been added to a queue */
				Exists = 3, /**< The name already has an owner and the name request was not added to a queue */
				AlreadyOwner = 4 /**< The application trying to request ownership of a name is already the owener of it */
			}
		}
	}
}
