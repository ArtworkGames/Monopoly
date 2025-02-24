namespace ArtworkGames.Signals
{
	public class WindowOpenedSignal
	{
		public string WindowName { get; private set; }

		public WindowOpenedSignal(string windowName)
		{
			WindowName = windowName;
		}
	}
}