using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleRPG
{
	class Item
	{
		public string Name;
		public string Description;
		public bool Useable;
		public ItemStats ItemStats;
		public ItemType ItemType;
		public int Slot;
		public int Rarity;
		public int MaxBonus;
		public int Enchants;

		public Item(string name, string description, ItemType itemType, int rarity, int slot, bool useable, int MaxRoll)
		{
			Name = name;
			Description = description;
			Rarity = rarity;
			Useable = useable;
			Slot = slot; // 0-7 are equippable, -1 is normal item
			ItemType = itemType;
			MaxBonus = MaxRoll;
			Enchants = 0;
			ItemStats.Armor = 100;
		}

		public void Use()
		{

		}
	}

	public struct ItemStats
	{
		public int Dmg;
		public int Str;
		public int Dex;
		public int Int;
		public int HP;
		public int MP;
		public int Armor;
		public int MagicResist;
		public int bonusStr;
		public int bonusDex;
		public int bonusInt;
		public int bonusHP;
		public int bonusMP;
		public int bonusArmor;
		public int bonusMagicResist;
	}

	public enum ItemType
	{
		Weapon,
		Armor,
		Consumable,
		QuestItem,
	}
}
