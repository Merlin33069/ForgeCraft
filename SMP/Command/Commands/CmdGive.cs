using System;
using System.Collections.Generic;

namespace SMP
{
	public class CmdGive : Command
	{
		public override string Name { get { return "give"; } }
        public override List<string> Shortcuts { get { return new List<string> {"item", "i"}; } }
        public override string Category { get { return "cheat"; } }
        public override bool ConsoleUseable { get { return true; } }
        public override string Description { get { return "Spawn items for a player"; } }
		public override string PermissionNode { get { return "core.cheat.give"; } }

        public override void Use(Player p, params string[] args)
        {
			if (args.Length == 0 || args[0].ToLower() == "help")
			{
				Help(p);
				return;
			}
			
			if (args.Length == 1)
			{
				try
				{
					short itemID = Convert.ToInt16(args[0]);
					p.SendItem(36, itemID, 1, 0);
					p.SendMessage(HelpBot + "Enjoy.");
				}
				catch{}
			}
			else if (args.Length == 2)
			{
				short itemID;
				short amount;
				try
				{
					if (!args[0].Contains(":"))
					{
						itemID = Convert.ToInt16(args[0]);
						amount = Convert.ToInt16(args[1]);
					}
					else
					{
						//TODO: item meta/damage	
					}
				}
				catch(FormatException)
				{
					Player pl = Player.FindPlayer(args[0]);
					
					if (pl != null)
					{
						try
						{
							itemID = Convert.ToInt16(args[0]);	
							pl.SendItem(36, itemID, 1, 0);
						}
						catch{}
					}
					else
					{
						p.SendMessage(HelpBot + "Can not find player.");
					}
				}
			}
			
		}
		
		public override void Help(Player p)
		{
			
		}
	}
}

