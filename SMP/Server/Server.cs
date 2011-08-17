using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using MonoTorrent.Client;
using System.Threading;

namespace SMP
{
	public class Server
	{
		public static Server s;
		public bool shuttingDown = false;
		public static Socket listen;
		public static World mainlevel;
		public static int protocolversion = 14;
		public static string Version = "0.1";
		public static string name = "sc";
		public static int port = 25565; //DEBUGGING CHANGE BACK TO 25565
		public static bool unsafe_plugin = false;
		public static Logger ServerLogger = new Logger();
		internal ConsolePlayer consolePlayer;
		
		public static string KickMessage = "You've been kicked!!";
		public static string Motd = "Powered By ForgeCraft.";
		public static int MaxPlayers = 16;
		
		public static System.Timers.Timer updateTimer = new System.Timers.Timer(100);
		public static MainLoop ml;

		public Server()
		{
			Log("Starting Server");
			s = this;
			consolePlayer = new ConsolePlayer(s);
			mainlevel = new World(0, 127, 0, "main", new Random().Next());
			World.worlds.Add(mainlevel);
			ml = new MainLoop("server");
			#region updatetimer
			ml.Queue(delegate
			{
				updateTimer.Elapsed += delegate
				{
					Player.GlobalUpdate();
				}; updateTimer.Start();
			});
			#endregion

			//Setup();

			Log("Server Started");
		}

		public bool Setup()
		{
			//TODO: (in order)
			//load configuration
			Command.InitCore();
			BlockChange.InitAll();
			Plugin.Load();
			//load groups			
			//load whitelist, banlist, VIPlist
			
			try
			{
				IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, port);
				listen = new Socket(endpoint.Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
				listen.Bind(endpoint);
				listen.Listen((int)SocketOptionName.MaxConnections);

				listen.BeginAccept(new AsyncCallback(Accept), null);
				return true;
			}
			catch (SocketException e) { Log(e.Message + e.StackTrace); return false; }
			catch (Exception e) { Log(e.Message + e.StackTrace); return false; }
		}

		void Accept(IAsyncResult result)
		{
			if (shuttingDown == false)
			{
				Player p = null;
				bool begin = false;
				try
				{
					p = new Player();
					
						p.socket = listen.EndAccept(result);
						new Thread(new ThreadStart(p.Start)).Start();
					
					listen.BeginAccept(new AsyncCallback(Accept), null);
					begin = true;
				}
				catch (SocketException e)
				{
					if (p != null)
						p.Disconnect();
					if (!begin)
						listen.BeginAccept(new AsyncCallback(Accept), null);
				}
				catch (Exception e)
				{
					Log(e.Message);
					Log(e.StackTrace);
					if (p != null)
						p.Disconnect();
					if (!begin)
						listen.BeginAccept(new AsyncCallback(Accept), null);
				}
			}
		}

		/// <summary>
		/// To be depracted and replaced with ServerLogger 
		/// </summary>
		/// <param name="message">
		/// A <see cref="System.String"/>
		/// </param>
		public static void Log(string message)
		{
			ServerLogger.Log(message);
		}
		
		public void Stop()
		{
			List<Player> templist = Player.players;
			Thread.Sleep(200);
			foreach (Player p in templist)
			{
				p.Kick("Server Shutting Down!");
			}
			Thread.Sleep(200);
			if (listen != null)
            {
                listen.Close();
                listen = null;
            }
		}
	}
}
