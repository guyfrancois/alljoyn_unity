//-----------------------------------------------------------------------
// <copyright file="SessionTest.cs" company="Qualcomm Innovation Center, Inc.">
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
using AllJoynUnity;
using Xunit;

namespace AllJoynUnityTest
{
	public class MsgArgTest
	{
		[Fact]
		public void BasicAssignment()
		{

			AllJoyn.MsgArg arg = new AllJoyn.MsgArg();
			arg = (byte)42;
			Assert.Equal((byte)42, (byte)arg);
			arg = true;
			Assert.True((bool)arg);

			arg = false;
			Assert.False((bool)arg);

			arg = (short)42;
			Assert.Equal((short)42, (short)arg);

			arg = (ushort)0xBEBE;
			Assert.Equal((ushort)0xBEBE, (ushort)arg);

			arg = (int)-9999;
			Assert.Equal((int)-9999, (int)arg);

			arg = (uint)0x32323232;
			Assert.Equal((uint)0x32323232, (uint)arg);

			arg = (long)-1;
			Assert.Equal((long)-1, (long)arg);

			arg = (ulong)0x6464646464646464;
			Assert.Equal((ulong)0x6464646464646464, (ulong)arg);

			arg = (float)1.61803f;
			Assert.Equal((float)1.61803f, (float)arg);

			arg = (double)3.14159265D;
			Assert.Equal((double)3.14159265D, (double)arg);

			arg = (string)"this is a string";
			Assert.Equal("this is a string", (string)arg);

			AllJoyn.MsgArg arg10 = new AllJoyn.MsgArg(10);
			arg10[0] = (byte)5;
			arg10[1] = true;
			arg10[2] = (short)42;
			arg10[3] = (ushort)45;
			arg10[4] = (int)99;
			arg10[5] = (uint)499;
			arg10[6] = (long)566;
			arg10[7] = (ulong)789301;
			arg10[8] = (double)2.7275;
			arg10[9] = (string)"I say Hello";

			Assert.Equal((byte)5, (byte)arg10[0]);
			Assert.Equal(true, (bool)arg10[1]);
			Assert.Equal((short)42, (short)arg10[2]);
			Assert.Equal((ushort)45, (ushort)arg10[3]);
			Assert.Equal((int)99, (int)arg10[4]);
			Assert.Equal((uint)499, (uint)arg10[5]);
			Assert.Equal((long)566, (long)arg10[6]);
			Assert.Equal((ulong)789301, (ulong)arg10[7]);
			Assert.Equal((double)2.7275, (double)arg10[8]);
			Assert.Equal((string)"I say Hello", (string)arg10[9]);

			//assignment of an array
			AllJoyn.MsgArg arg3 = new AllJoyn.MsgArg(3);
			arg3[0] = (byte)5;
			arg3[1] = (byte)13;
			arg3[2] = (byte)42;

			Assert.Equal((byte)5, (byte)arg3[0]);
			Assert.Equal((byte)13, (byte)arg3[1]);
			Assert.Equal((byte)42, (byte)arg3[2]);

			//older test code to be thrown removed
			AllJoyn.MsgArg argfoo = new AllJoyn.MsgArg();
			argfoo = -9999;
			Assert.Equal((int)-9999, (int)argfoo);

			AllJoyn.MsgArg args = new AllJoyn.MsgArg(10);
			for (int i = 0; i < 10; ++i)
			{
				args[i] = i;
			}

			for (int i = 0; i < 10; ++i)
			{
				Assert.Equal((int)i, (int)args[i]);
			}
		}

