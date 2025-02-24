using System.Linq;
using UnityEngine;

namespace ArtworkGames.DiceValley.Data.Public
{
	public class LevelsPublicModel : IPublicModel
	{
		private LevelPublicSchema[] _levels;

		public string Name => "levels";

		public void Init(PublicData data)
		{
			_levels = data.levels;
		}

		public LevelPublicSchema GetLevel(int levelId)
		{
			levelId = Mathf.Min(levelId, _levels.Length);
			return _levels.FirstOrDefault(level => level.level == levelId);
		}
	}
}
