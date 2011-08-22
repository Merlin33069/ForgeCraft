using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;
using System.Threading;

namespace RemoteConsole
{
	class Program
	{
		static bool shutdown = false;
		static byte loginstatus = 0;
		static string Hostname = "";
		static int port = 25565;
		static string Username = "";
		static string Password = "";
		static Socket s;
		static TcpClient tcpclient;
		static Stream stream;
		static ASCIIEncoding asen;
		static Thread SendThread;
		
		static void Main(string[] args)
		{
			Console.WriteLine("Enter the IP/Hostname and Port of the server to connect to, for example:");
			Console.WriteLine("71.75.125.321:25565");
			Console.WriteLine("www.Example.com:25565");

			while (!shutdown)
			{
				string read = Console.ReadLine();
				HandleCommand(read);
			}
		}

		static void HandleCommand(string a)
		{
			switch (loginstatus)
			{
				case(0):
					if (string.IsNullOrEmpty(a.Trim()))
					{
						Console.WriteLine("IP/Hostname cannot be empty!");
						return;
					}
					else
					{
						if (a.Contains(":"))
						{
							string[] hostname = a.Split(':');

							Hostname = hostname[0];
							port = Convert.ToInt32(hostname[1]);
							loginstatus = 1;
							Console.WriteLine("Enter your Username.");
							return;
						}
						else
						{
							Hostname = a;
							loginstatus = 1;
							Console.WriteLine("Enter your Username.");
							return;
						}
					}
				case(1):
					if (string.IsNullOrEmpty(a.Trim()))
					{
						Console.WriteLine("Username cannot be empty!");
						return;
					}
					else
					{
						Username = a;
						loginstatus = 2;
						Console.WriteLine("Enter your Password.");
						return;
					}
				case(2):
					if (string.IsNullOrEmpty(a.Trim()))
					{
						Console.WriteLine("Password cannot be empty!");
						return;
					}
					else
					{
						Password = a;
						Console.WriteLine(a);
						loginstatus = 3;
						if (Connect())
						{
							loginstatus = 4;
						}
						else
						{
							Console.Clear();
							Console.WriteLine("Error connecting to " + Hostname + ":" + port);
							Console.WriteLine("Resetting Connections...");
							Console.WriteLine("");
							loginstatus = 0;
							Hostname = "";
							Username = "";
							Password = "";
							Console.WriteLine("Enter the IP/Hostname and Port of the server to connect to, for example:");
							Console.WriteLine("71.75.125.321:25565");
							Console.WriteLine("www.Example.com:25565");
						}
						return;
					}
				case(3):
					Console.WriteLine("Not connected, input ignored.");
					break;
				case(4):
					Console.WriteLine("TODO - Handle real commands.");
					//Do Stuff
					break;
			}
			return;
		}
		static bool Connect()
		{
			Console.WriteLine("Connecting to " + Hostname + ":" + port);
			try
			{
				tcpclient = new TcpClient();
				tcpclient.Connect(Hostname, port);

				stream = tcpclient.GetStream();
				asen = new ASCIIEncoding();

				SendThread = new Thread(new ThreadStart(receive));
				SendThread.Start();

				if (!string.IsNullOrEmpty(Password))
				{
					SendRaw(0xFE);
				}
				else
				{
					Console.WriteLine("Password is empty...");
					Console.WriteLine(Password);
					Thread.Sleep(10000);
					return false;
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
				Console.WriteLine(e.StackTrace);
				Console.WriteLine("Server Does not appear to be Online. (F6 to try again)");
				Thread.Sleep(1000);
				return false;
			}
			Console.WriteLine("Connected.");

			return true;
		}

		static void receive()
		{
			NetworkStream clientStream = tcpclient.GetStream();

			byte[] message = new byte[4096];
			int bytesRead;

			while (!shutdown)
			{
				try
				{
					bytesRead = 0;

					try { bytesRead = clientStream.Read(message, 0, 4096); }
					catch { break; }

					if (bytesRead == 0) { break; }

					clientStream.Flush();


					byte[] buffer = new byte[bytesRead];
					for (int i = 0; i < bytesRead; i++)
					{
						buffer[i] = message[i];
					}

					HandleMessage(buffer);
				}
				catch
				{
					shutdown = true;
				}
			}
			Close();
		}
		static byte[] HandleMessage(byte[] buffer)
		{
			try
			{
				int length = 0; byte msg = buffer[0];
				// Get the length of the message by checking the first byte
				switch (msg)
				{
					case 0xFE: length = 0; break; //Ready for authentication
					
					default:
						Console.WriteLine("unhandled message id " + msg);
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
						case 0xFE: SendAuthentication(); break;
					}
					if (buffer.Length > 0)
						buffer = HandleMessage(buffer);
					else
						return new byte[0];
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
				Console.WriteLine(e.StackTrace);
			}
			return buffer;
		}

		static void SendRaw(int id) { SendRaw(id, new byte[0]); }
		static void SendRaw(int id, byte[] send)
		{
			try
			{
				byte[] buffer = new byte[send.Length + 1];
				buffer[0] = (byte)id;
				Buffer.BlockCopy(send, 0, buffer, 1, send.Length);
				int tries = 0;
			retry: try
				{
					if (!shutdown)
					{
						try
						{
							stream.Write(buffer, 0, buffer.Length);
						}
						catch
						{
							shutdown = true;
							tcpclient.Close();
							Console.WriteLine("networking stopped.");
						}
					}
				}
				catch (SocketException)
				{
					tries++;
					if (tries > 2)
						Close();
					else goto retry;
				}
				catch
				{
					//Console.WriteLine(e.Message);
					tries++;
					if (tries > 2)
						Close();
					else goto retry;
				}
			}
			catch
			{

			}
		}

		static void SendAuthentication()
		{
			int count = Username.Length + Password.Length + 4;
			byte[] bytes = new byte[count];

			BitConverter.GetBytes((short)Username.Length).CopyTo(bytes, 0);
			asen.GetBytes(Username).CopyTo(bytes, 2);
			BitConverter.GetBytes((short)Password.Length).CopyTo(bytes, Username.Length + 2);
			asen.GetBytes(Password).CopyTo(bytes, Username.Length + 4);

			SendRaw(0x01, bytes);
		}

		static void Close()
		{
			shutdown = true;
		}
	}
}
