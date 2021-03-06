﻿//-----------------------------------------------------------------------
// <copyright file="AllJoynTestCommon.cs" company="Qualcomm Innovation Center, Inc.">
// Copyright 2012-2013, Qualcomm Innovation Center, Inc.
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

namespace AllJoynUnityTest
{
	class AllJoynTestCommon
	{
		public static string GetConnectSpec()
		{
			if (Environment.OSVersion.Platform == System.PlatformID.Unix || Environment.OSVersion.Platform == System.PlatformID.MacOSX)
			{
				return "unix:abstract=alljoyn";
			}
			else
			{
				// Unix platforms are the only platforms we expect to have a non bundled daemon.
				// In the past we would return "tcp:addr=127.0.0.1,port=9956" however something
				// about the way xunit runs its test code caused this to have error when calling
				// BusAttachment.Connect(...)
				return "null:";
			}
		}
	}
}
