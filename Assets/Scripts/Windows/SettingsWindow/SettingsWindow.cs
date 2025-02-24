using ArtworkGames.DiceValley.Data.Private;
using ArtworkGames.Signals;
using ArtworkGames.Windows;
using MessagePipe;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace ArtworkGames.DiceValley.Windows.SettingsWindow
{
	public class SettingsWindowParams : BaseWindowParams
	{
	}

	public class SettingsWindow : BaseWindow<SettingsWindowParams>
	{
		public static string PrefabName = "SettingsWindow";

		private PrivateDataProvider _privateDataProvider;
		private IPublisher<OpenWindowSignal> _openWindowPublisher;

		[Inject]
		public void Construct(
			PrivateDataProvider privateDataProvider,
			IPublisher<OpenWindowSignal> openWindowPublisher)
		{
			_privateDataProvider = privateDataProvider;
			_openWindowPublisher = openWindowPublisher;
		}

		protected override void BeforeOpen()
		{
		}

		public void OnResetProgressButtonClick()
		{
			_openWindowPublisher.Publish(new OpenWindowSignal(ConfirmWindow.ConfirmWindow.PrefabName)
			{
				Params = new ConfirmWindow.ConfirmWindowParams()
				{
					MessageKey = "confirmwindow:resetprogress",
					OnYes = () =>
					{
						_privateDataProvider.Reset();
						ShowGameRestartWindow();
					}
				},
				BehaviourType = typeof(PopUpWindowBehaviour)
			});
		}

		private void ShowGameRestartWindow()
		{
			_openWindowPublisher.Publish(new OpenWindowSignal(GameRestartWindow.GameRestartWindow.PrefabName)
			{
				BehaviourType = typeof(HideOtherWindowBehaviour)
			});
		}
	}
}
