using System;
using System.Collections.Generic;

namespace SMP
{
    public class CmdDevs : Command
    {
<<<<<<< HEAD
        List<string> devs = new List<string> { "Silentneeb", "BizarreCake", "GamezGalaxy (hypereddie10)", "jakeanator14" }; //add your names here
=======
		List<string> devs = new List<string> { "Silentneeb", "BizarreCake", "GamezGalaxy (hypereddie10)", "Merlin33069" }; //add your names here
>>>>>>> 5833f6613299cb7608269b698bd2f2053cb02095

        public override string Name { get { return "devs"; } }
        public override List<string> Shortcuts { get { return new List<string> {"developers", "authors"}; } }
        public override string Category { get { return "information"; } }
        public override bool ConsoleUseable { get { return true; } }
        public override string Description { get { return "Shows you who developed ForgeCraft."; } }
		public override string PermissionNode { get { return "core.info.devs"; } }

        public override void Use(Player p, params string[] args)
        {
            if (args.Length > 0 && args[0] == "help")
            {
                Help(p);
                return;
            }

            string devlist = "";
            string temp;
            foreach (string dev in devs)
            {
                temp = dev.Substring(0, 1);
                temp = temp.ToUpper() + dev.Remove(0, 1);
                devlist += temp + ", ";
            }
            devlist = devlist.Remove(devlist.Length - 2);
            p.SendMessage(Color.DarkBlue + "ForgeCraft Development Team: " + Color.DarkRed + devlist, WrapMethod.Chat);  //lol it was ForgetCraft
			p.SendItem(36, 278, 1, 3);
        }

        public override void Help(Player p)
        {
            p.SendMessage("/devs - Displays the list of ForgeCraft developers.");
        }
    }
}
