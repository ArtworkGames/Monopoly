using ArtworkGames.Initialization;

namespace ArtworkGames.Signals
{
	public class LocaleChangedSignal
	{
		public string LocaleName;

		public LocaleChangedSignal(string localeName)
		{
			LocaleName = localeName;
		}
	}
}