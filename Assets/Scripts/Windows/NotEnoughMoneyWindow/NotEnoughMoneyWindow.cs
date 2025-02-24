using ArtworkGames.DiceValley.Data.Public;
using ArtworkGames.DiceValley.UI.Components;
using ArtworkGames.DiceValley.Windows.BankWindow;
using ArtworkGames.Signals;
using ArtworkGames.Windows;
using MessagePipe;
using TMPro;
using UnityEngine;
using VContainer;

namespace ArtworkGames.DiceValley.Windows.NotEnoughMoneyWindow
{
	public class NotEnoughMoneyWindowParams : BaseWindowParams
	{
		public string ItemId;
	}

	public class NotEnoughMoneyWindow : BaseWindow<NotEnoughMoneyWindowParams>
	{
		public static string PrefabName = "NotEnoughMoneyWindow";

		[SerializeField] private TMP_Text _title;
		[SerializeField] private IconImage _image;

		private IPublisher<OpenWindowSignal> _openWindowPublisher;

		[Inject]
		public void Construct(
			IPublisher<OpenWindowSignal> openWindowPublisher)
		{
			_openWindowPublisher = openWindowPublisher;
		}

		protected override void BeforeOpen()
		{
			_title.text = $"Not enough {Params.ItemId}";
			LoadImage();
		}

		private async void LoadImage()
		{
			_image.gameObject.SetActive(false);
			await _image.LoadIconAsync(Params.ItemId);
			_image.gameObject.SetActive(true);
		}

		public void OnOpenBankButtonClick()
		{
			CloseWindow();

			ProductType productType = ProductType.Bucks;
			if (Params.ItemId.Equals(SystemItemName.Coins))
			{
				productType = ProductType.Coins;
			}
			else if (Params.ItemId.Equals(SystemItemName.Energy))
			{
				productType = ProductType.Energy;
			}

			_openWindowPublisher.Publish(new OpenWindowSignal(BankWindow.BankWindow.PrefabName, new BankWindowParams()
			{
				ProductType = productType,
			})
			{
				BehaviourType = typeof(PopUpWindowBehaviour)
			});
		}

	}
}
