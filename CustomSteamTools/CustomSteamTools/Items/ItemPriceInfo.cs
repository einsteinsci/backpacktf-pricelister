﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomSteamTools.Items
{
	public sealed class ItemPriceInfo
	{
		public Item Item
		{ get; private set; }

		public Quality Quality
		{ get; private set; }

		public bool Craftable
		{ get; set; }

		public bool Tradable
		{ get; set; }

		public KillstreakType Killstreak
		{ get; set; }

		public UnusualEffect Unusual
		{ get; set; }

		public int? CrateSeries
		{ get; set; }

		public bool IsCrate => CrateSeries != null;

		public Skin Skin
		{
			get
			{
				if (_skin == null)
				{
					_skin = Item.GetSkin();
				}

				return _skin;
			}
		}
		private Skin _skin;

		public SkinWear? SkinWear
		{ get; private set; }

		public ItemPriceInfo(Item item, Quality quality = Quality.Unique)
		{
			Item = item;
			Quality = quality;
			Killstreak = KillstreakType.None;
			_skin = Item.GetSkin();

			Craftable = true;
			Tradable = true;
		}
		public ItemPriceInfo(ItemInstance inst)
		{
			Item = inst.Item;
			Quality = inst.Quality;
			Killstreak = inst.GetKillstreak();
			Unusual = inst.GetUnusual();
			CrateSeries = inst.GetCrateSeries();
			_skin = Item.GetSkin();
			SkinWear = inst.GetSkinWear();

			Craftable = inst.Craftable;
			Tradable = inst.Tradable;
		}

		public override string ToString()
		{
			string res = Item.ToString(Quality, Killstreak);
			if (Unusual != null)
			{
				res += " (" + Unusual.ToString() + ")";
			}
			else if (IsCrate)
			{
				res += " No. " + CrateSeries.Value.ToString();
			}
			else if (Skin != null)
			{
				res += " (" + SkinWear.Value.ToReadableString() + ")";
			}

			return res;
		}
	}
}