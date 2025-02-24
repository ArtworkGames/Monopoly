using ArtworkGames.DiceValley.Data.Public;
using ArtworkGames.DiceValley.UI.Components;
using System;
using TMPro;
using UnityEngine;

namespace ArtworkGames.DiceValley.Windows.BankWindow
{
	public class BankItem : MonoBehaviour
	{
		public Action<BankItem> OnClick;

		[SerializeField] private IconImage _image;
		[SerializeField] private TMP_Text _amount;
		[SerializeField] private TMP_Text _buyLabel;

		[HideInInspector] public ProductPublicSchema PublicSchema;

		private async void Start()
		{
			_amount.text = PublicSchema.Items[0].Value.ToString();
			_buyLabel.text = "BUY\n" + PublicSchema.realPrice.ToString() + "$";

			_image.gameObject.SetActive(false);
			await _image.LoadIconAsync(PublicSchema.Items[0].Key);
			_image.gameObject.SetActive(true);
		}

		private void OnDestroy()
		{
			OnClick = null;
			PublicSchema = null;
		}

		public void OnBuyButtonClick()
		{
			OnClick?.Invoke(this);
		}
	}
}
