using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleRPG
{
	class subLocation
	{
		public string Name;
		public string Description;
		public List<int> Exits = new List<int>();
		public List<Item> Items = new List<Item>();
		public List<Enemy> Enemies = new List<Enemy>();
		public List<NPC> NPCs = new List<NPC>();
		public int BattleChance;
		public bool visited;
		public bool seen;
		public COORD coordinate;

		public subLocation(
			string Name,
			string Description,
			List<int> Exits,
			List<Item> Items,
			List<Enemy> Enemies,
			List<NPC> NPCs,
			int BattleChance,
			int X,
			int Y,
			int World)
		{
			this.Name = Name;
			this.Description = Description;
			this.Exits = Exits;
			this.Items = Items;
			this.Enemies = Enemies;
			this.NPCs = NPCs;
			this.BattleChance = BattleChance;
			this.seen = false;
			this.visited = false;
			this.coordinate.X = X;
			this.coordinate.Y = Y;
			this.coordinate.World = World;
		}
	}
}
