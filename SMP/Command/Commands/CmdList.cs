using System;
using System.Text;
using System.Collections.Generic;

//TODO: group players by rank

namespace SMP
{
    public class CmdList : Command
    {
        public override string Name { get { return "list"; } }
        public override List<string> Shortcuts { get { return new List<string> { "players", "who", "online" }; } }
        public override string Category { get { return "information"; } }
        public override string Description { get { return "Shows who is online."; } }
        public override bool ConsoleUseable { get { return true; } }
        public override string PermissionNode { get { return "core.info.list"; } }

        public override void Use(Player p, params string[] args)
        {
            if (args.Length == 1 && args[0].ToLower() == "help")
            {
                Help(p);
                return;
            }

			if (Player.players.Count == 0)
			{
				p.SendMessage("Nobody is mincrafting right now. :(");
				return;
			}
			
			if (args.Length == 0)
			{
				StringBuilder sb = new StringBuilder();
				for (int i = 0; i < Player.players.Count; i++)
				{
					//sb.Append(Player.players[i].Group.GroupColor + Player.players[i].username + Color.White); // uncomment when groups are finished
					sb.Append(Player.players[i].username);
					
					if (i != Player.players.Count - 1)
                		sb.Append(", ");
				}
				p.SendMessage(sb.ToString(), WrapMethod.Chat);
			}
			else
			{
				if (args[0].ToLower() == "world")
				{
					foreach(World w in World.worlds)
					{
						StringBuilder sb = new StringBuilder();
						sb.Append(w.name + ": ");
						for (int i = 0; i < Player.players.Count; i++)
						{
							if (Player.players[i].level == w)
							{
								//sb.Append(p.Group.GroupColor + p.username + Color.White); // uncomment when groups are finished
								sb.Append(Player.players[i].username);
								
								if (i != Player.players.Count - 1)
	                        		sb.Append(", ");
							}
						}
						
						p.SendMessage(sb.ToString(), WrapMethod.Chat);
						
					}
				}
				
				if (args[0].ToLower() == "group")
				{
				 //TODO:	
					
				}
			}
        }

        public override void Help(Player p)
        {
            p.SendMessage("/list - Displays a list of who is online");
        }
    }
}
