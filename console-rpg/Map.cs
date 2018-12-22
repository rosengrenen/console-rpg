using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleRPG
{
	class Map
	{
		// IDEA: the map should be a scrollable map that shows where each location is, since each room is a square.
		// This requires a better soltuion for the Main border thing so it doesnt fck up all the time.
		// M to enter map and M/Esc to exit map. S can be used to search a certain location by number.
		// Use +/- to zoom out or in in subLocations or get out of subLocations to the Locations with -/+

	}

	public struct COORD
	{
		public int X;
		public int Y;
		public int World;
	}
}
