using System.Linq;

namespace ArtworkGames.DiceValley.Data.Public
{
	public class BuildingsPublicModel : IPublicModel
	{
		private BuildingPublicSchema[] _buildings;

		public string Name => "buildings";

		public void Init(PublicData data)
		{
			_buildings = data.buildings;
		}

		public BuildingPublicSchema Get(string buildingId)
		{
			return _buildings.First(building => building.id == buildingId);
		}
	}
}
