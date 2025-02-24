using ArtworkGames.UI;
using ArtworkGames.Windows;
using System;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace ArtworkGames.DiceValley.Windows.ConfirmWindow
{
	public class ConfirmWindowParams : BaseWindowParams
	{
		public string TitleKey;
		public string MessageKey;
		public string YesLabelKey;
		public string NoLabelKey;

		public Action OnYes;
		public Action OnNo;
	}

	public class ConfirmWindow : BaseWindow<ConfirmWindowParams>
	{
		public static string PrefabName = "ConfirmWindow";

		[Space]
		[SerializeField] private TMPTextLocalizer _title;
		[SerializeField] private TMPTextLocalizer _message;
		[SerializeField] private TMPTextLocalizer _yesLabel;
		[SerializeField] private TMPTextLocalizer _noLabel;

		protected override void BeforeOpen()
		{
			if (!string.IsNullOrEmpty(Params.TitleKey))
				_title.Localize(Params.TitleKey);

			if (!string.IsNullOrEmpty(Params.MessageKey))
				_message.Localize(Params.MessageKey);

			if (!string.IsNullOrEmpty(Params.YesLabelKey))
				_yesLabel.Localize(Params.YesLabelKey);

			if (!string.IsNullOrEmpty(Params.NoLabelKey))
				_noLabel.Localize(Params.NoLabelKey);
		}

		public void OnYesButtonClick()
		{
			CloseWindow();
			Params.OnYes?.Invoke();
		}

		public void OnNoButtonClick()
		{
			CloseWindow();
			Params.OnNo?.Invoke();
		}
	}
}
