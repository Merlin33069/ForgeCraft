using System;
using System.Collections.Generic;

namespace SMP
{
    public class CmdFire : Command
    {
        public override string Name { get { return "fire"; } }
        public override List<String> Shortcuts { get { return new List<string> { "catchfire" }; } }
        public override string Category { get { return "Other"; } }
        public override bool ConsoleUseable { get { return false; } }
        public override string Description { get { return "toggles whether player is on fire"; } } //used for displaying what the commands does when using /help
        public override string PermissionNode { get { return "core.other.fire"; } }

        public override void Use(Player p, params string[] args)
        {
            if (args.Length >= 2)
            {
                Help(p);
                return;
            }
            if (args.Length == 1)
            {
                Player who = Player.FindPlayer(args[0]); // cannot use a using here or players dissapear.
                if (who != null)
                {
                    if (!who.IsOnFire)
                    {
                        who.SetFire(true);
                        Player.GlobalMessage(String.Format("{0} was set on fire by {1}", who.username, p.username));
                    }
                    else
                    {
                        who.SetFire(false);
                        Player.GlobalMessage(String.Format("{0} was extinguished by {1}", who.username, p.username));
                    }
                    return;
                }
                Help(p);
            }
            if (args.Length == 0)
            {
                p.SetFire(!p.IsOnFire ? true : false);
                p.SendMessage("You are on fire = " + p.IsOnFire);
                return;
            }
        }

        public override void Help(Player p)
        {
            p.SendMessage(Description);
            p.SendMessage("/fire [player] (optional)");
        }
    }
}