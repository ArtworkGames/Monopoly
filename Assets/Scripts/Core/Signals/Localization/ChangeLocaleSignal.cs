using ArtworkGames.Initialization;

namespace ArtworkGames.Signals
{
	public class ChangeLocaleSignal
	{
		public string LocaleName;

		public ChangeLocaleSignal(string localeName)
		{
			LocaleName = localeName;
		}
	}
}