		[Fact]
		public void BasicSet()
		{
			AllJoyn.MsgArg arg = new AllJoyn.MsgArg();

			Assert.Equal(AllJoyn.QStatus.OK, arg.Set((byte)13));
			Assert.Equal((byte)13, (byte)arg);

			Assert.Equal(AllJoyn.QStatus.OK, arg.Set(true));
			Assert.True((bool)arg);

			Assert.Equal(AllJoyn.QStatus.OK, arg.Set((short)42));
			Assert.Equal((short)42, (short)arg);

			Assert.Equal(AllJoyn.QStatus.OK, arg.Set((ushort)0xBEBE));
			Assert.Equal((ushort)0xBEBE, (ushort)arg);

			Assert.Equal(AllJoyn.QStatus.OK, arg.Set((int)-9999));
			Assert.Equal((int)-9999, (int)arg);

			Assert.Equal(AllJoyn.QStatus.OK, arg.Set((uint)0x32323232));
			Assert.Equal((uint)0x32323232, (uint)arg);

			Assert.Equal(AllJoyn.QStatus.OK, arg.Set((long)-1));
			Assert.Equal((long)-1, (long)arg);

			Assert.Equal(AllJoyn.QStatus.OK, arg.Set((ulong)0x6464646464646464));
			Assert.Equal((ulong)0x6464646464646464, (ulong)arg);

			Assert.Equal(AllJoyn.QStatus.OK, arg.Set((float)1.61803f));
			Assert.Equal((float)1.61803f, (float)arg);

			Assert.Equal(AllJoyn.QStatus.OK, arg.Set((double)3.14159265D));
			Assert.Equal((double)3.14159265D, (double)arg);

			Assert.Equal(AllJoyn.QStatus.OK, arg.Set((string)"this is a string"));
			Assert.Equal("this is a string", (string)arg);

			AllJoyn.MsgArg arg10 = new AllJoyn.MsgArg(10);
			arg10[0].Set((byte)5);
			arg10[1].Set(true);
			arg10[2].Set((short)42);
			arg10[3].Set((ushort)45);
			arg10[4].Set((int)99);
			arg10[5].Set((uint)499);
			arg10[6].Set((long)566);
			arg10[7].Set((ulong)789301);
			arg10[8].Set((double)2.7275);
			arg10[9].Set((string)"I say Hello");

			Assert.Equal((byte)5, (byte)arg10[0]);
			Assert.Equal(true, (bool)arg10[1]);
			Assert.Equal((short)42, (short)arg10[2]);
			Assert.Equal((ushort)45, (ushort)arg10[3]);
			Assert.Equal((int)99, (int)arg10[4]);
			Assert.Equal((uint)499, (uint)arg10[5]);
			Assert.Equal((long)566, (long)arg10[6]);
			Assert.Equal((ulong)789301, (ulong)arg10[7]);
			Assert.Equal((double)2.7275, (double)arg10[8]);
			Assert.Equal((string)"I say Hello", (string)arg10[9]);

			//older test code to be thrown removed
			AllJoyn.MsgArg argfoo = new AllJoyn.MsgArg();
			argfoo.Set(-9999);
			Assert.Equal((int)-9999, (int)argfoo);

			AllJoyn.MsgArg args = new AllJoyn.MsgArg(10);
			for (int i = 0; i < 10; ++i)
			{
				args[i].Set(i * 3);
			}

			for (int i = 0; i < 10; ++i)
			{
				Assert.Equal((int)i * 3, (int)args[i]);
			}
		}

		[Fact]
		public void BasicGetBasicTypes()
		{
			AllJoyn.MsgArg arg = new AllJoyn.MsgArg();

			Assert.Equal(AllJoyn.QStatus.OK, arg.Set((byte)13));
			byte y;
			Assert.Equal(AllJoyn.QStatus.OK, arg.Get(out y));
			Assert.Equal((byte)13, y);

			Assert.Equal(AllJoyn.QStatus.OK, arg.Set(true));
			bool b;
			Assert.Equal(AllJoyn.QStatus.OK, arg.Get(out b));
			Assert.True(b);

			Assert.Equal(AllJoyn.QStatus.OK, arg.Set((short)42));
			short n;
			Assert.Equal(AllJoyn.QStatus.OK, arg.Get(out n));
			Assert.Equal((short)42, n);

			Assert.Equal(AllJoyn.QStatus.OK, arg.Set((ushort)0xBEBE));
			ushort q;
			Assert.Equal(AllJoyn.QStatus.OK, arg.Get(out q));
			Assert.Equal((ushort)0xBEBE, q);

			Assert.Equal(AllJoyn.QStatus.OK, arg.Set((int)-9999));
			int i;
			Assert.Equal(AllJoyn.QStatus.OK, arg.Get(out i));
			Assert.Equal((int)-9999, i);

			Assert.Equal(AllJoyn.QStatus.OK, arg.Set((uint)0x32323232));
			uint u;
			Assert.Equal(AllJoyn.QStatus.OK, arg.Get(out u));
			Assert.Equal((uint)0x32323232, u);

			Assert.Equal(AllJoyn.QStatus.OK, arg.Set((long)-1));
			long x;
			Assert.Equal(AllJoyn.QStatus.OK, arg.Get(out x));
			Assert.Equal((long)-1, x);

			Assert.Equal(AllJoyn.QStatus.OK, arg.Set((ulong)0x6464646464646464));
			ulong t;
			Assert.Equal(AllJoyn.QStatus.OK, arg.Get(out t));
			Assert.Equal((ulong)0x6464646464646464, t);

			Assert.Equal(AllJoyn.QStatus.OK, arg.Set((double)3.14159265D));
			double d;
			Assert.Equal(AllJoyn.QStatus.OK, arg.Get(out d));
			Assert.Equal((double)3.14159265D, d);

			Assert.Equal(AllJoyn.QStatus.OK, arg.Set((string)"this is a string"));
			string s;
			Assert.Equal(AllJoyn.QStatus.OK, arg.Get(out s));
			Assert.Equal("this is a string", s);
		}

