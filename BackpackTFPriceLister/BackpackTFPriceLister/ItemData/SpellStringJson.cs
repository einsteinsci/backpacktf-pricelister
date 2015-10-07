﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackpackTFPriceLister.ItemData
{
	[JsonObject(MemberSerialization = MemberSerialization.OptOut)]
	public class SpellStringJson
	{
		public int index
		{ get; set; }

		public string @string
		{ get; set; }
	}
}
