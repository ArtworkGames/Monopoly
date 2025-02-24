namespace ArtworkGames.Signals
{
	public class SceneShownSignal
	{
		public string SceneName;

		public SceneShownSignal(string sceneName)
		{
			SceneName = sceneName;
		}
	}
}