﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackpackTFPriceLister.ItemDataJson
{
	[JsonObject(MemberSerialization = MemberSerialization.OptOut)]
	public class AttributeJson
	{
		// Localized name
		public string name
		{ get; set; }

		// Attribute ID
		public int defindex
		{ get; set; }

		// Unlocalized name
		public string attribute_class
		{ get; set; }

		public double minvalue
		{ get; set; }

		public double maxvalue
		{ get; set; }

		// Format string for description
		public string description_string
		{ get; set; }

		public string description_format
		{ get; set; }

		// Positive or negative (or neutral)
		public string effect_type
		{ get; set; }

		public bool hidden
		{ get; set; }

		public bool stored_as_integer
		{ get; set; }

		public override string ToString()
		{
			if (description_string == null)
			{
				return "#" + defindex + ": " + attribute_class;
			}

			string exampleDesc = description_string.Replace("%s1", "X");

			return "#" + defindex + ": " + exampleDesc + " [" + attribute_class + "]";
		}
	}

	public static class AttributeEffectTypes
	{
		public const string POSITIVE = "positive";
		public const string NEUTRAL = "neutral";
		public const string NEGATIVE = "negative";

		public static AttributeEffectType Parse(string aet)
		{
			if (aet == POSITIVE)
			{
				return AttributeEffectType.Positive;
			}
			if (aet == NEUTRAL)
			{
				return AttributeEffectType.Neutral;
			}

			return AttributeEffectType.Negative;
		}

		public static string GetEffectType(this AttributeEffectType t)
		{
			switch (t)
			{
				case AttributeEffectType.Negative:
					return NEGATIVE;
				case AttributeEffectType.Neutral:
					return NEUTRAL;
				case AttributeEffectType.Positive:
					return POSITIVE;
				default:
					return "ERR";
			}
		}
	}
}
