using ArtworkGames.DiceValley.Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VContainer;

namespace ArtworkGames.DiceValley.UI.Rewards
{
	public class RewardTarget : MonoBehaviour
	{
		[Serializable]
		public class CustomFlyingReward
		{
			public string itemId;
			public string itemType;
			public GameObject prefab;
		}

		[SerializeField] private List<string> _itemIds;
		[SerializeField] private List<string> _itemTypes;
		[Space]
		[SerializeField] private GameObject _defaultFlyingReward;
		[SerializeField] private CustomFlyingReward[] _customFlyingRewards;

		public bool IsUniversal => (_itemIds.Count == 0) && (_itemTypes.Count == 0);
		public List<string> ItemIds => _itemIds;
		public List<string> ItemTypes => _itemTypes;

		private IObjectResolver _diContainer;
		private RewardManager _rewardManager;

		[Inject]
		public void Construct(
			IObjectResolver diContainer,
			RewardManager rewardManager)
		{
			_diContainer = diContainer;
			_rewardManager = rewardManager;
		}

		private void OnEnable()
		{
			_rewardManager.RegisterTarget(this);
		}

		private void OnDisable()
		{
			if (_rewardManager == null)
				return;

			_rewardManager.UnregisterTarget(this);
		}

		public GameObject GetFlyingReward(string itemId, string itemType)
		{
			CustomFlyingReward prefabData = default;
			if (_customFlyingRewards.Length > 0)
			{
				prefabData = _customFlyingRewards.FirstOrDefault(p => p.itemId.Equals(itemId));
				if (prefabData == default)
				{
					prefabData = _customFlyingRewards.FirstOrDefault(p => p.itemType.Equals(itemType));
				}
			}

			GameObject prefab = null;
			if (prefabData != default)
			{
				prefab = prefabData.prefab;
			}
			else
			{
				prefab = _defaultFlyingReward;
			}

			if (prefab == null)
			{
				Debug.LogError($"No flying reward found for \"{itemId}\" item.");
			}

			return prefab;
		}
	}
}
