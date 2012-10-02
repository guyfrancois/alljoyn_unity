//-----------------------------------------------------------------------
// <copyright file="SessionTest.cs" company="Qualcomm Innovation Center, Inc.">
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
using AllJoynUnity;
using Xunit;

namespace AllJoynUnityTest
{
	public class MsgArgTest
	{
		[Fact]
		public void BasicAssignment()
		{
			AllJoyn.MsgArgs arg = new AllJoyn.MsgArgs(1);
			arg[0] = (byte)42;
			Assert.Equal((byte)42, (byte)arg[0]);
			//Assert.Equal(typeof(byte), arg[0].GetType());
			arg[0] = true;
			Assert.True((bool)arg[0]);

			arg[0] = false;
			Assert.False((bool)arg[0]);

			arg[0] = (short)42;
			Assert.Equal((short)42, (short)arg[0]);

			arg[0] = (ushort)0xBEBE;
			Assert.Equal((ushort)0xBEBE, (ushort)arg[0]);

			arg[0] = (int)-9999;
			Assert.Equal((int)-9999, (int)arg[0]);

			arg[0] = (uint)0x32323232;
			Assert.Equal((uint)0x32323232, (uint)arg[0]);

			arg[0] = (long)-1;
			Assert.Equal((long)-1, (long)arg[0]);

			arg[0] = (ulong)0x6464646464646464;
			Assert.Equal((ulong)0x6464646464646464, (ulong)arg[0]);

			arg[0] = (float)1.61803f;
			Assert.Equal((float)1.61803f, (float)arg[0]);

			arg[0] = (double)3.14159265D;
			Assert.Equal((double)3.14159265D, (double)arg[0]);

			arg[0] = (string)"this is a string";
			Assert.Equal("this is a string", (string)arg[0]);

			arg[0].ObjectPath = "/org/foo/bar";
			Assert.Equal("/org/foo/bar", arg[0].ObjectPath);

			//assignment of an array
			AllJoyn.MsgArgs arg3 = new AllJoyn.MsgArgs(3);
			arg3[0] = (byte)5;
			arg3[1] = (byte)13;
			arg3[2] = (byte)42;

			Assert.Equal((byte)5, (byte)arg3[0]);
			Assert.Equal((byte)13, (byte)arg3[1]);
			Assert.Equal((byte)42, (byte)arg3[2]);
		}

		[Fact]
		public void BasicSet()
		{
			AllJoyn.MsgArgs arg = new AllJoyn.MsgArgs(1);

			Assert.Equal(AllJoyn.QStatus.OK, arg[0].Set((byte)13));
			Assert.Equal((byte)13, (byte)arg[0]);
			
			Assert.Equal(AllJoyn.QStatus.OK, arg[0].Set(true));
			Assert.True((bool)arg[0]);

			Assert.Equal(AllJoyn.QStatus.OK, arg[0].Set((short)42));
			Assert.Equal((short)42, (short)arg[0]);

			Assert.Equal(AllJoyn.QStatus.OK, arg[0].Set((ushort)0xBEBE));
			Assert.Equal((ushort)0xBEBE, (ushort)arg[0]);

			Assert.Equal(AllJoyn.QStatus.OK, arg[0].Set((int)-9999));
			Assert.Equal((int)-9999, (int)arg[0]);

			Assert.Equal(AllJoyn.QStatus.OK, arg[0].Set((uint)0x32323232));
			Assert.Equal((uint)0x32323232, (uint)arg[0]);

			Assert.Equal(AllJoyn.QStatus.OK, arg[0].Set((long)-1));
			Assert.Equal((long)-1, (long)arg[0]);

			Assert.Equal(AllJoyn.QStatus.OK, arg[0].Set((ulong)0x6464646464646464));
			Assert.Equal((ulong)0x6464646464646464, (ulong)arg[0]);

			Assert.Equal(AllJoyn.QStatus.OK, arg[0].Set((float)1.61803f));
			Assert.Equal((float)1.61803f, (float)arg[0]);

			Assert.Equal(AllJoyn.QStatus.OK, arg[0].Set((double)3.14159265D));
			Assert.Equal((double)3.14159265D, (double)arg[0]);

			Assert.Equal(AllJoyn.QStatus.OK, arg[0].Set((string)"this is a string"));
			Assert.Equal("this is a string", (string)arg[0]);

			arg[0].ObjectPath = "/org/foo/bar";
			Assert.Equal("/org/foo/bar", arg[0].ObjectPath);

			// todo  no support for signature type
		}
	}
}