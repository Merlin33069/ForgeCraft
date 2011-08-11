using System;
namespace SMP
{
	public class gotoLVL : Command
	{
		public gotoLVL()
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
				return "Go to a world!";
			}
		}
		public override string Name {
			get {
				return "goto";
			}
		}
		public override string PermissionNode {
			get {
				return "core.world.goto";
			}
		}
		public override System.Collections.Generic.List<string> Shortcuts {
			get {
				return new System.Collections.Generic.List<string>{ };
			}
		}
		public override void Use (Player p, params string[] args)
		{
			if (World.Find(args[0]) != null && p.level.name != args[0])
			{
				Player.players.ForEach(delegate(Player p1) { if (p1.level == p.level) p1.SendDespawn(p.id); p.SendDespawn(p1.id); });
				foreach (Chunk c in p.level.chunkData.Values) { p.SendPreChunk(c, 0); }
				p.level = World.Find(args[0]);
				p.Teleport_Player(p.level.SpawnX, p.level.SpawnY, p.level.SpawnZ);
				foreach (Chunk c in p.level.chunkData.Values) { p.SendPreChunk(c, 1); System.Threading.Thread.Sleep(10); p.SendChunk(c); }
				p.VisibleChunks.Clear();
				return;
			}
			p.SendMessage("GOTO FAILED");
		}
		public override void Help (Player p)
		{
			p.SendMessage("Goto a new level");
		}
	}
}

