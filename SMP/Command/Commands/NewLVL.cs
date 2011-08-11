using System;

namespace SMP
{
	public class NewLVL : Command
	{
		public NewLVL ()
		{
		}
		public override string Category {
			get {
				return "Mod";
			}
		}
		public override bool ConsoleUseable {
			get {
				return true;
			}
		}
		public override string Description {
			get {
				return "Create a new world!";
			}
		}
		public override string Name {
			get {
				return "newlvl";
			}
		}
		public override string PermissionNode {
			get {
				return "core.world.create";
			}
		}
		public override System.Collections.Generic.List<string> Shortcuts {
			get {
				return new System.Collections.Generic.List<string>{ };
			}
		}
		public override void Use (Player p, params string[] args)
		{
			if (args.Length == 0) { Help(p); return; }
			else if (args.Length == 1)
			{
				Random rand = new Random();
				long seed = new Random().Next();
				p.SendMessage("Creating world with seed: " + seed);
				double x = 0; double y = 127; double z = 0;
				World temp = new World(x, y, z, args[0]);
				//while (Chunk.GetChunk((int)x, (int)z, temp).GetBlock((int)x, (int)(y - 1), (int)z) == 0)
				//	y--;
				temp.SpawnY = y;
				World.worlds.Add(temp);
				p.SendMessage("World " + args[0] + " MADE!");
			}
		}
		public override void Help (Player p)
		{
			p.SendMessage("Create a new level");
		}
	}
}

