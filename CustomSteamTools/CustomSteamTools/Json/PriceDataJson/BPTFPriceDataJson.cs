﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomSteamTools.Json.PriceDataJson
{
	[JsonObject(MemberSerialization = MemberSerialization.OptOut)]
	public class BpTfPriceDataJson
	{
		public BPTFPriceDataResponseJson response
		{ get; set; }
	}
}
