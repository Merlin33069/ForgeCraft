using System;

namespace SMP
{

    /// <summary>
    /// Handles packets.
    /// </summary>
	public partial class Player : System.IDisposable
	{
		       
		
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
            //string message = bigStream.ReadString16();
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
            if (message[0] == '/') // in future use config defined character
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
            //Server.ServerLogger.Log(LogLevel.Info, Username + ": " + message);
        }

	}
}
