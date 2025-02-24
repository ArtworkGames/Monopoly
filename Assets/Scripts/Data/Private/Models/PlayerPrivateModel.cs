using ArtworkGames.DiceValley.Data.Public;
using ArtworkGames.DiceValley.Signals;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ArtworkGames.DiceValley.Data.Private
{
	public class PlayerPrivateModel : IPrivateModel
	{
		private PlayerPrivateSchema _player;

		public string Name => "player";

		public bool IsDirty { get; private set; }

		public void ResetDirty()
		{
			IsDirty = false;
		}

		public object GetData()
		{
			return _player;
		}

		public void Init(PrivateData data)
		{
			_player = data.player;
		}

		public int Level
		{
			get => _player.level;
			set
			{
				_player.level = value;
				IsDirty = true;
			}
		}
	}
}
