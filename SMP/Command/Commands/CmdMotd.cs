using System;
using System.Collections.Generic;

namespace SMP
{
	public class CmdMotd : Command
	{
		public override string Name { get { return "motd"; } }
        public override List<string> Shortcuts { get { return new List<string> {}; } }
        public override string Category { get { return "info"; } }
        public override bool ConsoleUseable { get { return true; } }
        public override string Description { get { return "Shows you the message of the day"; } }
		public override string PermissionNode { get { return "core.info.motd"; } }

        public override void Use(Player p, params string[] args)
		{
			p.SendMessage(Server.Motd, WrapMethod.Chat);
		}
		
		public override void Help(Player p)
		{
			p.SendMessage(HelpBot + Description, WrapMethod.Chat);
			p.SendMessage("/motd");
		}
	}
}