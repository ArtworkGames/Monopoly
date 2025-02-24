using ArtworkGames.DiceValley.Managers;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingPopup : MonoBehaviour
{
	[SerializeField] private Transform pivot;
	[SerializeField] private Image image;
	[SerializeField] private Image mask;

	private BuildingPlace buildingPlace;
	private ProductData product;

	private bool isShowTickAnimation;

	private Vector2 screenSize;

	private void Awake()
	{
		screenSize = PopupsManager.GetSize();
	}

	private void Start()
	{
		pivot.localScale = Vector3.zero;
		pivot.DOScale(0.7f, 0.3f)
			.SetEase(Ease.OutBack);
	}

	private void OnDestroy()
	{
		if (buildingPlace != null)
		{
			//buildingPlace.buildingData.OnProductsChanged -= OnProductsChanged;
		}
	}

	private void Update()
	{
		if (buildingPlace != null)
		{
			Vector3 popupPos = buildingPlace.building.popupPoint.position;
			popupPos = GameManager.instance.gameCamera.camera.WorldToViewportPoint(popupPos);
			popupPos = new Vector3((popupPos.x - 0.5f) * screenSize.x, (popupPos.y - 0.5f) * screenSize.y, 0.0f);
			transform.localPosition = popupPos;
		}
	}

	public void SetBuildingPlace(BuildingPlace buildingPlace)
	{
		this.buildingPlace = buildingPlace;
		//buildingPlace.buildingData.OnProductsChanged += OnProductsChanged;
		//OnProductsChanged(buildingPlace.buildingData);
	}

	private void OnProductsChanged(BuildingData buildingProfile)
	{
		ProductData newProduct = null;
		if (buildingProfile.products.Count == 0)
		{
			newProduct = null;
		}
		else
		{
			newProduct = buildingProfile.products[0];
		}

		if (product != newProduct)
		{
			if (product != null)
			{
				product.OnTicksChanged -= OnTicksChanged;
			}

			product = newProduct;

			if (product != null)
			{
				product.OnTicksChanged += OnTicksChanged;
			}
		}

		UpdateComponents();
	}

	private void OnTicksChanged(ProductData product)
	{
		isShowTickAnimation = true;

		int ticks = product.ticks;
		int ticksCount = product.ticksCount;

		mask.DOFillAmount(1.0f - (float)ticks / (float)ticksCount, 1.0f)
			.SetEase(Ease.OutCubic)
			.OnComplete(() =>
			{
				if (ticks == ticksCount)
				{
					FlyingIcons.ShowProduct(product, image.transform.position);
				}

				isShowTickAnimation = false;
				UpdateComponents();
			});
	}

	private void UpdateComponents()
	{
		if (isShowTickAnimation) return;

		if (product == null)
		{
			mask.fillAmount = 0.0f;
			image.sprite = Resources.Load<Sprite>("Products/slip_icon");
		}
		else
		{
			mask.fillAmount = 1.0f - (float)product.ticks / (float)product.ticksCount;
			image.sprite = Resources.Load<Sprite>("Products/" + product.type.ToString().ToLower() + "_icon");
		}
	}

}
