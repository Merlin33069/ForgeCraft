using System;
using System.Text;
using System.Collections.Generic;

namespace SMP
{

    /// <summary>
    /// Handles packets.
    /// </summary>
	public partial class Player : System.IDisposable
	{
		#region Login
		private void HandleLogin(byte[] message)
		{
			int version = util.EndianBitConverter.Big.ToInt32(message, 0);
			short length = util.EndianBitConverter.Big.ToInt16(message, 4);
			if (length > 32) { Kick("Username too long"); return; }
			username = Encoding.BigEndianUnicode.GetString(message, 6, (length * 2));
			Server.Log(ip + " Logged in as " + username);
			Player.GlobalMessage(username + " has joined the game!");
			
			if (version > Server.protocolversion)
	            {
	                Kick("Outdated server");
	                return;
	            }
	            else if (version < Server.protocolversion)
	            {
	                Kick("Outdated client");
	                return;
	            }
			
			if (Player.players.Count >= Server.MaxPlayers)
			{
				//TODO: Add VIPList checking here
				Kick("Server is Full");	
			}
			
			//TODO: Check ban list, and whitelist
			
			//TODO: load Player attributes like group, and other settings
			
			LoggedIn = true;
			SendLoginPass();
			//OnPlayerConnect Event
			if (PlayerAuth != null)
				PlayerAuth(this);
		}
		private void HandleHandshake(byte[] message)
		{
			//Server.Log("handshake-2");
			//short length = util.EndianBitConverter.Big.ToInt16(message, 0);
			//Server.Log(length + "");
			//Server.Log(Encoding.BigEndianUnicode.GetString(message, 2, length * 2));

			SendHandshake();
		}
		#endregion
		#region Chat
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
			if (!DoNotDisturb)
			{
				GlobalMessage(Color.DarkBlue + "<" + level.name + "> " + Color.White + username + ": " + m);
            	Server.ServerLogger.Log(LogLevel.Info, username + ": " + m);
			}
        }
		#endregion
		#region Movement stuffs
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

				//oldpos = pos;
				pos[0] = x;
				pos[1] = y;
				pos[2] = z;
				onground = onGround;

				e.UpdateChunks(false, false);
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

				//oldpos = pos;
				pos[0] = x;
				pos[1] = y;
				pos[2] = z;
				rot[0] = yaw;
				rot[1] = pitch;
				onground = onGround;

				e.UpdateChunks(false, false);
			}
			catch (Exception e)
			{
				Server.Log(e.Message);
				Server.Log(e.StackTrace);
			}
		}
		#endregion
		#region BlockChanges
		private void HandleDigging(byte[] message)
		{
			if (message[0] == 0)
			{
				// Send an animation to all nearby players.
                foreach( int i in VisibleEntities ) {
					Entity e = Entity.Entities[i];
					if (!e.isPlayer) continue;
					Player p = e.p;
                    if( p.level == level && p != this )
                        p.SendAnimation( id, 1 );
                }
			}
			if (message[0] == 2)
			{
				//Player is done digging
				int x = util.EndianBitConverter.Big.ToInt32(message, 1);
				byte y = message[5];
				int z = util.EndianBitConverter.Big.ToInt32(message, 6);

				byte id = e.level.GetBlock(x, y, z);
                if (id != 0)
                {
                    Item item = new Item(id, level) { count = 1, meta = level.GetMeta(x, y, z), pos = new double[3] { x + .5, y + .5, z + .5 }, rot = new byte[3] { 1, 1, 1 }, OnGround = true };
                    item.e.UpdateChunks(false, false);
                }
				
				level.BlockChange(x, y, z, 0, 0);
			}
			if (message[0] == 4)
			{
				//Player dropped item in hand
			}
		}
		private void HandleBlockPlacementPacket(byte[] message)
		{
			int blockX = util.EndianBitConverter.Big.ToInt32(message, 0);
			byte blockY = message[4];
			int blockZ = util.EndianBitConverter.Big.ToInt32(message, 5);
			byte direction = message[9];

			if (blockX == -1 && blockZ == -1)
			{
				//this is supposed to just tell the server to update food and stuffs
				return;
			}

			short blockID = util.EndianBitConverter.Big.ToInt16(message, 10);

			byte amount;
			short damage;
			if (message.Length == 15)  //incase it is the secondary packet size
			{
				amount = message[11];
				damage = util.EndianBitConverter.Big.ToInt16(message, 12);
			}

			//Server.Log(blockX + " " + blockY + " " + blockZ);

			foreach (Entity e1 in new List<Entity>(Entity.Entities.Values))
			{
				//Server.Log("checking " + e1.id + " " + (int)(e.pos[0]-1) + " " + (int)e.pos[1] + " " + (int)e.pos[2]);
				if (blockX == (int)e1.pos[0] && blockY == (int)(e1.pos[1]-1) && blockZ == (int)e1.pos[2])
				{
					//Server.Log("Entity found!");
					if (e1.isItem)
					{
						//move item
						continue;
					}
					if (e1.isObject)
					{
						//do stuff, like get in a minecart
						continue;
					}
					if (e1.isAI)
					{
						//do stuff, like shear sheep
						continue;
					}
					if (e1.isPlayer)
					{
						//dont do anything here? is there a case where you right click a player? a snowball maybe...
						//Check the players holding item, if they need to do something with it, do it.
						//anyway, if this is a player, then we dont place a block :D so return.
						return;
					}
				}
			}

			switch (direction)
			{
			case 0: blockY--; break;
			case 1: blockY++; break;
			case 2: blockZ--; break;
			case 3: blockZ++; break;
			case 4: blockX--; break;
			case 5: blockX++; break;				
			}

			if (blockID == -1)
			{
				//Players hand is empty
				//Player right clicked with empty hand!
			}
			else if (blockID >= 1 && blockID <= 127)
			{				
				level.BlockChange(blockX, (int)blockY, blockZ, (byte)blockID, 0);
			}
			else
			{
				//item to be placed? like a flinttinder bed door etc
			}
		}
		#endregion

		public void HandleHoldingChange(byte[] message)
		{
			try
			{
				current_slot_holding = util.EndianBitConverter.Big.ToInt16(message, 0);
				current_block_holding = inventory.items[current_slot_holding];
			}
			catch { }
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
