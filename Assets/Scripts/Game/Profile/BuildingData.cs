using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PropertyState
{
	NotOwned,
	Owned,
	InPledge
}

public enum ConstructionState
{
	NeedToClean,
	NotBuilt,
	InProgress,
	Completed
}

[Serializable]
public class BuildingData
{
	public Action<BuildingData> OnPropertyStateChanged;
	public Action<BuildingData> OnConstructionStateChanged;
	public Action<BuildingData> OnLevelChanged;
	public Action<BuildingData> OnProductsChanged;

	[NonSerialized]
	public DistrictData district;

	public string title;
	public int id;

	private PropertyState _propertyState;
	public PropertyState propertyState
	{
		get
		{
			return _propertyState;
		}
		set
		{
			if (_propertyState != value)
			{
				_propertyState = value;
				OnPropertyStateChanged?.Invoke(this);
			}
		}
	}

	private ConstructionState _constructionState;
	public ConstructionState constructionState
	{
		get
		{
			return _constructionState;
		}
		set
		{
			if (_constructionState != value)
			{
				_constructionState = value;
				if (_constructionState == ConstructionState.InProgress)
				{
					constructionStartTime = Time.realtimeSinceStartup;
				}
				OnConstructionStateChanged?.Invoke(this);
			}
		}
	}

	public int constructionPrice;
	public float constructionDuration;
	public float constructionStartTime;

	private int _level = 1;
	public int level
	{
		get
		{
			return _level;
		}
		set
		{
			if (_level != value)
			{
				_level = value;
				OnLevelChanged?.Invoke(this);
			}
		}
	}

	public List<BuildingLevelData> levels = new List<BuildingLevelData>();
	public List<ManufactureData> manufactures = new List<ManufactureData>();

	public List<ProductData> products = new List<ProductData>();

	public int GetRentIncrease()
	{
		int inc = 0;
		for (int i = 0; i < levels.Count; i++)
		{
			if (level == levels[i].level)
			{
				inc = levels[i].rentIncrease;
			}
		}
		return inc;
	}

	public int GetLevelUpPrice()
	{
		int price = 0;
		for (int i = 0; i < levels.Count; i++)
		{
			if ((level + 1) == levels[i].level)
			{
				price = levels[i].price;
			}
		}
		return price;
	}

	public ManufactureData GetManufacture(OldProductType type)
	{
		for (int i = 0; i < manufactures.Count; i++)
		{
			if (manufactures[i].type == type)
			{
				return manufactures[i];
			}
		}
		return null;
	}

	public void AddProduct(ProductData product)
	{
		product.OnStateChanged += OnProductStateChanged;
		products.Add(product);
		OnProductsChanged?.Invoke(this);

		if (products[0].state == ProductState.InQueue)
		{
			products[0].state = ProductState.InProgress;
		}
	}

	public void RemoveProduct(ProductData product)
	{
		product.OnStateChanged -= OnProductStateChanged;
		products.Remove(product);
		OnProductsChanged?.Invoke(this);

		if ((products.Count > 0) && (products[0].state == ProductState.InQueue))
		{
			products[0].state = ProductState.InProgress;
		}
	}

	private void OnProductStateChanged(ProductData product)
	{
		OnProductsChanged?.Invoke(this);
	}
}