		[Fact]
		public void BasicGet()
		{
			AllJoyn.MsgArg arg = new AllJoyn.MsgArg();

			Assert.Equal(AllJoyn.QStatus.OK, arg.Set((byte)13));
			object y;
			Assert.Equal(AllJoyn.QStatus.OK, arg.Get("y", out y));
			Assert.Equal((byte)13, (byte)y);

			Assert.Equal(AllJoyn.QStatus.OK, arg.Set(true));
			object b;
			Assert.Equal(AllJoyn.QStatus.OK, arg.Get("b", out b));
			Assert.True((bool)b);

			Assert.Equal(AllJoyn.QStatus.OK, arg.Set((short)42));
			object n;
			Assert.Equal(AllJoyn.QStatus.OK, arg.Get("n", out n));
			Assert.Equal((short)42, (short)n);

			Assert.Equal(AllJoyn.QStatus.OK, arg.Set((ushort)0xBEBE));
			object q;
			Assert.Equal(AllJoyn.QStatus.OK, arg.Get("q", out q));
			Assert.Equal((ushort)0xBEBE, (ushort)q);

			Assert.Equal(AllJoyn.QStatus.OK, arg.Set((int)-9999));
			object i;
			Assert.Equal(AllJoyn.QStatus.OK, arg.Get("i", out i));
			Assert.Equal((int)-9999, (object)i);

			Assert.Equal(AllJoyn.QStatus.OK, arg.Set((uint)0x32323232));
			object u;
			Assert.Equal(AllJoyn.QStatus.OK, arg.Get("u", out u));
			Assert.Equal((uint)0x32323232, (uint)u);

			Assert.Equal(AllJoyn.QStatus.OK, arg.Set((long)-1));
			object x;
			Assert.Equal(AllJoyn.QStatus.OK, arg.Get("x", out x));
			Assert.Equal((long)-1, (long)x);

			Assert.Equal(AllJoyn.QStatus.OK, arg.Set((ulong)0x6464646464646464));
			object t;
			Assert.Equal(AllJoyn.QStatus.OK, arg.Get("t", out t));
			Assert.Equal((ulong)0x6464646464646464, (ulong)t);

			Assert.Equal(AllJoyn.QStatus.OK, arg.Set((double)3.14159265D));
			object d;
			Assert.Equal(AllJoyn.QStatus.OK, arg.Get("d", out d));
			Assert.Equal((double)3.14159265D, (double)d);

			Assert.Equal(AllJoyn.QStatus.OK, arg.Set((string)"this is a string"));
			object s;
			Assert.Equal(AllJoyn.QStatus.OK, arg.Get("s", out s));
			Assert.Equal("this is a string", (string)s);

			// The only way to set and get object paths and signatures is to use
			// two param Get and Set methods
			Assert.Equal(AllJoyn.QStatus.OK, arg.Set("o", "/org/foo/bar"));
			object o;
			Assert.Equal(AllJoyn.QStatus.OK, arg.Get("o", out o));
			Assert.Equal("/org/foo/bar", (string)o);

			Assert.Equal(AllJoyn.QStatus.OK, arg.Set("g", "a{is}d(siiux)"));
			object g;
			Assert.Equal(AllJoyn.QStatus.OK, arg.Get("g", out g));
			Assert.Equal("a{is}d(siiux)", (string)g);
		}

