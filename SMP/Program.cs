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
	}
}
