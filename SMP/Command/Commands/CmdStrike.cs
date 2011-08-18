using System;
using System.Collections.Generic;

namespace SMP
{
    public class CmdStrike : Command
    {
        public override string Name { get { return "strike"; } }
        public override List<string> Shortcuts { get { return new List<string> {  }; } }
        public override string Category { get { return "other"; } }
        public override bool ConsoleUseable { get { return false; } }
        public override string Description { get { return "Strike other players using lightning"; } }
        public override string PermissionNode { get { return "core.weather.strike"; } }

        public override void Use(Player p, params string[] args)
        {
            if (args.Length == 0 || args.Length > 1) { Help(p); return; }

            Player q = Player.FindPlayer(args[0]);
            int x = (int)Math.Round(q.pos.X, 0, MidpointRounding.AwayFromZero);
            int y = (int)Math.Round(q.pos.Y, 0, MidpointRounding.AwayFromZero);
            int z = (int)Math.Round(q.pos.Z, 0, MidpointRounding.AwayFromZero);

            q.SendLightning(x,y,z, 10);
            q.hurt(6);
        }

        public override void Help(Player p)
        {
            p.SendMessage("/strike <victim>");
        }
    }
}