		[Fact]
		public void BasicArrayAssignment()
		{
			AllJoyn.MsgArg arg = new AllJoyn.MsgArg();

			//byte
			byte[] in_byte_array = { 9, 19, 29, 39, 49 };
			arg[0] = in_byte_array;
			byte[] out_byte_array = (byte[])arg[0];
			Assert.Equal(in_byte_array.Length, out_byte_array.Length);
			for (int i = 0; i < out_byte_array.Length; i++)
			{
				Assert.Equal(in_byte_array[i], out_byte_array[i]);
			}

			//bool
			bool[] in_bool_array = { false, false, true, true, false, true };
			arg[0].Set(in_bool_array);
			bool[] out_bool_array = (bool[])arg[0];
			Assert.Equal(in_bool_array.Length, out_bool_array.Length);
			for (int i = 0; i < out_bool_array.Length; i++)
			{
				Assert.Equal(in_bool_array[i], out_bool_array[i]);
			}


			AllJoyn.MsgArg arg2 = new AllJoyn.MsgArg(2);
			arg2[0].Set("ab", in_bool_array);
			arg2[1].Set("ab", in_bool_array);
			out_bool_array = (bool[])arg2[0];
			Assert.Equal(in_bool_array.Length, out_bool_array.Length);
			for (int i = 0; i < out_bool_array.Length; i++)
			{
				Assert.Equal(in_bool_array[i], out_bool_array[i]);
			}
			out_bool_array = (bool[])arg2[1];
			Assert.Equal(in_bool_array.Length, out_bool_array.Length);
			for (int i = 0; i < out_bool_array.Length; i++)
			{
				Assert.Equal(in_bool_array[i], out_bool_array[i]);
			}

			//short
			short[] in_short_array = { -9, -99, 999, 9999 };
			arg[0] = in_short_array;
			short[] out_short_array = (short[])arg[0];
			Assert.Equal(in_short_array.Length, out_short_array.Length);
			for (int i = 0; i < out_short_array.Length; i++)
			{
				Assert.Equal(in_short_array[i], out_short_array[i]);
			}

			//ushort
			ushort[] in_ushort_array = { 9, 99, 999, 9999 };
			arg[0] = in_ushort_array;
			ushort[] out_ushort_array = (ushort[])arg[0];
			Assert.Equal(in_ushort_array.Length, out_ushort_array.Length);
			for (int i = 0; i < out_short_array.Length; i++)
			{
				Assert.Equal(in_ushort_array[i], out_ushort_array[i]);
			}

			//int
			int[] in_int_array = { -8, -88, 888, 8888 };
			arg[0] = in_int_array;
			int[] out_int_array = (int[])arg[0];
			Assert.Equal(in_int_array.Length, out_int_array.Length);
			for (int i = 0; i < out_int_array.Length; i++)
			{
				Assert.Equal(in_int_array[i], out_int_array[i]);
			}

			//uint
			uint[] in_uint_array = { 8, 88, 888, 8888 };
			arg[0] = in_uint_array;
			uint[] out_uint_array = (uint[])arg[0];
			Assert.Equal(in_uint_array.Length, out_uint_array.Length);
			for (int i = 0; i < out_int_array.Length; i++)
			{
				Assert.Equal(in_uint_array[i], out_uint_array[i]);
			}

			//long
			long[] in_long_array = { -7, -77, 777, 7777 };
			arg[0] = in_long_array;
			long[] out_long_array = (long[])arg[0];
			Assert.Equal(in_long_array.Length, out_long_array.Length);
			for (int i = 0; i < out_long_array.Length; i++)
			{
				Assert.Equal(in_long_array[i], out_long_array[i]);
			}

			//ulong
			ulong[] in_ulong_array = { 7, 77, 777, 7777 };
			arg[0] = in_ulong_array;
			ulong[] out_ulong_array = (ulong[])arg[0];
			Assert.Equal(in_ulong_array.Length, out_ulong_array.Length);
			for (int i = 0; i < out_long_array.Length; i++)
			{
				Assert.Equal(in_ulong_array[i], out_ulong_array[i]);
			}

			//double
			double[] in_double_array = { 0.001, 0.01, 0.1, 1.0, 10.0, 100.0 };
			arg[0] = in_double_array;
			double[] out_double_array = (double[])arg[0];
			Assert.Equal(in_double_array.Length, out_double_array.Length);
			for (int i = 0; i < out_long_array.Length; i++)
			{
				Assert.Equal(in_double_array[i], out_double_array[i]);
			}

			//string
			string[] in_string_array = { "one", "two", "three", "four" };
			arg[0] = in_string_array;
			string[] out_string_array = (string[])arg[0];
			Assert.Equal(in_string_array.Length, out_string_array.Length);
			for (int i = 0; i < out_string_array.Length; i++)
			{
				Assert.Equal(in_string_array[i], out_string_array[i]);
			}
		}

