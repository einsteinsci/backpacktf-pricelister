﻿using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BackpackTFPriceLister
{
	public static class ClassifiedsScraper
	{
		public static List<ClassifiedsListing> GetClassifieds(Item item, Quality quality, 
			bool craftable = true, bool tradable = true)
		{
			string url = "http://backpack.tf/classifieds?item=";
			url += item.ImproperName;
			url += "&quality=" + ((int)quality).ToString();
			url += "&tradable=" + (tradable ? 1 : 0).ToString();
			url += "&craftable=" + (craftable ? 1 : 0).ToString();

			Logger.Log("Downloading listings from " + url + "...", MessageType.Debug);
			WebClient client = new WebClient();
			string html = client.DownloadString(url);
			Logger.Log("  Download complete.", MessageType.Debug);

			Logger.Log("Scraping listings from HTML...", MessageType.Debug);
			HtmlDocument doc = new HtmlDocument();
			doc.LoadHtml(html);

			List<ClassifiedsListing> results = new List<ClassifiedsListing>();

			HtmlNode root = doc.DocumentNode;
			HtmlNode sellOrderRoot = root.Descendants("ul").Where((n) => n.Attributes.Contains("class") && 
				n.Attributes["class"].Value == "media-list").FirstOrDefault();

			if (sellOrderRoot == null)
			{
				Logger.Log("  No sell orders found.", MessageType.Debug);
			}
			else
			{
				List<HtmlNode> sellOrderBases = sellOrderRoot.Descendants("li").ToList();

				foreach (HtmlNode sob in sellOrderBases)
				{
					HtmlNode sellData = sob.Descendants("li").FirstOrDefault();

					if (sellData == null)
					{
						continue;
					}

					string sellItemName = sellData.Attributes["data-name"].Value; // not really necessary
					string sellTradable = sellData.Attributes["data-tradable"].Value; // or this
					string sellCraftable = sellData.Attributes["data-craftable"].Value; // or this
					string sellQuality = sellData.Attributes["data-quality"].Value; // or even this
					string sellComment = sellData.Attributes["data-listing-comment"].Value;
					string sellPrice = sellData.Attributes["data-listing-price"].Value;
					string sellLevel = sellData.Attributes["data-level"].Value;
					string sellID = sellData.Attributes["data-id"].Value;
					string sellerSteamID64 = sellData.Attributes["data-listing-steamid"].Value;
					string sellOfferUrl = sellData.Attributes["data-listing-offers-url"].Value;
					string sellOriginalID = sellData.Attributes["data-original-id"].Value;
					string sellCustomName = sellData.Attributes["data-custom-name"].Value;
					string sellCustomDesc = sellData.Attributes["data-custom-desc"].Value;

					long id = long.Parse(sellID); // confusing syntax down here -v
					long? originalID = sellOriginalID != null ? new long?(long.Parse(sellOriginalID)) : null;
					Price price = Price.ParseFancy(sellPrice);
					int level = int.Parse(sellLevel);

					ItemInstance instance = new ItemInstance(item, id, level, quality, craftable,
						sellCustomName, sellCustomDesc, originalID, tradable);
					ClassifiedsListing listing = new ClassifiedsListing(instance, price, sellerSteamID64, sellOfferUrl, sellComment, false);

					results.Add(listing);
				}
				Logger.Log("  Sell order scrape complete.", MessageType.Debug);
			}

			HtmlNode buyOrderRoot = root.Descendants("ul").Where((n) => n.Attributes.Contains("class") &&
				n.Attributes["class"].Value == "media-list").LastOrDefault();

			if (buyOrderRoot == null)
			{
				Logger.Log("  No buy orders found.", MessageType.Debug);
			}
			else
			{
				List<HtmlNode> buyOrderBases = sellOrderRoot.Descendants("li").ToList();

				foreach (HtmlNode bob in buyOrderBases)
				{
					HtmlNode buyData = bob.Descendants("li").FirstOrDefault();

					if (buyData == null)
					{
						continue;
					}

					string buyItemName = buyData.Attributes["data-name"].Value; // not really necessary
					string buyTradable = buyData.Attributes["data-tradable"].Value; // or this
					string buyCraftable = buyData.Attributes["data-craftable"].Value; // or this
					string buyQuality = buyData.Attributes["data-quality"].Value; // or even this
					string buyComment = buyData.Attributes["data-listing-comment"].Value;
					string buyPrice = buyData.Attributes["data-listing-price"].Value;
					string buyLevel = buyData.Attributes["data-level"].Value;
					string buyID = buyData.Attributes["data-id"].Value;
					string buyerSteamID64 = buyData.Attributes["data-listing-steamid"].Value;
					string buyOfferUrl = buyData.Attributes["data-listing-offers-url"].Value;
					string buyOriginalID = buyData.Attributes["data-original-id"].Value;
					string buyCustomName = buyData.Attributes["data-custom-name"].Value;
					string buyCustomDesc = buyData.Attributes["data-custom-desc"].Value;

					long id = long.Parse(buyID); // confusing syntax down here -v
					long? originalID = buyOriginalID != null ? new long?(long.Parse(buyOriginalID)) : null;
					Price price = Price.ParseFancy(buyPrice);
					int level = int.Parse(buyLevel);

					ItemInstance instance = new ItemInstance(item, id, level, quality, craftable,
						buyCustomName, buyCustomDesc, originalID, tradable);
					ClassifiedsListing listing = new ClassifiedsListing(instance, price, buyerSteamID64, buyOfferUrl, buyComment, true);

					results.Add(listing);
				}

				Logger.Log("  Buy order scrape complete.", MessageType.Debug);
			}

			return results;
		}
	}
}