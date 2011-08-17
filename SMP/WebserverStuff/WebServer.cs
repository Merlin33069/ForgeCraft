using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace SMP
{
	class WebServer
	{
		public static Socket listen;
		private int port = 5050;  // Select any free port you wish 

		public void Start()
		{
			try
			{
				IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, port);
				listen = new Socket(endpoint.Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
				listen.Bind(endpoint);
				listen.Listen((int)SocketOptionName.MaxConnections);

				listen.BeginAccept(new AsyncCallback(Accept), null);
			}
			catch (SocketException e) { Server.Log(e.Message + e.StackTrace); }
			catch (Exception e) { Server.Log(e.Message + e.StackTrace); }
		}

		void Accept(IAsyncResult result)
		{
			if (Server.s.shuttingDown == false)
			{
				Player p = null;
				bool begin = false;
				try
				{
					Socket s = listen.EndAccept(result);
					listen.BeginAccept(new AsyncCallback(Accept), null);
					begin = true;

					//do stuff ?

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
					Server.Log(e.Message);
					Server.Log(e.StackTrace);
					if (p != null)
						p.Disconnect();
					if (!begin)
						listen.BeginAccept(new AsyncCallback(Accept), null);
				}
			}
		}
	}
}
