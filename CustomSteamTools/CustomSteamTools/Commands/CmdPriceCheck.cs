﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomSteamTools.Classifieds;
using CustomSteamTools.Items;
using CustomSteamTools.Utils;
using UltimateUtil;
using UltimateUtil.UserInteraction;

namespace CustomSteamTools.Commands
{
	[TradeCommand]
	public class CmdPriceCheck : ITradeCommand
	{
		public string[] Aliases => new string[] { "pc", "pricecheck", "price" };

		public string Description => "Gets the bp.tf price of an item.";

		public string RegistryName => "pc";

		public string Syntax => "pc {defindex | itemName | searchQuery}";

		public void RunCommand(CommandHandler sender, List<string> args)
		{
			if (args.Count == 0)
			{
				VersatileIO.Error("Usage: " + Syntax);
				return;
			}

			string query = string.Join(" ", args);
			Item item = CmdInfo.SearchItem(query);
			PriceCheckResults results = GetPriceInfo(item);

			if (results == null)
			{
				return;
			}

			if (!results.HasResults)
			{
				VersatileIO.Error("  No price data found for item " + item.Name);
				return;
			}

			if (results.HasCrates)
			{
				VersatileIO.Info("Crates by series:");
			}
			foreach (PricingCheck c in results.Uniques)
			{
				VersatileIO.WriteLine("  " + c.ToString(), c.Quality.GetColor());
			}

			foreach (PricingCheck c in results.Others)
			{
				VersatileIO.WriteLine("  " + c.ToString(), c.Quality.GetColor());
			}

			if (results.HasUnusuals)
			{
				string code = VersatileIO.GetSelection("  Enter a code or continue: ", true, 
					"U", "Get unusual prices.");
				if (code != null && code.Trim().EqualsIgnoreCase("u"))
				{
					VersatileIO.Info("{0} effects priced:", results.Unusuals.Count);
					Price total = Price.Zero;
					foreach (PricingCheck u in results.Unusuals)
					{
						VersatileIO.WriteLine("  " + u.GetUnusualEffectString(), ConsoleColor.DarkMagenta);
						total += u.Pricing.PriceMid;
					}
					Price avg = total / results.Unusuals.Count;
					VersatileIO.Info("Average Unusual Price: " + avg.ToString());
				}
			}
		}

		public static PriceCheckResults GetPriceInfo(Item item)
		{
			if (item == null)
			{
				return null;
			}

			PriceCheckResults res = new PriceCheckResults(item);
			List<ItemPricing> itemPricings = DataManager.PriceData.GetAllPriceData(item);
			foreach (ItemPricing p in itemPricings)
			{
				res.AddIfTradableNotChemSet(p);
			}

			return res;
		}

		public sealed class PricingCheck
		{
			public ItemPricing Pricing
			{ get; private set; }

			public Quality Quality => Pricing.Quality;

			public UnusualEffect Unusual
			{ get; private set; }

			public PricingCheck(ItemPricing pricing)
			{
				Pricing = pricing;
				if (Quality == Quality.Unusual)
				{
					Unusual = DataManager.Schema.Unusuals.First((ue) => ue.ID == pricing.PriceIndex);
				}
			}

			public string GetUnusualEffectString()
			{
				return "[#{1:D2}] {0}: {2}".Fmt(Unusual.Name, Unusual.ID, Pricing.GetPriceString());
			}

			public override string ToString()
			{
				string res = Pricing.CompiledTitleName;
				if (Unusual != null)
				{
					res += "(#{0}: {1})".Fmt(Unusual.ID, Unusual.Name);
				}
				res += ": " + Pricing.GetPriceString();

				return res;
			}
		}

		public sealed class PriceCheckResults
		{
			public Item Item
			{ get; private set; }

			public List<PricingCheck> Uniques
			{ get; private set; }

			public List<PricingCheck> Unusuals
			{ get; private set; }

			public List<PricingCheck> Others
			{ get; private set; }

			public bool NotTradable
			{ get; private set; }

			public bool HasResults
			{ get; private set; }

			public bool HasUnusuals => Unusuals.Count > 0;
			public bool HasCrates => Uniques.Count > 2;

			public List<PricingCheck> All
			{
				get
				{
					List<PricingCheck> res = new List<PricingCheck>();
					res.AddRange(Uniques);
					res.AddRange(Unusuals);
					res.AddRange(Others);

					return res;
				}
			}

			public PriceCheckResults(Item item)
			{
				Item = item;
				NotTradable = true;
				HasResults = false;

				Uniques = new List<PricingCheck>();
				Unusuals = new List<PricingCheck>();
				Others = new List<PricingCheck>();
			}

			public void Add(PricingCheck check)
			{
				if (check.Quality == Quality.Unique)
				{
					Uniques.Add(check);
				}
				else if (check.Quality == Quality.Unusual)
				{
					Unusuals.Add(check);
				}
				else
				{
					Others.Add(check);
				}

				HasResults = true;
			}
			public void Add(ItemPricing pricing)
			{
				Add(new PricingCheck(pricing));
			}

			public void AddIfTradable(ItemPricing pricing)
			{
				if (pricing.Tradable)
				{
					Add(pricing);
					NotTradable = false;
				}
			}

			public void AddIfTradableNotChemSet(ItemPricing pricing)
			{
				if (!pricing.IsChemistrySet)
				{
					AddIfTradable(pricing);
				}
			}

			public Dictionary<string, object> MakeVersatileOptions()
			{
				Dictionary<string, object> options = new Dictionary<string, object>();
				if (HasCrates)
				{
					foreach (PricingCheck c in Uniques)
					{
						string str = c.Pricing.PriceIndex.ToString();
						options.Add(str, "Price index " + str);
					}
				}
				if (HasUnusuals)
				{
					options.Add("u", Unusuals.Count.ToString() + " Unusual Effects");
				}
				return options;
			}
		}
	}
}