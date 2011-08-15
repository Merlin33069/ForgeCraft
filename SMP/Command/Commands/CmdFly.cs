using System;
using System.Collections.Generic;
using System.Threading;

namespace SMP
{
    public class CmdFly : Command
    {
        public override string Name { get { return "fly"; } }
        public override List<String> Shortcuts { get { return new List<string> { "flying" }; } }
        public override string Category { get { return "Other"; } }
        public override bool ConsoleUseable { get { return false; } }
        public override string Description { get { return "Toggles fly mode"; } } //used for displaying what the commands does when using /help
        public override string PermissionNode { get { return "core.other.fly"; } }

        public override void Use(Player p, params string[] args)
        {
            if (args.Length != 0 && args.Length != 1)
            {
                Help(p);
                return;
            }
            if (args.Length == 1)
            {
                int update = IntParseFast(args[0].ToLower());
                if (update > 0 & update < 10000)
                {
                    p.FlyingUpdate = update;
                    p.SendMessage("Flying update interval set to " + IntParseFast(args[0]));
                }
                else if (update == 61964 || update == 29964) Help(p);
                else p.SendMessage("Cant set interval to " + args[0]);
                return;
            }
            if (p.isFlying)
            {
                p.isFlying = false;
                p.SendMessage("Stopped flying");
                return;
            }
            p.SendMessage("You are now flying. &cJump!");
            p.isFlying = true;
            //Thread flyThread = new Thread(() =>
            //{
                //flyingcode(p);
            //}) { Name = "FlyThread-" + p.username };
            //flyThread.Start();
        }
		//static void flyingcode(Player p)
		//{
		//    Pos pos;
		//    Pos newpos = new Pos() { x = (int)p.pos[0], y = (int)p.pos[1], z = (int)p.pos[2] };
		//    Pos oldpos = new Pos() { x = (int)p.pos[0], y = (int)p.pos[1], z = (int)p.pos[2] };
		//    List<Pos> buffer = new List<Pos>();
		//    while (p.isFlying)
		//    {
		//        Thread.Sleep(p.FlyingUpdate);
		//        newpos = new Pos() { x = (int)p.pos[0], y = (int)p.pos[1], z = (int)p.pos[2] };
		//        if (newpos.x == oldpos.x && newpos.y == oldpos.y && newpos.z == oldpos.z)
		//            continue;
		//        oldpos = new Pos() { x = (int)p.pos[0], y = (int)p.pos[1], z = (int)p.pos[2] };
		//        try
		//        {
		//            List<Pos> tempbuffer = new List<Pos>();
		//            pos = new Pos() { x = (int)p.pos[0], y = (int)p.pos[1], z = (int)p.pos[2] };
		//            for (int xx = pos.x - 3; xx < pos.x + 3; xx++)
		//                for (int yy = pos.y - 2; yy < pos.y; yy++)
		//                    for (int zz = pos.z - 3; zz < pos.z + 3; zz++)
		//                        if (p.level.GetBlock(xx, yy, zz) == 0)
		//                            tempbuffer.Add(new Pos() { x = xx, y = yy, z = zz });
		//            for (int i = 0; i < buffer.Count; )
		//            {
		//                if (!tempbuffer.Contains(buffer[i]))
		//                {
		//                    p.SendBlockChange(buffer[i].x, (byte)buffer[i].y, buffer[i].z, 0, 0);
		//                    buffer.Remove(buffer[i]);
		//                    continue;
		//                }
		//                i++;
		//            }
		//            for (int i = 0; i < tempbuffer.Count; i++)
		//                if (!buffer.Contains(tempbuffer[i]))
		//                {
		//                    buffer.Add(tempbuffer[i]);
		//                    p.SendBlockChange(tempbuffer[i].x, (byte)tempbuffer[i].y, tempbuffer[i].z, 20, 0);
		//                }
		//            tempbuffer.Clear();
		//        }
		//        catch
		//        {
		//            p.SendMessage("Flying error");
		//        }
		//    }
		//    for (int i = 0; i < buffer.Count; i++)
		//        p.SendBlockChange(buffer[i].x, (byte)buffer[i].y, buffer[i].z, 0, 0);
		//    buffer.Clear();
		//    return;
		//}
        //struct Pos { public int x, y, z;}

        static int IntParseFast(string value)
        {
            int result = 0;
            for (int i = 0; i < value.Length; i++)
            {
                char letter = value[i];
                result = 10 * result + (letter - 48);
            }
            return result;
        }

        public override void Help(Player p)
        {
            p.SendMessage(Description);
            p.SendMessage("/fly [update] (optional)");
        }
    }
}