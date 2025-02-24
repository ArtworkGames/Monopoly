using ArtworkGames.DiceValley.Data.Public;
using System;
using TMPro;
using UnityEngine;

namespace ArtworkGames.DiceValley.Windows.StockWindow
{
	public class StockTabButton : MonoBehaviour
	{
		public Action<StockTabButton> OnClick;

		[SerializeField] private TMP_Text _title;

		[HideInInspector] public ItemType itemType;

		private void Start()
		{
			_title.text = itemType.ToString();
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
