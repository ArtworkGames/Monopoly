using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Profile
{
	public static Action OnWaveChanged;
	public static Action OnEnergyChanged;
	public static Action OnCurrencyChanged;
	public static Action OnExperienceChanged;
	public static Action OnLoanChanged;

	public const bool USE_LOAN = false;

	public static bool isInitialized;

	private static int _wave = 1;
	public static int wave
	{
		get
		{
			return _wave;
		}
		private set
		{
			if (_wave != value)
			{
				_wave = Mathf.Max(1, value);
				OnWaveChanged?.Invoke();
			}
		}
	}

	private static int _energy = 0;
	public static int energy
	{
		get
		{
			return _energy;
		}
		set
		{
			if (_energy != value)
			{
				_energy = Mathf.Max(0, value);
				OnEnergyChanged?.Invoke();
			}
		}
	}

	private static int _currency = 0;
	public static int currency
	{
		get
		{
			return _currency;
		}
		set
		{
			if (_currency != value)
			{
				_currency = Mathf.Max(0, value);
				OnCurrencyChanged?.Invoke();
			}
		}
	}

	private static int _experience;
	public static int experience
	{
		get
		{
			return _experience;
		}
		set
		{
			if (_experience != value)
			{
				_experience = Mathf.Max(0, value);
				OnExperienceChanged?.Invoke();
			}
		}
	}

	private static int _loanCount = 0;
	public static int loanCount
	{
		get
		{
			return _loanCount;
		}
		set
		{
			if (_loanCount != value)
			{
				_loanCount = value;
			}
		}
	}

	private static int _loan = 0;
	public static int loan
	{
		get
		{
			return _loan;
		}
		set
		{
			if (_loan != value)
			{
				_loan = value;
				OnLoanChanged?.Invoke();
			}
		}
	}

	public static int loanInterest
	{
		get
		{
			return (int)Mathf.Round(_loan * 0.1f * _loanCount);
		}
	}

	public static List<DistrictData> districts;
	public static List<ProductData> products;
	public static List<OrderData> orders;

	public static void LocalInit()
	{
		string jsonString = Resources.Load<TextAsset>("products").ToString();
		ParseProducts(jsonString);

		jsonString = Resources.Load<TextAsset>("city").ToString();
		ParseCity(jsonString);

		orders = new List<OrderData>();
		FillOrders();

		isInitialized = true;
	}

	public static IEnumerator Init()
	{
		LocalInit();
		yield return new WaitForEndOfFrame();

		//string productsUrl = "https://drive.google.com/file/d/16pUhhzi07htGfoXtX3bHQJYB7OcEhhOR/view?usp=sharing";
		//string cityUrl = "https://drive.google.com/file/d/1asm0XFHapWNYwHF2NoHIgMJ_V0ihBChT/view?usp=sharing";

		//UnityWebRequest productsRequest = UnityWebRequest.Get(productsUrl);
		//yield return productsRequest.SendWebRequest();

		//if (productsRequest.result == UnityWebRequest.Result.Success)
		//{
		//	string jsonString = productsRequest.downloadHandler.text;
		//	ParseProducts(jsonString);
		//}
		//else
		//{
		//	string jsonString = Resources.Load<TextAsset>("products").ToString();
		//	ParseProducts(jsonString);
		//}

		//UnityWebRequest cityRequest = UnityWebRequest.Get(cityUrl);
		//yield return cityRequest.SendWebRequest();

		//if (cityRequest.result == UnityWebRequest.Result.Success)
		//{
		//	string jsonString = cityRequest.downloadHandler.text;
		//	ParseCity(jsonString);
		//}
		//else
		//{
		//	string jsonString = Resources.Load<TextAsset>("city").ToString();
		//	ParseCity(jsonString);
		//}

		//orders = new List<OrderData>();
		//FillOrders();

		isInitialized = true;
	}

	private static void ParseProducts(string jsonString)
	{
		ProductsArrayData productsArray = JsonConvert.DeserializeObject<ProductsArrayData>(jsonString);
		products = new List<ProductData>(productsArray.products);
	}

	private static void ParseCity(string jsonString)
	{
		CityData city = JsonConvert.DeserializeObject<CityData>(jsonString);
		energy = city.energy;
		currency = city.currency;
		districts = new List<DistrictData>(city.districts);

		for (int i = 0; i < districts.Count; i++)
		{
			for (int j = 0; j < districts[i].buildings.Length; j++)
			{
				districts[i].buildings[j].district = districts[i];
				districts[i].buildings[j].OnConstructionStateChanged += OnBuildingConstructionStateChanged;

				for (int k = 0; k < districts[i].buildings[j].levels.Count; k++)
				{
					districts[i].buildings[j].levels[k].building = districts[i].buildings[j];
				}

				for (int k = 0; k < districts[i].buildings[j].manufactures.Count; k++)
				{
					districts[i].buildings[j].manufactures[k].building = districts[i].buildings[j];
				}
			}
		}
	}

	private static void OnBuildingConstructionStateChanged(BuildingData building)
	{
		if ((building.constructionState == ConstructionState.Completed) &&
			(building.district.wave == Profile.wave))
		{
			int districtWave = building.district.wave;
			List<DistrictData> waveDistricts = GetDistrictsByWave(districtWave);

			bool isCompleted = true;
			for (int i = 0; i < waveDistricts.Count; i++)
			{
				for (int j = 0; j < waveDistricts[i].buildings.Length; j++)
				{
					if (waveDistricts[i].buildings[j].constructionState != ConstructionState.Completed)
					{
						isCompleted = false;
					}
				}
			}

			if (isCompleted)
			{
				Profile.wave++;
			}
		}
	}

	public static List<DistrictData> GetDistrictsByWave(int wave)
	{
		List<DistrictData> waveDistricts = new List<DistrictData>();

		for (int i = 0; i < districts.Count; i++)
		{
			if (districts[i].wave == wave)
			{
				waveDistricts.Add(districts[i]);
			}
		}
		return waveDistricts;
	}

	public static DistrictData GetDistrict(DistrictColor color, int id)
	{
		for (int i = 0; i < districts.Count; i++)
		{
			if ((districts[i].color == color) && (districts[i].id == id))
				return districts[i];
		}
		return null;
	}

	public static BuildingData GetBuilding(DistrictColor districtColor, int districtId, int buildingId)
	{
		DistrictData district = GetDistrict(districtColor, districtId);
		if (district != null)
		{
			return district.GetBuilding(buildingId);
		}
		return null;
	}

	public static BuildingData FindBuilding(OldProductType productType)
	{
		for (int i = 0; i < districts.Count; i++)
		{
			for (int j = 0; j < districts[i].buildings.Length; j++)
			{
				for (int l = 0; l < districts[i].buildings[j].manufactures.Count; l++)
				{
					if (districts[i].buildings[j].manufactures[l].type == productType)
					{
						return districts[i].buildings[j];
					}
				}
			}
		}
		return null;
	}

	public static List<BuildingData> GetBuildingsInPledge()
	{
		List<BuildingData> buildings = new List<BuildingData>();
		for (int i = 0; i < districts.Count; i++)
		{
			for (int j = 0; j < districts[i].buildings.Length; j++)
			{
				if (districts[i].buildings[j].propertyState == PropertyState.InPledge)
				{
					buildings.Add(districts[i].buildings[j]);
				}
			}
		}
		return buildings;
	}

	public static ProductData GetProduct(OldProductType type)
	{
		for (int i = 0; i < products.Count; i++)
		{
			if (products[i].type == type)
			{
				return products[i];
			}
		}
		return null;
	}

	public static void AddProduct(OldProductType type, int count)
	{
		ProductData availableProduct = GetProduct(type);
		availableProduct.count += count;
	}

	public static bool TickProducts(DistrictColor districtColor, int districtId)
	{
		bool hasTick = false;

		DistrictData district = GetDistrict(districtColor, districtId);
		for (int i = 0; i < district.buildings.Length; i++)
		{
			//for (int j = 0; j < district.buildings[i].products.Count; j++)
			//{
			//	district.buildings[i].products[j].ticks++;
			//}

			if (district.buildings[i].products.Count > 0)
			{
				district.buildings[i].products[0].ticks++;
				hasTick = true;

				if (district.buildings[i].products[0].state == ProductState.Completed)
				{
					AddProduct(district.buildings[i].products[0].type, district.buildings[i].products[0].count);
					district.buildings[i].RemoveProduct(district.buildings[i].products[0]);
				}
			}
		}
		return hasTick;
	}

	private static void FillOrders()
	{
		int ordersCount = 50;
		Randomizer productRandomizer = null;

		for (int i = 0; i < ordersCount; i++)
		{
			OrderData order = new OrderData();
			order.id = i + 1;
			order.title = "Order " + (i + 1);

			int currencyReward = 0;
			int energyReward = 0;
			int experienceReward = 0;

			int maxProductType = Mathf.Min(i * 3 / 4 + 3, (int)OldProductType.Count);
			Randomizer newProductRandomizer = new Randomizer(maxProductType, maxProductType / 2);
			if (productRandomizer != null)
			{
				newProductRandomizer.SetLastIndexes(productRandomizer.GetLastIndexes());
			}
			productRandomizer = newProductRandomizer;

			for (int j = 0; j < UnityEngine.Random.Range(1, 4); j++)
			{
				int productIndex = productRandomizer.GetNextIndex();
				ProductData product = products[productIndex];

				product.ordersCount++;
				int minProductCount = Mathf.Clamp(product.ordersCount - product.maxCountInOrder, 1, product.maxCountInOrder);
				int maxProductCount = Mathf.Min(product.ordersCount, product.maxCountInOrder);
				int productCount = UnityEngine.Random.Range(minProductCount, maxProductCount + 1);

				//float countFactor = ((float)i / (float)ordersCount) * 3.0f;
				//int maxProductCount = Mathf.Clamp((int)Mathf.Round((float)((int)ProductType.Count - (int)productType) * countFactor), 1, 5);
				//int productCount = UnityEngine.Random.Range(1, maxProductCount + 1);

				order.AddProduct(product.type, productCount);

				currencyReward += product.price * productCount;
				energyReward += product.ticksCount * productCount;
				experienceReward += product.ticksCount * productCount * 10;
			}

			order.currencyReward = Mathf.Max(1, currencyReward * 12 / 10);
			order.energyReward = Mathf.Max(1, energyReward * 2 / 3);
			order.experienceReward = experienceReward;

			orders.Add(order);
		}
	}
}
