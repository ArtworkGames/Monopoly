using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum OrderState
{
	Hidden,
	InProgress,
	Ready,
	Completed
};

public class OrderData
{
	public Action<OrderData> OnStateChanged;

	public int id;
	public string title;
	public int energyReward;
	public int currencyReward;
	public int experienceReward;

	private OrderState _state = OrderState.Hidden;
	public OrderState state
	{
		get
		{
			return _state;
		}
		private set
		{
			if (_state != value)
			{
				_state = value;
				OnStateChanged?.Invoke(this);
				CheckCompletion();
			}
		}
	}

	public List<OrderProductData> orderProducts = new List<OrderProductData>();

	public void AddProduct(OldProductType type, int count)
	{
		OrderProductData orderProduct = new OrderProductData();
		orderProduct.type = type;
		orderProduct.count = count;
		orderProducts.Add(orderProduct);

		ProductData product = Profile.GetProduct(type);
		product.OnCountChanged += OnProductCountChanged;
	}

	private void OnProductCountChanged(ProductData product)
	{
		CheckCompletion();
	}

	public void Show()
	{
		state = OrderState.InProgress;
	}

	private void CheckCompletion()
	{
		if (state == OrderState.Hidden) return;
		if (state == OrderState.Completed) return;

		bool isComplete = true;
		for (int i = 0; i < orderProducts.Count; i++)
		{
			ProductData product = Profile.GetProduct(orderProducts[i].type);
			if (product.count < orderProducts[i].count)
			{
				isComplete = false;
				break;
			}
		}
		state = isComplete ? OrderState.Ready : OrderState.InProgress;
	}

	public void Complete()
	{
		if (state != OrderState.Ready) return;

		for (int i = 0; i < orderProducts.Count; i++)
		{
			ProductData product = Profile.GetProduct(orderProducts[i].type);
			product.OnCountChanged -= OnProductCountChanged;
			product.count -= orderProducts[i].count;
		}

		state = OrderState.Completed;

		Profile.energy += energyReward;
		Profile.currency += currencyReward;
		Profile.experience += experienceReward;
	}
}
