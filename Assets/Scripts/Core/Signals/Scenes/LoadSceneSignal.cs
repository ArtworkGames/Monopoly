namespace ArtworkGames.Signals
{
	public class LoadSceneSignal
	{
		public string SceneName;

		public LoadSceneSignal(string sceneName)
		{
			SceneName = sceneName;
		}
	}
}