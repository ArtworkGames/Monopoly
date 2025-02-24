using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DistrictColor
{
	None = 0,
	Purple,
	Cyan,
	Magenta,
	Orange,
	Red,
	Yellow,
	Green,
	Blue
};

[Serializable]
public class DistrictData
{
	public Action<DistrictData> OnActivated;

	public string title;
	public DistrictColor color;
	public int id;
	public int wave;
	public bool isActive;
	public int activationPrice;
	public int[] rent;

	public BuildingData[] buildings;

	public void Activate()
	{
		isActive = true;
		OnActivated?.Invoke(this);
	}

	public BuildingData GetBuilding(int id)
	{
		for (int i = 0; i < buildings.Length; i++)
		{
			if (buildings[i].id == id) return buildings[i];
		}
		return null;
	}

	public int GetCompletedBuildingsCount()
	{
		int count = 0;
		for (int i = 0; i < buildings.Length; i++)
		{
			if ((buildings[i].constructionState == ConstructionState.Completed) &&
				(buildings[i].propertyState != PropertyState.NotOwned))
				count++;
		}
		return count;
	}

	public int GetRent()
	{
		int r = 0;
		int completedBuildingsCount = GetCompletedBuildingsCount();
		if (completedBuildingsCount > 0)
		{
			r = rent[completedBuildingsCount - 1];
		}

		for (int i = 0; i < buildings.Length; i++)
		{
			if ((buildings[i].constructionState == ConstructionState.Completed) &&
				(buildings[i].propertyState != PropertyState.NotOwned))
				r += buildings[i].GetRentIncrease();
		}
		return r;
	}

	public static Color GetColor(DistrictColor color)
	{
		string hexColor = "";
		switch (color)
		{
			case DistrictColor.Purple: hexColor = "67449e"; break;
			case DistrictColor.Cyan: hexColor = "5ac9e4"; break;
			case DistrictColor.Magenta: hexColor = "e843ac"; break;
			case DistrictColor.Orange: hexColor = "ff992a"; break;
			case DistrictColor.Red: hexColor = "e01f32"; break;
			case DistrictColor.Yellow: hexColor = "fece00"; break;
			case DistrictColor.Green: hexColor = "579e50"; break;
			case DistrictColor.Blue: hexColor = "3160ae"; break;
		}
		return MyUtils.HexToColor(hexColor);
	}
}
