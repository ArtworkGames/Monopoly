using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OrderItem : MonoBehaviour
{
	[SerializeField] private TMP_Text title;
	[SerializeField] private GameObject sourceProductItem;
	[SerializeField] private GameObject frame;

	[HideInInspector] public OrderData orderProfile;

	private List<ProductItem> products;

	private void Start()
	{
		title.text = orderProfile.title;

		sourceProductItem.SetActive(false);
		products = new List<ProductItem>();
		for (int i = 0; i < orderProfile.orderProducts.Count; i++)
		{
			GameObject productObject = Instantiate<GameObject>(sourceProductItem, sourceProductItem.transform.parent, false);
			productObject.SetActive(true);

			ProductItem product = productObject.GetComponent<ProductItem>();
			product.orderProductProfile = orderProfile.orderProducts[i];
			products.Add(product);
		}

		frame.SetActive(false);

		orderProfile.OnStateChanged += OnOrderStateChanged;
		OnOrderStateChanged(orderProfile);
	}

	private void OnDestroy()
	{
		if (orderProfile != null)
		{
			orderProfile.OnStateChanged -= OnOrderStateChanged;
		}
	}

	private void OnOrderStateChanged(OrderData order)
	{
		frame.SetActive(orderProfile.state == OrderState.Ready);
	}

	public void OnClick()
	{
		//GameManager.instance.ShowOrderWindow(orderProfile);
	}
}
