namespace ArtworkGames.DiceValley.Signals
{
	public class StockItemAddedSignal
	{
		public string ItemId;

		public StockItemAddedSignal(string itemId)
		{
			ItemId = itemId;
		}
	}
}