		[Fact]
		public void BasicArraySetGet()
		{
			AllJoyn.MsgArg arg = new AllJoyn.MsgArg();

			//byte
			byte[] in_byte_array = { 9, 19, 29, 39, 49 };
			Assert.Equal(AllJoyn.QStatus.OK, arg.Set("ay", in_byte_array));
			object ay;
			Assert.Equal(AllJoyn.QStatus.OK, arg.Get("ay", out ay));
			byte[] out_byte_array = (byte[])ay;
			Assert.Equal(in_byte_array.Length, out_byte_array.Length);
			for (int i = 0; i < out_byte_array.Length; i++)
			{
				Assert.Equal(in_byte_array[i], out_byte_array[i]);
			}

			//bool
			bool[] in_bool_array = { false, false, true, true, false, true };
			Assert.Equal(AllJoyn.QStatus.OK, arg.Set("ab", in_bool_array));
			object ab;
			Assert.Equal(AllJoyn.QStatus.OK, arg.Get("ab", out ab));
			bool[] out_bool_array = (bool[])ab;
			Assert.Equal(in_bool_array.Length, out_bool_array.Length);
			for (int i = 0; i < out_bool_array.Length; i++)
			{
				Assert.Equal(in_bool_array[i], out_bool_array[i]);
			}

			//short
			short[] in_short_array = { -9, -99, 999, 9999 };
			Assert.Equal(AllJoyn.QStatus.OK, arg.Set("an", in_short_array));
			object an;
			Assert.Equal(AllJoyn.QStatus.OK, arg.Get("an", out an));
			short[] out_short_array = (short[])an;
			Assert.Equal(in_short_array.Length, out_short_array.Length);
			for (int i = 0; i < out_short_array.Length; i++)
			{
				Assert.Equal(in_short_array[i], out_short_array[i]);
			}

			//ushort
			ushort[] in_ushort_array = { 9, 99, 999, 9999 };
			Assert.Equal(AllJoyn.QStatus.OK, arg.Set("aq", in_ushort_array));
			object aq;
			Assert.Equal(AllJoyn.QStatus.OK, arg.Get("aq", out aq));
			ushort[] out_ushort_array = (ushort[])aq;
			Assert.Equal(in_ushort_array.Length, out_ushort_array.Length);
			for (int i = 0; i < out_short_array.Length; i++)
			{
				Assert.Equal(in_ushort_array[i], out_ushort_array[i]);
			}

			//int
			int[] in_int_array = { -8, -88, 888, 8888 };
			Assert.Equal(AllJoyn.QStatus.OK, arg.Set("ai", in_int_array));
			object ai;
			Assert.Equal(AllJoyn.QStatus.OK, arg.Get("ai", out ai));
			int[] out_int_array = (int[])ai;
			Assert.Equal(in_int_array.Length, out_int_array.Length);
			for (int i = 0; i < out_int_array.Length; i++)
			{
				Assert.Equal(in_int_array[i], out_int_array[i]);
			}

			//uint
			uint[] in_uint_array = { 8, 88, 888, 8888 };
			Assert.Equal(AllJoyn.QStatus.OK, arg.Set("au", in_uint_array));
			object au;
			Assert.Equal(AllJoyn.QStatus.OK, arg.Get("au", out au));
			uint[] out_uint_array = (uint[])au;
			Assert.Equal(in_uint_array.Length, out_uint_array.Length);
			for (int i = 0; i < out_int_array.Length; i++)
			{
				Assert.Equal(in_uint_array[i], out_uint_array[i]);
			}

			//long
			long[] in_long_array = { -7, -77, 777, 7777 };
			Assert.Equal(AllJoyn.QStatus.OK, arg.Set("ax", in_long_array));
			object ax;
			Assert.Equal(AllJoyn.QStatus.OK, arg.Get("ax", out ax));
			long[] out_long_array = (long[])ax;
			Assert.Equal(in_long_array.Length, out_long_array.Length);
			for (int i = 0; i < out_long_array.Length; i++)
			{
				Assert.Equal(in_long_array[i], out_long_array[i]);
			}

			//ulong
			ulong[] in_ulong_array = { 7, 77, 777, 7777 };
			Assert.Equal(AllJoyn.QStatus.OK, arg.Set("at", in_ulong_array));
			object at;
			Assert.Equal(AllJoyn.QStatus.OK, arg.Get("at", out at));
			ulong[] out_ulong_array = (ulong[])at;
			Assert.Equal(in_ulong_array.Length, out_ulong_array.Length);
			for (int i = 0; i < out_long_array.Length; i++)
			{
				Assert.Equal(in_ulong_array[i], out_ulong_array[i]);
			}

			//double
			double[] in_double_array = { 0.001, 0.01, 0.1, 1.0, 10.0, 100.0 };
			Assert.Equal(AllJoyn.QStatus.OK, arg.Set("ad", in_double_array));
			object ad;
			Assert.Equal(AllJoyn.QStatus.OK, arg.Get("ad", out ad));
			double[] out_double_array = (double[])ad;
			Assert.Equal(in_double_array.Length, out_double_array.Length);
			for (int i = 0; i < out_long_array.Length; i++)
			{
				Assert.Equal(in_double_array[i], out_double_array[i]);
			}

			//string
			string[] in_string_array = { "one", "two", "three", "four" };
			Assert.Equal(AllJoyn.QStatus.OK, arg.Set("as", in_string_array));
			object sa;
			Assert.Equal(AllJoyn.QStatus.OK, arg.Get("as", out sa));
			string[] out_string_array = (string[])sa;
			Assert.Equal(in_string_array.Length, out_string_array.Length);
			for (int i = 0; i < out_string_array.Length; i++)
			{
				Assert.Equal(in_string_array[i], out_string_array[i]);
			}

			//object path
			string[] in_path_array = { "/org/one", "/org/two", "/org/three", "/org/four" };
			Assert.Equal(AllJoyn.QStatus.OK, arg.Set("ao", in_path_array));
			object ao;
			Assert.Equal(AllJoyn.QStatus.OK, arg.Get("ao", out ao));
			string[] out_path_array = (string[])ao;
			Assert.Equal(in_path_array.Length, out_path_array.Length);
			for (int i = 0; i < out_path_array.Length; i++)
			{
				Assert.Equal(in_path_array[i], out_path_array[i]);
			}

			//signature
			string[] in_signature_array = { "s", "sss", "as", "a(iiiiuu)" };
			Assert.Equal(AllJoyn.QStatus.OK, arg.Set("ag", in_signature_array));
			object sg;
			Assert.Equal(AllJoyn.QStatus.OK, arg.Get("ag", out sg));
			string[] out_signature_array = (string[])sg;
			Assert.Equal(in_signature_array.Length, out_signature_array.Length);
			for (int i = 0; i < out_signature_array.Length; i++)
			{
				Assert.Equal(in_signature_array[i], out_signature_array[i]);
			}
		}

