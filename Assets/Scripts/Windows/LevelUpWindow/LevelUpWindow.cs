using ArtworkGames.DiceValley.Data.Public;
using ArtworkGames.DiceValley.Signals;
using ArtworkGames.Windows;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using MessagePipe;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using VContainer.Unity;

namespace ArtworkGames.DiceValley.Windows.LevelUpWindow
{
	public class LevelUpWindowParams : BaseWindowParams
	{
		public int Level;
		public LevelPublicSchema LevelPublicSchema;
		public Action OnConfirm;
	}

	public class LevelUpWindow : BaseWindow<LevelUpWindowParams>
	{
		public static string PrefabName = "LevelUpWindow";

		[Space]
		[SerializeField] private Transform _levelStar;
		[SerializeField] private TMP_Text _levelValue;
		[Space]
		[SerializeField] private GameObject _sourceRewardItem;
		[Space]
		[SerializeField] private Button _confirmButton;

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
			_levelStar.localScale = Vector3.zero;
			_levelValue.text = Params.Level.ToString();

			_confirmButton.gameObject.SetActive(false);

			_sourceRewardItem.SetActive(false);
			CreateItems();

			ShowItems();
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
			for (int i = 0; i < Params.LevelPublicSchema.Reward.Count; i++)
			{
				GameObject itemObject = _diContainer.Instantiate(_sourceRewardItem, _sourceRewardItem.transform.parent, false);
				itemObject.SetActive(true);

				RewardItem item = itemObject.GetComponent<RewardItem>();
				item.PublicSchema = _itemsPublicModel.GetItem(Params.LevelPublicSchema.Reward[i].Key);
				item.Count = Params.LevelPublicSchema.Reward[i].Value;
				items.Add(item);

				item.Content.localScale = Vector3.zero;
			}
		}

		private async void ShowItems()
		{
			_levelStar.DOScale(1.0f, 0.5f)
				.SetEase(Ease.OutBack);

			await UniTask.WaitForSeconds(0.2f);

			for (int i = 0; i < items.Count; i++)
			{
				items[i].Content.DOScale(Vector3.one, 0.5f)
					.SetEase(Ease.OutBack);

				await UniTask.WaitForSeconds(0.2f);
			}
			await UniTask.WaitForSeconds(0.3f);

			_confirmButton.gameObject.SetActive(true);
		}

		public async void OnConfirmButtonClick()
		{
			_confirmButton.onClick.RemoveAllListeners();
			_confirmButton.gameObject.SetActive(false);

			Vector3[] positions = new Vector3[Params.LevelPublicSchema.Reward.Count];
			for (int i = 0; i < Params.LevelPublicSchema.Reward.Count; i++)
			{
				positions[i] = items[i].RewardPoint.position;
			}
			_rewardPublisher.Publish(new RewardSignal(Params.LevelPublicSchema.Reward)
			{
				Type = RewardType.FlyingRewards,
				Positions = positions,
				ByPiece = true
			});

			Params.OnConfirm?.Invoke();

			await UniTask.WaitForSeconds(0.5f);

			CloseWindow();
		}
	}
}
