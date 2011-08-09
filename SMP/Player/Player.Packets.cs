using System;
using System.Text;

namespace SMP
{

    /// <summary>
    /// Handles packets.
    /// </summary>
	public partial class Player : System.IDisposable
	{
		private void HandleLogin(byte[] message)
		{
			int version = util.EndianBitConverter.Big.ToInt32(message, 0);
			short length = util.EndianBitConverter.Big.ToInt16(message, 4);
			if (length > 32) { Kick("Username too long"); return; }
			username = Encoding.BigEndianUnicode.GetString(message, 6, (length * 2));
			Server.Log(ip + " Logged in as " + username);

			LoggedIn = true;
			SendLoginPass();
		}
		private void HandleHandshake(byte[] message)
		{
			Server.Log("handshake-2");
			//short length = util.EndianBitConverter.Big.ToInt16(message, 0);
			//Server.Log(length + "");
			//Server.Log(Encoding.BigEndianUnicode.GetString(message, 2, length * 2));

			SendHandshake();
		}

		private void HandleDigging(byte[] message)
		{
			if (message[0] == 0)
			{
				//Player is digging
			}
			if (message[0] == 2)
			{
				//Server.Log("Blockchange");
				//Player is done digging
				int x = util.EndianBitConverter.Big.ToInt32(message, 1);
				byte y = message[5];
				int z = util.EndianBitConverter.Big.ToInt32(message, 6);
				Server.Log(x + " " + y + " " + z);
				Server.mainlevel.BlockChange(x, y, z, 0, 0);
			}
			if (message[0] == 4)
			{
				//Player dropped item
			}
		}

        private void HandleChatMessagePacket(byte[] message)
        {
			short length = util.EndianBitConverter.Big.ToInt16(message, 0);
			string m = Encoding.BigEndianUnicode.GetString(message, 2, length * 2);

            if (m.Length > 119)
            {
                Kick("Too many characters in message!");
                return;
            }
            foreach (char ch in m)
            {
                if (ch < 32 || ch >= 127)
                {
                    Kick("Illegal character in chat message!");
                    return;
                }
            }
            
            // Test for commands
            if (m[0] == '/') //in future maybe use config defined character
            {
                m = m.Remove(0, 1);

                int pos = m.IndexOf(' ');
                if (pos == -1)
                {
                    HandleCommand(m.ToLower(), "");
                    return;
                }

                string cmd = m.Substring(0, pos).ToLower();

                HandleCommand(cmd, m);
                return;
            }

            // TODO: Rank coloring
            //GlobalMessage(this.PlayerColor + "{1}§f: {2}", WrapMethod.Chat, this.Prefix, Username, message);
            Server.ServerLogger.Log(LogLevel.Info, username + ": " + m);
			foreach (Player p in players)
			{
				p.SendMessage(username + ": " + m);
			}
        }
		
		private void HandlePlayerPacket(byte[] message)
		{
			try
			{
				byte onGround = message[0];

				if (onGround == onground)
					return;

				// TODO: Handle fall damage.

				onground = onGround;
			}
			catch (Exception e)
			{
				Server.Log(e.Message);
				Server.Log(e.StackTrace);
			}
		}
		private void HandlePlayerPositionPacket(byte[] message)
		{
			try
			{
				double x = util.EndianBitConverter.Big.ToDouble(message, 0);
				double y = util.EndianBitConverter.Big.ToDouble(message, 8);
				double stance = util.EndianBitConverter.Big.ToDouble(message, 16);
				double z = util.EndianBitConverter.Big.ToDouble(message, 24);
				byte onGround = message[32];

				// Return if position hasn't changed.
				if (x == pos[0] && y == pos[1] && z == pos[2] && stance == Stance && onGround == onground)
					return;

				// Check stance
				if (stance - y < 0.1 || stance - y > 1.65)
				{
					Kick("Illegal Stance");
					return;
				}

				// Check position
				//if (Math.Abs(x - this.X) + Math.Abs(y - this.Y) + Math.Abs(z - this.Z) > 100)
				//{
				//    Kick("You moved to quickly :( (Hacking?)");
				//    return;
				//}
				/*else */
				if (Math.Abs(x) > 3.2E7D || Math.Abs(z) > 3.2E7D)
				{
					Kick("Illegal position");
					return;
				}

				pos[0] = x;
				pos[1] = y;
				pos[2] = z;
				onground = onGround;

				//TODO Update player chunks
			}
			catch (Exception e)
			{
				Server.Log(e.Message);
				Server.Log(e.StackTrace);
			}
		}
		private void HandlePlayerLookPacket(byte[] message)
		{
			try
			{
				float yaw = util.EndianBitConverter.Big.ToSingle(message, 0);
				float pitch = util.EndianBitConverter.Big.ToSingle(message, 4);
				byte onGround = message[8];

				// Return if position hasn't changed.
				if (yaw == rot[0] && pitch == rot[1] && onGround == onground)
					return;

				rot[0] = yaw;
				rot[1] = pitch;
				onground = onGround;
			}
			catch (Exception e)
			{
				Server.Log(e.Message);
				Server.Log(e.StackTrace);
			}
		}
		private void HandlePlayerPositionAndLookPacket(byte[] message)
		{
			try
			{
				double x = util.EndianBitConverter.Big.ToDouble(message, 0);
				double y = util.EndianBitConverter.Big.ToDouble(message, 8);
				double stance = util.EndianBitConverter.Big.ToDouble(message, 16);
				double z = util.EndianBitConverter.Big.ToDouble(message, 24);
				float yaw = util.EndianBitConverter.Big.ToSingle(message, 32);
				float pitch = util.EndianBitConverter.Big.ToSingle(message, 36);
				byte onGround = message[40];

				// Return if position hasn't changed.
				if (x == pos[0] && y == pos[1] && z == pos[2] && stance == Stance &&
					yaw == rot[0] && pitch == rot[1] && onGround == onground)
					return;

				// Check stance
				if (stance - y < 0.1 || stance - y > 1.65)
				{
					Kick("Illegal Stance");
					return;
				}

				// Check position
				//if (Math.Abs(x - this.X) + Math.Abs(y - this.Y) + Math.Abs(z - this.Z) > 100)
				//{
				//    Kick("You moved to quickly :( (Hacking?)");
				//    return;
				//}
				/*else */
				if (Math.Abs(x) > 3.2E7D || Math.Abs(z) > 3.2E7D)
				{
					Kick("Illegal position");
					return;
				}

				pos[0] = x;
				pos[1] = y;
				pos[2] = z;
				rot[0] = yaw;
				rot[1] = pitch;
				onground = onGround;

				//TODO UPDATE CHUNKS
			}
			catch (Exception e)
			{
				Server.Log(e.Message);
				Server.Log(e.StackTrace);
			}
		}

		private void HandleDC(byte[] message)
		{
			Server.Log(username + " Disconnected.");
			GlobalMessage(username + " Left.");
			Disconnect();
			//TODO completely delete player.
		}
	}
}
