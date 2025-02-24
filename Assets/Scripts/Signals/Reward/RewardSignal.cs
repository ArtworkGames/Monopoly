using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArtworkGames.DiceValley.Signals
{
	public enum RewardType
	{
		FlyingRewards,
		Chest,
		ItemWindow,
		ItemsListWindow,
		BestItemWindow
	}

	public class RewardSignal
	{
		public List<KeyValuePair<string, int>> Items;

		public RewardType Type = RewardType.FlyingRewards;
		public Vector3[] Positions;
		public bool ByPiece;
		public Type WindowBehaviourType;
		public bool OnlyConfirmPendingItems;

		public RewardSignal(List<KeyValuePair<string, int>> items)
		{
			Items = items;
		}

		public RewardSignal(string itemId, int count)
		{
			Items = new List<KeyValuePair<string, int>> { new KeyValuePair<string, int>(itemId, count) };
		}

		public RewardSignal(KeyValuePair<string, int> item)
		{
			Items = new List<KeyValuePair<string, int>> { item };
		}
	}
}
