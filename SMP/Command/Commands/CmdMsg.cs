using System;
using System.Collections.Generic;

namespace SMP
{
	public class CmdMsg : Command
	{
		public override string Name { get { return "msg"; } }
        public override List<string> Shortcuts { get { return new List<string> {"m", "tell"}; } }
        public override string Category { get { return "general"; } }
        public override bool ConsoleUseable { get { return true; } }
        public override string Description { get { return "Send a player a message"; } }
		public override string PermissionNode { get { return "core.general.message"; } }

        public override void Use(Player p, params string[] args)
		{
			if (args.Length <= 1)
			{
				Help(p);
				return;
			}
			
			Player targetP = Player.FindPlayer(args[0]);
			if (targetP != null)
			{
				targetP.SendMessage(Color.DarkRed + "[" + p.username + ">>> Me]" + Color.White + MakeString(args, 1, args.Length));
				p.SendMessage(HelpBot + "Message Sent.");
			}
			else
			{
				p.SendMessage(HelpBot + "Could not find specified player.");	
			}
		}
		
		public override void Help(Player p)
		{
			p.SendMessage(HelpBot + Description);
			p.SendMessage("/msg <player> <message>");
		}
	}
}