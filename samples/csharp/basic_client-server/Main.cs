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
