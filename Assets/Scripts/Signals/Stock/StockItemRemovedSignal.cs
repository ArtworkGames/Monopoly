namespace ArtworkGames.DiceValley.Signals
{
	public class StockItemRemovedSignal
	{
		public string ItemId;

		public StockItemRemovedSignal(string itemId)
		{
			ItemId = itemId;
		}
	}
}