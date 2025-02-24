using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrdersList : MonoBehaviour
{
	[SerializeField] private GameObject sourceItem;

	private List<OrderItem> items;

	private bool canUpdateOrders = true;

	private void Awake()
	{
		sourceItem.SetActive(false);
	}

	private IEnumerator Start()
	{
		yield return new WaitForEndOfFrame();

		items = new List<OrderItem>();

		UpdateOrders();
		UpdateItems();

		for (int i = 0; i < Profile.orders.Count; i++)
		{
			Profile.orders[i].OnStateChanged += OnOrderStateChanged;
		}
	}

	private void OnOrderStateChanged(OrderData order)
	{
		UpdateOrders();
		UpdateItems();
	}

	private void UpdateOrders()
	{
		if (!canUpdateOrders) return;
		canUpdateOrders = false;

		int shownOrdersCount = 0;
		for (int i = 0; i < Profile.orders.Count; i++)
		{
			if ((Profile.orders[i].state == OrderState.InProgress) ||
				(Profile.orders[i].state == OrderState.Ready))
			{
				shownOrdersCount++;
			}
		}
		if (shownOrdersCount < 4)
		{
			List<OrderData> ordersToShow = new List<OrderData>();
			for (int i = 0; i < Profile.orders.Count; i++)
			{
				if (Profile.orders[i].state == OrderState.Hidden)
				{
					ordersToShow.Add(Profile.orders[i]);
					shownOrdersCount++;
					if (shownOrdersCount == 4)
						break;
				}
			}
			for (int i = 0; i < ordersToShow.Count; i++)
			{
				ordersToShow[i].Show();
			}
		}

		canUpdateOrders = true;
	}

	private void UpdateItems()
	{
		List<OrderItem> itemsToRemove = new List<OrderItem>();
		for (int i = 0; i < items.Count; i++)
		{
			if ((items[i].orderProfile.state == OrderState.Hidden) ||
				(items[i].orderProfile.state == OrderState.Completed))
			{
				itemsToRemove.Add(items[i]);
			}
		}
		for (int i = 0; i < itemsToRemove.Count; i++)
		{
			items.Remove(itemsToRemove[i]);
			Destroy(itemsToRemove[i].gameObject);
		}
		for (int i = 0; i < Profile.orders.Count; i++)
		{
			if ((Profile.orders[i].state == OrderState.InProgress) ||
				(Profile.orders[i].state == OrderState.Ready))
			{
				OrderItem availableItem = GetItem(Profile.orders[i]);
				if (availableItem == null)
				{
					GameObject itemObject = Instantiate<GameObject>(sourceItem, sourceItem.transform.parent, false);
					itemObject.SetActive(true);

					OrderItem item = itemObject.GetComponent<OrderItem>();
					item.orderProfile = Profile.orders[i];
					items.Add(item);
				}
			}
		}
	}

	private OrderItem GetItem(OrderData order)
	{
		for (int i = 0; i < items.Count; i++)
		{
			if (items[i].orderProfile == order)
			{
				return items[i];
			}
		}
		return null;
	}
}
