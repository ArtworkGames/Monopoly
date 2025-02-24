using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ArtworkGames.DiceValley.Data.Public
{
	[Serializable]
	public class PublicData
	{
		public ProductPublicSchema[] products;
		public ItemPublicSchema[] items;
		public LevelPublicSchema[] levels;
		public BuildingPublicSchema[] buildings;
	}

	public enum ProductType
	{
		Bucks,
		Coins,
		Energy
	}

	[Serializable]
	public class ProductPublicSchema
	{
		public string id;
		public ProductType type;
		public string items;
		public float realPrice;

		[JsonIgnore] public List<KeyValuePair<string, int>> Items;

		[OnDeserialized]
		private void OnDeserialized(StreamingContext context)
		{
			Items = JsonTools.ToStringIntPairs(items);
		}
	}

	public enum ItemType
	{
		System,
		Food
	}

	public class SystemItemName
	{
		public static string Bucks = "bucks";
		public static string Coins = "coins";
		public static string Energy = "energy";
		public static string Xp = "xp";
	}

	[Serializable]
	public class ItemPublicSchema
	{
		public string id;
		public ItemType type;
		public string titleKey;
		public string descKey;
	}

	[Serializable]
	public class LevelPublicSchema
	{
		public int level;
		public int xp;
		public string reward;

		[JsonIgnore]
		public List<KeyValuePair<string, int>> Reward;

		[OnDeserialized]
		private void OnDeserialized(StreamingContext context)
		{
			Reward = JsonTools.ToStringIntPairs(reward);
		}
	}

	[Serializable]
	public class BuildingPublicSchema
	{
		public string id;
		public string titleKey;
		public string descrKey;
	}
}
