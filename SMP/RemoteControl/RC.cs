using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;

namespace SMP
{
	class RC
	{
		Socket s;
		bool disconnected = false;
		string ip;
		byte[] buffer = new byte[0];
		byte[] tempbuffer = new byte[0xFF];
		static ASCIIEncoding asen;
		string username;
		string password;

		public RC(Socket a)
		{
			s = a;
			try
			{
				ip = a.RemoteEndPoint.ToString().Split(':')[0];
				Server.Log(ip + " REMOTE CONSOLE connected to the server.");
				asen = new ASCIIEncoding();

				a.BeginReceive(tempbuffer, 0, tempbuffer.Length, SocketFlags.None, new AsyncCallback(Receive), this);
				SendRaw(0xFE);
			}
			catch (Exception e)
			{
				Server.Log(e.Message);
				Server.Log(e.StackTrace);
			}
		}

		static void Receive(IAsyncResult result)
		{
			RC rc = (RC)result.AsyncState;
			if (rc.disconnected || rc.s == null)
				return;
			try
			{
				int length = rc.s.EndReceive(result);
				if (length == 0) { rc.Disconnect(); return; }

				byte[] b = new byte[rc.buffer.Length + length];
				Buffer.BlockCopy(rc.buffer, 0, b, 0, rc.buffer.Length);
				Buffer.BlockCopy(rc.tempbuffer, 0, b, rc.buffer.Length, length);

				rc.buffer = rc.HandleMessage(b);
				rc.s.BeginReceive(rc.tempbuffer, 0, rc.tempbuffer.Length, SocketFlags.None,
									  new AsyncCallback(Receive), rc);
			}
			catch (SocketException e)
			{
				rc.Disconnect();
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
				switch (msg)
				{
					case 0x00: length = 0; break; //Keep alive
					case 0x01: length = BitConverter.ToInt16(buffer, 1) + 
						BitConverter.ToInt16(buffer, BitConverter.ToInt16(buffer, 1) + 3) + 4; break; //Username/Password
					
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
						case 0x01: HandleLogin(message); break;
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

		void SendRaw(byte id)
		{
			SendRaw(id, new byte[0]);
		}
		public void SendRaw(byte id, byte[] send)
		{
			if (s == null || !s.Connected)
				return;
			byte[] buffer = new byte[send.Length + 1];
			buffer[0] = (byte)id;
			send.CopyTo(buffer, 1);

			try
			{
				s.Send(buffer);
				buffer = null;
			}
			catch (SocketException)
			{
				buffer = null;
				Disconnect();
			}
		}

		void HandleLogin(byte[] a)
		{
			short count1 = BitConverter.ToInt16(a, 0);
			short count2 = BitConverter.ToInt16(a, 2 + count1);

			username = asen.GetString(a, 2, count1);
			password = asen.GetString(a, 4 + count1, count2);

			Console.WriteLine(username + " " + password);
		}

		void Kick(string a)
		{

		}
		void Disconnect()
		{

		}
	}
}
