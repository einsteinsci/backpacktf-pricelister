﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackpackTFPriceLister.ItemData
{
	[JsonObject(MemberSerialization = MemberSerialization.OptOut)]
	public class StrangeLevelJson
	{
		public int level
		{ get; set; }

		public int required_score
		{ get; set; }

		public string name
		{ get; set; }
	}
}
