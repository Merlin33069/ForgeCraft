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
			ItemRightClick.Add((short)Items.Milk, new BCD(DrinkMilk));
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
			return true;
		}
		public static bool BucketWater(Player a, BCS b)
		{
			return false;
		}
		public static bool BucketLava(Player a, BCS b)
		{
			return false;
		}
		public static bool OpenDispenser(Player a, BCS b)
		{
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
			return false;
		}
		public static bool UseDye(Player a, BCS b)
		{
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
	}
	public struct BCS //BlockChangeStruct (This is used to hold the blockchange information)
	{
		Point3 pos;
		short ID;
		byte Direction;
		byte Amount;
		short Damage;

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
