/**
 * @file AllJoyn.cs provides namespace for AllJoyn and methods to request
 * the library for version information.
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
using System.Diagnostics;
using System.Threading;

namespace AllJoynUnity
{
/**
 * %AllJoyn namespace and methods to request the library for version information, also starts AllJoyn processing for callbacks.
 */
	public partial class AllJoyn
	{
		// DLL name for externs
		private const string DLL_IMPORT_TARGET = "alljoyn_c";
        private const string UNITY_VERSION = ".3";

        private static readonly int DEFERED_CALLBACK_WAIT_TIMER = 50;

        private static Thread callbackPumpThread = null;
        private static Boolean isProcessing = false;

	/**
	 * Request version of the C# binding
	 *
	 * @return string representing the version of the csharp language binding
	 */
        public static string GetExtensionVersion()
        {
            return UNITY_VERSION;
        }

	/**
	 *  Get the version string from AllJoyn.
	 *
	 * @return string representiong the version from AllJoyn
	 */
	public static string GetVersion()
	{
		return Marshal.PtrToStringAnsi(alljoyn_getversion());
	}

	/**
	 *  Get the build info string from AllJoyn
	 *
	 * @return string representiong the buid info from AllJoyn
	 */
	public static string GetBuildInfo()
	{
		return Marshal.PtrToStringAnsi(alljoyn_getbuildinfo());
	}

	/**
	 * Starts a thread to process AllJoyn callback data.
	 */
        public static void StartAllJoynCallbackProcessing()
        {
            if (callbackPumpThread == null)
            {
                alljoin_unity_set_deferred_callback_mainthread_only(1); //FOR ANDROID THIS NEEDS TO BE SET TO 1 INSTEAD OF 0
                callbackPumpThread = new Thread((object o) =>
                {
                    int numprocessed = 0;
                    while (isProcessing)
                    {
                        numprocessed = alljoyn_unity_deferred_callbacks_process();
                        Thread.Sleep(DEFERED_CALLBACK_WAIT_TIMER);
                    }
                });
            }
            if (!callbackPumpThread.IsAlive)
            {
                isProcessing = true;
                callbackPumpThread.Start();
            }
            alljoyn_unity_deferred_callbacks_process();
        }

	/**
	 * Stops the thread processing AllJoyn callback data.
	 */
        public static void StopAllJoynProcessing()
        {
            if (callbackPumpThread != null && callbackPumpThread.IsAlive)
            {
                isProcessing = false;
                Thread.Sleep(DEFERED_CALLBACK_WAIT_TIMER);
                callbackPumpThread.Join();
            }
            callbackPumpThread = null;
        }

	
	/**
	 * Call to trigger callbacks on main thread.
	 */
        private static int TriggerCallbacks()
        {
            return alljoyn_unity_deferred_callbacks_process();
        }

	/**
	 * Enable/disable main-thread-only callbacks.
	 */ 
        private static void SetMainThreadOnlyCallbacks(bool mainThreadOnly)
        {
            alljoin_unity_set_deferred_callback_mainthread_only(mainThreadOnly ? 1 : 0);
        }

	[Flags]
	/** Bitmask of all transport types */
	public enum TransportMask : ushort
	{
		None = 0x0000, /**< no transports */
		Any = 0xFFFF, /**< ANY transport */
		Local = 0x0001, /**< Local (same device) transport */
		Bluetooth = 0x0002, /**< Bluetooth transport */
		WLAN = 0x0004, /**< Wireless local-area network transport */
		WWAN = 0x0008, /**< Wireless wide-area network transport */
		LAN = 0x0010 /**< Wired local-area network transport */
	}

	#region DLL Imports
	[DllImport(DLL_IMPORT_TARGET, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
	private extern static IntPtr alljoyn_getversion();

	[DllImport(DLL_IMPORT_TARGET)]
	private extern static IntPtr alljoyn_getbuildinfo();

	[DllImport(DLL_IMPORT_TARGET)]
	private extern static int alljoyn_unity_deferred_callbacks_process();

	[DllImport(DLL_IMPORT_TARGET)]
	private extern static void alljoin_unity_set_deferred_callback_mainthread_only(int mainthread_only);

	#endregion
	}
}

