using ArtworkGames.UI;
using ArtworkGames.Windows;
using System;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace ArtworkGames.DiceValley.Windows.GameRestartWindow
{
	public class GameRestartWindowParams : BaseWindowParams
	{
		public string TitleKey;
		public string MessageKey;
		public string RestartLabelKey;
	}

	public class GameRestartWindow : BaseWindow<GameRestartWindowParams>
	{
		public static string PrefabName = "GameRestartWindow";

		[Space]
		[SerializeField] private TMPTextLocalizer _title;
		[SerializeField] private TMPTextLocalizer _message;
		[SerializeField] private TMPTextLocalizer _restartLabel;

		protected override void BeforeOpen()
		{
			if (!string.IsNullOrEmpty(Params.TitleKey))
				_title.Localize(Params.TitleKey);

			if (!string.IsNullOrEmpty(Params.MessageKey))
				_message.Localize(Params.MessageKey);

			if (!string.IsNullOrEmpty(Params.RestartLabelKey))
				_restartLabel.Localize(Params.RestartLabelKey);
		}

		public void OnRestartButtonClick()
		{
			Application.Quit();
		}
	}
}
