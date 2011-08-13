using System;
using System.Collections.Generic;

namespace SMP
{
	public class CmdAFK : Command
	{
		public override string Name { get { return "afk"; } }
        public override List<string> Shortcuts { get { return new List<string> {"away"}; } }
        public override string Category { get { return "general"; } }
        public override bool ConsoleUseable { get { return false; } }
        public override string Description { get { return "Sets your status to away."; } }
		public override string PermissionNode { get { return "core.general.afk"; } }

        public override void Use(Player p, params string[] args)
		{
			if (p.AFK)
			{
				p.AFK = false;
				Player.GlobalMessage(p.username + "is back.");
				return;
			}
			if (args.Length == 0)
			{
				p.AFK = true;
				Player.GlobalMessage(p.username + " is AFK");
				return;
			}
			else
			{
				p.AFK = true;
				Player.GlobalMessage(p.username + " is away, " + MakeString(args, 0, args.Length));
				return;
			}
		}
		
		public override void Help(Player p)
		{
			p.SendMessage(HelpBot + Description, WrapMethod.Chat);
			p.SendMessage("/afk");
		}
	}
}