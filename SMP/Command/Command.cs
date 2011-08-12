using System;
using System.Collections.Generic;
using System.Text;

namespace SMP
{
    public abstract class Command
    {
        /// <summary>
        /// Declarations
        /// Note the variance between this and Mclawl's command structure to save yourself some headaches.
        /// </summary>
        public abstract string Name { get; }
        public abstract List<String> Shortcuts { get; }
        public abstract string Category { get; }
        public abstract bool ConsoleUseable { get; }
        public abstract string Description { get; } //used for displaying what the commands does when using /help
		public abstract string PermissionNode { get; }
        public abstract void Use(Player p, params string[] args);
        public abstract void Help(Player p);
        public static CommandList all = new CommandList();
        public static CommandList core = new CommandList();
        public static List<List<string>> CommandCategories = new List<List<string>>();
        public static string HelpBot = Color.Purple + "HelpBot V12: "; //we can totally change it, or keep it for lawls. hint, hint

		/// <summary>
		/// Initializes core commands
		/// </summary>
        public static void InitCore()
        {
            //please put in alphabetical order and use core.add now not all.add
            //core.Add(new CmdBan());
            core.Add(new CmdDevs());
			core.Add(new CmdDND());
            //core.Add(new CmdGive());
			core.Add(new CmdGod());
			core.Add(new gotoLVL());
            core.Add(new CmdHelp());
			core.Add(new CmdHackz());
            core.Add(new CmdKick());
            core.Add(new CmdList());
            //core.Add(new CmdMBan());
            core.Add(new CmdMe());
            //core.Add(new CmdMKick());
			core.Add(new NewLVL());
            //core.Add(new CmdReserveList());
            core.Add(new CmdSay());
			core.Add(new SetTime());
            //core.Add(new CmdSpawn());
            //core.Add(new CmdUnban());
            //core.Add(new CmdWhiteList());
            core.Add(new CmdTeleport());
            all.commands = new List<Command>(core.commands);
            InitCommandTypes();
        }

		/// <summary>
		/// Init core types
		/// </summary>
        public static void InitCommandTypes()
        {
            CommandCategories.Add(new List<string> { "Build", " for Building Commands" });
            CommandCategories.Add(new List<string> { "Mod", " for Moderation Commands" });
            CommandCategories.Add(new List<string> { "Information", " for Informative Commands" });
			CommandCategories.Add(new List<string> { "Cheats", " Commands for Wusses" });
            CommandCategories.Add(new List<string> { "Core", " for Non-plugin Commands Commands" });
            CommandCategories.Add(new List<string> { "Short", " for Command Shortcuts" });
            CommandCategories.Add(new List<string> { "Other", " for Uncategorized Commands" });

            //TODO: add types when plugins are added
                //might just have it done in the plugin initialization
        }

        /// <summary>
        /// Concatenates a string array to a singel string. If stopindex is >= args.length, stopindex will become args.length - 1.
        /// </summary>
        /// <param name="args"></param>
        /// <param name="startindex"></param>
        /// <param name="stopindex"></param>
        /// <returns>string</returns>
        public string MakeString(string[] args, int startindex, int stopindex)
        {
            if (stopindex >= args.Length)
                stopindex = args.Length - 1;
            StringBuilder message = new StringBuilder();
            for (int i = startindex; i <= stopindex; i++)
            {
                message.Append(args[i] + " ");
            }

            message.Remove(message.Length - 1, 1);
            return message.ToString();
        }
    }
}
