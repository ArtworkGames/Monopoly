using System.Collections.Generic;
using System.Linq;

namespace ArtworkGames.DiceValley.Data.Private
{
	public class BuildingsPrivateModel : IPrivateModel
	{
		private List<BuildingPrivateSchema> _buildings = new List<BuildingPrivateSchema>();

		public string Name => "buildings";

		public bool IsDirty { get; private set; }

		public void ResetDirty()
		{
			IsDirty = false;
		}

		public void Init(PrivateData data)
		{
			_buildings = data.buildings;
		}

		public object GetData()
		{
			return _buildings;
		}

		public BuildingPrivateSchema Get(string buildingId)
		{
			return _buildings.First(building => building.id == buildingId);
		}
	}
}
