using System;
using System.Collections.Generic;

namespace SMP
{
	public class CmdDND : Command
	{
		public override string Name { get { return "donotdisturb"; } }
        public override List<string> Shortcuts { get { return new List<string> {"dnd", "mineinpeace"}; } }
        public override string Category { get { return "other"; } }
        public override bool ConsoleUseable { get { return false; } }
        public override string Description { get { return "Mine in peace and quiet."; } }
		public override string PermissionNode { get { return "core.other.donotdisturb"; } }

        public override void Use(Player p, params string[] args)
		{
			if (args.Length >= 1)
			{
				Help(p);
			}
			
			if (!p.DoNotDisturb)
			{
				p.DoNotDisturb = true;
				p.SendMessage("You will not be able to recieve or send any global chat. Type /dnd again to recieve chat again", WrapMethod.Chat);
			}
			else if (p.DoNotDisturb)
			{
				p.DoNotDisturb = false;
				p.SendMessage("You will now be able to recieve and send global chat again. I don't why you'd want to though.", WrapMethod.Chat);
			}
		}
		
		public override void Help(Player p)
		{
			p.SendMessage(Description);
			p.SendMessage("/dnd");
		}
	}
}

