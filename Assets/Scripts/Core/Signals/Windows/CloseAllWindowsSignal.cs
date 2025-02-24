namespace ArtworkGames.Signals
{
	public class CloseAllWindowsSignal
	{
		public bool Immediately;

		public CloseAllWindowsSignal(bool immediately = false)
		{
			Immediately = immediately;
		}
	}
}
