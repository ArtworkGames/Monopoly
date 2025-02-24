using ArtworkGames.DiceValley.Data.Public;
using ArtworkGames.DiceValley.Managers;
using ArtworkGames.DiceValley.Signals;
using DG.Tweening;
using MessagePipe;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace ArtworkGames.DiceValley.UI.HUD
{
	public class LevelPanel : MonoBehaviour, IDisposable
	{
		[SerializeField] private TMP_Text _levelValue;
		[SerializeField] private TMP_Text _xpValue;
		[SerializeField] private Slider _progress;

		private StockManager _stockManager;
		private LevelManager _levelManager;
		private IDisposable _subscriptions;

		private Tween progressTween;

		[Inject]
		public void Construct(
			StockManager stockManager,
			LevelManager levelManager,
			ISubscriber<StockItemAddedSignal> itemAddedSubscriber,
			ISubscriber<StockItemChangedSignal> itemChangedSubscriber,
			ISubscriber<StockItemRemovedSignal> itemRemovedSubscriber,
			ISubscriber<LevelUpSignal> levelUpSubscriber)
		{
			_stockManager = stockManager;
			_levelManager = levelManager;

			DisposableBagBuilder bag = DisposableBag.CreateBuilder();
			itemAddedSubscriber.Subscribe(OnItemAdded).AddTo(bag);
			itemChangedSubscriber.Subscribe(OnItemChanged).AddTo(bag);
			itemRemovedSubscriber.Subscribe(OnItemRemoved).AddTo(bag);
			levelUpSubscriber.Subscribe(OnLevelUp).AddTo(bag);
			_subscriptions = bag.Build();

			UpdateComponents(false);
		}

		public void Dispose()
		{
			_subscriptions.Dispose();
		}

		private void UpdateComponents(bool animated)
		{
			int xp = _stockManager.GetItemCount(SystemItemName.Xp);
			float progress = (float)xp / (float)_levelManager.LevelPublicSchema.xp;
			progress = Mathf.Clamp01(progress);

			_levelValue.text = _levelManager.Level.ToString();
			_xpValue.text = $"{xp}/{_levelManager.LevelPublicSchema.xp}";

			progressTween?.Kill();

			if (animated)
			{
				progressTween = _progress.DOValue(progress, 0.5f)
					.SetEase(Ease.OutCubic);
			}
			else
			{
				_progress.value = progress;
			}
		}

		private void OnItemAdded(StockItemAddedSignal signal)
		{
			if (signal.ItemId.Equals(SystemItemName.Xp))
			{
				UpdateComponents(true);
			}
		}

		private void OnItemChanged(StockItemChangedSignal signal)
		{
			if (signal.ItemId.Equals(SystemItemName.Xp))
			{
				UpdateComponents(true);
			}
		}

		private void OnItemRemoved(StockItemRemovedSignal signal)
		{
			if (signal.ItemId.Equals(SystemItemName.Xp))
			{
				UpdateComponents(true);
			}
		}

		private void OnLevelUp(LevelUpSignal signal)
		{
			UpdateComponents(false);
		}
	}
}
