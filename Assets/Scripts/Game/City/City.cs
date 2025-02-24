using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class City : MonoBehaviour
{
	public District[] districts;

	[HideInInspector] public CityData data;

	public void Init(CityData cityData)
	{
		data = cityData;

		for (int i = 0; i < data.districts.Length; i++)
		{
			if (i < districts.Length)
			{
				districts[i].Init(data.districts[i]);
			}
		}
	}

	public District GetDistrict(DistrictColor districtColor, int districtId)
	{
		//for (int i = 0; i < districts.Length; i++)
		//{
		//	if ((districts[i].color == districtColor) && (districts[i].id == districtId))
		//	{
		//		return districts[i];
		//	}
		//}
		return null;
	}

	public BuildingPlace GetBuildingPlace(DistrictColor districtColor, int districtId, int buildingId)
	{
		//for (int i = 0; i < districts.Length; i++)
		//{
		//	if ((districts[i].color == districtColor) && (districts[i].id == districtId))
		//	{
		//		for (int j = 0; j < districts[i].buildingPlaces.Length; j++)
		//		{
		//			if (districts[i].buildingPlaces[j].id == buildingId)
		//			{
		//				return districts[i].buildingPlaces[j];
		//			}
		//		}
		//	}
		//}
		return null;
	}
}