namespace ArtworkGames.DiceValley.Data.Private
{
	public interface IPrivateModel
	{
		string Name { get; }
		bool IsDirty { get; }
		void ResetDirty();
		void Init(PrivateData data);
		object GetData();
	}
}
