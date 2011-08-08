using System;
using System.Collections.Generic;
using System.Text;

namespace SMP
{
    public class CmdSay : Command
    {
        public override string Name { get { return "say"; } }
        public override List<String> Shortcuts { get { return new List<string> {"broadcast"}; } }
        public override string Category { get { return "information"; } }
        public override bool ConsoleUseable { get { return true; } }
        public override string Description { get { return "Announces a message to the server"; } } //used for displaying what the commands does when using /help
		public override string PermissionNode { get { return "core.info.say"; } }

        public override void Use(Player p, params string[] args)
        {
            if (args.Length == 0 || (args.Length == 1 && args[0] == "help"))
            {
                Help(p);
                return;
            }

            Player.GlobalMessage(MakeString(args, 0, args.Length), WrapMethod.Chat);
        }

        public override void Help(Player p)
        {
            p.SendMessage(Description);
            p.SendMessage("/say (message)");
        }
    }
}