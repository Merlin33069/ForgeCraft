using System;
using System.Collections.Generic;

// for LULZ
namespace SMP
{
	public class CmdHackz : Command
	{
		public override string Name { get { return "hackz"; } }
        public override List<string> Shortcuts { get { return new List<string> {"hacks", "hack"}; } }
        public override string Category { get { return "cheats"; } }
        public override bool ConsoleUseable { get { return false; } }
        public override string Description { get { return "Hack the server like a pro."; } }
		public override string PermissionNode { get { return "core.cheat.hackz"; } }

        public override void Use(Player p, params string[] args)
		{
			p.Kick(HelpBot + Color.DarkRed + "YOU FAIL!");
		}
		
		public override void Help(Player p)
		{
			p.SendMessage(Description);
			p.SendMessage("/hackz");
		}
	}
}
