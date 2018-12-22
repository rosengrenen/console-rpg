using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Console_RPG;
using System.Diagnostics;
using System.Threading;


namespace ConsoleRPG
{
	class Game
	{
		#region Declarations
		static int consoleW = 139;
		static int consoleH = 40;
		public enum GameStates { Start, Play, Quit, Battle, NPCTalk, Map };
		public static GameStates GameState = GameStates.Start;
		static bool run = true;
		public static string GameColor = "dgreen";
		public static Player player;
		public static bool uGameBorder = true;
		public static bool uEquipment = true;
		public static bool uInventory = true;
		public static bool uItems = true;
		public static bool uNPC = true;
		public static bool uExits = true;
		public static bool uItemInfo = true;
		public static bool uCharInfo = true;
		public static bool uActions = true;
		public static bool uActivityLog = true;
		public static List<Activity> ActivityLog = new List<Activity>();
		public static int ActivityLogScroll = 0;
		public static int MaxActivityLogItems = 200; // Has to be at least 25
		public static string SaveLocation = string.Format(
			Environment.GetFolderPath(Environment.SpecialFolder.Personal),
			Path.DirectorySeparatorChar,
			"My Games",
			Path.DirectorySeparatorChar,
			"Console RPG");
		public static Stopwatch watch = new Stopwatch();
		public static bool autosave = true;
		public static Thread save;
		public static Random rand = new Random();
		public static int BoxSelected = 0;
		public static int BoxItemSelected = 0;
		public static int InventoryScroll = 0;
		public static int ItemsScroll = 0;
		public static int NPCScroll = 0;
		public static int maxLvL = 50;
		#endregion

		/// <ideas>
		/// Each WORLD should contain about 100 subLocations
		/// Toggle windows using keys?
		/// </ideas>

		public static void Main()
		{
			Console.ForegroundColor = ConsoleColor.White;
			World.Init();
			Console.Title = "Console RPG";
			Console.SetWindowSize(consoleW, consoleH);
			Console.SetBufferSize(consoleW, consoleH);
			Console.CursorVisible = false;
			save = new Thread(BackgroundActions);
			save.Start();
			while (run)
			{
				switch (GameState)
				{
					case GameStates.Start:
						StartState();
						break;
					case GameStates.Play:
						PlayState();
						break;
					case GameStates.Battle:
						BattleState();
						break;
					case GameStates.NPCTalk:
						NPCTalk();
						break;
					case GameStates.Map:
						MapState();
						break;
					case GameStates.Quit:
						QuitState();
						break;
				}
			}
		}

		public static void BackgroundActions()
		{
			while (run)
			{
				if (autosave)
				{
					if (watch.IsRunning & (GameState == GameStates.Start | GameState == GameStates.Quit))
					{
						watch.Stop();
					}
					else if (!watch.IsRunning & (GameState != GameStates.Start | GameState != GameStates.Quit))
					{
						watch.Start();
					}
					if (watch.Elapsed.Minutes >= 5)
					{
						Save();
						watch.Reset();
					}
				}
				System.Threading.Thread.Sleep(50);
			}
		}

		#region GameStates
		public static void StartState()
		{
			string[] MenuItems = { "Main menu", "New game", "Load", "Options", "Exit" };
			int getSelected = Menu(MenuItems);
			switch (getSelected)
			{
				case 1:
					NewGame();
					break;
				case 2:
					LoadGame();
					break;
				case 3:
					Settings();
					break;
				case 4:
					run = false;
					break;
			}
		}

