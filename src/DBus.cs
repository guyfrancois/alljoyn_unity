/**
 * @file
 * This file provides definitions for standard DBus interfaces
 *
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
		 * This file provides definitions for standard DBus interfaces
		 */
		public class DBus
		{
			[Flags]
			/**
			 * @name DBus RequestName input params
			 * org.freedesktop.DBus.RequestName input params (see DBus spec)
			 */
			public enum NameFlags : uint
			{
				AllowReplacement = 0x01, /**< RequestName input flag: Allow others to take ownership of this name */
				ReplaceExisting = 0x02, /**< RequestName input flag: Attempt to take ownership of name if already taken */
				DoNotQueue = 0x04 /**< RequestName input flag: Fail if name cannot be immediately obtained */
			}
		}
	}
}
