namespace ArtworkGames.DiceValley.Data.Public
{
	public interface IPublicModel
	{
		string Name { get; }
		void Init(PublicData data);
	}
}
