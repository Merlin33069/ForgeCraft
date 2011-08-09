using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace SMP
{
	public partial class Player : System.IDisposable
	{
		public static List<Player> players = new List<Player>();
		Socket socket;
		public World level;
		static Random random = new Random();

		byte[] buffer = new byte[0];
		byte[] tempbuffer = new byte[0xFF];
		bool disconnected = false;
        public bool LoggedIn { get; protected set; }

		double Stance { get { return e.Stance; } set { e.Stance = value; } }
		double[] pos { get { return e.pos; } set { e.pos = value; } }
		float[] rot { get { return e.rot; } set { e.rot = value; } }
		byte onground { get { return e.OnGround; } set { e.OnGround = value; } } //really a bool, but were going to hold it as a byte (1 or 0 ONLY) so we can send it easier
		int id { get { return e.id; } }
		byte dimension { get { return e.dimension; } set { e.dimension = value; } } //-1 for nether, 0 normal, 1 skyworld?

		Entity e;
		public string ip;
		public string username;
		bool hidden = false;

		bool MapSent = false;
		

		public Player(Socket s)
		{
			try
			{
				e = new Entity(new double[3] { 0, 72, 0 }, new float[2] { 0, 0 }, this);
				socket = s;
				ip = socket.RemoteEndPoint.ToString().Split(':')[0];
				Server.Log(ip + " connected to the server.");
				level = Server.mainlevel;
				dimension = 0;
				players.Add(this);
				socket.BeginReceive(tempbuffer, 0, tempbuffer.Length, SocketFlags.None, new AsyncCallback(Receive), this);
			}
			catch (Exception e)
			{
				Server.Log(e.Message);
				Server.Log(e.StackTrace);
			}
		}

		static void Receive(IAsyncResult result)
		{
			Player p = (Player)result.AsyncState;
			if (p.disconnected || p.socket == null)
				return;
			try
			{
				int length = p.socket.EndReceive(result);
				if (length == 0) { p.Disconnect(); return; }

				byte[] b = new byte[p.buffer.Length + length];
				Buffer.BlockCopy(p.buffer, 0, b, 0, p.buffer.Length);
				Buffer.BlockCopy(p.tempbuffer, 0, b, p.buffer.Length, length);

				p.buffer = p.HandleMessage(b);
				p.socket.BeginReceive(p.tempbuffer, 0, p.tempbuffer.Length, SocketFlags.None,
									  new AsyncCallback(Receive), p);
			}
			catch (SocketException e)
			{
				p.Disconnect();
			}
			catch (ObjectDisposedException e)
			{
				
			}
			catch (Exception e)
			{
				Server.Log(e.Message);
			}
		}
		byte[] HandleMessage(byte[] buffer)
		{
			try
			{
				int length = 0; byte msg = buffer[0];
				// Get the length of the message by checking the first byte
				switch (msg)
				{
					case 0x00: length = 0; break; //Keep alive
					case 0x01: /*Server.Log("auth start");*/ length = ((util.EndianBitConverter.Big.ToInt16(buffer, 5) * 2) + 15); break; //Login Request
					case 0x02: length = ((util.EndianBitConverter.Big.ToInt16(buffer, 1) * 2) + 2); break; //Handshake
					case 0x03: length = ((util.EndianBitConverter.Big.ToInt16(buffer, 1) * 2) + 2); break; //Chat
					case 0x07: length = 9; break; //Entity Use
					case 0x09: length = 1; break; //respawn
					
					case 0x0A: length = 1; break; //OnGround incoming
					case 0x0B: length = 33; break; //Pos incoming
					case 0x0C: length = 9; break; //Look Incoming
					case 0x0D: length = 41; break; //Pos and look incoming

					case 0x0E: length = 11; break; //Digging
					case 0x0F: if (util.EndianBitConverter.Big.ToInt16(buffer, 11) >= 0) length = 15; else length = 12; break; //Block Placement
					case 0x10: length = 2; break; //Holding Change
					case 0x12: length = 5; break; //Animation Change
					case 0x13: length = 5; break; //Entity Action

					case 0x65: length = 1; break; //Close Window
					case 0x66:
						length = 9;
						if (util.EndianBitConverter.Big.ToInt16(buffer, 8) != -1) length += 3;
						break; //Clicked window
					case 0x82:
						short a = (short)(util.EndianBitConverter.Big.ToInt16(buffer, 10) * 2);
						short b = (short)(util.EndianBitConverter.Big.ToInt16(buffer, 12 + (a/2)) * 2);
						short c = (short)(util.EndianBitConverter.Big.ToInt16(buffer, 14 + (a/2)+(b/2)) * 2);
						short d = (short)(util.EndianBitConverter.Big.ToInt16(buffer, 16 + (a/2) + (b/2) + (c/2)) * 2);
						length = 18 + a + b + c + d;
						break;
					case 0xFF: length = ((util.EndianBitConverter.Big.ToInt16(buffer, 1) * 2) + 2); break;

					default:
						Server.Log("unhandled message id " + msg);
					    Kick("Unknown Packet id: " + msg);
						return new byte[0];
				}
				if (buffer.Length > length)
				{
					byte[] message = new byte[length];
					Buffer.BlockCopy(buffer, 1, message, 0, length);

					byte[] tempbuffer = new byte[buffer.Length - length - 1];
					Buffer.BlockCopy(buffer, length + 1, tempbuffer, 0, buffer.Length - length - 1);

					buffer = tempbuffer;

					//Server.Log(msg + "");
					switch (msg)
					{
						case 0x01:
							Server.Log("Authentication");
							HandleLogin(message);
							break;
						case 0x02:
							Server.Log("Handshake");
							HandleHandshake(message);
							break;
						case 0x03:
							Server.Log("Chat Message");
							HandleChatMessagePacket(message); //needs to pass data still
							break;
						case 0x0A: if (!MapSent) { MapSent = true; SendMap(); } break; //Player onground Incoming
						case 0x0B: if (!MapSent) { MapSent = true; SendMap(); } break; //Pos incoming
						case 0x0C: if (!MapSent) { MapSent = true; SendMap(); } break; //Look incoming
						case 0x0D: if (!MapSent) { MapSent = true; SendMap(); } break; //Pos and look incoming
					}
					if (buffer.Length > 0)
						buffer = HandleMessage(buffer);
					else
						return new byte[0];
				}
			}
			catch (Exception e)
			{
				Server.Log(e.Message);
			}
			return buffer;
		}

		#region OUTGOING
		void SendRaw(byte id)
		{
			SendRaw(id, new byte[0]);
		}
		public void SendRaw(byte id, byte[] send)
		{
			if (socket == null || !socket.Connected)
				return;
			byte[] buffer = new byte[send.Length + 1];
			buffer[0] = (byte)id;
			for (int i = 0; i < send.Length; i++)
			{
				buffer[i + 1] = send[i];
			}

			try
			{
				socket.Send(buffer);
				buffer = null;
			}
			catch (SocketException)
			{
				buffer = null;
				Disconnect();
			}
		}

		void SendLoginPass()
		{
			try
			{
				long seed = 0;
				short length = (short)Server.name.Length;
				byte[] bytes = new byte[(length * 2) + 15];

				util.EndianBitConverter.Big.GetBytes(Server.protocolversion).CopyTo(bytes, 0);
				util.EndianBitConverter.Big.GetBytes(length).CopyTo(bytes, 4);
				Encoding.BigEndianUnicode.GetBytes(Server.name).CopyTo(bytes, 6);
				util.EndianBitConverter.Big.GetBytes(seed).CopyTo(bytes, bytes.Length - 9);
				bytes[bytes.Length - 1] = dimension;

				SendRaw(1, bytes);
			}
			catch(Exception e)
			{
				Server.Log(e.Message);
				Server.Log(e.StackTrace);
			}
			//SendMap();
		}
		void SendHandshake()
		{
			string st = "-";
			byte[] bytes = new byte[(st.Length * 2) + 2];
			util.EndianBitConverter.Big.GetBytes((ushort)st.Length).CopyTo(bytes, 0);
			Encoding.BigEndianUnicode.GetBytes(st).CopyTo(bytes, 2);
			//foreach (byte b in bytes)
			//{
			//    Server.Log(b + " <");
			//}
			SendRaw(2, bytes);
		}

		void SendInventory()
		{

		}

		void SendMap()
		{
			Server.Log("Sending");
			int i = 0;
			foreach (Chunk c in Server.mainlevel.chunkData.Values)
			{
				SendChunk(c);
				i++;
			}
			//Server.Log(i + " Chunks sent");

			SendSpawnPoint();
			SendLoginDone();
			GlobalSpawn();
		}
		void SendChunk(Chunk c)
		{
			//Send Chunk Init
			byte[] tosend1 = new byte[9];
			util.EndianBitConverter.Big.GetBytes(c.x).CopyTo(tosend1, 0);
			util.EndianBitConverter.Big.GetBytes(c.z).CopyTo(tosend1, 4);
			tosend1[8] = 1;
			SendRaw(0x32, tosend1);

			//Server.Log(c.x + " " + c.z + " " + c.x * 16 + " " + c.z * 16);

			//Send Chunk Data
			byte[] CompressedData = c.GetCompressedData();
			byte[] bytes = new byte[17 + CompressedData.Length];
			util.EndianBitConverter.Big.GetBytes((int)(c.x * 16)).CopyTo(bytes, 0);
			util.EndianBitConverter.Big.GetBytes((int)0).CopyTo(bytes, 4);
			util.EndianBitConverter.Big.GetBytes((int)(c.z * 16)).CopyTo(bytes, 6);
			bytes[10] = 15;
			bytes[11] = 127;
			bytes[12] = 15;
			//util.EndianBitConverter.Big.GetBytes((16 * 128 * 16 * 2.5)).CopyTo(bytes, 13);
			util.EndianBitConverter.Big.GetBytes(CompressedData.Length).CopyTo(bytes, 13);
			CompressedData.CopyTo(bytes, 17);
			SendRaw(0x33, bytes);

			//Server.Log((16 * 128 * 16 * 2.5) + " " + CompressedData.Length);
		}
		void SendSpawnPoint()
		{
			byte[] bytes = new byte[12];
			util.EndianBitConverter.Big.GetBytes((int)level.SpawnX).CopyTo(bytes, 0);
			util.EndianBitConverter.Big.GetBytes((int)level.SpawnY).CopyTo(bytes, 4);
			util.EndianBitConverter.Big.GetBytes((int)level.SpawnZ).CopyTo(bytes, 8);
			SendRaw(0x06, bytes);
		}
		void SendLoginDone()
		{
			Server.Log("Login Done");

			byte[] bytes = new byte[41];
			util.EndianBitConverter.Big.GetBytes(pos[0]).CopyTo(bytes, 0);
			util.EndianBitConverter.Big.GetBytes(Stance).CopyTo(bytes, 8);
			util.EndianBitConverter.Big.GetBytes(pos[1]).CopyTo(bytes, 16);
			util.EndianBitConverter.Big.GetBytes(pos[2]).CopyTo(bytes, 24);
			util.EndianBitConverter.Big.GetBytes(rot[0]).CopyTo(bytes, 32);
			util.EndianBitConverter.Big.GetBytes(rot[1]).CopyTo(bytes, 36);
			bytes[40] = onground;
			SendRaw(0x0D, bytes);

			//Server.Log(pos[0] + " " + pos[1] + " " + pos[2]);
		}

		void SendNamedEntitySpawn(Player p)
		{
			try
			{
				//byte[] bytes2 = new byte[4];
				//util.EndianBitConverter.Big.GetBytes(p.id).CopyTo(bytes2, 0);
				//SendRaw(0x1E, bytes2);

				short length = (short)p.username.Length;
				byte[] bytes = new byte[22 + (length * 2)];

				util.EndianBitConverter.Big.GetBytes(p.id).CopyTo(bytes, 0);
				util.EndianBitConverter.Big.GetBytes(length).CopyTo(bytes, 4);

				Encoding.BigEndianUnicode.GetBytes(p.username).CopyTo(bytes, 6);

				util.EndianBitConverter.Big.GetBytes((int)(p.pos[0] * 32)).CopyTo(bytes, (22 + (length * 2)) - 16);
                util.EndianBitConverter.Big.GetBytes((int)(p.pos[1] * 32)).CopyTo(bytes, (22 + (length * 2)) - 12);
                util.EndianBitConverter.Big.GetBytes((int)(p.pos[2] * 32)).CopyTo(bytes, (22 + (length * 2)) - 8);

				bytes[(22 + (length * 2)) - 4] = 0;
				bytes[(22 + (length * 2)) - 3] = 0;

				util.EndianBitConverter.Big.GetBytes((short)0).CopyTo(bytes, (22 + (length * 2)) - 2);

				SendRaw(0x14, bytes);

				SendEntityEquipment(p.id, -1, -1, -1, -1, -1);
			}
			catch (Exception e)
			{
				Server.Log(e.Message);
				Server.Log(e.StackTrace);
			}
		}
		void SendEntityPosVelocity()
		{

		}
		void SendEntityEquipment(int id, short hand, short a1, short a2, short a3, short a4)
		{
			byte[] bytes = new byte[10];

			util.EndianBitConverter.Big.GetBytes(id).CopyTo(bytes, 0);
			util.EndianBitConverter.Big.GetBytes((short)0).CopyTo(bytes, 4);
			util.EndianBitConverter.Big.GetBytes(hand).CopyTo(bytes, 6);
			util.EndianBitConverter.Big.GetBytes((short)0).CopyTo(bytes, 8);
			SendRaw(0x05, bytes);

			util.EndianBitConverter.Big.GetBytes((short)1).CopyTo(bytes, 4);
			util.EndianBitConverter.Big.GetBytes(a1).CopyTo(bytes, 6);
			util.EndianBitConverter.Big.GetBytes((short)0).CopyTo(bytes, 8);
			SendRaw(0x05, bytes);

			util.EndianBitConverter.Big.GetBytes((short)2).CopyTo(bytes, 4);
			util.EndianBitConverter.Big.GetBytes(a2).CopyTo(bytes, 6);
			util.EndianBitConverter.Big.GetBytes((short)0).CopyTo(bytes, 8);
			SendRaw(0x05, bytes);

			util.EndianBitConverter.Big.GetBytes((short)3).CopyTo(bytes, 4);
			util.EndianBitConverter.Big.GetBytes(a3).CopyTo(bytes, 6);
			util.EndianBitConverter.Big.GetBytes((short)0).CopyTo(bytes, 8);
			SendRaw(0x05, bytes);

			util.EndianBitConverter.Big.GetBytes((short)4).CopyTo(bytes, 4);
			util.EndianBitConverter.Big.GetBytes(a4).CopyTo(bytes, 6);
			util.EndianBitConverter.Big.GetBytes((short)0).CopyTo(bytes, 8);
			SendRaw(0x05, bytes);
		}
		#endregion
		#region INCOMING
		void HandleCommand(string cmd, string message)
		{
		  	Command command = Command.all.Find(cmd);
            if (command == null)
            {
                Server.ServerLogger.Log(LogLevel.Info, this.username + " tried using /" + cmd);
                Server.ServerLogger.Log(LogLevel.Info, "Unrecognised command: " + cmd);
                SendMessage(Color.Purple + "HelpBot V12: Command /" + cmd + " not recognized");
                return;
            }
			
			//TO BE REMOVED WHEN GROUPS ARE ADDED
			List<string> args = new List<string>();
			while (true)
            {
                if (message.IndexOf(' ') != -1)
                {
                    message = message.Substring(message.IndexOf(' ') + 1);
                    if (message.IndexOf(' ') != -1)
                    args.Add(message.Substring(0, message.IndexOf(' ')));
                    else
                    {
                        args.Add(message);
                        break;
                    }
                }
                else if (message.IndexOf(' ') == -1)
                    break;
            }
			
			command.Use(this, args.ToArray());
            Server.ServerLogger.Log(LogLevel.Info, this.username + " used /" + command.Name);
			
			//will uncomment when group system is added for now everybody can use every command ;)
            /*if (Group.CheckPermission(this, command))
            {
            List<string> args = new List<string>();
            while (true)
            {
                if (message.IndexOf(' ') != -1)
                {
                    message = message.Substring(message.IndexOf(' ') + 1);
                    if (message.IndexOf(' ') != -1)
                    args.Add(message.Substring(0, message.IndexOf(' ')));
                    else
                    {
                        args.Add(message);
                        break;
                    }
                }
                else if (message.IndexOf(' ') == -1)
                    break;
            }

            command.Use(this, args.ToArray());
            Server.ServerLogger.Log(LogLevel.Info, this.username + " used /" + command.Name);
            }
            else if (!Group.CheckPermission(this, command))
            {
                Server.ServerLogger.Log(LogLevel.Info, this.username + " tried using /" + cmd + ", but doesn't have appropiate permissions.");
                SendMessage(Color.Purple + "HelpBot V12: You don't have access to command /" + cmd + ".");
            }*/	
		}
		#endregion
		#region MESSAGING
		
		#region GLOBAL
		/// <summary>
		/// Sends a message serverwide, mainly used for chat. 
		/// </summary>
		/// <param name="message">
		/// A <see cref="System.String"/>
		/// </param>
		public static void GlobalMessage(string message)
        {
            GlobalMessage(message, WrapMethod.Default);
        }

		/// <summary>
		/// Sends a message serverwide, mainly used for chat.
		/// </summary>
		/// <param name="message">
		/// A <see cref="System.String"/>
		/// </param>
		/// <param name="method">
		/// A <see cref="WrapMethod"/>
		/// </param>
        public static void GlobalMessage(string message, WrapMethod method)
        {
            string[] lines = WordWrap.GetWrappedText(message, method);
            for (int i = 0; i < lines.Length; i++)
            {				
				//somebody check if this is right please :s
				byte[] bytes = new byte[(lines[i].Length * 2) + 2];
				util.EndianBitConverter.Big.GetBytes((ushort)lines[i].Length).CopyTo(bytes, 0);
				Encoding.BigEndianUnicode.GetBytes(lines[i]).CopyTo(bytes, 2);

                for (int j = 0; j < players.Count; j++)
                {
                    if (!players[j].disconnected)
                    {
                        players[j].SendRaw((byte)KnownPackets.ChatMessage, bytes);
                    }
                }
            }
        }

		/// <summary>
		///Sends a message serverwide, mainly used for chat. 
		/// </summary>
		/// <param name="message">
		/// A <see cref="System.String"/>
		/// </param>
		/// <param name="method">
		/// A <see cref="WrapMethod"/>
		/// </param>
		/// <param name="args">
		/// A <see cref="System.Object[]"/>
		/// </param>
        public static void GlobalMessage(string message, WrapMethod method, params object[] args)
        {
            if (method == WrapMethod.None)
                GlobalMessage(string.Format(message, args));
            else
                GlobalMessage(string.Format(message, args), method);
        }
		#endregion
		
		#region TARGETED
		protected virtual void SendMessageInternal(string message)
        {
            //once again please check			
			byte[] bytes = new byte[(message.Length * 2) + 2];
			util.EndianBitConverter.Big.GetBytes((ushort)message.Length).CopyTo(bytes, 0);
			Encoding.BigEndianUnicode.GetBytes(message).CopyTo(bytes, 2);
			this.SendRaw((byte)KnownPackets.ChatMessage, bytes);
			
        }
        public void SendMessage(string message)
        {
            SendMessage(message, WrapMethod.Default);
        }
        public void SendMessage(string message, WrapMethod method)
        {
            string[] lines = WordWrap.GetWrappedText(message, method);
            for (int i = 0; i < lines.Length; i++)
            {
                SendMessageInternal(lines[i]);
            }
        }
        public void SendMessage(string message, WrapMethod method, params object[] args)
        {
            if (method == WrapMethod.None)
                SendMessageInternal(string.Format(message, args));
            else
                SendMessage(string.Format(message, args), method);
        }
		#endregion
		
		#endregion

		void GlobalSpawn()
		{
			if (players.Count <= 1) return;
			foreach (Player p in players)
			{
				SendNamedEntitySpawn(p);
				p.SendNamedEntitySpawn(this);
			}
		}
		public static void GlobalUpdate()
		{
			players.ForEach(delegate(Player p)
			{
				p.SendRaw(0);
				if (!p.LoggedIn) return;
				p.SendRaw(0);
				p.SendTick();
				if (!p.hidden)
				{
					p.UpdatePosition();
				}
			});
		}
		void UpdatePosition()
		{

		}
		void SendTick()
		{

		}

		/// <summary>
		/// Kicks a player with a reason 
		/// </summary>
		/// <param name="reason">
		/// A <see cref="System.String"/>
		/// </param>
		public void Kick(string message)
		{
			if (disconnected) return;
			
			disconnected = true;
			
			if (message != null)
			{
			//	Server.ServerLogger.Log(LogLevel.Notice, "{0}{1} kicked: {2}",
            //    	LoggedIn ? "" : "/", LoggedIn ? username : ip, message);
			}
			else
			{
			//	Server.ServerLogger.Log(LogLevel.Notice, "{0}{1} kicked: {2}",
            //    	LoggedIn ? "" : "/", LoggedIn ? username : ip, Server.KickMessage);				
			}
            if (LoggedIn)
                GlobalMessage("§5{0} §fhas been kicked from the server!", WrapMethod.None, username);
            LoggedIn = false;
			
			try
			{
				//hopefully it is right
				byte[] bytes = new byte[(message.Length * 2) + 2];
				util.EndianBitConverter.Big.GetBytes((ushort)message.Length).CopyTo(bytes, 0);
				Encoding.BigEndianUnicode.GetBytes(message).CopyTo(bytes, 2);
				this.SendRaw((byte)KnownPackets.Disconnect, bytes);
			}
			catch{}
			
			//TODO: Despawn
			this.Dispose();
		}
		public void Disconnect()
		{
			if (disconnected) return;
			
			disconnected = true;
			
			
			//Server.ServerLogger.Log(LogLevel.Notice, "{0}{1} kicked: {2}",
            //	LoggedIn ? "" : "/", LoggedIn ? username : ip);
            if (LoggedIn)
                GlobalMessage("§5{0} §fhas been kicked from the server!", WrapMethod.None, username);
            LoggedIn = false;
			
			//TODO: Despawn
			this.Dispose();
		}
		public void Dispose()
		{

		}
		
		#region TOOLS
		// <summary>
        /// Finds a player by string or partial string
        /// </summary>
        /// <param name="name">username to search for</param>
        /// <returns>Player</returns>
        public static Player FindPlayer(string name)
        {
            List<Player> tempList = new List<Player>();
            tempList.AddRange(players);
            Player tempPlayer = null; bool returnNull = false;

            foreach (Player p in tempList)
            {
                if (p.username.ToLower() == name.ToLower()) return p;
                if (p.username.ToLower().IndexOf(name.ToLower()) != -1)
                {
                    if (tempPlayer == null) tempPlayer = p;
                    else returnNull = true;
                }
            }

            if (returnNull == true) return null;
            if (tempPlayer != null) return tempPlayer;
            return null;
        }
		#endregion

	}
}
