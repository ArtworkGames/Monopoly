namespace ArtworkGames.DiceValley.Signals
{
	public class StockItemChangedSignal
	{
		public string ItemId;

		public StockItemChangedSignal(string itemId)
		{
			ItemId = itemId;
		}
	}
}