using ArtworkGames.DiceValley.Data.Public;
using ArtworkGames.DiceValley.Managers;
using ArtworkGames.DiceValley.Signals;
using MessagePipe;
using System;
using TMPro;
using UnityEngine;
using VContainer;

namespace ArtworkGames.DiceValley.UI.HUD
{
	public class ItemPanel : MonoBehaviour, IDisposable
	{
		[SerializeField] private string _itemId;
		[SerializeField] private TMP_Text _value;

		private StockManager _stockManager;
		private IDisposable _subscriptions;

		[Inject]
		public void Construct(
			StockManager stockManager,
			ISubscriber<StockItemAddedSignal> itemAddedSubscriber,
			ISubscriber<StockItemChangedSignal> itemChangedSubscriber,
			ISubscriber<StockItemRemovedSignal> itemRemovedSubscriber)
		{
			_stockManager = stockManager;

			DisposableBagBuilder bag = DisposableBag.CreateBuilder();
			itemAddedSubscriber.Subscribe(OnItemAdded).AddTo(bag);
			itemChangedSubscriber.Subscribe(OnItemChanged).AddTo(bag);
			itemRemovedSubscriber.Subscribe(OnItemRemoved).AddTo(bag);
			_subscriptions = bag.Build();

			UpdateComponents();
		}

		public void Dispose()
		{
			_subscriptions.Dispose();
		}

		private void UpdateComponents()
		{
			int itemCount = _stockManager.GetItemCount(_itemId);
			_value.text = itemCount.ToString();
		}

		private void OnItemAdded(StockItemAddedSignal signal)
		{
			if (signal.ItemId.Equals(_itemId))
			{
				UpdateComponents();
			}
		}

		private void OnItemChanged(StockItemChangedSignal signal)
		{
			if (signal.ItemId.Equals(_itemId))
			{
				UpdateComponents();
			}
		}

		private void OnItemRemoved(StockItemRemovedSignal signal)
		{
			if (signal.ItemId.Equals(_itemId))
			{
				UpdateComponents();
			}
		}
	}
}