		[Fact]
		public void Variants()
		{
			double d = 3.14159265;
			string s = "this is a string";
			AllJoyn.MsgArg arg = new AllJoyn.MsgArg();
			AllJoyn.MsgArg variantArg = new AllJoyn.MsgArg();

			Assert.Equal(AllJoyn.QStatus.OK, arg.Set("i", 420));
			Assert.Equal(AllJoyn.QStatus.OK, variantArg.Set("v", arg));
			int i;
			long x;
			Assert.Equal(AllJoyn.QStatus.BUS_SIGNATURE_MISMATCH, variantArg.Get(out x));
			Assert.Equal(AllJoyn.QStatus.OK, variantArg.Get(out i));
			Assert.Equal(420, i);

			Assert.Equal(AllJoyn.QStatus.OK, arg.Set("d", d));
			Assert.Equal(AllJoyn.QStatus.OK, variantArg.Set("v", arg));
			double d_out;
			Assert.Equal(AllJoyn.QStatus.BUS_SIGNATURE_MISMATCH, variantArg.Get(out i));
			Assert.Equal(AllJoyn.QStatus.BUS_SIGNATURE_MISMATCH, variantArg.Get(out s));
			Assert.Equal(AllJoyn.QStatus.OK, variantArg.Get(out d_out));
			Assert.Equal(d, d_out);

			Assert.Equal(AllJoyn.QStatus.OK, arg.Set("s", s));
			Assert.Equal(AllJoyn.QStatus.OK, variantArg.Set("v", arg));
			Assert.Equal(AllJoyn.QStatus.BUS_SIGNATURE_MISMATCH, variantArg.Get(out i));
			string s_out;
			Assert.Equal(AllJoyn.QStatus.OK, variantArg.Get(out s_out));
			Assert.Equal(s, s_out);
		}

