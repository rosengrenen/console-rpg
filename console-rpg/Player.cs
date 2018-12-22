using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleRPG
{
	class Player
	{
		public Stats Stats;
		public string Name;
		public Race Race;
		public Class Class;
		public List<Item> Inventory;
		public int Location;
		public int World;
		public Item[] Equipment;
		public int Mode;
		public int Lvl;
		public int Exp;
		// Equipment in following order: Head, Neck, Top, Hands, Pants, Feet, Main-H, Off-H

		public Player(string name, Race race, Class _class, int mode)
		{
			Name = name;
			Race = race;
			Class = _class;
			Mode = mode;
			Inventory = new List<Item>();
			Equipment = new Item[8];
			Lvl = 1;
			Exp = 0;
			Stats.maxHealth = 314;
			Stats.curHealth = Stats.maxHealth;
			Stats.maxMana = 507;
			Stats.curMana = Stats.maxMana;
			// Stats are created depending on class race and mode
			// Location it determined by class and race, pretty much
			Location = 0;
			Stats.Dexterity = 1;
			World = 0;
		}
	}

	public struct Stats
	{
		public int maxHealth;
		public int curHealth;
		public int maxMana;
		public int curMana;
		public int Strength;
		public int Dexterity;
		public int Intelligence;
		public int Armor;
		public int MagicReists;
	}

	public enum Race
	{
		Human,
		Elf,
		Dwarf,
	}

	public enum Class
	{
		Fighter,
		Archer,
		Wizard,
	}
}
