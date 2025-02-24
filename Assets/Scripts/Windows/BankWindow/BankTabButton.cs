using ArtworkGames.DiceValley.Data.Public;
using System;
using TMPro;
using UnityEngine;

namespace ArtworkGames.DiceValley.Windows.BankWindow
{
	public class BankTabButton : MonoBehaviour
	{
		public Action<BankTabButton> OnClick;

		[SerializeField] private TMP_Text _title;

		[HideInInspector] public ProductType productType;

		private void Start()
		{
			_title.text = productType.ToString();
		}

		private void OnDestroy()
		{
			OnClick = null;
		}

		public void OnButtonClick()
		{
			OnClick?.Invoke(this);
		}
	}
}
