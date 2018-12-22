using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleRPG
{
	class Location
	{
		public string Name;
		public string Description;
		public COORD coordinate;

		public Location(string name, string description, int X, int Y)
		{
			Name = name;
			Description = description;
			coordinate.X = X;
			coordinate.Y = Y;
		}
	}
}
