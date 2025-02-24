using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProductItem : MonoBehaviour
{
	public Action<ProductItem> OnClick;

	[SerializeField] private Image image;
	[SerializeField] private TMP_Text title;
	[SerializeField] private TMP_Text count;
	[SerializeField] private GameObject checkIcon;
	[SerializeField] private GameObject searchIcon;

	[HideInInspector] public OrderProductData orderProductProfile;

	private ProductData product;
	private BuildingData productBuilding;

	private void Start()
	{
		product = Profile.GetProduct(orderProductProfile.type);
		product.OnCountChanged += OnProductCountChanged;

		productBuilding = Profile.FindBuilding(product.type);
		productBuilding.OnConstructionStateChanged += OnBuildingConstructionStateChanged;

		image.sprite = Resources.Load<Sprite>("Products/" + product.type.ToString().ToLower() + "_icon");
		if (title != null) title.text = product.title;

		UpdateComponents();
	}

	private void OnDestroy()
	{
		if (product != null)
		{
			product.OnCountChanged -= OnProductCountChanged;
			product = null;
		}
		if (productBuilding != null)
		{
			productBuilding.OnConstructionStateChanged -= OnBuildingConstructionStateChanged;
			productBuilding = null;
		}
	}

	private void OnProductCountChanged(ProductData product)
	{
		UpdateComponents();
	}

	private void OnBuildingConstructionStateChanged(BuildingData building)
	{
		UpdateComponents();
	}

	private void UpdateComponents()
	{
		int productCount = Mathf.Min(product.count, orderProductProfile.count);
		count.text = productCount + "/" + orderProductProfile.count;

		checkIcon.SetActive(productCount == orderProductProfile.count);
		count.gameObject.SetActive(!checkIcon.activeSelf);

		if (searchIcon != null)
		{
			searchIcon.SetActive(!checkIcon.activeSelf &&
				(productBuilding.constructionState == ConstructionState.Completed));
		}
	}

	public void OnItemClick()
	{
		//if (productBuilding.constructionState == ConstructionState.Completed)
		//{
		//	BuildingPlace buildingPlace = GameManager.instance.city.GetBuildingPlace(
		//		productBuilding.district.color, productBuilding.district.id, productBuilding.id);

		//	if (GameManager.instance.selectedBuildingPlace != buildingPlace)
		//	{
		//		GameManager.instance.SelectBuildingPlace(buildingPlace);
		//	}
		//	else
		//	{
		//		GameManager.instance.FocusOnBuildingPlace(buildingPlace);
		//	}
		//	GameManager.instance.ShowBuildingManufactureWindow(buildingPlace);

		//	OnClick?.Invoke(this);
		//}
	}
}
