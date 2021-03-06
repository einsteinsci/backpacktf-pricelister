﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomSteamTools.Json.BackpackDataJson
{
	[JsonObject(MemberSerialization = MemberSerialization.OptOut)]
	public class AppliedItemInstanceAttributeJson
	{
		public int defindex
		{ get; set; }

		public object value
		{ get; set; }

		public double float_value
		{ get; set; }

		public AppliedAttributeSteamIDJson account_info
		{ get; set; }

		public override string ToString()
		{
			if (account_info != null)
			{
				return "#" + defindex + ": " + account_info;
			}

			return "#" + defindex + ": " + float_value;
		}
	}

	[JsonObject(MemberSerialization = MemberSerialization.OptOut)]
	public class AppliedAttributeSteamIDJson
	{
		public ulong steamid
		{ get; set; }

		public string personaname
		{ get; set; }

		public override string ToString()
		{
			return personaname + " (#" + steamid + ")";
		}
	}
}
