using ArtworkGames.DiceValley.Data.Public;
using ArtworkGames.DiceValley.Signals;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ArtworkGames.DiceValley.Data.Private
{
	public class RewardsPrivateModel : IPrivateModel
	{
		private List<RewardPrivateSchema> _rewards = new List<RewardPrivateSchema>();

		public string Name => "rewards";

		public bool IsDirty { get; private set; }

		public void ResetDirty()
		{
			IsDirty = false;
		}

		public object GetData()
		{
			return _rewards;
		}

		public void Init(PrivateData data)
		{
			_rewards = data.rewards;
		}

		public int GetRewardsCount()
		{
			return _rewards.Count;
		}

		public RewardSignal GetReward(int idx)
		{
			if (idx >= _rewards.Count) return default;

			RewardSignal signal = new RewardSignal(JsonTools.ToStringIntPairs(_rewards[idx].items))
			{
				Type = (RewardType)_rewards[idx].type
			};
			return signal;
		}

		public void AddReward(RewardSignal signal)
		{
			string itemsStr = JsonTools.FromStringIntPairs(signal.Items);
			int typeInt = (int)signal.Type;

			RewardPrivateSchema reward = default;
			if (_rewards == null)
			{
				_rewards = new List<RewardPrivateSchema>();
			}
			else
			{
				reward = _rewards.FirstOrDefault(s => s.items.Equals(itemsStr) && (s.type == typeInt));
			}

			if (reward == default)
			{
				reward = new RewardPrivateSchema();

				reward.items = itemsStr;
				reward.type = typeInt;

				_rewards.Add(reward);
				IsDirty = true;
			}
		}

		public void RemoveReward(RewardSignal signal)
		{
			if (_rewards == null) return;

			string itemsStr = JsonTools.FromStringIntPairs(signal.Items);
			int typeInt = (int)signal.Type;

			RewardPrivateSchema reward = _rewards.FirstOrDefault(s => s.items.Equals(itemsStr) && (s.type == typeInt));
			if (reward != default)
			{
				_rewards.Remove(reward);
				IsDirty = true;
			}
		}
	}
}
