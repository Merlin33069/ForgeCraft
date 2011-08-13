using System;
using System.Collections.Generic;

namespace SMP
{
	public class CmdGod : Command
	{
		public override string Name { get { return "god"; } }
        public override List<string> Shortcuts { get { return new List<string> {"invincible"}; } }
        public override string Category { get { return "cheats"; } }
        public override bool ConsoleUseable { get { return false; } }
        public override string Description { get { return "Mine like a coward!"; } }
		public override string PermissionNode { get { return "core.cheat.god"; } }

        public override void Use(Player p, params string[] args)
		{
			//maybe add a silent option
			if (args.Length >= 1)
			{
				Help(p);
			}
			
			p.SendMessage("Currently doesn't do anything. :(");
			if (!p.GodMode)
			{
				p.GodMode = true;
				p.SendMessage("You are now invincible. Type /god again to be a mortal", WrapMethod.Chat);
				Player.GlobalMessage(p.username + " is now being cheap and immortal, kill them!", WrapMethod.Chat);
			}
			else if (p.DoNotDisturb)
			{
				p.DoNotDisturb = false;
				p.SendMessage("You are no longer invincible.", WrapMethod.Chat);
				Player.GlobalMessage(p.username + " is no longer being a wuss, don't kill them", WrapMethod.Chat);
			}
		}
		
		public override void Help(Player p)
		{
			p.SendMessage(Description);
			p.SendMessage("/god");
		}
	}
}


