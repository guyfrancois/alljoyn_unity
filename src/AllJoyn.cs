//-----------------------------------------------------------------------
// <copyright file="AllJoyn.cs" company="Qualcomm Innovation Center, Inc.">
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
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Threading;

namespace AllJoynUnity
{
    public partial class AllJoyn
    {
        // DLL name for externs
        private const string DLL_IMPORT_TARGET = "alljoyn_c";
        private const string UNITY_VERSION = ".3";

        private static Thread callbackPumpThread = null;
        private static Boolean isProcessing = false;

        public static string GetExtensionVersion()
        {
            return UNITY_VERSION;
        }

        /// Get the version string from AllJoyn.
        public static string GetVersion()
        {
            return Marshal.PtrToStringAnsi(alljoyn_getversion());
        }

        /// Get the build info string from AllJoyn.
        public static string GetBuildInfo()
        {
            return Marshal.PtrToStringAnsi(alljoyn_getbuildinfo());
        }

        public static void StartAllJoynCallbackProcessing()
        {
            if (callbackPumpThread == null)
            {
                alljoin_unity_set_deferred_callback_mainthread_only(1);//FOR ANDROID THIS NEEDS TO BE SET TO 1
                callbackPumpThread = new Thread((object o) =>
                {
                    int numprocessed = 0;
                    while (isProcessing)
                    {
                        numprocessed = alljoyn_unity_deferred_callbacks_process();
                        Thread.Sleep(50);
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

        public static void StopAllJoynProcessing()
        {
            if (callbackPumpThread != null && callbackPumpThread.IsAlive)
            {
                isProcessing = false;
                Thread.Sleep(25);
                callbackPumpThread.Join();
            }
            callbackPumpThread = null;
        }

        /// Call to trigger callbacks on main thread.
        public static int TriggerCallbacks()
        {
            return alljoyn_unity_deferred_callbacks_process();
        }

        ///// Enable/disable main-thread-only callbacks.
        private static void SetMainThreadOnlyCallbacks(bool mainThreadOnly)
        {
            alljoin_unity_set_deferred_callback_mainthread_only(mainThreadOnly ? 1 : 0);
        }

        [Flags]
        public enum TransportMask : ushort
        {
            None = 0x0000,
            Any = 0xFFFF,
            Local = 0x0001,
            Bluetooth = 0x0002,
            WLAN = 0x0004,
            WWAN = 0x0008,
            LAN = 0x0010
        }

        #region DLL Imports
        [DllImport(DLL_IMPORT_TARGET, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private extern static IntPtr alljoyn_getversion();

        [DllImport(DLL_IMPORT_TARGET)]
        private extern static IntPtr alljoyn_getbuildinfo();

        [DllImport(DLL_IMPORT_TARGET)]
        private extern static IntPtr QCC_StatusText(int status);

        [DllImport(DLL_IMPORT_TARGET)]
        private extern static int alljoyn_unity_deferred_callbacks_process();

        [DllImport(DLL_IMPORT_TARGET)]
        private extern static void alljoin_unity_set_deferred_callback_mainthread_only(int mainthread_only);

        #endregion
    }
}

