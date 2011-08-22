using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMP
{
	public static class BlockChange
	{
		public delegate bool BCD(Player player, BCS bcs); //BlockChangeDelegate

		public static Dictionary<short, BCD> RightClickedOn = new Dictionary<short, BCD>();
		public static Dictionary<short, BCD> ItemRightClick = new Dictionary<short, BCD>();
		public static Dictionary<short, BCD> Placed = new Dictionary<short, BCD>();
		public static Dictionary<short, BCD> LeftClicked = new Dictionary<short, BCD>();
		public static Dictionary<short, BCD> Destroyed = new Dictionary<short, BCD>();


		//Init all the blockchange stuff
		public static void InitAll()
		{
			//Try to keep this organized by ID please.
			//BLOCKCHANGE METHODS HAVE TO RETURN whether or not the blockplace/digging method should continue, if these return false, the method will return.

			//Right Clicked ON Delegates (Holds delegates for when blocks are right clicked)
			RightClickedOn.Add((short)Blocks.Grass, new BCD(Till));
			RightClickedOn.Add((short)Blocks.Dirt, new BCD(Till));
			//RightClickedOn.Add(8, new BCD(BucketWater)); //These are handled in the "right clicked with an item" part
			//RightClickedOn.Add(9, new BCD(BucketWater));
			//RightClickedOn.Add(10, new BCD(BucketLava));
			//RightClickedOn.Add(11, new BCD(BucketLava));
			RightClickedOn.Add((short)Blocks.Dispenser, new BCD(OpenDispenser));
			RightClickedOn.Add((short)Blocks.NoteBlock, new BCD(ChangeNoteblock));
			RightClickedOn.Add((short)Blocks.Bed, new BCD(GetInBed));
			RightClickedOn.Add((short)Blocks.Chest, new BCD(OpenChest));
			RightClickedOn.Add((short)Blocks.CraftingTable, new BCD(OpenCraftingTable));
			RightClickedOn.Add((short)Blocks.Furnace, new BCD(OpenFurnace));
			RightClickedOn.Add((short)Blocks.FurnaceOn, new BCD(OpenFurnace));
			RightClickedOn.Add((short)Blocks.Jukebox, new BCD(PlayMusic));
			RightClickedOn.Add((short)Blocks.CakeBlock, new BCD(EatCake));
			RightClickedOn.Add((short)Blocks.RedstoneRepeaterOff, new BCD(ChangeRepeater));
			RightClickedOn.Add((short)Blocks.RedstoneRepeaterOn, new BCD(ChangeRepeater));

			//Item RightClick Deletgates (Holds Delegates for when the player right clicks with specific items)
			ItemRightClick.Add((short)Items.FlintAndSteel, new BCD(LightFire));
			ItemRightClick.Add((short)Items.AppleRed, new BCD(EatApple));
			ItemRightClick.Add((short)Items.Bow, new BCD(FireBow));
			ItemRightClick.Add((short)Items.SoupMushroom, new BCD(EatSoup));
			ItemRightClick.Add((short)Items.Seeds, new BCD(PlantSeeds));
			ItemRightClick.Add((short)Items.Bread, new BCD(EatBread));
			ItemRightClick.Add((short)Items.PorkchopRaw, new BCD(EatPorkchopRaw));
			ItemRightClick.Add((short)Items.PorkchopCooked, new BCD(EatPorkchopCooked));
			ItemRightClick.Add((short)Items.AppleGolden, new BCD(EatGoldenApple));
			ItemRightClick.Add((short)Items.Sign, new BCD(PlaceSign));
			ItemRightClick.Add((short)Items.Paintings, new BCD(PlacePainting));
			ItemRightClick.Add((short)Items.DoorWooden, new BCD(PlaceWoodenDoor));
			ItemRightClick.Add((short)Items.DoorIron, new BCD(PlaceIronDoor));
			ItemRightClick.Add((short)Items.Bucket, new BCD(UseBucket));
			ItemRightClick.Add((short)Items.BucketWater, new BCD(UseWaterBucket));
			ItemRightClick.Add((short)Items.BucketLava, new BCD(UseLavaBucket));
			ItemRightClick.Add((short)Items.Milk, new BCD(DrinkMilk)); //not really drink milk, but when you right click, it likes to empty milk, and its fuckin annoying >_>
			ItemRightClick.Add((short)Items.Minecart, new BCD(PlaceMinecart));
			ItemRightClick.Add((short)Items.Saddle, new BCD(UseSaddle));
			ItemRightClick.Add((short)Items.Redstone, new BCD(PlaceRedstone));
			ItemRightClick.Add((short)Items.Snowball, new BCD(ThrowSnowball));
			ItemRightClick.Add((short)Items.Boat, new BCD(PlaceBoat));
			ItemRightClick.Add((short)Items.Slimeball, new BCD(ThrowSlimeball));
			ItemRightClick.Add((short)Items.MinecartPowered, new BCD(PlacePoweredMinecart));
			ItemRightClick.Add((short)Items.MinecartStorage, new BCD(PlaceStorageMinecart));
			ItemRightClick.Add((short)Items.Egg, new BCD(ThrowEgg));
			ItemRightClick.Add((short)Items.FishingRod, new BCD(UseFishingRod));
			ItemRightClick.Add((short)Items.FishRaw, new BCD(EatFishRaw));
			ItemRightClick.Add((short)Items.FishCooked, new BCD(EatFishCooked));
			ItemRightClick.Add((short)Items.Dye, new BCD(UseDye));
			ItemRightClick.Add((short)Items.Cake, new BCD(PlaceCake));
			ItemRightClick.Add((short)Items.Bed, new BCD(PlaceBed));
			ItemRightClick.Add((short)Items.RedstoneRepeater, new BCD(PlaceRepeater));
			ItemRightClick.Add((short)Items.Cookie, new BCD(EatCookie));
			//ItemRightClick.Add((short)Items.Map, new BCD(IDFK?)); //?
			ItemRightClick.Add((short)Items.Shears, new BCD(UseShears));
			ItemRightClick.Add((short)Items.GoldMusicDisc, new BCD(GoldMusicDisk));
			ItemRightClick.Add((short)Items.GreenMusicDisc, new BCD(GreenMusicDisk));

			//Block Place Delegates, like water/lava and to get furnaces/dispensers/etc to lay correctly
			Placed.Add((short)Blocks.Dirt, new BCD(PlaceDirt)); //We need a timer of sorts to change this to grass? or something... idk
			Placed.Add((short)Blocks.AWater, new BCD(PlaceWater));
			Placed.Add((short)Blocks.SWater, new BCD(PlaceWater));
			Placed.Add((short)Blocks.ALava, new BCD(PlaceLava));
			Placed.Add((short)Blocks.SLava, new BCD(PlaceLava));
			Placed.Add((short)Blocks.Sand, new BCD(PlaceSand));
			Placed.Add((short)Blocks.Gravel, new BCD(PlaceGravel));
			Placed.Add((short)Blocks.Dispenser, new BCD(PlaceDispenser));
			Placed.Add((short)Blocks.RailPowered, new BCD(PlaceRailPower));
			Placed.Add((short)Blocks.RailDetector, new BCD(PlaceRailDetect));
			Placed.Add((short)Blocks.PistonSticky, new BCD(PlaceStickyPiston));
			Placed.Add((short)Blocks.Piston, new BCD(PlaceNormalPiston));
			Placed.Add((short)Blocks.Slabs, new BCD(PlaceSlabs));
			Placed.Add((short)Blocks.Torch, new BCD(PlaceTorch));
			Placed.Add((short)Blocks.StairsWooden, new BCD(PlaceStairsWooden));
			Placed.Add((short)Blocks.Chest, new BCD(PlaceChest));
			Placed.Add((short)Blocks.Furnace, new BCD(PlaceFurnace));
			Placed.Add((short)Blocks.Ladder, new BCD(PlaceLadder));
			Placed.Add((short)Blocks.Rails, new BCD(PlaceRail));
			Placed.Add((short)Blocks.StairsCobblestone, new BCD(PlaceStairsCobblestone));
			Placed.Add((short)Blocks.Lever, new BCD(PlaceLever));
			Placed.Add((short)Blocks.RedstoneTorchOff, new BCD(PlaceRedstoneTorch));
			Placed.Add((short)Blocks.ButtonStone, new BCD(PlaceButtonStone));
			Placed.Add((short)Blocks.Cactus, new BCD(PlaceCactus));
			Placed.Add((short)Blocks.SugarCane, new BCD(PlaceSugarCane));
			Placed.Add((short)Blocks.Fence, new BCD(PlaceFence));
			Placed.Add((short)Blocks.Pumpkin, new BCD(PlacePumpkin));
			Placed.Add((short)Blocks.JackOLantern, new BCD(PlaceJackOLantern));
			Placed.Add((short)Blocks.Trapdoor, new BCD(PlaceTrapdoor));

			//Block LeftClick Delegates: (Holds Delegates for when a player hits specific items)
			LeftClicked.Add((short)Blocks.NoteBlock, new BCD(PlayNoteblock));
			LeftClicked.Add((short)Blocks.DoorWooden, new BCD(OpenDoor));
			LeftClicked.Add((short)Blocks.Lever, new BCD(SwitchLever));
			LeftClicked.Add((short)Blocks.ButtonStone, new BCD(HitButton));
			LeftClicked.Add((short)Blocks.Jukebox, new BCD(EjectCd));
			LeftClicked.Add((short)Blocks.Trapdoor, new BCD(OpenTrapdoor));

			//Block Delete Delegates (Holds Delegates for when specific items are placed)
			Destroyed.Add((short)Blocks.Dispenser, new BCD(DestroyDispenser)); //Drop all Item's from the dispenser
			Destroyed.Add((short)Blocks.Bed, new BCD(DestroyBed)); //Delete the other half of the door
			Destroyed.Add((short)Blocks.SlabsDouble, new BCD(DestroyDoubleSlab)); //Drop two
			Destroyed.Add((short)Blocks.Chest, new BCD(DestroyChest)); //Drop Contents
			Destroyed.Add((short)Blocks.Seeds, new BCD(DestroyWheat)); //Drop Wheat, drop two seeds if needed
			Destroyed.Add((short)Blocks.Furnace, new BCD(DestroyFurnace)); //Drop Contents
			Destroyed.Add((short)Blocks.FurnaceOn, new BCD(DestroyFurnace)); //Drop Contents
			Destroyed.Add((short)Blocks.DoorWooden, new BCD(DestroyDoorWood)); //Delete the other half
			Destroyed.Add((short)Blocks.DoorIron, new BCD(DestroyDoorIron)); //Delete the other half
			Destroyed.Add((short)Blocks.RedStoneOre, new BCD(DestroyRedstoneOre)); //Drop random amount
			Destroyed.Add((short)Blocks.RedStoneOreGlow, new BCD(DestroyRedstoneOre)); //Drop random amount
			Destroyed.Add((short)Blocks.Snow, new BCD(DestroySnow)); //if(iteminhand==shovel) then drop snowball
			Destroyed.Add((short)Blocks.Cactus, new BCD(DestroyCacti)); //break/drop other cacti
			Destroyed.Add((short)Blocks.ClayBlock, new BCD(DestroyClay)); //Drop random amount
			Destroyed.Add((short)Blocks.SugarCane, new BCD(DestroySugarCane)); //Destroy Other canes
			Destroyed.Add((short)Blocks.Jukebox, new BCD(DestroyJukebox)); //Drop Contents
			Destroyed.Add((short)Blocks.GlowstoneBlock, new BCD(DestroyGlowStone)); //Drop random amount
		}

		public static bool Till(Player a, BCS b)
		{
			if (a.inventory.current_item.item == (short)Items.DiamondHoe || a.inventory.current_item.item == (short)Items.IronHoe || a.inventory.current_item.item == (short)Items.GoldHoe || a.inventory.current_item.item == (short)Items.StoneHoe || a.inventory.current_item.item == (short)Items.WoodenHoe)
			if (Blockclicked(a, b) == (byte)Blocks.Grass)
			{
				a.level.BlockChange((int)b.pos.x, (int)b.pos.y, (int)b.pos.z, (byte)Blocks.FarmLand, 0);
				a.inventory.current_item.meta++; //damage of the item
			}
			return true;
		}
		public static bool OpenDispenser(Player a, BCS b)
		{
			//TODO, open a dispenser window and redirect clent window handling to it.
			return false;
		}
		public static bool ChangeNoteblock(Player a, BCS b)
		{
			return false;
		}
		public static bool GetInBed(Player a, BCS b)
		{
			return false;
		}
		public static bool OpenChest(Player a, BCS b)
		{
			return false;
		}
		public static bool OpenCraftingTable(Player a, BCS b)
		{
			return false;
		}
		public static bool OpenFurnace(Player a, BCS b)
		{
			return false;
		}
		public static bool PlayMusic(Player a, BCS b)
		{
			return false;
		}
		public static bool EatCake(Player a, BCS b)
		{
			return false;
		}
		public static bool ChangeRepeater(Player a, BCS b)
		{
			return false;
		}

		public static bool DrinkMilk(Player a, BCS b)
		{
			return false;
		}
		public static bool EatApple(Player a, BCS b)
		{
			return false;
		}
		public static bool EatBread(Player a, BCS b)
		{
			return false;
		}
		public static bool EatCookie(Player a, BCS b)
		{
			return false;
		}
		public static bool EatFishCooked(Player a, BCS b)
		{
			return false;
		}
		public static bool EatFishRaw(Player a, BCS b)
		{
			return false;
		}
		public static bool EatGoldenApple(Player a, BCS b)
		{
			return false;
		}
		public static bool EatPorkchopCooked(Player a, BCS b)
		{
			return false;
		}
		public static bool EatPorkchopRaw(Player a, BCS b)
		{
			return false;
		}
		public static bool EatSoup(Player a, BCS b)
		{
			return false;
		}
		public static bool FireBow(Player a, BCS b)
		{
			//pew pew pew.
			return false;
		}
		public static bool GoldMusicDisk(Player a, BCS b)
		{
			return false;
		}
		public static bool GreenMusicDisk(Player a, BCS b)
		{
			return false;
		}
		public static bool LightFire(Player a, BCS b)
		{
			return false;
		}
		public static bool PlaceBed(Player a, BCS b)
		{
			return false;
		}
		public static bool PlaceBoat(Player a, BCS b)
		{
			return false;
		}
		public static bool PlaceCake(Player a, BCS b)
		{
			return false;
		}
		public static bool PlaceIronDoor(Player a, BCS b)
		{
			return false;
		}
		public static bool PlaceMinecart(Player a, BCS b)
		{
			return false;
		}
		public static bool PlacePainting(Player a, BCS b)
		{
			return false;
		}
		public static bool PlacePoweredMinecart(Player a, BCS b)
		{
			return false;
		}
		public static bool PlaceRedstone(Player a, BCS b)
		{
			return false;
		}
		public static bool PlaceRepeater(Player a, BCS b)
		{
			return false;
		}
		public static bool PlaceSign(Player a, BCS b)
		{
			return false;
		}
		public static bool PlaceStorageMinecart(Player a, BCS b)
		{
			return false;
		}
		public static bool PlaceWoodenDoor(Player a, BCS b)
		{
			return false;
		}
		public static bool PlantSeeds(Player a, BCS b)
		{
			return false;
		}
		public static bool ThrowEgg(Player a, BCS b)
		{
			return false;
		}
		public static bool ThrowSlimeball(Player a, BCS b)
		{
			return false;
		}
		public static bool ThrowSnowball(Player a, BCS b)
		{
			return false;
		}
		public static bool UseBucket(Player a, BCS b)
		{
			if (Blockclicked(a, b) == (byte)Blocks.AWater || Blockclicked(a, b) == (byte)Blocks.SWater)
			{
				a.level.BlockChange((int)b.pos.x, (int)b.pos.y, (int)b.pos.z, 0, 0);
				a.inventory.current_item.item = (short)Items.BucketWater;
			}
			else if (Blockclicked(a, b) == (byte)Blocks.ALava || Blockclicked(a, b) == (byte)Blocks.SLava)
			{
				a.level.BlockChange((int)b.pos.x, (int)b.pos.y, (int)b.pos.z, 0, 0);
				a.inventory.current_item.item = (short)Items.BucketLava;
			}
			return false;
		}
		public static bool UseDye(Player a, BCS b)
		{
			//We dont need to do this, duh, just catch when some things are right clicked on, like saplings.
			return false;
		}
		public static bool UseFishingRod(Player a, BCS b)
		{
			return false;
		}
		public static bool UseLavaBucket(Player a, BCS b)
		{
			return false;
		}
		public static bool UseSaddle(Player a, BCS b)
		{
			return false;
		}
		public static bool UseShears(Player a, BCS b)
		{
			return false;
		}
		public static bool UseWaterBucket(Player a, BCS b)
		{
			return false;
		}

		public static bool PlaceButtonStone(Player a, BCS b)
		{
			if (!BlockData.CanPlaceAgainst(Blockclicked(a,b))) return false;
			if (a.level.GetBlock((int)b.pos.x, (int)b.pos.y, (int)b.pos.z) != 0) return false;

			switch (b.Direction)
			{
				case (0):
				case (1):
					return false;

				case ((byte)Directions.East):
					b.Direction = (byte)Buttons.West;
					break;
				case ((byte)Directions.West):
					b.Direction = (byte)Buttons.East;
					break;
				case ((byte)Directions.North):
					b.Direction = (byte)Buttons.South;
					break;
				case ((byte)Directions.South):
					b.Direction = (byte)Buttons.North;
					break;

				default:
					return false;
			}

			a.level.BlockChange((int)b.pos.x, (int)b.pos.y, (int)b.pos.z, (byte)b.ID, b.Direction);
			a.inventory.Remove(a.inventory.current_index, 1);
			return false;

		}
		public static bool PlaceCactus(Player a, BCS b)
		{
			return true;
		}
		public static bool PlaceChest(Player a, BCS b)
		{
			return false;
		}
		public static bool PlaceDirt(Player a, BCS b)
		{
			return true;
		}
		public static bool PlaceDispenser(Player a, BCS b)
		{
			switch (b.Direction)
			{
				case (0):
				case (1):
					return false;

				case ((byte)Directions.South):
					b.Direction = (byte)Dispenser.South;
					break;
				case ((byte)Directions.North):
					b.Direction = (byte)Dispenser.North;
					break;
				case ((byte)Directions.West):
					b.Direction = (byte)Dispenser.West;
					break;
				case ((byte)Directions.East):
					b.Direction = (byte)Dispenser.East;
					break;

				default:
					return false;
			}

			a.level.BlockChange((int)b.pos.x, (int)b.pos.y, (int)b.pos.z, (byte)b.ID, b.Direction);
			a.inventory.Remove(a.inventory.current_index, 1);
			return false;
		}
		public static bool PlaceFence(Player a, BCS b)
		{
			return false;
		}
		public static bool PlaceFurnace(Player a, BCS b)
		{
			switch (b.Direction)
			{
				case (0):
				case (1):
					return false;

				case ((byte)Directions.East):
					b.Direction = (byte)Furnace.East;
					break;
				case ((byte)Directions.West):
					b.Direction = (byte)Furnace.West;
					break;
				case ((byte)Directions.North):
					b.Direction = (byte)Furnace.North;
					break;
				case ((byte)Directions.South):
					b.Direction = (byte)Furnace.South;
					break;

				default:
					return false;
			}

			a.level.BlockChange((int)b.pos.x, (int)b.pos.y, (int)b.pos.z, (byte)b.ID, b.Direction);
			a.inventory.Remove(a.inventory.current_index, 1);
			return false;
		}
		public static bool PlaceGravel(Player a, BCS b)
		{
			return true;
		}
		public static bool PlaceJackOLantern(Player a, BCS b)
		{
			return false;
		}
		public static bool PlaceLadder(Player a, BCS b)
		{
			switch (b.Direction)
			{
				case (0):
				case (1):
					return false;

				case ((byte)Directions.South):
					b.Direction = (byte)Ladder.South;
					break;
				case ((byte)Directions.North):
					b.Direction = (byte)Furnace.North;
					break;
				case ((byte)Directions.West):
					b.Direction = (byte)Furnace.West;
					break;
				case ((byte)Directions.East):
					b.Direction = (byte)Furnace.East;
					break;

				default:
					return false;
			}

			a.level.BlockChange((int)b.pos.x, (int)b.pos.y, (int)b.pos.z, (byte)b.ID, b.Direction);
			a.inventory.Remove(a.inventory.current_index, 1);
			return false;
		}
		public static bool PlaceLava(Player a, BCS b)
		{
			return true;
		}
		public static bool PlaceLever(Player a, BCS b)
		{
			return false;
		}
		public static bool PlaceNormalPiston(Player a, BCS b)
		{
			return false;
		}
		public static bool PlaceStikyPiston(Player a, BCS b)
		{
			return false;
		}
		public static bool PlacePumpkin(Player a, BCS b)
		{
			return false;
		}
		public static bool PlaceRail(Player a, BCS b)
		{
			return false;
		}
		public static bool PlaceRailDetect(Player a, BCS b)
		{
			return false;
		}
		public static bool PlaceRailPower(Player a, BCS b)
		{
			return false;
		}
		public static bool PlaceRedstoneTorch(Player a, BCS b)
		{
			return false;
		}
		public static bool PlaceSand(Player a, BCS b)
		{
			return true;
		}
		public static bool PlaceSlabs(Player a, BCS b)
		{
			return false;
		}
		public static bool PlaceStairsCobblestone(Player a, BCS b)
		{
			return false;
		}
		public static bool PlaceStairsWooden(Player a, BCS b)
		{
			return false;
		}
		public static bool PlaceStickyPiston(Player a, BCS b)
		{
			return false;
		}
		public static bool PlaceSugarCane(Player a, BCS b)
		{
			return true;
		}
		public static bool PlaceTorch(Player a, BCS b)
		{
			return false;
		}
		public static bool PlaceTrapdoor(Player a, BCS b)
		{
			return false;
		}
		public static bool PlaceWater(Player a, BCS b)
		{
			return true;
		}

		public static bool PlayNoteblock(Player a, BCS b)
		{
			return false;
		}
		public static bool OpenDoor(Player a, BCS b)
		{
			return false;
		}
		public static bool SwitchLever(Player a, BCS b)
		{
			return false;
		}
		public static bool HitButton(Player a, BCS b)
		{
			return false;
		}
		public static bool EjectCd(Player a, BCS b)
		{
			return false;
		}
		public static bool OpenTrapdoor(Player a, BCS b)
		{
			return false;
		}

		public static bool DestroyDispenser(Player a, BCS b)
		{
			return false;
		}
		public static bool DestroyBed(Player a, BCS b)
		{
			return false;
		}
		public static bool DestroyDoubleSlab(Player a, BCS b)
		{
			return false;
		}
		public static bool DestroyChest(Player a, BCS b)
		{
			return false;
		}
		public static bool DestroyWheat(Player a, BCS b)
		{
			return false;
		}
		public static bool DestroyFurnace(Player a, BCS b)
		{
			return false;
		}
		public static bool DestroyDoorWood(Player a, BCS b)
		{
			return false;
		}
		public static bool DestroyDoorIron(Player a, BCS b)
		{
			return false;
		}
		public static bool DestroyRedstoneOre(Player a, BCS b)
		{
			return false;
		}
		public static bool DestroySnow(Player a, BCS b)
		{
			return false;
		}
		public static bool DestroyCacti(Player a, BCS b)
		{
			return false;
		}
		public static bool DestroyClay(Player a, BCS b)
		{
			return false;
		}
		public static bool DestroySugarCane(Player a, BCS b)
		{
			return false;
		}
		public static bool DestroyJukebox(Player a, BCS b)
		{
			return false;
		}
		public static bool DestroyGlowStone(Player a, BCS b)
		{
			return false;
		}

		/// <summary>
		/// this one reverses the direction offset and returns the block id that was clicked
		/// this does not always need to be used, only if the direction offset has already been applied
		/// in the packet handling.
		/// </summary>
		/// <param name="p"></param>
		/// <param name="a"></param>
		/// <returns></returns>
		public static byte Blockclicked(Player p, BCS a)
		{
			int x = (int)a.pos.x;
			int y = (int)a.pos.y;
			int z = (int)a.pos.z;

			switch (a.Direction)
			{
				case 0: y++; break;
				case 1: y--; break;
				case 2: z++; break;
				case 3: z--; break;
				case 4: x++; break;
				case 5: x--; break;
			}

			return p.level.GetBlock(x, y, z);
		}
	}
	public struct BCS //BlockChangeStruct (This is used to hold the blockchange information)
	{
		public Point3 pos;
		public short ID;
		public byte Direction;
		public byte Amount;
		public short Damage;

		public BCS(Point3 pos, short id, byte direction)
		{
			this.pos = pos;
			ID = id;
			Direction = direction;
			Amount = 0;
			Damage = 0;
		}
		public BCS(Point3 pos, short id, byte direction, byte amount, short damage)
		{
			this.pos = pos;
			ID = id;
			Direction = direction;
			Amount = amount;
			Damage = damage;
		}
	}
}
