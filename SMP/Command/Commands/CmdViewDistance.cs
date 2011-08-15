using System;
using System.Collections.Generic;

namespace SMP
{
	public class CmdViewDistance : Command
	{
		public override string Name { get { return "viewdistance"; } }
		public override List<string> Shortcuts { get { return new List<string> { "vd" }; } }
		public override string Category { get { return "general"; } }
		public override bool ConsoleUseable { get { return false; } }
		public override string Description { get { return "Change your view Distance."; } }
		public override string PermissionNode { get { return "core.general.vd"; } }

		public override void Use(Player p, params string[] args)
		{
			if (args.Length != 1) { Help(p); return; }
			int radius;
			try
			{
				radius = Convert.ToInt16(args[0]);
			}
			catch
			{
				p.SendMessage("Invalid radius.");
				return;
			}
			if (radius >= 11)
			{
				p.SendMessage("Radius too big");
				return;
			}
			p.viewdistance = radius;
		}

		public override void Help(Player p)
		{
			p.SendMessage(HelpBot + Description, WrapMethod.Chat);
			p.SendMessage("/vd <radius>");
		}
	}
}