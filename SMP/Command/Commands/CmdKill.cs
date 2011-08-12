using System;
using System.Collections.Generic;
	
namespace SMP
{
	public class CmdKill : Command
	{
		public override string Name { get { return "kill"; } }
        public override List<string> Shortcuts { get { return new List<string> {"murder"}; } }
        public override string Category { get { return "mod"; } }
        public override bool ConsoleUseable { get { return true; } }
        public override string Description { get { return "Tasty Murder!!"; } }
		public override string PermissionNode { get { return "core.mod.devs"; } }

        public override void Use(Player p, params string[] args)
        {
			// CURRENTLY JUST USING FOR DEBUG
			
			p.health = 0;
			p.SendHealth();
		}
		
		public override void Help(Player p)
		{
			
		}
	}
}

