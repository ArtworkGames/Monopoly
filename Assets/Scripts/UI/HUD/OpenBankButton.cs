using ArtworkGames.DiceValley.Data.Public;
using ArtworkGames.DiceValley.Windows.BankWindow;
using ArtworkGames.Signals;
using MessagePipe;
using UnityEngine;
using VContainer;

namespace ArtworkGames.DiceValley.UI.HUD
{
	public class OpenBankButton : MonoBehaviour
	{
		[SerializeField] private ProductType _productType;

		private IPublisher<OpenWindowSignal> _openWindowPublisher;

		[Inject]
		public void Construct(
			IPublisher<OpenWindowSignal> openWindowPublisher)
		{
			_openWindowPublisher = openWindowPublisher;
		}

		public void OnButtonClick()
		{
			_openWindowPublisher.Publish(new OpenWindowSignal(BankWindow.PrefabName, new BankWindowParams()
			{
				ProductType = _productType
			}));
		}
	}
}
