using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public enum OldProductType
{
	None = -1,

	Tomato,
	Cabbage,
	Potato,

	Cactus,
	Tultip,
	Sunflower,

	Burger,
	Cola,
	Fries,

	Jacket,
	TShirt,
	Pants,

	Bricks,
	Pipes,
	Shingles,

	Teapot,
	Microwave,
	Fridge,

	Ring,
	Bracelet,
	Necklace,

	Unicycle,
	Scooter,
	Motorbike,

	Wash,
	Cafe,
	Massage,

	Parking,
	Shower,
	Cleaning,

	Watch,
	Phone,
	Laptop,

	Japan,
	Italy,
	Mexico,

	Sculpt,
	Book,
	Movie,

	Count
};

public enum ProductState
{
	InQueue,
	InProgress,
	Completed
};

[Serializable]
public class ProductData
{
	public Action<ProductData> OnStateChanged;
	public Action<ProductData> OnTicksChanged;
	public Action<ProductData> OnCountChanged;

	public OldProductType type;
	public string title;
	public int price;
	public int ticksCount;
	public int maxCountInOrder;
	public int ordersCount;

	private ProductState _state = ProductState.InQueue;
	public ProductState state
	{
		get
		{
			return _state;
		}
		set
		{
			if (_state != value)
			{
				_state = value;
				OnStateChanged?.Invoke(this);
			}
		}
	}

	private int _ticks = 0;
	public int ticks
	{
		get
		{
			return _ticks;
		}
		set
		{
			if ((_ticks < ticksCount) && (_ticks != value))
			{
				_ticks = Mathf.Clamp(value, 0, ticksCount);
				OnTicksChanged?.Invoke(this);
				if (_ticks == ticksCount)
				{
					state = ProductState.Completed;
				}
			}
		}
	}

	private int _count = 0;
	public int count
	{
		get
		{
			return _count;
		}
		set
		{
			if (_count != value)
			{
				_count = value;
				OnCountChanged?.Invoke(this);
			}
		}
	}
}
