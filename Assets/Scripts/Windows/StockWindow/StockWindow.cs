using ArtworkGames.DiceValley.Data.Public;
using ArtworkGames.DiceValley.Managers;
using ArtworkGames.DiceValley.Signals;
using ArtworkGames.Windows;
using MessagePipe;
using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace ArtworkGames.DiceValley.Windows.StockWindow
{
	public class StockWindowParams : BaseWindowParams
	{
	}

	public class StockWindow : BaseWindow<StockWindowParams>, IDisposable
	{
		public static string PrefabName = "StockWindow";

		[Space]
		[SerializeField] private GameObject _sourceTabButton;
		[SerializeField] private GameObject _sourceStockItem;

		private IObjectResolver _diContainer;
		private StockManager _stockManager;
		private ItemsPublicModel _itemsPublicModel;
		private IDisposable _subscriptions;

		private List<StockTabButton> tabs;
		private List<StockItem> items;

		private bool isShown;
		private ItemType selectedItemType = ItemType.Food;

		[Inject]
		public void Construct(
			IObjectResolver diContainer,
			StockManager stockManager,
			PublicDataProvider publicDataProvider,
			ISubscriber<StockItemAddedSignal> itemAddedSubscriber,
			ISubscriber<StockItemRemovedSignal> itemRemovedSubscriber)
		{
			_diContainer = diContainer;
			_stockManager = stockManager;
			_itemsPublicModel = publicDataProvider.Get<ItemsPublicModel>();

			DisposableBagBuilder bag = DisposableBag.CreateBuilder();
			itemAddedSubscriber.Subscribe(OnItemAdded).AddTo(bag);
			itemRemovedSubscriber.Subscribe(OnItemRemoved).AddTo(bag);
			_subscriptions = bag.Build();
		}

		public void Dispose()
		{
			_subscriptions.Dispose();
		}

		protected override void BeforeOpen()
		{
			_sourceTabButton.SetActive(false);
			_sourceStockItem.SetActive(false);

			if (tabs == null)
			{
				ItemType[] itemTypes = new ItemType[] { ItemType.Food, ItemType.System };

				tabs = new List<StockTabButton>();
				for (int i = 0; i < itemTypes.Length; i++)
				{
					GameObject tabObject = _diContainer.Instantiate(_sourceTabButton, _sourceTabButton.transform.parent, false);
					tabObject.SetActive(true);

					StockTabButton tab = tabObject.GetComponent<StockTabButton>();
					tab.itemType = itemTypes[i];
					tab.OnClick += OnTabClick;
					tabs.Add(tab);
				}
			}

			UpdateItems(selectedItemType);
			isShown = true;
		}

		protected override void AfterClose()
		{
			isShown = false;
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
			items = new List<StockItem>();
		}

		private void UpdateItems(ItemType type)
		{
			ClearItems();

			selectedItemType = type;

			(ItemPublicSchema, int)[] itemsData = _stockManager.GetItems(type, true);

			for (int i = 0; i < itemsData.Length; i++)
			{
				GameObject itemObject = _diContainer.Instantiate(_sourceStockItem, _sourceStockItem.transform.parent, false);
				itemObject.SetActive(true);

				StockItem item = itemObject.GetComponent<StockItem>();
				item.PublicSchema = itemsData[i].Item1;
				item.Count = itemsData[i].Item2;
				item.OnClick += OnItemClick;
				items.Add(item);
			}
		}

		private void OnItemAdded(StockItemAddedSignal signal)
		{
			if (!isShown) return;

			ItemPublicSchema itemPublicSchema = _itemsPublicModel.GetItem(signal.ItemId);
			if (itemPublicSchema.type == selectedItemType)
			{
				UpdateItems(selectedItemType);
			}
		}

		private void OnItemRemoved(StockItemRemovedSignal signal)
		{
			if (!isShown) return;

			ItemPublicSchema itemPublicSchema = _itemsPublicModel.GetItem(signal.ItemId);
			if (itemPublicSchema.type == selectedItemType)
			{
				UpdateItems(selectedItemType);
			}
		}

		private void OnTabClick(StockTabButton tab)
		{
			UpdateItems(tab.itemType);
		}

		private void OnItemClick(StockItem item)
		{
			//CloseWindow();
		}
	}
}
