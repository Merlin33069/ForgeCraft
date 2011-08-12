using System;
using System.Collections.Generic;

namespace SMP
{
    public class CmdSpawn : Command
    {
        public override string Name { get { return "spawn"; } }
        public override List<String> Shortcuts { get { return new List<string> { "respawnme" }; } }
        public override string Category { get { return "Mod"; } }
        public override bool ConsoleUseable { get { return false; } }
        public override string Description { get { return "Spawns you at spawn"; } } //used for displaying what the commands does when using /help
        public override string PermissionNode { get { return "core.mod.spawn"; } }

        public override void Use(Player p, params string[] args)
        {
            if (args.Length != 0)
            {
                Help(p);
                return;
            }
            p.Teleport_Player(p.level.SpawnX, p.level.SpawnY, p.level.SpawnZ);
            /*byte[] bytes = new byte[41];
            util.EndianBitConverter.Big.GetBytes(p.level.SpawnX).CopyTo(bytes, 0);
            util.EndianBitConverter.Big.GetBytes(p.Stance).CopyTo(bytes, 8);
            util.EndianBitConverter.Big.GetBytes(p.level.SpawnY).CopyTo(bytes, 16);
            util.EndianBitConverter.Big.GetBytes(p.level.SpawnZ).CopyTo(bytes, 24);
            util.EndianBitConverter.Big.GetBytes(p.rot[0]).CopyTo(bytes, 32);
            util.EndianBitConverter.Big.GetBytes(p.rot[1]).CopyTo(bytes, 36);
            bytes[40] = p.onground;
            p.SendRaw(0x0D, bytes);*/
        }

        public override void Help(Player p)
        {
            p.SendMessage(Description);
            p.SendMessage("/spawn");
        }
    }
}