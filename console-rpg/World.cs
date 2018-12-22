using System;
using System.Collections.Generic;

namespace ConsoleRPG
{
	class World
	{
		public static List<Item> Items = new List<Item>();
		public static List<subLocation> Map = new List<subLocation>();
		public static List<Enemy> Enemies = new List<Enemy>();
		public static List<NPC> NPCs = new List<NPC>();
		public static List<Location> theWorld = new List<Location>();

		public static void Init()
		{
			GenerateWorld();
			GenerateItems();
			GenerateEnemies();
			GenerateNPCs();
			GenerateMap();
		}

		public static void GenerateWorld()
		{
			theWorld.Add(new Location("Name", "Description", 1, 2));
			theWorld.Add(new Location("Name", "Description", 1, 3));
			theWorld.Add(new Location("Name", "Description", 2, 2));
			theWorld.Add(new Location("Name", "Description", 2, 3));
			theWorld.Add(new Location("Name", "Description", 4, 1));
		}

		public static void GenerateMap()
		{
			// In the first WORLD
			Map.Add(new subLocation(
				"Home",
				"Home sweet home!",
				new List<int>() { 1, 2 },
				new List<Item>() { Items[0], Items[1] },
				new List<Enemy>() { Enemies[0], Enemies[0] },
				new List<NPC>() { NPCs[0], NPCs[0], NPCs[0] },
				0,
				0, 0, 0));
			Map.Add(new subLocation(
				"TestExit",
				"I SAID TEST!",
				new List<int>() { 0, 2 },
				new List<Item>() { },
				new List<Enemy>() { },
				new List<NPC>() { },
				0,
				1, 0, 0));
			Map.Add(new subLocation(
				 "TestExit2",
				 "I SAID TEST!",
				 new List<int>() { 0, 1 },
				 new List<Item>() { },
				 new List<Enemy>() { },
				 new List<NPC>() { },
				 0,
				 0, 1, 0));

			// In the second WORLD

			// In the third WORLD

			// In the fourth WORLD

			// In the fight and last WORLD
		}

		public static void GenerateItems()
		{

			Items.Add(new Item("The Sword of Doom", "a huge black sword", ItemType.Weapon, 1, 6, false, 50));
			Items.Add(new Item("Swedish Dagger", "a small dagger", ItemType.Weapon, 2, 7, true, 50));
			Items.Add(new Item("Wooden shield", "a small shield", ItemType.Weapon, 0, 7, true, 50));
		}

		public static void GenerateEnemies()
		{
			Enemies.Add(new Enemy("Direwolf", "A big wolf"));
		}

		public static void GenerateNPCs()
		{
			NPCs.Add(new NPC("Thee Name", "The true master of magics"));
		}
	}
}
