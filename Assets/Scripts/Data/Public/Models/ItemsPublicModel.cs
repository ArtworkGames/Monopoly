using System.Linq;

namespace ArtworkGames.DiceValley.Data.Public
{
	public class ItemsPublicModel : IPublicModel
	{
		private ItemPublicSchema[] _items;

		public string Name => "items";

		public void Init(PublicData data)
		{
			_items = data.items;
		}

		public ItemPublicSchema GetItem(string id)
		{
			return _items.FirstOrDefault(item => item.id == id);
		}

		public ItemPublicSchema[] GetItems(ItemType type)
		{
			return _items.Where(i => i.type == type).ToArray();
		}
	}
}
