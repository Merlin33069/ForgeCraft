using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMP
{
    public class CmdKick : Command
    {
        public override string Name { get { return "kick"; } }
        public override List<String> Shortcuts { get { return new List<string> { }; } }
        public override string Category { get { return "mod"; } }
        public override bool ConsoleUseable { get { return true; } }
        public override string Description { get { return "Kicks a player."; } } //used for displaying what the commands does when using /help
		public override string PermissionNode { get { return "core.mod.kick"; } }
			
        public override void Use(Player p, params string[] args)
        {
            //TODO: Add in checks so you can't kick people higher ranked than you
            if (args.Length == 0 || args[0].ToLower() == "help")
            {
                Help(p);
                return;
            }

            Player KickPlayer = Player.FindPlayer(args[0]);
            if (KickPlayer != null && KickPlayer != p)
            {
                if (args.Length >= 2)
                {
                    StringBuilder reason = new StringBuilder();
                    for (int i = 1; i < args.Length; i++)
                    {
                        reason.Append(args[i] + " ");
                    }
                    reason.Remove(reason.Length - 1, 1);

                    KickPlayer.Kick(reason.ToString());
                }
                else
                {
                    KickPlayer.Kick("You were kicked by " + p.username);
                }
            }
            else if (KickPlayer == p)
            {
                p.SendMessage(HelpBot + "Why are you trying to kick yourself??");
            }
            else if (KickPlayer == null)
                p.SendMessage(HelpBot + "Cannot find player: " + args[0]);
        }
        public override void Help(Player p)
        {
            p.SendMessage(Description);
            p.SendMessage("/kick (Player) [reason]");
        }
    }
}
