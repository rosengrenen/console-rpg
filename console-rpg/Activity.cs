using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleRPG
{
	[Serializable]
	class Activity
	{
		public string part1;
		public string part1Color;
		public string part2;
		public string part2Color;
		public string part3;
		public string part3Color;
		public string part4;
		public string part4Color;

		public Activity(string p1, string p1c, string p2, string p2c, string p3, string p3c, string p4, string p4c)
		{
			part1 = p1;
			part2 = p2;
			part3 = p3;
			part1Color = p1c;
			part2Color = p2c;
			part3Color = p3c;
			part4 = p4;
			part4Color = p4c;
		}
	}
}