		[Fact]
		public void arraysOfArrays()
		{
			int[][] int_array2D2 = new int[][]
			{
				new int[] { 1, 2, 3 },
				new int[] { 4, 5, 6 }
			};

			AllJoyn.MsgArg arg = new AllJoyn.MsgArg();
			Assert.Equal(AllJoyn.QStatus.OK, arg.Set("aai", int_array2D2));

			object out_array;
			Assert.Equal(AllJoyn.QStatus.OK, arg.Get("aai", out out_array));
			
			object[] i_a1 =(object[])out_array; 
			int i_size1 = i_a1.Length;
			int[][] result = new int[i_size1][];
			for(int j = 0; j < i_size1; j++) {
				result[j] = (int[])i_a1[j];
			}

			Assert.Equal(1, result[0][0]);
			Assert.Equal(2, result[0][1]);
			Assert.Equal(3, result[0][2]);
			Assert.Equal(4, result[1][0]);
			Assert.Equal(5, result[1][1]);
			Assert.Equal(6, result[1][2]);

			string[][][] str_array = new string[][][]
			{
				new string[][]
				{
					new string[] {"one", "two", "three"},
					new string[] {"red", "yellow", "blue"}
				},
				new string[][]
				{
					new string[] {"dog", "cat", "fish", "bird"},
					new string[] {"high", "low"},
					new string[] {"strong", "weak", "indifferent", "lazy"}
				}
			};
			Assert.Equal(AllJoyn.QStatus.OK, arg.Set("aaas", str_array));

			object out_str_array;
			Assert.Equal(AllJoyn.QStatus.OK, arg.Get("aai", out out_str_array));

			object[] s_a1 = (object[])out_str_array;
			string[][][] str_result = new string[s_a1.Length][][];
			for (int j = 0; j < s_a1.Length; ++j)
			{
				object[] s_a2 = (object[])s_a1[j];
				str_result[j] = new string[s_a2.Length][];
				for (int k = 0; k < s_a2.Length; ++k)
				{
					str_result[j][k] = (string[])s_a2[k];
				}
			}
			
			for (int x = 0; x < str_result.Length; ++x)
			{
				for (int y = 0; y < str_result[x].Length; ++y)
				{
					for (int z = 0; z < str_result[x][y].Length; ++z)
					{
						Assert.Equal(str_array[x][y][z], str_result[x][y][z]);
					}
				}
			}
		}

		[Fact]
		public void dictionaries()
		{
			System.Collections.Generic.Dictionary<object, object> dict = new System.Collections.Generic.Dictionary<object, object>();
			dict.Add("apple", 2);
			dict.Add("pear", 1);
			dict.Add("bannana", 0);
			dict.Add("kiwi", -1);

			AllJoyn.MsgArg arg = new AllJoyn.MsgArg();
			AllJoyn.QStatus status = arg.Set("a{si}", dict);
			Assert.Equal(AllJoyn.QStatus.OK, status);

			object out_dict;
			status = arg.Get("a{si}", out out_dict);
			Assert.Equal(AllJoyn.QStatus.OK, status);
			Assert.Equal(dict, (System.Collections.Generic.Dictionary<object, object>)out_dict);
		}

