using System;
using System.Collections.Generic;
using System.Text;

namespace SMP
{
    public class CmdHelp : Command
    {

        public override string Name { get { return "help"; } }
        public override List<string> Shortcuts { get { return new List<string> { "?" }; } }
        public override string Category { get { return "information"; } }
        public override string Description { get { return "Displays the help"; } }
        public override bool ConsoleUseable { get { return true; } }
		public override string PermissionNode { get { return "core.info.help"; } }
			
        public override void Use(Player p, params string[] args)
        {            
            /*bool cmdsFound = false;
            bool catFound = false;

            if (args.Length == 0)
            {
                p.SendMessage(Color.Purple + "HelpBot V12: The Annoying/Pshycotic Help system");
                for (int i = 0; i < CommandCategories.Count; ++i)
                {
                    List<string> tempList = new List<string>(CommandCategories[i]);
                    p.SendMessage("Use" + Color.Purple + " /help " + tempList[0] + Color.ServerDefaultColor + tempList[1]);
                }
            }

            if (args.Length >= 1)
            {
                string categoryArg = args[0].ToLower();

                if (categoryArg == "info") //just in case someone uses info fo information
                    categoryArg = "information";

                if (categoryArg == "core")
                {
                    foreach (Command c in Command.core.commands)
                    {
                        if (Group.CheckPermission(p, Command.all.Find(c.Name)))
                        p.SendMessage("/" + c.Name + " - " + c.Description);
                    }
                }
                else if (categoryArg == "short")
                {
                    foreach (Command c in Command.all.commands)
                    {
                        if (c.Shortcuts.Count > 0 && Group.CheckPermission(p, Command.all.Find(c.Name)))
                        {
                            StringBuilder shorts = new StringBuilder();
                            shorts.Append(c.Shortcuts[0] + ", ");
                            for (int i = 1; i < c.Shortcuts.Count; i++)
                            {
                                shorts.Append(c.Shortcuts[i] + ", ");
                            }
                            shorts.Remove(shorts.Length - 2, 2);
                            p.SendMessage(Color.Purple + "/" + c.Name + Color.ServerDefaultColor + " - " + shorts.ToString());
                        }
                    }
                    return;
                }
                else
                {
                    foreach (var sublist in CommandCategories)
                    {
                        if (sublist[0].ToLower() == categoryArg)
                        {
                            catFound = true;
                            foreach (Command c in Command.all.commands)
                            {
                                if (c.Category.ToLower() == categoryArg && Group.CheckPermission(p, Command.all.Find(c.Name)))
                                {
                                    p.SendMessage("/" + c.Name + " - " + c.Description);
                                    cmdsFound = true;
                                }
                            }
                        }
                        if (cmdsFound && catFound)
                            break;
                    }
                        
                    if (catFound && !cmdsFound)
                    {
                        p.SendMessage(Color.Purple + "HelpBot V12: Couldn't find any commands in the " + args[0] + " category.");
                    }
                    else if (!catFound)
                    {
                        Command cmd = Command.all.Find(categoryArg);
                        if (cmd != null && Group.CheckPermission(p, Command.all.Find(cmd.Name)))
                        {
                            cmd.Help(p);
                            return;
                        }
                        if (cmd != null && !Group.CheckPermission(p, Command.all.Find(cmd.Name)))
                        {
                            p.SendMessage(Color.Purple + "HelpBot V12: You can't use /" + categoryArg + ", so no sense in showing you.", WrapMethod.Chat);
                            return;
                        }
                    }

                }
            }*/
        }

        public override void Help(Player p)
        {
            p.SendMessage("§5HelpBot V12: ... Really? You need help with the /help command? Do you need help breathing, too?", WrapMethod.Chat);
        }
    }
}
