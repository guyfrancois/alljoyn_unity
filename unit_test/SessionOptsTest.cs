//-----------------------------------------------------------------------
// <copyright file="SessionOptsTest.cs" company="Qualcomm Innovation Center, Inc.">
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
	public class SessionOptsTest
	{
		[Fact]
		public void AccessorFunctions()
		{
			
			AllJoyn.SessionOpts opts = new AllJoyn.SessionOpts(AllJoyn.SessionOpts.TrafficType.Messages,
				false, AllJoyn.SessionOpts.ProximityType.Any, AllJoyn.TransportMask.Any);

			Assert.Equal(AllJoyn.SessionOpts.TrafficType.Messages, opts.Traffic);
			Assert.False(opts.IsMultipoint);
			Assert.Equal(AllJoyn.SessionOpts.ProximityType.Any, opts.Proximity);
			Assert.Equal(AllJoyn.TransportMask.Any, opts.Transports);

			opts.Traffic = AllJoyn.SessionOpts.TrafficType.RawReliable;
			Assert.Equal(AllJoyn.SessionOpts.TrafficType.RawReliable, opts.Traffic);

			opts.IsMultipoint = true;
			Assert.True(opts.IsMultipoint);

			opts.Proximity = AllJoyn.SessionOpts.ProximityType.Network;
			Assert.Equal(AllJoyn.SessionOpts.ProximityType.Network, opts.Proximity);

			opts.Transports = AllJoyn.TransportMask.Bluetooth;
			Assert.Equal(AllJoyn.TransportMask.Bluetooth, opts.Transports);
		}
	}
}