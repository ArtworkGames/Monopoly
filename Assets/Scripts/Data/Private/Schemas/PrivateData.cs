using ArtworkGames.DiceValley.Signals;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ArtworkGames.DiceValley.Data.Private
{
	[Serializable]
	public class PrivateData
	{
		public PlayerPrivateSchema player;
		public List<ItemPrivateSchema> items;
		public List<RewardPrivateSchema> rewards;
		public List<BuildingPrivateSchema> buildings;
	}

	[Serializable]
	public class PlayerPrivateSchema
	{
		public int level = 1;
	}

	[Serializable]
	public class RewardPrivateSchema
	{
		public string items;
		public int type;
	}

	[Serializable]
	public class ItemPrivateSchema
	{
		public string id;
		public int count;
		public int pending;
	}

	[Serializable]
	public class BuildingPrivateSchema
	{
		public string id;
	}
}
