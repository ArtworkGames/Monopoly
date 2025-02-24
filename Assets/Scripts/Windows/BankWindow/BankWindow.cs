using ArtworkGames.DiceValley.Data.Public;
using ArtworkGames.DiceValley.Managers;
using ArtworkGames.Windows;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace ArtworkGames.DiceValley.Windows.BankWindow
{
	public class BankWindowParams : BaseWindowParams
	{
		public ProductType ProductType;
	}

	public class BankWindow : BaseWindow<BankWindowParams>
	{
		public static string PrefabName = "BankWindow";

		[Space]
		[SerializeField] private GameObject _sourceTabButton;
		[SerializeField] private GameObject _sourceBankItem;

		private IObjectResolver _diContainer;
		private BankManager _bankManager;

		private List<BankTabButton> tabs;
		private List<BankItem> items;

		[Inject]
		public void Construct(
			IObjectResolver diContainer,
			BankManager bankManager)
		{
			_diContainer = diContainer;
			_bankManager = bankManager;
		}

		protected override void BeforeOpen()
		{
			_sourceTabButton.SetActive(false);
			_sourceBankItem.SetActive(false);

			if (tabs == null)
			{
				ProductType[] productTypes = new ProductType[] { ProductType.Bucks, ProductType.Coins, ProductType.Energy };

				tabs = new List<BankTabButton>();
				for (int i = 0; i < productTypes.Length; i++)
				{
					GameObject tabObject = _diContainer.Instantiate(_sourceTabButton, _sourceTabButton.transform.parent, false);
					tabObject.SetActive(true);

					BankTabButton tab = tabObject.GetComponent<BankTabButton>();
					tab.productType = productTypes[i];
					tab.OnClick += OnTabClick;
					tabs.Add(tab);
				}
			}

			UpdateItems(Params.ProductType);
		}

		protected override void AfterClose()
		{
			ClearItems();
		}

		private void ClearItems()
		{
			if (items != null)
			{
				for (int i = 0; i < items.Count; i++)
				{
					Destroy(items[i].gameObject);
				}
			}
			items = new List<BankItem>();
		}

		private void UpdateItems(ProductType type)
		{
			ClearItems();

			ProductPublicSchema[] products = _bankManager.GetProducts(type);

			for (int i = 0; i < products.Length; i++)
			{
				GameObject itemObject = _diContainer.Instantiate(_sourceBankItem, _sourceBankItem.transform.parent, false);
				itemObject.SetActive(true);

				BankItem item = itemObject.GetComponent<BankItem>();
				item.PublicSchema = products[i];
				item.OnClick += OnItemClick;
				items.Add(item);
			}
		}

		private void OnTabClick(BankTabButton tab)
		{
			UpdateItems(tab.productType);
		}

		private void OnItemClick(BankItem item)
		{
			_bankManager.BuyProduct(item.PublicSchema);
		}
	}
}
