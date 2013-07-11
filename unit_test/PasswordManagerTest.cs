//-----------------------------------------------------------------------
// <copyright file="PasswordManagerTest.cs" company="Qualcomm Innovation Center, Inc.">
// Copyright 2013, Qualcomm Innovation Center, Inc.
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
using AllJoynUnity;
using Xunit;

namespace AllJoynUnityTest
{
	public class PasswordManagerTest
	{
		[Fact]
		public void SetCredentials()
		{
			AllJoyn.BusAttachment busAttachment = new AllJoyn.BusAttachment("PasswordManagerTest", false);
			Assert.Equal(AllJoyn.QStatus.OK, AllJoyn.PasswordManager.SetCredentials("ALLJOYN_PIN_KEYX", "1234"));
			busAttachment.Dispose();
		}
	}
}