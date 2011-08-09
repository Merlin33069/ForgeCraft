using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace SMP
{
	class Program
	{
		static bool exit = false;
		static void Main(string[] args)
		{
			//Application.ThreadException += new ThreadExceptionEventHandler(Application_ThreadException);
			//AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

			new Thread(new ThreadStart(StartServer)).Start();
			while (!exit)
			{
				Console.ReadLine();
			}
		}
		public static void StartServer()
		{
			new Server();
		}

		static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			
		}
	}
}
