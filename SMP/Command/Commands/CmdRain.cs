using System;
using System.Collections.Generic;

namespace SMP
{
    public class CmdRain : Command
    {
        public override string Name { get { return "rain"; } }
        public override List<string> Shortcuts { get { return new List<string> {  }; } }
        public override string Category { get { return "other"; } }
        public override bool ConsoleUseable { get { return false; } }
        public override string Description { get { return "Makes it rain?"; } }
        public override string PermissionNode { get { return "core.weather.rain"; } }

        public override void Use(Player p, params string[] args)
        {
            World w = World.Find(p.level.name);
            
            

           
           

            if (args.Length == 1)
            {

                if (args[0] == "off")
                {
                    foreach (Player q in Player.players)
                    {
                        w.rain(false, q);
                    }
                    p.SendMessage(Color.Red + "Stopping rain..");
                    w.Israining = false;
                   // p.SendMessage("rain is: " + w.isRain().ToString());
                    
                }
                if (args[0] == "on")
                {
                    foreach (Player q in Player.players)
                    {
                        w.SendLightning(1, 1, 100, 2, q);
                        w.rain(true, q);   
                    }
                    w.Israining = true;
                  //  p.SendMessage("rain is: " + w.isRain().ToString());
                    
                    p.SendMessage(Color.Green + "Starting rain...");
                }
                if (args[0] == "status") { p.SendMessage(Color.Purple + "Rain is: " + w.Israining); }
                
              
            }
            else { Help(p); return; }
            
        }

        public override void Help(Player p)
        {
           
            p.SendMessage(Color.Blue + "/rain <on:off>");
        }
    }
}