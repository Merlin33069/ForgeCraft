using System;
using System.Text;

namespace SMP
{

    /// <summary>
    /// Handles packets.
    /// </summary>
	public partial class Player : System.IDisposable
	{
		//////////////////////////////////////////////////"http://www.wiki.vg/Protocol"///
        // 0x01 - Login Request
		// --------------------
		// Sent by the client after the handshake to finish logging in. 
		// If the version is outdated or any field is invalid, the 
		// server will disconnect the client with a kick. If the client
		// is started in offline mode, the player's username will 
		// default to Player, making LAN play with more than one player
		// impossible (without authenticating) as the server will 
		// prevent multiple users with the same name. 
		// --Payload---------------------------------
		//  - int, Protocol Version
		//  - string16, Username
		//  - long, Map seed
		//  - byte, Dimension
		private void HandleLogin(byte[] message)
		{
			int version = util.EndianBitConverter.Big.ToInt32(message, 0);
			short length = util.EndianBitConverter.Big.ToInt16(message, 4);
			if (length > 32) { Kick("Username too long"); return; }
			username = Encoding.BigEndianUnicode.GetString(message, 6, (length * 2));
			Server.Log(username);

			LoggedIn = true;
			SendLoginPass();
		}
		
		//////////////////////////////////////////////////"http://www.wiki.vg/Protocol"///
        // 0x02 - Handshake
		// ----------------
		// This is the first packet sent when the client connects and
		// is used for Authentication. 
		// --Payload------------------
		//  - string16, Username
		private void HandleHandshake(byte[] message)
		{
			short length = util.EndianBitConverter.Big.ToInt16(message, 0);
			//Server.Log(length + "");
			Server.Log(Encoding.BigEndianUnicode.GetString(message, 2, length * 2));

			SendHandshake();
		}       
		
		//////////////////////////////////////////////////"http://www.wiki.vg/Protocol"///
        // 0x03 - Chat Message
        // -------------------
        // A message from the client.
        // The server will kick the client if the
        // message has over 119 characters in it.
        // --Payload-----------------------------
        //  - string16, Message
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


	}
}
