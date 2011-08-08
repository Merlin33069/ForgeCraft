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
			username = Encoding.BigEndianUnicode.GetString(message, 6, (length * 2));
			Server.Log(username);

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
			Server.Log(length + "");
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
        private void HandleChatMessagePacket()
        {
            //string message = bigStream.ReadString16(); //relic code
			string message = "HELLO!!!!"; // temp
            if (message.Length > 119)
            {
                Kick("Too many characters in message!");
                return;
            }
            foreach (char ch in message)
            {
                if (ch < 32 || ch >= 127)
                {
                    Kick("Illegal character in chat message!");
                    return;
                }
            }
            
            // Test for commands
            if (message[0] == '/') //in future maybe use config defined character
            {
                message = message.Remove(0, 1);

                int pos = message.IndexOf(' ');
                if (pos == -1)
                {
                    HandleCommand(message.ToLower(), "");
                    return;
                }

                string cmd = message.Substring(0, pos).ToLower();

                HandleCommand(cmd, message);
                return;
            }

            // TODO: Rank coloring
            //GlobalMessage(this.PlayerColor + "{1}§f: {2}", WrapMethod.Chat, this.Prefix, Username, message);
            Server.ServerLogger.Log(LogLevel.Info, username + ": " + message);
        }

	}
}
