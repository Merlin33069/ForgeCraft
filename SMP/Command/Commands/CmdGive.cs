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
        public override string Description { get { return "Spawns items."; } }
		public override string PermissionNode { get { return "core.cheat.give"; } }

        public override void Use(Player p, params string[] args)
        {
			if (args.Length == 0 || args[0].ToLower() == "help")
			{
				Help(p);
				return;
			}
			
			//probably an easier way, but couldn't think of it
			
			Player toPlayer = null;
			short slot = 36;
			short itemID = 0;
			byte count = 1;
			short meta = 0;
			
			short s; //doesn't actually do anything important
			
			//first arg
			try
			{
				if (short.TryParse(args[0], out s))
				{
					itemID = short.Parse(args[0]);
				}
				else if (args[0].Contains(":"))
				{
					itemID = short.Parse(args[0].Substring(0, args[0].IndexOf(":")));
					meta = short.Parse(args[0].Substring(args[0].IndexOf(":") + 1));
					
				}
				else
				{
					toPlayer = Player.FindPlayer(args[0]);	
				}
			}
			catch
			{
				p.SendMessage(HelpBot + "Something is wrong with your first argument.", WrapMethod.Chat);	
			}
			
			if (args.Length == 1)
			{
				if (toPlayer != null)
				{
					p.SendMessage(HelpBot + "Not enough arguments.");
					Help(p);
					return;
				}
				else
				{
					p.SendItem(slot, itemID, count, meta);
					p.SendMessage(HelpBot + "Enjoy!");
					return;
				}
			}
			
			//second arg
			try
			{
				if (toPlayer != null)
				{
					if (short.TryParse(args[1], out s))
					{
						itemID = short.Parse(args[1]);
					}
					else if (args[1].Contains(":"))
					{
						itemID = short.Parse(args[1].Substring(0, args[1].IndexOf(":")));
						meta = short.Parse(args[1].Substring(args[1].IndexOf(":") + 1));
					}
					else 
					{
						p.SendMessage(HelpBot + "Something is wrong with your second argument.", WrapMethod.Chat);	
						return;
					}
				}
				else
				{
					count = byte.Parse(args[1]);
				}
			}
			catch
			{
				p.SendMessage(HelpBot + "Something is wrong with your second argument.", WrapMethod.Chat);	
				return;
			}
			
			if (args.Length == 2)
			{
				if (toPlayer != null)
				{
					toPlayer.SendItem(slot, itemID, count, meta);
					toPlayer.SendMessage(HelpBot + "Enjoy your gift!");
					p.SendMessage(HelpBot + "Gift Given");
					return;
				}
				else
				{
					p.SendItem(slot, itemID, count, meta);
					p.SendMessage(HelpBot + "Enjoy!");
					return;
				}
			}
			
			//third arg
			try
			{	
				count = byte.Parse(args[2]);	
			}
			catch
			{
				p.SendMessage(HelpBot + "Third Argument is invalid.");
				return;
			}
			
			toPlayer.SendItem(slot, itemID, count, meta);
			toPlayer.SendMessage(HelpBot + "Enjoy your gift!");
			p.SendMessage(HelpBot + "Gift Given");			
			
		}
		
		public override void Help(Player p)
		{
			p.SendMessage("Spawns item(s), and if specified to a player.");
			p.SendMessage("/give (player) <item(:value)> (amount)");
		}
	}
}

