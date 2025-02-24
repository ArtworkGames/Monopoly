using ArtworkGames.DiceValley.Data.Public;
using ArtworkGames.DiceValley.Windows.SettingsWindow;
using ArtworkGames.Signals;
using MessagePipe;
using UnityEngine;
using VContainer;

namespace ArtworkGames.DiceValley.UI.HUD
{
	public class SettingsButton : MonoBehaviour
	{
		private IPublisher<OpenWindowSignal> _openWindowPublisher;

		[Inject]
		public void Construct(
			IPublisher<OpenWindowSignal> openWindowPublisher)
		{
			_openWindowPublisher = openWindowPublisher;
		}

		public void OnButtonClick()
		{
			_openWindowPublisher.Publish(new OpenWindowSignal(SettingsWindow.PrefabName));
		}
	}
}
