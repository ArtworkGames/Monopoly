using ArtworkGames.DiceValley.Data.Public;
using ArtworkGames.DiceValley.Signals;
using ArtworkGames.Windows;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using MessagePipe;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using VContainer.Unity;

namespace ArtworkGames.DiceValley.Windows.RewardChestWindow
{
	public class RewardChestWindowParams : BaseWindowParams
	{
		public List<KeyValuePair<string, int>> Items;
		public Action OnRewardTaken;
	}

	public class RewardChestWindow : BaseWindow<RewardChestWindowParams>
	{
		public static string PrefabName = "RewardChestWindow";

		[Space]
		[SerializeField] private GameObject _sourceRewardItem;
		[Space]
		[SerializeField] private GameObject _closedChest;
		[SerializeField] private GameObject _openedChest;
		[SerializeField] private Transform _rewardsPoint;
		[Space]
		[SerializeField] private Button _openButton;
		[SerializeField] private Button _takeButton;

		private IObjectResolver _diContainer;
		private ItemsPublicModel _itemsPublicModel;
		private IPublisher<RewardSignal> _rewardPublisher;

		private List<RewardItem> items;

		[Inject]
		public void Construct(
			IObjectResolver diContainer,
			PublicDataProvider publicDataProvider,
			IPublisher<RewardSignal> rewardPublisher)
		{
			_diContainer = diContainer;
			_itemsPublicModel = publicDataProvider.Get<ItemsPublicModel>();
			_rewardPublisher = rewardPublisher;
		}

		protected override void BeforeOpen()
		{
			_closedChest.SetActive(true);
			_openedChest.SetActive(false);

			_openButton.gameObject.SetActive(true);

			_takeButton.interactable = true;
			_takeButton.gameObject.SetActive(false);

			_sourceRewardItem.SetActive(false);
			CreateItems();
		}

		protected override void AfterClose()
		{
			ClearItems();
		}

		private void ClearItems()
		{
			if (items != null)
			{
				for (int i = 0; i < items.Count; i++)
				{
					Destroy(items[i].gameObject);
				}
			}
			items = new List<RewardItem>();
		}

		private void CreateItems()
		{
			items = new List<RewardItem>();
			for (int i = 0; i < Params.Items.Count; i++)
			{
				GameObject itemObject = _diContainer.Instantiate(_sourceRewardItem, _sourceRewardItem.transform.parent, false);
				itemObject.SetActive(true);

				RewardItem item = itemObject.GetComponent<RewardItem>();
				item.PublicSchema = _itemsPublicModel.GetItem(Params.Items[i].Key);
				item.Count = Params.Items[i].Value;
				items.Add(item);

				item.Content.localScale = Vector3.zero;
			}
		}

		public async void OnOpenButtonClick()
		{
			_openButton.gameObject.SetActive(false);

			_closedChest.SetActive(false);
			_openedChest.SetActive(true);

			for (int i = 0; i < items.Count; i++)
			{
				items[i].Content.position = _rewardsPoint.position;

				items[i].Content.DOLocalMove(Vector3.zero, 0.5f)
					.SetEase(Ease.OutCubic);
				items[i].Content.DOScale(Vector3.one, 0.5f)
					.SetEase(Ease.OutCubic);

				await UniTask.WaitForSeconds(0.2f);
			}
			await UniTask.WaitForSeconds(0.3f);

			_takeButton.gameObject.SetActive(true);
		}

		public async void OnTakeButtonClick()
		{
			_takeButton.interactable = false;

			Vector3[] positions = new Vector3[Params.Items.Count];
			for (int i = 0; i < Params.Items.Count; i++)
			{
				positions[i] = items[i].RewardPoint.position;
			}
			_rewardPublisher.Publish(new RewardSignal(Params.Items)
			{
				Type = RewardType.FlyingRewards,
				Positions = positions,
				ByPiece = true,
				//OnlyConfirmPendingItems = true
			});
			Params.OnRewardTaken?.Invoke();

			await UniTask.WaitForSeconds(0.5f);

			CloseWindow();
		}
	}
}
