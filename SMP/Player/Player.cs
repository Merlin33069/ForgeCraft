using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

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

		public string ip;
		int id;
		public string username;
		byte dimension; //-1 for nether, 0 normal, 1 skyworld?

		public Player(Socket s)
		{
			id = random.Next();
			socket = s;
			ip = socket.RemoteEndPoint.ToString().Split(':')[0];
			Server.Log(ip + " connected to the server.");
			level = Server.mainlevel;
			dimension = 0;
			socket.BeginReceive(tempbuffer, 0, tempbuffer.Length, SocketFlags.None, new AsyncCallback(Receive), this);
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
		
		/// <summary>
		/// Handles Incoming Packets 
		/// </summary>
		/// <param name="buffer">
		/// A <see cref="System.Byte[]"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.Byte[]"/>
		/// </returns>
		byte[] HandleMessage(byte[] buffer)
		{
			try
			{
				int length = 0; byte msg = buffer[0];
				// Get the length of the message by checking the first byte
				switch (msg)
				{
					case 0x00: length = 0; break; //Keep alive
					case 0x01: Server.Log("auth start"); length = ((util.EndianBitConverter.Big.ToInt16(buffer, 5) * 2) + 15); break; //Login Request
					case 0x02: length = ((util.EndianBitConverter.Big.ToInt16(buffer, 1) * 2) + 2); break; //Handshake
					case 0x03: length = ((util.EndianBitConverter.Big.ToInt16(buffer, 1) * 2) + 2); break; //Chat
					//case 0x04: length = 0; break;
					//case 0x05: length = 0; break;
					//case 0x06: length = 0; break;
					//case 0x07: length = 0; break;
					//case 0x08: length = 0; break;
					case 0x0b: length = 33; break; //Pos incoming
					case 0x0d: length = 41; break; //Pos incoming
					default:
						Server.Log("unhandled message id " + msg);
						return new byte[0];
				}
				if (buffer.Length > length)
				{
					byte[] message = new byte[length];
					Buffer.BlockCopy(buffer, 1, message, 0, length);

					byte[] tempbuffer = new byte[buffer.Length - length - 1];
					Buffer.BlockCopy(buffer, length + 1, tempbuffer, 0, buffer.Length - length - 1);

					buffer = tempbuffer;

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
							HandleChatMessagePacket(); //needs to pass data still
							break;
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
		}
		void SendHandshake()
		{
			string st = "+";
			byte[] bytes = new byte[(st.Length * 2) + 2];
			util.EndianBitConverter.Big.GetBytes((ushort)st.Length).CopyTo(bytes, 0);
			Encoding.BigEndianUnicode.GetBytes(st).CopyTo(bytes, 2);
			foreach (byte b in bytes)
			{
			    Server.Log(b + " <");
			}
			SendRaw(2, bytes);
		}
		#endregion
		#region INCOMING
		
		
		void HandleCommand(string cmd, string message)
		{
		  	//TODO	
		}
		#endregion
		
		/// <summary>
		/// Kicks a player with a reason 
		/// </summary>
		/// <param name="reason">
		/// A <see cref="System.String"/>
		/// </param>
		public void Kick(string reason)
		{
			
		}

		public void Disconnect()
		{
			
		}
		public void Dispose()
		{

		}

	}
}
