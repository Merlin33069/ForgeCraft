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
		public Socket socket;
		public World level;
		static Random random = new Random();
		public short current_slot_holding;
		public Item current_block_holding { get { return inventory.current_item; } set { inventory.current_item = value; SendHealth(); } }
		byte[] buffer = new byte[0];
		byte[] tempbuffer = new byte[0xFF];
		bool disconnected = false;
        public bool LoggedIn { get; protected set; }
		public short health { get { return e.meta; } set { e.meta = value; } }
		double Stance { get { return e.Stance; } set { e.Stance = value; } }
		public double[] pos { get { return e.pos; } set { e.pos = value; } }
		double[] oldpos { get { return e.oldpos; } set { e.oldpos = value; } }
		float[] rot { get { return e.rot; } set { e.rot = value; } }
		byte onground { get { return e.OnGround; } set { e.OnGround = value; } } //really a bool, but were going to hold it as a byte (1 or 0 ONLY) so we can send it easier
		public int id { get { return e.id; } }
		byte dimension { get { return e.dimension; } set { e.dimension = value; } } //-1 for nether, 0 normal, 1 skyworld?
		public Chunk chunk { get { return e.CurrentChunk; } }
		public Inventory inventory;
		public List<Point> VisibleChunks = new List<Point>();
		public List<int> VisibleEntities = new List<int>();
		#region Custom Command / Plugin Event
		//Events for Custom Command and Plugins ------------------------------------
		public delegate void OnPlayerConnect(Player p);
		public delegate void OnPlayerAuth(Player p);
		public event OnPlayerConnect PlayerConnect;
		public event OnPlayerAuth PlayerAuth;
		public delegate void OnPlayerChat(string message, Player p);
		public delegate void OnPlayerCommand(string cmd, string message, Player p);
		//Events for Custom Command and Plugins -------------------------------------
		#endregion
		//Groups and Permissions
		public Group Group;
		public List<string> AdditionalPermissions;
		
		//Other Player settings Donotdisturb, god mode etc.
		public bool DoNotDisturb = false; //blocks all incoming chat except pm's
		public bool GodMode = false; //obvious, but not used anywhere yet
		
		Entity e;
		public string ip;
		public string username;
		bool hidden = false;

		bool MapSent = false;
		/// <summary>
		/// Initializes a new instance of the <see cref="SMP.Player"/> class.
		/// </summary>
		public Player()
		{
			
		}
		/// <summary>
		/// Start this instance.
		/// </summary>
		public void Start()
		{
			try
			{
				level = Server.mainlevel;
				e = new Entity(new double[3] { 0, 18, 0 }, new float[2] { 0, 0 }, this);
				pos[1] = 128;
				Stance = 128;
				//socket = s;
				ip = socket.RemoteEndPoint.ToString().Split(':')[0];
				Server.Log(ip + " connected to the server.");
				dimension = 0;
				inventory = new Inventory();
				players.Add(this);
				//Event --------------------
				if (PlayerConnect != null)
					PlayerConnect(this);
				//Event --------------------
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
				Server.Log(e.StackTrace);
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
					case 0xFF: length = ((util.EndianBitConverter.Big.ToInt16(buffer, 1) * 2) + 2); break; //DC

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

					//if(username!= "Merlin33069") Server.Log(msg + "");
					switch (msg)
					{
						case 0x01:
							//Server.Log("Authentication");
							HandleLogin(message);
							break;
						case 0x02:
							//Server.Log("Handshake");
							HandleHandshake(message);
							break;
						case 0x03:
							//Server.Log("Chat Message");
							HandleChatMessagePacket(message);
							break;
						case 0x0A: if (!MapSent) { MapSent = true; SendMap(); } HandlePlayerPacket(message); break; //Player onground Incoming
						case 0x0B: if (!MapSent) { MapSent = true; SendMap(); } HandlePlayerPositionPacket(message); break; //Pos incoming
						case 0x0C: if (!MapSent) { MapSent = true; SendMap(); } HandlePlayerLookPacket(message); break; //Look incoming
						case 0x0D: if (!MapSent) { MapSent = true; SendMap(); } HandlePlayerPositionAndLookPacket(message); break; //Pos and look incoming
						case 0x0E: HandleDigging(message); break; //Digging
						case 0xFF: HandleDC(message); break; //DC
						case 0x10: HandleHoldingChange(message); break; //Holding Change
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
				Server.Log(e.StackTrace);
			}
			return buffer;
		}

		#region OUTGOING
		void SendRaw(byte id)
		{
			SendRaw(id, new byte[0]);
		}
		/// <summary>
		/// Send Data over to the client
		/// </summary>
		/// <param name='id'>
		/// Identifier. The packet ID that you want to send
		/// </param>
		/// <param name='send'>
		/// Send. The byte[] information you want to send
		/// </param>
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
		/// <summary>
		/// Update the players time
		/// </summary>
		public void SendTime()
		{
			byte[] tosend = new byte[9];
			util.EndianBitConverter.Big.GetBytes(level.time).CopyTo(tosend, 0);
			SendRaw(0x04, tosend);
		}
		/// <summary>
		/// Update the players health
		/// </summary>
		public void SendHealth()
		{
			byte[] tosend = new byte[3];
			util.EndianBitConverter.Big.GetBytes(e.meta).CopyTo(tosend, 0);
			SendRaw(0x08, tosend);
		}
		public void Teleport_Player(double x, double y, double z)
		{
			Teleport_Player(x, y, z, rot[0], rot[1]);
		}
		public void Teleport_Player(double x, double y, double z, float yaw, float pitch)
		{
			byte[] tosend = new byte[41];
			util.EndianBitConverter.Big.GetBytes(x).CopyTo(tosend, 0);
			util.EndianBitConverter.Big.GetBytes(y + 1.65).CopyTo(tosend, 8);
			util.EndianBitConverter.Big.GetBytes(y).CopyTo(tosend, 16);
			util.EndianBitConverter.Big.GetBytes(z).CopyTo(tosend, 24);
			util.EndianBitConverter.Big.GetBytes(yaw).CopyTo(tosend, 32);
			util.EndianBitConverter.Big.GetBytes(pitch).CopyTo(tosend, 36);
			tosend[40] = onground;
			SendRaw(0x0D, tosend);
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
			//Server.Log("Handshake out");
			string st = "-";
			byte[] bytes = new byte[(st.Length * 2) + 2];
			util.EndianBitConverter.Big.GetBytes((ushort)st.Length).CopyTo(bytes, 0);
			Encoding.BigEndianUnicode.GetBytes(st).CopyTo(bytes, 2);
			//foreach (byte b in bytes)
			//{
			//    Server.Log(b + " <");
			//}
			//Server.Log("Handshake out-1");
			SendRaw(2, bytes);
			//Server.Log("Handshake out-2");
		}

		void SendInventory()
		{

		}

		void SendMap()
		{
			//Server.Log("Sending");
			//int i = 0;
			//foreach (Chunk c in Server.mainlevel.chunkData.Values.ToArray())
			//{
			//	SendChunk(c);
			//	i++;
			//}
			//Server.Log(i + " Chunks sent");

			e.UpdateChunks(true, false);
			SendSpawnPoint();
			SendLoginDone();
			//GlobalSpawn();
		}
		/// <summary>
		/// Updates players chunks.
		/// </summary>
		/// <param name='force'>
		/// Force. Force it to update the current chunk
		/// </param>
		/// <param name='forcesend'>
		/// Forcesend. For it to send all the chunk, even if the player already see's it (Good for map switching)
		/// </param>
		public void UpdateChunks(bool force, bool forcesend)
		{
			e.UpdateChunks(force, forcesend);
		}
		/// <summary>
		/// Prepare the client before sending the chunk
		/// </summary>
		/// <param name='c'>
		/// C. The chunk to send
		/// </param>
		/// <param name='load'>
		/// Load. Weather to unload or load the chunk (0 is unload otherwise it will load)
		/// </param>
		public void SendPreChunk(Chunk c, byte load)
		{
			byte[] bytes = new byte[9];
			util.EndianBitConverter.Big.GetBytes(c.x).CopyTo(bytes, 0);
			util.EndianBitConverter.Big.GetBytes(c.z).CopyTo(bytes, 4);
			bytes[8] = load;
			SendRaw(0x32, bytes);
		}
		public void SendItem(short slot, short Item){ SendItem(slot, Item, 1, 3); }
		public void SendItem(short slot, short Item, byte count, short use)
		{
			byte[] tosend;
			if (Item == -1)
				tosend = new byte[5];
			else
				tosend = new byte[8];
			tosend[0] = 0;
			util.EndianBitConverter.Big.GetBytes(slot).CopyTo(tosend, 1);
			util.EndianBitConverter.Big.GetBytes(Item).CopyTo(tosend, 3);
			tosend[5] = count;
			if (Item != -1)
				util.EndianBitConverter.Big.GetBytes(use).CopyTo(tosend, 6);
			SendRaw(0x67, tosend);
		}
		/// <summary>
		/// Sends a player a Chunk
		/// </summary>
		/// <param name='c'>
		/// C. The chunk to send
		/// </param>
		public void SendChunk(Chunk c)
		{
			SendPreChunk(c, 1);

			//Send Chunk Data
			byte[] CompressedData = c.GetCompressedData();
			byte[] bytes = new byte[17 + CompressedData.Length];
			util.EndianBitConverter.Big.GetBytes((int)(c.x * 16)).CopyTo(bytes, 0);
			util.EndianBitConverter.Big.GetBytes((int)0).CopyTo(bytes, 4);
			util.EndianBitConverter.Big.GetBytes((int)(c.z * 16)).CopyTo(bytes, 6);
			bytes[10] = 15;
			bytes[11] = 127;
			bytes[12] = 15;
			util.EndianBitConverter.Big.GetBytes(CompressedData.Length).CopyTo(bytes, 13);
			CompressedData.CopyTo(bytes, 17);
			SendRaw(0x33, bytes);

			VisibleChunks.Add(c.point);
		}
		/// <summary>
		/// Send the player the spawn point (Only usable after login)
		/// </summary>
		public void SendSpawnPoint()
		{
			byte[] bytes = new byte[12];
			util.EndianBitConverter.Big.GetBytes((int)level.SpawnX).CopyTo(bytes, 0);
			util.EndianBitConverter.Big.GetBytes((int)level.SpawnY).CopyTo(bytes, 4);
			util.EndianBitConverter.Big.GetBytes((int)level.SpawnZ).CopyTo(bytes, 8);
			SendRaw(0x06, bytes);
		}
		void SendLoginDone()
		{
			//Server.Log("Login Done");

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
		/// <summary>
		/// Sends a player a blockchange
		/// </summary>
		/// <param name='x'>
		/// X. The x cords of the block
		/// </param>
		/// <param name='y'>
		/// Y. The y cords of the block
		/// </param>
		/// <param name='z'>
		/// Z. The z cords of the block
		/// </param>
		/// <param name='type'>
		/// Type. The ID of the block
		/// </param>
		/// <param name='meta'>
		/// Meta. The meta data of the block
		/// </param>
		public void SendBlockChange(int x, byte y, int z, byte type, byte meta)
		{
			byte[] bytes = new byte[11];
			util.EndianBitConverter.Big.GetBytes(x).CopyTo(bytes, 0);
			bytes[4] = y;
			util.EndianBitConverter.Big.GetBytes(z).CopyTo(bytes, 5);
			bytes[9] = type;
			bytes[10] = meta;
			SendRaw(0x35, bytes);
		}

		public void SendNamedEntitySpawn(Player p)
		{
			try
			{
				if (p == null)
				{
					if(VisibleEntities.Contains(p.id)) VisibleEntities.Remove(p.id);
					return;
				}
				if (!LoggedIn)
				{
					if(VisibleEntities.Contains(p.id)) VisibleEntities.Remove(p.id);
					return;
				}
				if (!p.LoggedIn)
				{
					if(VisibleEntities.Contains(p.id)) VisibleEntities.Remove(p.id);
					return;
				}
				
				short length = (short)p.username.Length;
				byte[] bytes = new byte[22 + (length * 2)];

				util.EndianBitConverter.Big.GetBytes(p.id).CopyTo(bytes, 0);
				util.EndianBitConverter.Big.GetBytes(length).CopyTo(bytes, 4);

				Encoding.BigEndianUnicode.GetBytes(p.username).CopyTo(bytes, 6);

				util.EndianBitConverter.Big.GetBytes((int)(p.pos[0] * 32)).CopyTo(bytes, (22 + (length * 2)) - 16);
<<<<<<< HEAD
				util.EndianBitConverter.Big.GetBytes((int)(p.pos[1] * 32)).CopyTo(bytes, (22 + (length * 2)) - 12);
				util.EndianBitConverter.Big.GetBytes((int)(p.pos[2] * 32)).CopyTo(bytes, (22 + (length * 2)) - 8);
=======
                util.EndianBitConverter.Big.GetBytes((int)(p.pos[1] * 32)).CopyTo(bytes, (22 + (length * 2)) - 12);
                util.EndianBitConverter.Big.GetBytes((int)(p.pos[2] * 32)).CopyTo(bytes, (22 + (length * 2)) - 8);
>>>>>>> 973809de4a28148fbbbba3b468142cb71811e9ef

				bytes[(22 + (length * 2)) - 4] = (byte)(rot[0] / 1.40625);
				bytes[(22 + (length * 2)) - 3] = (byte)(rot[1] / 1.40625);

				util.EndianBitConverter.Big.GetBytes((short)0).CopyTo(bytes, (22 + (length * 2)) - 2);

				SendRaw(0x14, bytes);

				//SendEntityEquipment(p.id, -1, -1, -1, -1, -1);
			}
			catch (Exception e)
			{
				Server.Log(e.Message);
				Server.Log(e.StackTrace);
			}
		}
		public void SendPickupSpawn(Entity e)
		{
			if (!LoggedIn) return;

			byte[] bytes = new byte[24];
			util.EndianBitConverter.Big.GetBytes(e.id).CopyTo(bytes, 0);
			util.EndianBitConverter.Big.GetBytes((short)e.itype).CopyTo(bytes, 4);
			bytes[6] = e.count;
			util.EndianBitConverter.Big.GetBytes(e.meta).CopyTo(bytes, 7);
			util.EndianBitConverter.Big.GetBytes((int)e.pos[0]).CopyTo(bytes, 9);
			util.EndianBitConverter.Big.GetBytes((int)e.pos[1]).CopyTo(bytes, 13);
			util.EndianBitConverter.Big.GetBytes((int)e.pos[2]).CopyTo(bytes, 17);
			bytes[21] = e.irot[0];
			bytes[22] = e.irot[1];
			bytes[23] = e.irot[2];
			SendRaw(0x15, bytes);
		}

		public void SendEntityPosVelocity()
		{
			if (!LoggedIn) return;
		}
		public void SendEntityEquipment(int id, short hand, short a1, short a2, short a3, short a4)
		{
			if (!LoggedIn) return;

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
		public void SendDespawn(int id) //Despawn ALL types of Entities (player mod item)
		{
			//if (!LoggedIn) return;
			byte[] bytes = new byte[4];
			util.EndianBitConverter.Big.GetBytes(id).CopyTo(bytes, 0);
			SendRaw(0x1D, bytes);
		}

		public static void GlobalUpdate()
		{
			players.ForEach(delegate(Player p)
			{
				p.SendRaw(0);
				if (!p.LoggedIn) return;
				p.SendRaw(0);
				p.SendTime();
				if (!p.hidden)
				{
					p.UpdatePosition();
				}
			});
		}
		void UpdatePosition()
		{
			e.UpdateEntities();
			if (!LoggedIn) return;

			int diffX = (int)(oldpos[0] * 32) - (int)(pos[0] * 32);
			int diffY = (int)(oldpos[1] * 32) - (int)(pos[1] * 32);
			int diffZ = (int)(oldpos[2] * 32) - (int)(pos[2] * 32);

			if (Math.Abs(diffX) == 0 && Math.Abs(diffY) == 0 && Math.Abs(diffZ) == 0)
			{
				byte[] bytes = new byte[6];
				util.EndianBitConverter.Big.GetBytes(id).CopyTo(bytes, 0);
				bytes[4] = (byte)(rot[0] / 1.40625);
				bytes[5] = (byte)(rot[1] / 1.40625);
				foreach (Player p in players.ToArray())
				{
					if (p != this && p.level == level)
					{
						if (p.LoggedIn)
							p.SendRaw(0x20, bytes);
					}
				}
			}
			else if (Math.Abs(diffX) <= 4 && Math.Abs(diffY) <= 4 && Math.Abs(diffZ) <= 4)
			{
				byte[] bytes = new byte[9];
				util.EndianBitConverter.Big.GetBytes(id).CopyTo(bytes, 0);
				bytes[4] = (byte)diffX;
				bytes[5] = (byte)diffY;
				bytes[6] = (byte)diffZ;
				bytes[7] = (byte)(rot[0] / 1.40625);
				bytes[8] = (byte)(rot[1] / 1.40625);
				foreach (Player p in players.ToArray())
				{
					if (p != this && p.level == level)
					{
						if(VisibleEntities.Contains(p.id))
							if (p.LoggedIn)
								p.SendRaw(0x21, bytes);
					}
				}
			}
			else
			{
				byte[] bytes = new byte[0x22];
				util.EndianBitConverter.Big.GetBytes(id).CopyTo(bytes, 0);
				util.EndianBitConverter.Big.GetBytes((int)(pos[0] * 32)).CopyTo(bytes, 4);
				util.EndianBitConverter.Big.GetBytes((int)(pos[1] * 32)).CopyTo(bytes, 8);
				util.EndianBitConverter.Big.GetBytes((int)(pos[2] * 32)).CopyTo(bytes, 12);
				bytes[16] = (byte)(rot[0] / 1.40625);
				bytes[17] = (byte)(rot[1] / 1.40625);
				foreach (Player p in players.ToArray())
				{
					if (p != this && p.level == level)
					{
						if (p.LoggedIn)
							p.SendRaw(0x22, bytes);
					}
				}
			}
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
            /*if (Group.CheckPermission(this, command.PermissionNode))
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
            else if (!Group.CheckPermission(this, command.PermissionNode))
            {
                Server.ServerLogger.Log(LogLevel.Info, this.username + " tried using /" + cmd + ", but doesn't have appropiate permissions.");
                SendMessage(Color.Purple + "HelpBot V12: You don't have access to command /" + cmd + ".");
            }*/
		}
		#endregion
		#region Messaging
		#region GLOBAL
		public static void GlobalMessage(string message)
        {
            GlobalMessage(message, WrapMethod.Default);
        }
        public static void GlobalMessage(string message, WrapMethod method)
        {
            string[] lines = WordWrap.GetWrappedText(message, method);
            for (int i = 0; i < lines.Length; i++)
            {				
				//somebody check if this is right please :s
				//LooksGood to me ~Merlin33069
				byte[] bytes = new byte[(lines[i].Length * 2) + 2];
				util.EndianBitConverter.Big.GetBytes((ushort)lines[i].Length).CopyTo(bytes, 0);
				Encoding.BigEndianUnicode.GetBytes(lines[i]).CopyTo(bytes, 2);

                for (int j = 0; j < players.Count; j++)
                {
                    if (!players[j].disconnected)
                    {
						if (!players[j].DoNotDisturb)
						{
                        	players[j].SendRaw((byte)KnownPackets.ChatMessage, bytes);
						}
                    }
                }
            }
        }
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
                GlobalMessage("§5{0} §fhas disconnected.", WrapMethod.None, username);
            LoggedIn = false;
			
			//TODO: Despawn
			this.Dispose();
		}
		public void Dispose()
		{
			players.Remove(this);
			e.CurrentChunk.Entities.Remove(e);

			foreach (Player p in players)
			{
			    p.SendDespawn(id);
			}
		}
		
		#region TOOLS
		/// <summary>
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
