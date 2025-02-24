namespace ArtworkGames.Signals
{
	public class CloseWindowSignal
	{
		public string WindowName;
		public bool Immediately;

		public CloseWindowSignal(string windowName, bool immediately = false)
		{
			WindowName = windowName;
			Immediately = immediately;
		}
	}
}