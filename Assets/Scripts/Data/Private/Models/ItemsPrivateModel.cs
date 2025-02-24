using ArtworkGames.DiceValley.Data.Public;
using System.Collections.Generic;
using System.Linq;

namespace ArtworkGames.DiceValley.Data.Private
{
	public class ItemsPrivateModel : IPrivateModel
	{
		private List<ItemPrivateSchema> _items = new List<ItemPrivateSchema>();

		public string Name => "items";

		public bool IsDirty { get; private set; }

		public void Init(PrivateData data)
		{
			_items = data.items;
		}

		public void ResetDirty()
		{
			IsDirty = false;
		}

		public object GetData()
		{
			return _items;
		}

		public ItemPrivateSchema GetItem(string itemId)
		{
			return _items.FirstOrDefault(item => item.id == itemId);
		}

		private ItemPrivateSchema AddItem(string itemId)
		{
			ItemPrivateSchema item = new ItemPrivateSchema()
			{
				id = itemId,
				count = 0,
				pending = 0
			};
			_items.Add(item);
			IsDirty = true;

			return item;
		}

		private void RemoveItem(string itemId)
		{
			ItemPrivateSchema item = GetItem(itemId);
			if (item != default)
			{
				_items.Remove(item);
				IsDirty = true;
			}
		}

		public void AddItemCount(string itemId, int count)
		{
			ItemPrivateSchema item = GetItem(itemId);
			if (item == default)
			{
				item = AddItem(itemId);
			}

			int oldCount = item.count;
			item.count += count;
			if (item.count < 0) item.count = 0;
			if (oldCount != item.count) IsDirty = true;

			if ((item.count == 0) && (item.pending == 0))
			{
				RemoveItem(itemId);
			}
		}

		public void AddItemPending(string itemId, int pending)
		{
			ItemPrivateSchema item = GetItem(itemId);
			if (item == default)
			{
				item = AddItem(itemId);
			}

			int oldPending = item.pending;
			item.pending += pending;
			if (item.pending < 0) item.pending = 0;
			if (oldPending != item.pending) IsDirty = true;

			if ((item.count == 0) && (item.pending == 0))
			{
				RemoveItem(itemId);
			}
		}

		public ItemPrivateSchema[] GetAllPendingItems()
		{
			return _items.Where(i => i.pending > 0).ToArray();
		}
	}
}
