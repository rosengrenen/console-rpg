using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleRPG
{
	class Text
	{
		public static void WriteAt(string aString, int left = 0, int top = 0, string fgcolor = "", string bgcolor = "")
		{
			ConsoleColor fg = Console.ForegroundColor;
			ConsoleColor bg = Console.BackgroundColor;
			Color(fgcolor, bgcolor);
			Console.SetCursorPosition(left, top);
			Console.Write(aString);
			Console.ForegroundColor = fg;
			Console.BackgroundColor = bg;
		}

		public static string Symbols(int amt = 1, string symbol = "-")
		{
			string aString = "";
			for (int i = 0; i < amt; i++)
			{
				aString += symbol;
			}
			return aString;
		}

		public static void WriteC(string aString, int top = 0, string fgcolor = "", string bgcolor = "")
		{
			ConsoleColor fg = Console.ForegroundColor;
			ConsoleColor bg = Console.BackgroundColor;
			Color(fgcolor, bgcolor);
			Console.SetCursorPosition((Console.WindowWidth - aString.Length) / 2, top);
			Console.Write(aString);
			Console.ForegroundColor = fg;
			Console.BackgroundColor = bg;
		}

		public static void Color(string fg, string bg)
		{
			switch (fg)
			{
				case "black":
					Console.ForegroundColor = ConsoleColor.Black;
					break;
				case "blue":
					Console.ForegroundColor = ConsoleColor.Blue;
					break;
				case "cyan":
					Console.ForegroundColor = ConsoleColor.Cyan;
					break;
				case "dblue":
					Console.ForegroundColor = ConsoleColor.DarkBlue;
					break;
				case "dcyan":
					Console.ForegroundColor = ConsoleColor.DarkCyan;
					break;
				case "dgray":
					Console.ForegroundColor = ConsoleColor.DarkGray;
					break;
				case "dgreen":
					Console.ForegroundColor = ConsoleColor.DarkGreen;
					break;
				case "dmagenta":
					Console.ForegroundColor = ConsoleColor.DarkMagenta;
					break;
				case "dred":
					Console.ForegroundColor = ConsoleColor.DarkRed;
					break;
				case "dyellow":
					Console.ForegroundColor = ConsoleColor.DarkYellow;
					break;
				case "gray":
					Console.ForegroundColor = ConsoleColor.Gray;
					break;
				case "green":
					Console.ForegroundColor = ConsoleColor.Green;
					break;
				case "magenta":
					Console.ForegroundColor = ConsoleColor.Magenta;
					break;
				case "red":
					Console.ForegroundColor = ConsoleColor.Red;
					break;
				case "yellow":
					Console.ForegroundColor = ConsoleColor.Yellow;
					break;
				case "white":
					Console.ForegroundColor = ConsoleColor.White;
					break;
			}

			switch (bg)
			{
				case "black":
					Console.BackgroundColor = ConsoleColor.Black;
					break;
				case "blue":
					Console.BackgroundColor = ConsoleColor.Blue;
					break;
				case "cyan":
					Console.BackgroundColor = ConsoleColor.Cyan;
					break;
				case "dblue":
					Console.BackgroundColor = ConsoleColor.DarkBlue;
					break;
				case "dcyan":
					Console.BackgroundColor = ConsoleColor.DarkCyan;
					break;
				case "dgray":
					Console.BackgroundColor = ConsoleColor.DarkGray;
					break;
				case "dgreen":
					Console.BackgroundColor = ConsoleColor.DarkGreen;
					break;
				case "dmagenta":
					Console.BackgroundColor = ConsoleColor.DarkMagenta;
					break;
				case "dred":
					Console.BackgroundColor = ConsoleColor.DarkRed;
					break;
				case "dyellow":
					Console.BackgroundColor = ConsoleColor.DarkYellow;
					break;
				case "gray":
					Console.BackgroundColor = ConsoleColor.Gray;
					break;
				case "green":
					Console.BackgroundColor = ConsoleColor.Green;
					break;
				case "magenta":
					Console.BackgroundColor = ConsoleColor.Magenta;
					break;
				case "red":
					Console.BackgroundColor = ConsoleColor.Red;
					break;
				case "yellow":
					Console.BackgroundColor = ConsoleColor.Yellow;
					break;
				case "white":
					Console.BackgroundColor = ConsoleColor.White;
					break;
			}
		}
	}
}