		public static void PlayState()
		{
			#region Game Update Calls
			if (uGameBorder)
			{
				MainGameBorder(0, true);
				uGameBorder = false;
			}
			if (uEquipment)
			{
				UpdateEquipment();
				uEquipment = false;
			}
			if (uInventory)
			{
				UpdateInventory();
				uInventory = false;
			}
			if (uItems)
			{
				UpdateItems();
				uItems = false;
			}
			if (uNPC)
			{
				UpdateNPC();
				uNPC = false;
			}
			if (uExits)
			{
				UpdateExits();
				uExits = false;
			}
			if (uItemInfo)
			{
				UpdateItemInfo();
				uItemInfo = false;
			}
			if (uCharInfo)
			{
				UpdateCharInfo();
				uCharInfo = false;
			}
			if (uActions)
			{
				UpdateActions();
				uActions = false;
			}
			if (uActivityLog)
			{
				UpdateActivityLog();
				uActivityLog = false;
			}
			#endregion
			#region Prevent Typing
			Console.SetCursorPosition(25, 38);
			ConsoleColor clr = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.Black;
			ConsoleKeyInfo key = Console.ReadKey();
			Console.ForegroundColor = clr;
			#endregion
			#region ActivityLog Scroll
			if (key.Key == ConsoleKey.PageDown)
			{
				UpdateActivityLog(false, true);
			}
			else if (key.Key == ConsoleKey.PageUp)
			{
				UpdateActivityLog(true);
			}
			#endregion
			#region TrollKeys
			else if (key.Key == ConsoleKey.D1)
			{
				AddActivity("[Player] ", "white", "Rasmus ", "green", "has leveled up to level ", "white", "25!", "yellow");
			}
			else if (key.Key == ConsoleKey.D2)
			{
				AddActivity(watch.IsRunning.ToString(), "white", "Rasmus ", "green", "aquired ", "white ", "Diamond Cutlass", "cyan");
			}
			else if (key.Key == ConsoleKey.D5)
			{
				MainGameBorder(0, false);
			}
			else if (key.Key == ConsoleKey.D6)
			{
				MainGameBorder(1, false);
			}
			else if (key.Key == ConsoleKey.D7)
			{
				player.Exp += 100000;
			}
			#endregion
			#region LeftArrow
			else if (key.Key == ConsoleKey.LeftArrow)
			{
				switch (BoxSelected)
				{
					case 1:
						if (BoxItemSelected >= player.Inventory.Count)
						{
							BoxItemSelected = player.Inventory.Count - 1;
						}
						uInventory = true;
						uItems = true;
						break;
					case 2:
						if (BoxItemSelected >= World.Map[player.Location].Items.Count)
						{
							BoxItemSelected = World.Map[player.Location].Items.Count - 1;
						}
						uItems = true;
						uNPC = true;
						break;
					case 3:
						if (BoxItemSelected >= World.Map[player.Location].Enemies.Count + World.Map[player.Location].NPCs.Count)
						{
							BoxItemSelected = World.Map[player.Location].Enemies.Count + World.Map[player.Location].NPCs.Count - 1;
						}
						uNPC = true;
						uExits = true;
						break;
				}
				uActions = true;
				BoxSelected--;
				BoxItemSelected = BoxItemSelected < 0 ? 0 : BoxItemSelected;
				BoxSelected = BoxSelected < 0 ? 0 : BoxSelected;
			}
			#endregion
			#region UpArrow
			else if (key.Key == ConsoleKey.UpArrow)
			{
				if (BoxItemSelected > 0 & BoxSelected != 4)
				{
					BoxItemSelected--;
					switch (BoxSelected)
					{
						case 0:
							uInventory = true;
							uActions = true;
							break;
						case 1:
							uItems = true;
							uActions = true;
							break;
						case 2:
							uNPC = true;
							uActions = true;
							break;
						case 3:
							uExits = true;
							break;
					}
				}
				else if (BoxSelected == 4)
				{
					if (BoxItemSelected == 0)
					{
						if (player.Inventory.Count == 0)
						{
							BoxItemSelected = 0;
						}
						else if (player.Inventory.Count > 0)
						{
							BoxItemSelected = player.Inventory.Count - 1;
						}
						BoxSelected = 0;
						uEquipment = true;
						uInventory = true;
						uActions = true;
					}
					else
					{
						BoxItemSelected--;
						uEquipment = true;
						uActions = true;
					}
				}
			}
			#endregion
			#region RightArrow
			else if (key.Key == ConsoleKey.RightArrow)
			{
				switch (BoxSelected)
				{
					case 0:
						if (BoxItemSelected >= World.Map[player.Location].Items.Count)
						{
							BoxItemSelected = World.Map[player.Location].Items.Count - 1;
						}
						uInventory = true;
						uItems = true;
						break;
					case 1:
						if (BoxItemSelected >= World.Map[player.Location].Enemies.Count + World.Map[player.Location].NPCs.Count)
						{
							BoxItemSelected = World.Map[player.Location].Enemies.Count + World.Map[player.Location].NPCs.Count - 1;
						}
						uItems = true;
						uNPC = true;
						break;
					case 2:
						if (BoxItemSelected >= World.Map[player.Location].Exits.Count)
						{
							BoxItemSelected = World.Map[player.Location].Exits.Count - 1;
						}
						uNPC = true;
						uExits = true;
						break;
				}
				uActions = true;
				BoxItemSelected = BoxItemSelected < 0 ? 0 : BoxItemSelected;
				BoxSelected++;
				BoxSelected = BoxSelected > 3 ? 3 : BoxSelected;
			}
			#endregion
			#region DownArrow
			else if (key.Key == ConsoleKey.DownArrow)
			{
				if (BoxSelected == 0)
				{
					if ((BoxItemSelected == player.Inventory.Count - 1 & player.Inventory.Count > 0) | (player.Inventory.Count == 0))
					{
						BoxItemSelected = 0;
						BoxSelected = 4;
						uInventory = true;
						uEquipment = true;
						uActions = true;
					}
					else if (BoxItemSelected < player.Inventory.Count - 1 & player.Inventory.Count > 0)
					{
						BoxItemSelected++;
						uInventory = true;
						uActions = true;
					}
				}
				else if (BoxSelected == 1)
				{
					if (World.Map[player.Location].Items.Count > 0 & BoxItemSelected < World.Map[player.Location].Items.Count - 1)
					{
						BoxItemSelected++;
						uItems = true;
					}
				}
				else if (BoxSelected == 2)
				{
					int count = World.Map[player.Location].Enemies.Count + World.Map[player.Location].NPCs.Count;
					if (count > 0 & BoxItemSelected < count - 1)
					{
						BoxItemSelected++;
						uNPC = true;
						uActions = true;
					}
				}
				else if (BoxSelected == 3)
				{
					if (World.Map[player.Location].Exits.Count > 0 & BoxItemSelected < World.Map[player.Location].Exits.Count - 1)
					{
						BoxItemSelected++;
						uExits = true;
					}
				}
				else if (BoxSelected == 4)
				{
					if (BoxItemSelected < 7)
					{
						BoxItemSelected++;
						uEquipment = true;
						uActions = true;
					}
				}
			}
			#endregion
			// Game Actions::
			#region Uneqip (N)
			else if (key.Key == ConsoleKey.N)
			{
				if (BoxSelected == 4)
				{
					if (player.Equipment[BoxItemSelected] != null)
					{
						if (player.Inventory.Count < 30)
						{
							AddActivity("[Player]Uneqipped ", "white", player.Equipment[BoxItemSelected].Name, RarityToColor(player.Equipment[BoxItemSelected].Rarity));
							player.Inventory.Add(ItemCopy(player.Equipment[BoxItemSelected]));
							player.Equipment[BoxItemSelected] = null;
							uEquipment = true;
							uInventory = true;
							uActions = true;
						}
						else
						{
							AddActivity("Error: Inventory is too full", "red");
						}

					}
				}
			}
			#endregion
			#region Equip (Q)
			else if (key.Key == ConsoleKey.Q)
			{
				if (BoxSelected == 0)
				{
					if (player.Inventory[BoxItemSelected].Slot >= 0)
					{
						AddActivity("[Player]Equipped ", "white", player.Inventory[BoxItemSelected].Name, RarityToColor(player.Inventory[BoxItemSelected].Rarity));
						if (player.Equipment[player.Inventory[BoxItemSelected].Slot] != null)
						{
							Item tmpItem = player.Equipment[player.Inventory[BoxItemSelected].Slot];
							player.Equipment[player.Inventory[BoxItemSelected].Slot] = ItemCopy(player.Inventory[BoxItemSelected]);
							player.Inventory[BoxItemSelected] = ItemCopy(tmpItem);
						}
						else
						{
							player.Equipment[player.Inventory[BoxItemSelected].Slot] = ItemCopy(player.Inventory[BoxItemSelected]);
							player.Inventory.Remove(player.Inventory[BoxItemSelected]);
						}
						uInventory = true;
						uActions = true;
						uEquipment = true;
						BoxItemSelected = BoxItemSelected >= player.Inventory.Count ? player.Inventory.Count - 1 : BoxItemSelected;
					}
				}
			}
			#endregion
			#region Examine (E)
			else if (key.Key == ConsoleKey.E)
			{
				if (BoxSelected == 1 & World.Map[player.Location].Items.Count > 0)
				{
					AddActivity("[Examine]", "white", World.Map[player.Location].Items[BoxItemSelected].Name, RarityToColor(World.Map[player.Location].Items[BoxItemSelected].Rarity), ": " + World.Map[player.Location].Items[BoxItemSelected].Description, "white");
				}
				else if (BoxSelected == 2 & World.Map[player.Location].Enemies.Count + World.Map[player.Location].NPCs.Count > 0)
				{
					int num = World.Map[player.Location].Enemies.Count;
					int num2 = World.Map[player.Location].NPCs.Count;
					if (num > 0 & num2 > 0)
					{
						if (BoxItemSelected >= num)
						{
							AddActivity("[Examine]", "white", World.Map[player.Location].NPCs[BoxItemSelected - num].Name, "green", ": " + World.Map[player.Location].NPCs[BoxItemSelected - num].Description, "white");
						}
						else
						{
							AddActivity("[Examine]", "white", World.Map[player.Location].Enemies[BoxItemSelected].Name, "red", ": " + World.Map[player.Location].Enemies[BoxItemSelected].Description, "white");
						}
					}
					else if (num > 0)
					{
						AddActivity("[Examine]", "white", World.Map[player.Location].Enemies[BoxItemSelected].Name, "red", ": " + World.Map[player.Location].Enemies[BoxItemSelected].Description, "white");
					}
					else if (num2 > 0)
					{
						AddActivity("[Examine]", "white", World.Map[player.Location].NPCs[BoxItemSelected].Name, "green", ": " + World.Map[player.Location].NPCs[BoxItemSelected].Description, "white");
					}
				}
				else if (BoxSelected == 3 & World.Map[player.Location].Exits.Count > 0)
				{
					AddActivity("[Examine]" + World.Map[World.Map[player.Location].Exits[BoxItemSelected]].Name + ": " + World.Map[World.Map[player.Location].Exits[BoxItemSelected]].Description, "white");
				}
			}
			#endregion
			#region Use (U)
			else if (key.Key == ConsoleKey.U)
			{
				if (BoxSelected == 0 & player.Inventory.Count > 0)
				{
					if (player.Inventory[BoxItemSelected].Useable)
					{
						player.Inventory[BoxItemSelected].Use();
					}
				}
				else if (BoxSelected == 4 & player.Equipment[BoxItemSelected] != null)
				{
					if (player.Equipment[BoxItemSelected].Useable)
					{
						player.Equipment[BoxItemSelected].Use();
					}
				}
			}
			#endregion
			#region Goto (G)
			else if (key.Key == ConsoleKey.G)
			{
				if (World.Map[player.Location].Exits.Count > 0 & BoxSelected == 3)
				{
					player.Location = World.Map[player.Location].Exits[BoxItemSelected];
					uExits = true;
					uNPC = true;
					uItems = true;
				}
			}
			#endregion
			#region Attack (A) // WIP
			else if (key.Key == ConsoleKey.A)
			{

			}
			#endregion
			#region Take (T)
			else if (key.Key == ConsoleKey.T)
			{
				if (World.Map[player.Location].Items.Count > 0 & BoxSelected == 1)
				{
					if (player.Inventory.Count < 30)
					{
						player.Inventory.Add(ItemCopy(World.Map[player.Location].Items[BoxItemSelected]));
						World.Map[player.Location].Items.Remove(World.Map[player.Location].Items[BoxItemSelected]);
						uActions = true;
						uItems = true;
						uInventory = true;
						BoxItemSelected = BoxItemSelected >= World.Map[player.Location].Items.Count ? World.Map[player.Location].Items.Count - 1 : BoxItemSelected;
						BoxItemSelected = BoxItemSelected < 0 ? 0 : BoxItemSelected;
					}
				}
			}
			#endregion
			#region Drop (D)
			else if (key.Key == ConsoleKey.D)
			{
				if (BoxSelected == 0)
				{
					if (player.Inventory.Count > 0)
					{
						World.Map[player.Location].Items.Add(ItemCopy(player.Inventory[BoxItemSelected]));
						player.Inventory.Remove(player.Inventory[BoxItemSelected]);
						uActions = true;
						uItems = true;
						uInventory = true;
						BoxItemSelected = BoxItemSelected >= player.Inventory.Count ? player.Inventory.Count - 1 : BoxItemSelected;
						BoxItemSelected = BoxItemSelected < 0 ? 0 : BoxItemSelected;
					}
				}
				else if (BoxSelected == 4)
				{
					World.Map[player.Location].Items.Add(ItemCopy(player.Equipment[BoxItemSelected]));
					player.Equipment[BoxItemSelected] = null;
					uActions = true;
					uItems = true;
					uEquipment = true;
				}
			}
			#endregion
			#region Speak (S) // WIP
			else if (key.Key == ConsoleKey.S)
			{

			}
			#endregion

			#region Enter/Leave Map WIP
			else if (key.Key == ConsoleKey.M)
			{
				GameState = GameStates.Map;
			}
			#endregion

			#region Settings (Esc) // WIP
			#endregion
			#region Checking LevelUp
			if (player.Lvl < 50)
			{
				while (player.Exp >= 10 * (int)Math.Pow(1.3, player.Lvl + 1))
				{
					player.Exp -= 10 * (int)Math.Pow(1.3, player.Lvl + 1);
					player.Lvl++;
					int i = rand.Next(0, 10);
					switch (i)
					{
						case 0:
							break;
						case 1:
							break;
						case 2:
							break;
						case 3:
							break;
						case 4:
							break;
						case 5:
							break;
						case 6:
							break;
						case 7:
							break;
						case 8:
							break;
						case 9:
							break;
					}
					AddActivity("You are now level ", "white", player.Lvl.ToString() + "!", "yellow", " You grow stronger.", "white");
					uCharInfo = true;
				}
			}
			else if (player.Lvl >= 50 & player.Exp > 0)
			{
				player.Exp = 0;
			}
			#endregion
		}

