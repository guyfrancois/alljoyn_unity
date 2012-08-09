//-----------------------------------------------------------------------
// <copyright file="Main.cs" company="Qualcomm Innovation Center, Inc.">
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

namespace basic_clientserver
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			Console.WriteLine("AllJoyn Library version: " + AllJoyn.GetVersion());
			Console.WriteLine("AllJoyn Library buildInfo: " + AllJoyn.GetBuildInfo());

			BasicServer basicServer = new BasicServer();
			BasicClient basicClient = new BasicClient();

			basicClient.Connect();

			while(!basicClient.Connected)
			{
				System.Threading.Thread.Sleep(1);
			}

			Console.WriteLine("BasicClient.CallRemoteMethod returned '{0}'", basicClient.CallRemoteMethod());
            
			while(basicServer.KeepRunning)
			{
				System.Threading.Thread.Sleep(1);
                //System.GC.Collect();
                //System.GC.WaitForPendingFinalizers();
                //System.GC.WaitForFullGCComplete();
                //System.GC.Collect();
                Console.WriteLine("BasicClient.CallRemoteMethod returned '{0}'", basicClient.CallRemoteMethod());

			}
		}
	}
}