		public struct TestStruct //(issi)
		{
			public int a;
			public string b;
			public string c;
			public int d;
		}

		[Fact]
		public void AllJoynStructs()
		{
			TestStruct testStruct = new TestStruct();
			testStruct.a = 42;
			testStruct.b = "Hello";
			testStruct.c = "World";
			testStruct.d = 88;

			AllJoyn.MsgArg arg = new AllJoyn.MsgArg();
			object[] mystruct = new object[4];
			mystruct[0] = testStruct.a;
			mystruct[1] = testStruct.b;
			mystruct[2] = testStruct.c;
			mystruct[3] = testStruct.d;
			AllJoyn.QStatus status = arg.Set("(issi)", mystruct);
			Assert.Equal(AllJoyn.QStatus.OK, status);

			object outstruct;
			status = arg.Get("(issi)", out outstruct);
			Assert.Equal(AllJoyn.QStatus.OK, status);
			object[] outstructa = (object[])outstruct;
			Assert.Equal(4, outstructa.Length);
			Assert.Equal(testStruct.a, (int)outstructa[0]);
			Assert.Equal(testStruct.b, (string)outstructa[1]);
			Assert.Equal(testStruct.c, (string)outstructa[2]);
			Assert.Equal(testStruct.d, (int)outstructa[3]);
			arg.Dispose();
			arg = new AllJoyn.MsgArg();

			//struct within a struct
			object[] mystruct2 = new object[2];
			mystruct2[0] = "bob";
			mystruct2[1] = mystruct;
			status = arg.Set("(s(issi))", mystruct2);
			Assert.Equal(AllJoyn.QStatus.OK, status);

			status = arg.Get("(s(issi))", out outstruct);
			object[] outstruct1 = (object[])outstruct;
			Assert.Equal(2, outstruct1.Length);
			object[] outstruct2 = (object[])outstruct1[1];
			Assert.Equal(4, outstruct2.Length);

			Assert.Equal("bob", (string)outstruct1[0]);
			Assert.Equal(testStruct.a, (int)outstruct2[0]);
			Assert.Equal(testStruct.b, (string)outstruct2[1]);
			Assert.Equal(testStruct.c, (string)outstruct2[2]);
			Assert.Equal(testStruct.d, (int)outstruct2[3]);
		}

		[Fact]
		public void InvalidAssignment()
		{
			AllJoyn.MsgArg arg = new AllJoyn.MsgArg();
			Assert.Throws<System.InvalidCastException>(() => arg.Set("i", true));
			Assert.Throws<System.InvalidCastException>(() => arg.Set("i", (byte)7));
			Assert.Throws<System.InvalidCastException>(() => arg.Set("i", (short)42));
			Assert.Throws<System.InvalidCastException>(() => arg.Set("i", (ushort)42));
			Assert.Throws<System.InvalidCastException>(() => arg.Set("i", "Error Gold"));
		}

		[Fact]
		public void SplitSignature()
		{
			string[] a = AllJoyn.MsgArg.splitSignature("issi");
			Assert.Equal(4, a.Length);
			Assert.Equal(a[0], "i");
			Assert.Equal(a[1], "s");
			Assert.Equal(a[2], "s");
			Assert.Equal(a[3], "i");

			a = AllJoyn.MsgArg.splitSignature("aas(issi)a{is}(i)(i(suasi(issi)(a{sv})))");
			Assert.Equal(5, a.Length);
			Assert.Equal(a[0], "aas");
			Assert.Equal(a[1], "(issi)");
			Assert.Equal(a[2], "a{is}");
			Assert.Equal(a[3], "(i)");
			Assert.Equal(a[4], "(i(suasi(issi)(a{sv})))");

			//unbalanced containers
			a = AllJoyn.MsgArg.splitSignature("(ai(ss)");
			Assert.Null(a);
			a = AllJoyn.MsgArg.splitSignature("a{si");
			Assert.Null(a);
		}
	}
}