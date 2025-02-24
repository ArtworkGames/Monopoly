using ArtworkGames.DiceValley.Data.Private;
using ArtworkGames.DiceValley.Data.Public;
using ArtworkGames.DiceValley.Signals;
using ArtworkGames.DiceValley.Windows.NotEnoughMoneyWindow;
using ArtworkGames.Initialization;
using ArtworkGames.Signals;
using ArtworkGames.Windows;
using Cysharp.Threading.Tasks;
using MessagePipe;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ArtworkGames.DiceValley.Managers
{
	public class StockManager : BaseInitializable
	{
		private PublicDataProvider _publicDataProvider;
		private PrivateDataProvider _privateDataProvider;
		private IPublisher<StockItemAddedSignal> _itemAddedPublisher;
		private IPublisher<StockItemRemovedSignal> _itemRemovedPublisher;
		private IPublisher<StockItemChangedSignal> _itemChangedPublisher;
		private IPublisher<OpenWindowSignal> _openWindowPublisher;

		private ItemsPublicModel _itemsPublicModel;
		private ItemsPrivateModel _itemsPrivateModel;

		protected override List<Type> Dependencies => new List<Type> { typeof(PublicDataProvider), typeof(PrivateDataProvider) };

		public StockManager(
			IPublisher<RegisterInitializableSignal> registerPublisher,
			PublicDataProvider publicDataProvider,
			PrivateDataProvider privateDataProvider,
			IPublisher<StockItemAddedSignal> itemAddedPublisher,
			IPublisher<StockItemRemovedSignal> itemRemovedPublisher,
			IPublisher<StockItemChangedSignal> itemChangedPublisher,
			IPublisher<OpenWindowSignal> openWindowPublisher) : base(registerPublisher)
		{
			_publicDataProvider = publicDataProvider;
			_privateDataProvider = privateDataProvider;

			_itemAddedPublisher = itemAddedPublisher;
			_itemRemovedPublisher = itemRemovedPublisher;
			_itemChangedPublisher = itemChangedPublisher;
			_openWindowPublisher = openWindowPublisher;
		}

		public override async UniTask InitializeAsync()
		{
			_itemsPublicModel = _publicDataProvider.Get<ItemsPublicModel>();
			_itemsPrivateModel = _privateDataProvider.Get<ItemsPrivateModel>();

			ConfirmAllPendingItems();
		}

		private void Save()
		{
			_privateDataProvider.SaveModel<ItemsPrivateModel>();
		}

		#region Get items
		public int GetItemCount(string id)
		{
			ItemPrivateSchema itemPrivateSchema = _itemsPrivateModel.GetItem(id);
			if (itemPrivateSchema != default)
			{
				return itemPrivateSchema.count;
			}
			return 0;
		}

		public bool HasItemCount(string id, int count)
		{
			int itemCount = GetItemCount(id);
			return itemCount >= count;
		}

		public (ItemPublicSchema, int) GetItem(string id)
		{
			ItemPublicSchema itemPublicSchema = _itemsPublicModel.GetItem(id);
			ItemPrivateSchema itemPrivateSchema = _itemsPrivateModel.GetItem(id);
			if (itemPrivateSchema != default)
			{
				return (itemPublicSchema, itemPrivateSchema.count);
			}
			return (itemPublicSchema, 0);
		}

		public (ItemPublicSchema, int)[] GetItems(ItemType type, bool includeZeroCount = false)
		{
			List<(ItemPublicSchema, int)> items = new List<(ItemPublicSchema, int)>();
			ItemPublicSchema[] itemPublicSchemas = _itemsPublicModel.GetItems(type);

			for (int i = 0; i < itemPublicSchemas.Length; i++)
			{
				ItemPrivateSchema itemPrivateSchema = _itemsPrivateModel.GetItem(itemPublicSchemas[i].id);
				if (itemPrivateSchema != default)
				{
					items.Add((itemPublicSchemas[i], itemPrivateSchema.count));
				}
				else if (includeZeroCount)
				{
					items.Add((itemPublicSchemas[i], 0));
				}
			}
			return items.ToArray();
		}
		#endregion

		#region Add items
		public void AddItem(string id, int count, bool silent = false)
		{
			if (count > 0)
			{
				bool itemChanged = AddItemCount(id, count, silent);
				if (itemChanged) Save();
			}
		}

		public void AddItem(KeyValuePair<string, int> item, bool silent = false)
		{
			if (item.Value > 0)
			{
				bool itemChanged = AddItemCount(item.Key, item.Value, silent);
				if (itemChanged) Save();
			}
		}

		public void AddItems(List<KeyValuePair<string, int>> items, bool silent = false)
		{
			bool itemChanged = false;
			for (int i = 0; i < items.Count; i++)
			{
				if (items[i].Value > 0)
				{
					itemChanged |= AddItemCount(items[i].Key, items[i].Value, silent);
				}
			}
			if (itemChanged) Save();
		}

		public void TakeItem(string id, int count, bool silent = false)
		{
			if (count > 0)
			{
				bool itemChanged = AddItemCount(id, -count, silent);
				if (itemChanged) Save();
			}
		}

		public void TakeItem(KeyValuePair<string, int> item, bool silent = false)
		{
			if (item.Value > 0)
			{
				bool itemChanged = AddItemCount(item.Key, -item.Value, silent);
				if (itemChanged) Save();
			}
		}

		private bool AddItemCount(string id, int count, bool silent = false)
		{
			bool itemExists = false;
			int oldCount = 0;
			ItemPrivateSchema itemPrivateSchema = _itemsPrivateModel.GetItem(id);
			if ((itemPrivateSchema != default) && (itemPrivateSchema.count > 0))
			{
				itemExists = true;
				oldCount = itemPrivateSchema.count;
			}

			if ((count < 0) && (Mathf.Abs(count) > oldCount))
			{
				if (id.Equals(SystemItemName.Bucks) || id.Equals(SystemItemName.Coins) || id.Equals(SystemItemName.Energy))
				{
					_openWindowPublisher.Publish(new OpenWindowSignal(NotEnoughMoneyWindow.PrefabName)
					{
						Params = new NotEnoughMoneyWindowParams()
						{
							ItemId = id,
						},
						BehaviourType = typeof(PopUpWindowBehaviour)
					});
				}
				else
				{
					Debug.LogError($"Not enough \"{id}\" item in stock");
				}
				return false;
			}

			_itemsPrivateModel.AddItemCount(id, count);

			itemPrivateSchema = _itemsPrivateModel.GetItem(id);
			if ((itemPrivateSchema != default) && (itemPrivateSchema.count > 0))
			{
				if (!itemExists)
				{
					if (!silent) _itemAddedPublisher.Publish(new StockItemAddedSignal(id));
					return true;
				}
				else if (oldCount != itemPrivateSchema.count)
				{
					if (!silent) _itemChangedPublisher.Publish(new StockItemChangedSignal(id));
					return true;
				}
			}
			else if (itemExists && ((itemPrivateSchema == default) || (itemPrivateSchema.count == 0)))
			{
				if (!silent) _itemRemovedPublisher.Publish(new StockItemRemovedSignal(id));
				return true;
			}
			return false;
		}
		#endregion

		#region Add pending items
		public void AddPendingItem(string id, int pending)
		{
			if (pending > 0)
			{
				AddPendingCount(id, pending);
				Save();
			}
		}

		public void AddPendingItem(KeyValuePair<string, int> item)
		{
			if (item.Value > 0)
			{
				AddPendingCount(item.Key, item.Value);
				Save();
			}
		}

		public void AddPendingItems(List<KeyValuePair<string, int>> items)
		{
			bool itemChanged = false;
			for (int i = 0; i < items.Count; i++)
			{
				if (items[i].Value > 0)
				{
					AddPendingCount(items[i].Key, items[i].Value);
					itemChanged = true;
				}
			}
			if (itemChanged) Save();
		}

		private void AddPendingCount(string id, int pending)
		{
			_itemsPrivateModel.AddItemPending(id, pending);
		}
		#endregion

		#region Confirm pending items
		public void ConfirmPendingItem(string id, int pending, bool silent = false)
		{
			if (pending > 0)
			{
				bool itemChanged = ConfirmPendingCount(id, pending, silent);
				if (itemChanged) Save();
			}
		}

		public void ConfirmPendingItem(KeyValuePair<string, int> item, bool silent = false)
		{
			if (item.Value > 0)
			{
				bool itemChanged = ConfirmPendingCount(item.Key, item.Value, silent);
				if (itemChanged) Save();
			}
		}

		public void ConfirmPendingItems(List<KeyValuePair<string, int>> items, bool silent = false)
		{
			bool itemChanged = false;
			for (int i = 0; i < items.Count; i++)
			{
				if (items[i].Value > 0)
				{
					itemChanged |= ConfirmPendingCount(items[i].Key, items[i].Value, silent);
				}
			}
			if (itemChanged) Save();
		}

		private bool ConfirmPendingCount(string id, int pending, bool silent = false)
		{
			ItemPrivateSchema itemPrivateSchema = _itemsPrivateModel.GetItem(id);
			if ((itemPrivateSchema != default) && (itemPrivateSchema.pending >= pending))
			{
				AddPendingCount(id, -pending);
				AddItemCount(id, pending, silent);
				return true;
			}
			else
			{
				Debug.LogError($"Not enough pending \"{id}\" item in stock");
			}
			return false;
		}

		private void ConfirmAllPendingItems()
		{
			ItemPrivateSchema[] itemPrivateSchemas = _itemsPrivateModel.GetAllPendingItems();
			bool itemChanged = false;
			for (int i = 0; i < itemPrivateSchemas.Length; i++)
			{
				if (itemPrivateSchemas[i].pending > 0)
				{
					itemChanged |= ConfirmPendingCount(itemPrivateSchemas[i].id, itemPrivateSchemas[i].pending);
				}
			}
			if (itemChanged) Save();
		}
		#endregion
	}
}
