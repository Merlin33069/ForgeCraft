using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMP
{
    public class CmdMe : Command
    {
        public override string Name { get { return "me"; } }
        public override List<String> Shortcuts { get { return new List<string> { }; } }
        public override string Category { get { return "information"; } }
        public override bool ConsoleUseable { get { return true; } }
        public override string Description { get { return "Tells everybody what you are doing."; } } //used for displaying what the commands does when using /help
		public override string PermissionNode { get { return "core.info.me"; } }
        
        public override void Use(Player p, params string[] args)
        {
           if (args.Length == 0 || args[0].ToLower() == "help")
            {
                Help(p);
                return;
            }
            
            Player.GlobalMessage(p.username + " " + MakeString(args, 0, args.Length));
        }

        public override void Help(Player p)
        {
            p.SendMessage("Shows you doing something.");
            p.SendMessage("/me (message)");
        }
    }
}

