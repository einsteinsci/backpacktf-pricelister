﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomSteamTools.Schema;
using CustomSteamTools.Skins;
using CustomSteamTools.Utils;
using UltimateUtil;
using UltimateUtil.Logging;
using UltimateUtil.UserInteraction;

namespace CustomSteamTools.Commands
{
	[TradeCommand]
	public class CmdInfo : ITradeCommand
	{
		public string[] Aliases => new string[] { "info", "item" };

		public string Description => "Lists info about an item.";

		public string RegistryName => "info";

		public string Syntax => "info {defindex | itemName | searchQuery}";

		public void RunCommand(CommandHandler sender, List<string> args)
		{
			if (args.Count == 0)
			{
				VersatileIO.WriteLine("Usage: " + Syntax, ConsoleColor.Red);
				return;
			}

			string query = string.Join(" ", args);
			Item item = SearchItem(query);

			if (item == null)
			{
				return;
			}

			VersatileIO.WriteLine(item.ToString(), ConsoleColor.White);
			VersatileIO.WriteLine("  Description: " + item.Description?.Shorten(120).Replace('\n', ' ') ?? "", ConsoleColor.Gray);
			VersatileIO.WriteLine("  Defindex: " + item.Defindex, ConsoleColor.Gray);
			VersatileIO.WriteLine("  Slot: {0} ({1})".Fmt(item.PlainSlot, item.Slot), ConsoleColor.Gray);
			VersatileIO.WriteLine("  Classes: " + item.ValidClasses.ToReadableString(includeBraces: false));
			VersatileIO.WriteLine("  " + item.GetSubtext());
			VersatileIO.WriteComplex("  Default Quality: {0}" + item.DefaultQuality, item.DefaultQuality.GetColor());
			if (!item.Styles.IsNullOrEmpty())
			{
				VersatileIO.WriteLine("  Styles: " + item.Styles.ToReadableString(includeBraces: false));
			}
			if (item.IsSkin())
			{
				Skin skin = item.GetSkin();
				VersatileIO.WriteLine("  " + skin.Description, skin.Grade.GetColor());
			}
			if (item.CanBeAustralium())
			{
				VersatileIO.WriteLine("  Can be Australium", ConsoleColor.Yellow);
			}
			if (item.IsCheapWeapon())
			{
				VersatileIO.WriteLine("  Drop weapon", ConsoleColor.Gray);
			}
			if (item.HalloweenOnly || item.HasHauntedVersion == true)
			{
				VersatileIO.WriteLine("  Halloween only", ConsoleColor.Cyan);
			}
		}

		public static Item SearchItem(string query)
		{
			int id = -1;
			bool isNum = int.TryParse(query, out id);

			Item item = null;

			#region lookup
			// shortcut
			if (query.ToLower() == "key")
			{
				item = DataManager.Schema.GetItem(5021);
			}
			else
			{
				foreach (Item i in DataManager.Schema.Items)
				{
					if (isNum)
					{
						if (i.Defindex == id)
						{
							item = i;
							break;
						}
					}
					else
					{
						if (i.Name.EqualsIgnoreCase(query))
						{
							item = i;
							break;
						}
					}
				}

				foreach (Skin s in WeaponSkins.Skins)
				{
					if (s.Name.EqualsIgnoreCase(query))
					{
						item = s.GetItemForm(DataManager.Schema);
						break;
					}
				}
			}
			#endregion lookup

			#region search
			if (item == null)
			{
				VersatileIO.WriteLine("Searching items...", ConsoleColor.Gray);

				List<Item> possibleItems = GetMatchingItems(query);

				if (possibleItems.IsEmpty())
				{
					VersatileIO.WriteLine("No items found matching '{0}'.".Fmt(query), ConsoleColor.Red);
					return null;
				}

				int index = -1;
				if (possibleItems.Count == 1)
				{
					index = 0;
				}
				else
				{
					index = VersatileIO.GetSelection("Select an item: ", possibleItems.ConvertAll((i) => i.ToString()));
				}

				item = possibleItems[index];
			}
			#endregion

			return item;
		}

		public static List<Item> GetMatchingItems(string query, int maxResults = 50)
		{
			List<Item> possibleItems = new List<Item>();
			if (query.IsNullOrWhitespace())
			{
				return possibleItems;
			}

			foreach (Item i in DataManager.Schema.Items)
			{
				if (possibleItems.Count >= maxResults)
				{
					break;
				}

				if (i.Name.ContainsIgnoreCase(query) || i.UnlocalizedName.ContainsIgnoreCase(query))
				{
					possibleItems.Add(i);
					continue;
				}

				if (i.IsSkin())
				{
					Skin s = i.GetSkin();

					if (s.Name.ContainsIgnoreCase(query) || s.Name.ContainsIgnoreCase(query))
					{
						possibleItems.Add(i);
					}
				}
			}

			return possibleItems;
		}
	}
}