		/// <summary>
		/// WORK IN PROGRESS
		/// </summary>
		public static void BattleState()
		{

		}

		/// <summary>
		/// WORK IN PROGRESS
		/// </summary>
		public static void NPCTalk()
		{

		}

		public static void MapState()
		{
			Console.Clear();
			bool mapChange = true;
			int XPOS = 0;
			int YPOS = 0;
			int oldXPOS = 0;
			int oldYPOS = 0;
			int mapZoom = 1;
			int oldMapZoom = mapZoom - 1;
			int mapX = 0;
			int mapY = 0;
			while (GameState == GameStates.Map) // if map position has changed
			{
				if (mapChange)
				{
					if (mapZoom != oldMapZoom)
					{
						oldMapZoom = mapZoom;
						mapX = 0;
						mapY = 0;
						if (mapZoom == 0)
						{

						}
						else if (mapZoom == 1)
						{
							for (int i = 0; i < World.Map.Count; i++)
							{
								if (World.Map[i].coordinate.World == player.World)
								{
									if (World.Map[i].coordinate.X + 1 > mapX)
									{
										mapX = World.Map[i].coordinate.X + 1;
									}
									if (World.Map[i].coordinate.Y + 1 > mapY)
									{
										mapY = World.Map[i].coordinate.Y + 1;
									}
								}
							}
						}
						else if (mapZoom == 2)
						{

						}
					}
					if (mapZoom == 1 & mapY > 0 & mapX > 0)
					{
						if (XPOS != oldXPOS | YPOS != oldYPOS)
						{

						}
						Text.WriteAt("/" + Text.Symbols(mapX * 8 + 3) + @"\");
						for (int j = 0; j < mapX * 7 + 1; j++)
						{
							Text.WriteAt("|" + Text.Symbols(mapX * 8 + 3, " ") + "|", 0, 1 + j);
						}
						for (int i = 0; i < mapX; i++)
						{
							for (int k = 0; k < mapY; k++)
							{
								for (int c = 0; c < World.Map.Count; c++)
								{
									if (World.Map[c].coordinate.X == i & World.Map[c].coordinate.Y == k & World.Map[c].coordinate.World == player.World)
									{
										//Text.WriteAt("╔═══════╗", 3 + 8 * i, 2 + 6 * k);
										//Text.WriteAt("║       ║", 3 + 8 * i, 3 + 6 * k);
										//Text.WriteAt("║       ║", 3 + 8 * i, 4 + 6 * k);
										//Text.WriteAt("║       ║", 3 + 8 * i, 5 + 6 * k);
										//Text.WriteAt("╚═══════╝", 3 + 8 * i, 6 + 6 * k);
										Text.WriteAt("╔═══╗", 3 + 8 * i, 2 + 6 * k);
										Text.WriteAt("║001║", 3 + 8 * i, 3 + 6 * k);
										Text.WriteAt("╚═══╝", 3 + 8 * i, 4 + 6 * k);

									}
								}
							}
						}

						//Text.WriteAt("╔════╗ ╔════╗", 3, 2);
						//Text.WriteAt("║    ║ ║    ║", 3, 3);
						//Text.WriteAt("║ 01 ╠═╣ 02 ║", 3, 4);
						//Text.WriteAt("║    ║ ║    ║", 3, 5);
						//Text.WriteAt("╚════╝ ╚════╝", 3, 6);
					}
				}




				if (mapZoom == 0) // this is for the world overview
				{

				}
				else if (mapZoom == 1) // this is for the normal map view
				{

				}
				else if (mapZoom == 2) // this one is more zoomed in and for more details about the place...
				{

				}

				ConsoleKeyInfo key = Console.ReadKey();

				if (key.Key == ConsoleKey.M)
				{
					GameState = GameStates.Play;
					uActions = true;
					uActivityLog = true;
					uCharInfo = true;
					uEquipment = true;
					uExits = true;
					uGameBorder = true;
					uInventory = true;
					uItemInfo = true;
					uItems = true;
					uNPC = true;
				}
			}
		}

		/// <summary>
		/// SHOULD ASK IF YOU WANT TO LEAVE, UNSAVED PROGRESS WILL BE ABANDONDED
		/// </summary>
		public static void QuitState()
		{

		}
		#endregion

		public static string RarityToColor(int rarity)
		{
			switch (rarity)
			{
				case 0:
					return "white";
				case 1:
					return "cyan";
				case 2:
					return "blue";
				default:
					return "white";
			}
		}

		#region Game Updates
		public static void UpdateEquipment()
		{
			for (int i = 0; i < 8; i++)
			{
				if (BoxSelected == 4 & BoxItemSelected == i & player.Equipment[i] != null)
				{
					if (RarityToColor(player.Equipment[i].Rarity) == "white")
					{
						Text.WriteAt(player.Equipment[i].Name + Text.Symbols(19 - player.Equipment[i].Name.Length, " "), 3, 24 + 2 * i, "black", "white");
					}
					else
					{
						Text.WriteAt(player.Equipment[i].Name + Text.Symbols(19 - player.Equipment[i].Name.Length, " "), 3, 24 + 2 * i, RarityToColor(player.Equipment[i].Rarity), "white");
					}
				}
				else if (BoxSelected == 4 & BoxItemSelected == i & player.Equipment[i] == null)
				{
					Text.WriteAt(Text.Symbols(19, " "), 3, 24 + 2 * i, "", "white");
				}
				else if ((BoxSelected == 4 & BoxItemSelected != i) | (BoxSelected != 4))
				{
					if (player.Equipment[i] != null)
						Text.WriteAt(player.Equipment[i].Name + Text.Symbols(19 - player.Equipment[i].Name.Length, " "), 3, 24 + 2 * i, RarityToColor(player.Equipment[i].Rarity));
					else
						Text.WriteAt(Text.Symbols(19, " "), 3, 24 + 2 * i);
				}
			}
		}

		public static void UpdateInventory()
		{
			if (BoxSelected == 0)
				InventoryScroll = BoxItemSelected - 14 < 0 ? 0 : BoxItemSelected - 14;
			if (player.Inventory.Count > 0)
			{
				for (int i = 0; i < player.Inventory.Count - InventoryScroll & i < 15; i++)
				{
					if (BoxSelected == 0 & BoxItemSelected == i + InventoryScroll)
					{
						if (RarityToColor(player.Inventory[i].Rarity) == "white")
						{
							Text.WriteAt(player.Inventory[i + InventoryScroll].Name + Text.Symbols(20 - player.Inventory[i + InventoryScroll].Name.Length, " "), 2, 5 + i, "black", "white");
						}
						else
						{
							Text.WriteAt(player.Inventory[i + InventoryScroll].Name + Text.Symbols(20 - player.Inventory[i + InventoryScroll].Name.Length, " "), 2, 5 + i, RarityToColor(player.Inventory[i + InventoryScroll].Rarity), "white");
						}
					}
					else if ((BoxSelected == 0 & BoxItemSelected != i + InventoryScroll) | (BoxSelected != 0))
					{
						Text.WriteAt(player.Inventory[i + InventoryScroll].Name + Text.Symbols(20 - player.Inventory[i + InventoryScroll].Name.Length, " "), 2, 5 + i, RarityToColor(player.Inventory[i + InventoryScroll].Rarity));
					}
				}
			}
			else if (player.Inventory.Count == 0)
			{
				if (BoxSelected == 0)
				{
					Text.WriteAt(Text.Symbols(20, " "), 2, 5, "", "white");
				}
				else if (BoxSelected != 0)
				{
					Text.WriteAt(Text.Symbols(20, " "), 2, 5, "", "black");
				}
			}
			for (int j = 15; j > (player.Inventory.Count == 0 ? 1 : player.Inventory.Count); j--)
			{
				Text.WriteAt(Text.Symbols(20, " "), 2, 20 + (player.Inventory.Count == 0 ? 1 : player.Inventory.Count) - j);
			}
		}

		public static void UpdateItems()
		{
			if (BoxSelected == 1)
				ItemsScroll = BoxItemSelected - 14 < 0 ? 0 : BoxItemSelected - 14;
			if (World.Map[player.Location].Items.Count > 0)
			{
				for (int i = 0; i < World.Map[player.Location].Items.Count - ItemsScroll & i < 15; i++)
				{
					if (BoxSelected == 1 & BoxItemSelected == i + ItemsScroll)
					{
						if (RarityToColor(World.Map[player.Location].Items[i + ItemsScroll].Rarity) == "white")
						{
							Text.WriteAt(World.Map[player.Location].Items[i + ItemsScroll].Name + Text.Symbols(20 - World.Map[player.Location].Items[i + ItemsScroll].Name.Length, " "), 25, 5 + i, "black", "white");
						}
						else
						{
							Text.WriteAt(World.Map[player.Location].Items[i + ItemsScroll].Name + Text.Symbols(20 - World.Map[player.Location].Items[i + ItemsScroll].Name.Length, " "), 25, 5 + i, RarityToColor(World.Map[player.Location].Items[i + ItemsScroll].Rarity), "white");
						}
					}
					else if ((BoxSelected == 1 & BoxItemSelected != i + ItemsScroll) | (BoxSelected != 1))
					{
						Text.WriteAt(World.Map[player.Location].Items[i + ItemsScroll].Name + Text.Symbols(20 - World.Map[player.Location].Items[i + ItemsScroll].Name.Length, " "), 25, 5 + i, RarityToColor(World.Map[player.Location].Items[i + ItemsScroll].Rarity));
					}
				}
			}
			else if (World.Map[player.Location].Items.Count == 0)
			{
				if (BoxSelected == 1)
				{
					Text.WriteAt(Text.Symbols(20, " "), 25, 5, "", "white");
				}
				else if (BoxSelected != 1)
				{
					Text.WriteAt(Text.Symbols(20, " "), 25, 5, "", "black");
				}
			}
			for (int j = 15; j > (World.Map[player.Location].Items.Count == 0 ? 1 : World.Map[player.Location].Items.Count); j--)
			{
				Text.WriteAt(Text.Symbols(20, " "), 25, 20 + (World.Map[player.Location].Items.Count == 0 ? 1 : World.Map[player.Location].Items.Count) - j);
			}
		}

		public static void UpdateNPC()
		{
			int count = World.Map[player.Location].Enemies.Count + World.Map[player.Location].NPCs.Count;
			if (count > 0)
			{
				for (int i = 0; i < World.Map[player.Location].Enemies.Count; i++)
				{
					if (BoxSelected == 2 & BoxItemSelected == i)
					{
						if (World.Map[player.Location].Enemies.Count > 0)
						{
							Text.WriteAt(World.Map[player.Location].Enemies[i].Name + Text.Symbols(20 - World.Map[player.Location].Enemies[i].Name.Length, " "), 48, 5 + i, "red", "white");
						}
					}
					else if ((BoxSelected == 2 & BoxItemSelected != i) | (BoxSelected != 2))
					{
						if (World.Map[player.Location].Enemies.Count > 0)
						{
							Text.WriteAt(World.Map[player.Location].Enemies[i].Name + Text.Symbols(20 - World.Map[player.Location].Enemies[i].Name.Length, " "), 48, 5 + i, "red");
						}
					}
				}
				for (int j = 0; j < World.Map[player.Location].NPCs.Count; j++)
				{
					if (BoxSelected == 2 & BoxItemSelected == j + World.Map[player.Location].Enemies.Count)
					{
						if (World.Map[player.Location].NPCs.Count > 0)
						{
							Text.WriteAt(World.Map[player.Location].NPCs[j].Name + Text.Symbols(20 - World.Map[player.Location].NPCs[j].Name.Length, " "), 48, 5 + j + World.Map[player.Location].Enemies.Count, "green", "white");
						}
					}
					else if ((BoxSelected == 2 & BoxItemSelected != j + World.Map[player.Location].Enemies.Count) | (BoxSelected != 2))
					{
						if (World.Map[player.Location].NPCs.Count > 0)
						{
							Text.WriteAt(World.Map[player.Location].NPCs[j].Name + Text.Symbols(20 - World.Map[player.Location].NPCs[j].Name.Length, " "), 48, 5 + j + World.Map[player.Location].Enemies.Count, "green");
						}
					}
				}
			}
			else if (count == 0)
			{
				if (BoxSelected == 2)
				{
					Text.WriteAt(Text.Symbols(20, " "), 48, 5, "", "white");
				}
				else if (BoxSelected != 2)
				{
					Text.WriteAt(Text.Symbols(20, " "), 48, 5, "", "black");
				}
			}
			for (int j = 15; j > (count == 0 ? 1 : count); j--)
			{
				Text.WriteAt(Text.Symbols(20, " "), 48, 20 + (count == 0 ? 1 : count) - j);
			}
		}

		public static void UpdateExits()
		{
			if (World.Map[player.Location].Exits.Count > 0)
			{
				for (int i = 0; i < World.Map[player.Location].Exits.Count; i++)
				{
					if (BoxSelected == 3 & BoxItemSelected == i)
					{
						Text.WriteAt(World.Map[World.Map[player.Location].Exits[i]].Name + Text.Symbols(20 - World.Map[World.Map[player.Location].Exits[i]].Name.Length, " "), 71, 5 + i, "black", "white");
					}
					else if ((BoxSelected == 3 & BoxItemSelected != i) | (BoxSelected != 3))
					{
						Text.WriteAt(World.Map[World.Map[player.Location].Exits[i]].Name + Text.Symbols(20 - World.Map[World.Map[player.Location].Exits[i]].Name.Length, " "), 71, 5 + i);
					}
				}
			}
			else if (World.Map[player.Location].Exits.Count == 0)
			{
				if (BoxSelected == 3)
				{
					Text.WriteAt(Text.Symbols(20, " "), 71, 5, "", "white");
				}
				else if (BoxSelected != 3)
				{
					Text.WriteAt(Text.Symbols(20, " "), 71, 5, "", "black");
				}
			}
			for (int j = 15; j > (World.Map[player.Location].Exits.Count == 0 ? 1 : World.Map[player.Location].Exits.Count); j--)
			{
				Text.WriteAt(Text.Symbols(20, " "), 71, 20 + (World.Map[player.Location].Exits.Count == 0 ? 1 : World.Map[player.Location].Exits.Count) - j);
			}
		}

		public static void UpdateItemInfo()
		{

		}

		public static void UpdateCharInfo()
		{
			#region Health
			int num3 = player.Stats.curHealth;
			int num4 = player.Stats.maxHealth;
			string s1 = num3.ToString();
			if (num3 >= 1000 & num3 < 1000000)
			{
				s1 = num3 / 1000 + "K";
			}
			else if (num3 >= 1000000)
			{
				s1 = num3 / 1000000 + "M";
			}
			string s2 = num4.ToString();
			if (num4 >= 1000 & num4 < 1000000)
			{
				s2 = num4 / 1000 + "K";
			}
			else if (num4 >= 1000000)
			{
				s2 = num4 / 1000000 + "M";
			}
			int num = (20 - s1.Length - s2.Length - 6) / 2;
			int num2 = 20 - num - s1.Length - s2.Length - 6;
			Text.WriteAt(Text.Symbols(num2, " ") + "HP (" + s1 + "/" + s2 + ")" + Text.Symbols(num, " "), 117, 5);
			Text.WriteAt("[                  ]", 117, 6);
			int HPpart = 36 * num3 / num4;
			Text.WriteAt(Text.Symbols(HPpart / 2, "=") + (HPpart % 2 == 1 ? "-" : "") + Text.Symbols(18 - HPpart / 2 - (HPpart % 2 == 1 ? 1 : 0), " "), 118, 6, "red");
			#endregion
			#region Mana
			num3 = player.Stats.curMana;
			num4 = player.Stats.maxMana;
			s1 = num3.ToString();
			if (num3 >= 1000 & num3 < 1000000)
			{
				s1 = num3 / 1000 + "K";
			}
			else if (num3 >= 1000000)
			{
				s1 = num3 / 1000000 + "M";
			}
			s2 = num4.ToString();
			if (num4 >= 1000 & num4 < 1000000)
			{
				s2 = num4 / 1000 + "K";
			}
			else if (num4 >= 1000000)
			{
				s2 = num4 / 1000000 + "M";
			}
			num = (20 - s2.Length - s2.Length - 6) / 2;
			num2 = 20 - num - s1.Length - s2.Length - 6;
			Text.WriteAt(Text.Symbols(num2, " ") + "MP (" + s1 + "/" + s2 + ")" + Text.Symbols(num, " "), 117, 7);
			Text.WriteAt("[                  ]", 117, 8);
			int MPpart = 36 * num3 / num4;
			Text.WriteAt(Text.Symbols(MPpart / 2, "=") + (MPpart % 2 == 1 ? "-" : "") + Text.Symbols(18 - MPpart / 2 - (MPpart % 2 == 1 ? 1 : 0), " "), 118, 8, "green");
			#endregion
			#region Experience

			num3 = player.Exp;
			num4 = 10 * (int)Math.Pow(1.3, player.Lvl + 1);
			s1 = num3.ToString();
			if (num3 >= 1000 & num3 < 1000000)
			{
				s1 = num3 / 1000 + "K";
			}
			else if (num3 >= 1000000)
			{
				s1 = num3 / 1000000 + "M";
			}
			s2 = num4.ToString();
			if (num4 >= 1000 & num4 < 1000000)
			{
				s2 = num4 / 1000 + "K";
			}
			else if (num4 >= 1000000)
			{
				s2 = num4 / 1000000 + "M";
			}
			if (player.Lvl < 50)
			{
				num = (20 - s1.Length - s2.Length - player.Lvl.ToString().Length - 10) / 2;
				num2 = 20 - num - s1.Length - s2.Length - 10 - player.Lvl.ToString().Length;
				Text.WriteAt(Text.Symbols(num2, " ") + "EXP (" + s1 + "/" + s2 + ") Lv" + player.Lvl + Text.Symbols(num, " "), 117, 9);
			}
			else
			{
				Text.WriteAt("      MAX Lv50      ", 117, 9);
			}
			Text.WriteAt("[                  ]", 117, 10);
			int EXPpart = 36 * num3 / num4;
			Text.WriteAt(Text.Symbols(EXPpart / 2, "=") + (EXPpart % 2 == 1 ? "-" : "") + Text.Symbols(18 - EXPpart / 2 - (EXPpart % 2 == 1 ? 1 : 0), " "), 118, 10, "blue");

			#endregion
			Text.WriteAt("Name: " + player.Name, 117, 12);
			string s;
			if (player.Race == Race.Dwarf)
			{
				s = "Dwarven";
			}
			else if (player.Race == Race.Elf)
			{
				s = "Elven";
			}
			else
			{
				s = "Human";
			}
			Text.WriteAt("Type: " + s + " " + player.Class.ToString().ToLower(), 117, 13);
			Text.WriteAt("Str: " + player.Stats.Strength, 117, 15, "red");
			Text.WriteAt("Dex: " + player.Stats.Dexterity, 117, 16, "green");
			Text.WriteAt("Int: " + player.Stats.Intelligence, 117, 17, "blue");
			Text.WriteAt("Armor: " + player.Stats.Armor, 117, 18, "gray");
			Text.WriteAt("MResist: " + player.Stats.MagicReists, 117, 19, "cyan");
		}

		public static void UpdateActions()
		{
			List<string> actions = new List<string>();
			if (BoxSelected == 0 & player.Inventory.Count > 0)
			{
				actions.Add("D - Drop item");
				if (player.Inventory[BoxItemSelected].Useable)
				{
					actions.Add("U - Use item");
				}
				if (player.Inventory[BoxItemSelected].Slot >= 0)
				{
					actions.Add("Q - Equip item");
				}
			}
			else if (BoxSelected == 1)
			{
				if (World.Map[player.Location].Items.Count > 0)
				{
					actions.Add("T - Take item");
					actions.Add("E - Examine");
				}
			}
			else if (BoxSelected == 2)
			{
				int npc = World.Map[player.Location].NPCs.Count;
				int enemy = World.Map[player.Location].Enemies.Count;
				if (npc + enemy > 0)
				{
					if (npc == 0)
					{
						actions.Add("A - Attack");
						actions.Add("E - Examine");
					}
					else if (enemy == 0)
					{
						actions.Add("S - Speak with");
						actions.Add("E - Examine");
					}
					else
					{
						if (BoxItemSelected < enemy)
						{
							actions.Add("A - Attack");
							actions.Add("E - Examine");
						}
						else
						{
							actions.Add("S - Speak with");
							actions.Add("E - Examine");
						}
					}
				}
			}
			else if (BoxSelected == 3)
			{
				if (World.Map[player.Location].Exits.Count > 0)
				{
					actions.Add("G - Go to location");
					actions.Add("E - Examine");
				}
			}
			else if (BoxSelected == 4)
			{
				if (player.Equipment[BoxItemSelected] != null)
				{
					actions.Add("D - Drop item");
					actions.Add("N - Unequip item");
					if (player.Equipment[BoxItemSelected].Useable)
					{
						actions.Add("U - Use item");
					}
				}
			}
			for (int i = 0; i < actions.Count; i++)
			{
				Text.WriteAt(actions[i] + Text.Symbols(20 - actions[i].Length, " "), 117, 23 + i);
			}
			for (int j = actions.Count; j < 16; j++)
			{
				Text.WriteAt(Text.Symbols(20, " "), 117, 23 + j);
			}
			actions.Clear();
			Text.WriteAt("PgUp/PgDown - Scroll", 117, 37);
			Text.WriteAt("Esc - Settings", 117, 38);
		}

		public static void UpdateActivityLog(bool scrollUp = false, bool scrollDown = false)
		{
			if (scrollDown & scrollUp)
			{
				scrollDown = false;
			}
			if (scrollDown & ActivityLogScroll > 0)
			{
				ActivityLogScroll--;
			}
			if (scrollUp & ActivityLogScroll < ActivityLog.Count - 15)
			{
				ActivityLogScroll++;
			}
			if (ActivityLog.Count > 0)
			{
				int b = 0;
				for (int i = ActivityLog.Count - 1 - ActivityLogScroll; i >= 0 & i > ActivityLog.Count - 16 - ActivityLogScroll; i--)
				{
					Text.WriteAt(ActivityLog[i].part1, 25, 37 - b, ActivityLog[i].part1Color);
					Text.WriteAt(ActivityLog[i].part2, 25 + ActivityLog[i].part1.Length, 37 - b, ActivityLog[i].part2Color);
					Text.WriteAt(ActivityLog[i].part3, 25 + ActivityLog[i].part1.Length + ActivityLog[i].part2.Length, 37 - b, ActivityLog[i].part3Color);
					Text.WriteAt(ActivityLog[i].part4, 25 + ActivityLog[i].part1.Length + ActivityLog[i].part2.Length + ActivityLog[i].part3.Length, 37 - b, ActivityLog[i].part4Color);
					b++;
				}
			}
		}
		#endregion

		public static void AddActivity(string p1, string p1c, string p2 = "", string p2c = "", string p3 = "", string p3c = "", string p4 = "", string p4c = "")
		{
			while (ActivityLog.Count >= MaxActivityLogItems)
			{
				ActivityLog.Remove(ActivityLog[0]);
			}
			p4 += Text.Symbols(89 - p1.Length - p2.Length - p3.Length - p4.Length, " ");
			ActivityLog.Add(new Activity(p1, p1c, p2, p2c, p3, p3c, p4, p4c));
			ActivityLogScroll = 0;
			UpdateActivityLog();
		}

		public static void NewGame()
		{
			string tmpName;
			Race tmpRace;
			Class tmpClass;
			int tmpMode;
			int selectedMenu;
			#region Race
			string[] Races = { "Choose race:", "Human", "Elf", "Dwarf" };
			selectedMenu = Menu(Races);
			switch (selectedMenu)
			{
				case 1:
					tmpRace = Race.Human;
					break;
				case 2:
					tmpRace = Race.Elf;
					break;
				case 3:
					tmpRace = Race.Dwarf;
					break;
				default:
					tmpRace = Race.Human;
					break;
			}
			#endregion
			#region Class
			string[] Classes = { "Choose class:", "Swordsman", "Bowmaster", "Sorcerer" };
			selectedMenu = Menu(Classes);
			switch (selectedMenu)
			{
				case 1:
					tmpClass = Class.Fighter;
					break;
				case 2:
					tmpClass = Class.Archer;
					break;
				case 3:
					tmpClass = Class.Wizard;
					break;
				default:
					tmpClass = Class.Fighter;
					break;
			}
			#endregion
			#region Mode
			string[] Modes = { "Choose mode:", "Easy", "Hard", "Extreme" };
			selectedMenu = Menu(Modes);
			switch (selectedMenu)
			{
				case 1:
					tmpMode = 0;
					break;
				case 2:
					tmpMode = 1;
					break;
				case 3:
					tmpMode = 2;
					break;
				default:
					tmpMode = 0;
					break;
			}
			#endregion
			#region Name
			tmpName = "CharName";
			#endregion
			player = new Player(tmpName, tmpRace, tmpClass, tmpMode);

			player.Inventory.Add(ItemCopy(World.Items[0]));
			player.Inventory.Add(ItemCopy(World.Items[2]));
			player.Inventory.Add(ItemCopy(World.Items[0]));
			player.Inventory.Add(ItemCopy(World.Items[1]));
			player.Inventory.Add(ItemCopy(World.Items[2]));
			player.Equipment[6] = World.Items[0];
			player.Equipment[7] = World.Items[2];
			GameState = GameStates.Play;
		}

		public static Item ItemCopy(Item item)
		{
			Item tmpItem = new Item(item.Name, item.Description, item.ItemType, item.Rarity, item.Slot, item.Useable, item.MaxBonus);
			return tmpItem;
		}

		public static Item ItemReroll(Item item, bool all, int stat)
		{
			if (all)
			{
				item.ItemStats.bonusStr = rand.Next(1, item.MaxBonus + 1);
				item.ItemStats.bonusDex = rand.Next(1, item.MaxBonus + 1);
				item.ItemStats.bonusInt = rand.Next(1, item.MaxBonus + 1);
			}
			else
			{
				switch (stat)
				{
					case 1:
						item.ItemStats.bonusStr = rand.Next(1, item.MaxBonus + 1);
						break;
					case 2:
						item.ItemStats.bonusDex = rand.Next(1, item.MaxBonus + 1);
						break;
					case 3:
						item.ItemStats.bonusInt = rand.Next(1, item.MaxBonus + 1);
						break;
				}
			}
			return ItemCopy(item);
		}

		public static void LoadGame()
		{

		}

		public static void Settings()
		{

		}

		public static int Menu(string[] aStrings)
		{
			Console.Clear();
			int getSelected = 1;
			ConsoleKeyInfo keyInfo;
			Text.WriteC(@"/---------------------\", 12, GameColor);
			Text.WriteC("|                     |", 13, GameColor);
			Text.WriteC(aStrings[0], 13);
			Text.WriteC(">---------------------<", 14, GameColor);
			for (int i = 0; i < aStrings.Length + 1; i++)
			{
				Text.WriteC("|                     |", 15 + i, GameColor);
			}
			Text.WriteC(@"\---------------------/", 16 + aStrings.Length, GameColor);
			do
			{
				for (int i = 1; i < aStrings.Length; i++)
				{
					if (i == getSelected)
						Text.WriteC(aStrings[i], 15 + i, "black", "white");
					else
						Text.WriteC(aStrings[i], 15 + i);
				}
				keyInfo = Console.ReadKey();
				if (keyInfo.Key == ConsoleKey.DownArrow & getSelected != aStrings.Length - 1)
				{
					getSelected++;
				}
				else if (keyInfo.Key == ConsoleKey.UpArrow & getSelected != 1)
				{
					getSelected--;
				}
			} while (keyInfo.Key != ConsoleKey.Enter);
			return getSelected;
		}

		public static void MainGameBorder(int code, bool refresh)
		{
			// Refresh means that all the other window items also have to be updated... Like items, locaiton etc.
			if (refresh)
			{
				Text.WriteAt("|" + Text.Symbols(137, " ") + "|", 0, 1, GameColor);
				Text.WriteAt(">" + Text.Symbols(22) + "v" + Text.Symbols(22) + Text.Symbols(47, " ") + Text.Symbols(22) + "v" + Text.Symbols(22) + "<", 0, 2, GameColor);
				Text.WriteAt("|" + Text.Symbols(22, " ") + "|" + Text.Symbols(22, " ") + Text.Symbols(47, " ") + Text.Symbols(22, " ") + "|" + Text.Symbols(22, " ") + "|", 0, 3, GameColor);
				Text.WriteAt(">" + Text.Symbols(22) + "#" + Text.Symbols(22) + Text.Symbols(47, " ") + Text.Symbols(22) + "#" + Text.Symbols(22) + "<", 0, 4, GameColor);
				for (int i = 0; i < 15; i++)
				{
					Text.WriteAt("|" + Text.Symbols(22, " ") + "|" + Text.Symbols(22, " ") + Text.Symbols(47, " ") + Text.Symbols(22, " ") + "|" + Text.Symbols(22, " ") + "|", 0, 5 + i, GameColor);
				}
				Text.WriteAt(">" + Text.Symbols(22) + "#" + Text.Symbols(22) + Text.Symbols(47, " ") + Text.Symbols(22) + "#" + Text.Symbols(22) + "<", 0, 20, GameColor);
				Text.WriteAt("|" + Text.Symbols(22, " ") + "|" + Text.Symbols(91, " ") + "|" + Text.Symbols(22, " ") + "|", 0, 21, GameColor);
				Text.WriteAt(">" + Text.Symbols(22) + "#" + Text.Symbols(91) + "#" + Text.Symbols(22) + "<", 0, 22, GameColor);
				for (int i = 0; i < 16; i++)
				{
					Text.WriteAt("|" + Text.Symbols(22, " ") + "|" + Text.Symbols(91, " ") + "|" + Text.Symbols(22, " ") + "|", 0, 23 + i, GameColor);
				}
				Text.WriteAt(@"\" + Text.Symbols(22) + "^" + Text.Symbols(91) + "^" + Text.Symbols(22) + "/", 0, 39, GameColor);
				Console.SetCursorPosition(0, 0);
				Console.MoveBufferArea(0, 0, 139, 39, 0, 1);
				Text.WriteAt("/" + Text.Symbols(137) + @"\", 0, 0, GameColor);
				Text.WriteAt("Inventory", 7, 3);
				Text.WriteAt("Items", 33, 3);
				Text.WriteAt("Character info", 120, 3);
				Text.WriteAt("Item info", 100, 3);
				Text.WriteAt("Equipment", 7, 21);
				Text.WriteC("Activity log", 21);
				Text.WriteAt("Actions", 124, 21);
				//Print stats and stuff like name, type, all the constants
				//Print Head,Chest etc.
				Text.WriteAt("Head:", 2, 23);
				Text.WriteAt("Neck:", 2, 25);
				Text.WriteAt("Hands:", 2, 27);
				Text.WriteAt("Chest:", 2, 29);
				Text.WriteAt("Legs:", 2, 31);
				Text.WriteAt("Feet:", 2, 33);
				Text.WriteAt("Main-hand:", 2, 35);
				Text.WriteAt("Off-hand:", 2, 37);
			}
			//Print navigation
			//
			if (code == 0)
			{
				Text.WriteAt("v" + Text.Symbols(22) + "v" + Text.Symbols(22) + "v", 46, 2, GameColor);
				Text.WriteAt("|" + Text.Symbols(22, " ") + "|" + Text.Symbols(22, " ") + "|", 46, 3, GameColor);
				Text.WriteAt("#" + Text.Symbols(22) + "#" + Text.Symbols(22) + "#", 46, 4, GameColor);
				for (int i = 0; i < 15; i++)
				{
					Text.WriteAt("|" + Text.Symbols(22, " ") + "|" + Text.Symbols(22, " ") + "|", 46, 5 + i, GameColor);
				}
				Text.WriteAt("^" + Text.Symbols(22) + "^" + Text.Symbols(22) + "^", 46, 20, GameColor);
				Text.WriteAt("NPC", 57, 3);
				Text.WriteAt("Exits", 79, 3);
				uExits = true;
				uNPC = true;
			}
			else if (code == 1)
			{
				Text.WriteAt("v" + Text.Symbols(45) + "v", 46, 2, GameColor);
				Text.WriteAt("|" + Text.Symbols(45, " ") + "|", 46, 3, GameColor);
				Text.WriteAt("#" + Text.Symbols(45) + "#", 46, 4, GameColor);
				for (int i = 0; i < 15; i++)
				{
					Text.WriteAt("|" + Text.Symbols(45, " ") + "|", 46, 5 + i, GameColor);
				}
				Text.WriteAt("^" + Text.Symbols(45) + "^", 46, 20, GameColor);
				Text.WriteAt("Reply", 67, 3);
			}
			else if (code == 2)
			{
				// WIP, the battle mode
			}
		}

		public static void Save()
		{
			if (!Directory.Exists(SaveLocation))
			{
				Directory.CreateDirectory(SaveLocation);
			}
			SaveGame s = new SaveGame();
			bool fail = false;
			try
			{
				using (Stream stream = File.Open(SaveLocation + @"\Rasmus.plr", FileMode.Create))
				{
					BinaryFormatter bin = new BinaryFormatter();
					bin.Serialize(stream, s);
					stream.Close();
				}
			}
			catch (FileNotFoundException)
			{

			}
			catch (AccessViolationException)
			{

			}
			catch (Exception)
			{
				fail = true;
			}
			if (fail)
				AddActivity("[Game] ", "white", "Game save failed...", "red", "", "");
			else
				AddActivity("[Game] ", "white", "Game saved successfully!", "green", "", "");

		}

		public static void Load()
		{
			if (!Directory.Exists(SaveLocation))
			{
				Directory.CreateDirectory(SaveLocation);
			}
			try
			{
				using (Stream stream = File.Open(SaveLocation + @"\Rasmus.plr", FileMode.Open))
				{
					BinaryFormatter bin = new BinaryFormatter();
					SaveGame s = (SaveGame)bin.Deserialize(stream);
					s.Save();
				}
			}
			catch (FileNotFoundException)
			{

			}
			catch (AccessViolationException)
			{

			}
			catch (Exception)
			{

			}
		}

	}
	[Serializable]
	class SaveGame
	{
		List<Activity> ActivityLog = Game.ActivityLog;

		public void Save()
		{
			Game.ActivityLog = ActivityLog;
		}
	}
}
