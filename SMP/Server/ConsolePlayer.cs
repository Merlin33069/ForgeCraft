using System;
using System.Text;

//for console commands mainly and removes color codes from text

namespace SMP {

    /// <summary>
    /// Pseudo-player for handling console commands.
    /// </summary>
    public class ConsolePlayer : Player
    {

        public ConsolePlayer(Server server)
            : base()
        {
            this.group = new ConsoleGroup();
            username = Server.ConsoleName;
            ip = "127.0.0.1";
        }

        protected override void SendMessageInternal(string message)
        {
            Server.ServerLogger.Log(LogLevel.Info, ParseColors(message) );
        }

        /// <summary>
        /// Removes color codes from a string.
        /// </summary>
        private static string ParseColors(string text)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == '§' && text.Length > i + 1 && ((text[i + 1] >= 'a' && text[i + 1] <= 'f') ||
                    (text[i + 1] >= 'A' && text[i + 1] <= 'F') || (text[i + 1] >= '0' && text[i + 1] <= '9')))
                {
                    i++;
                }
                else
                {
                    sb.Append(text[i]);
                }
            }
            return sb.ToString();
        }
		
		public void SetUsername(string name)
		{
			 username = name;
		}
    }
}
