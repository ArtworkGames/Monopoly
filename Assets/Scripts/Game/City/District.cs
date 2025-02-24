using ArtworkGames.Signals;
using ArtworkGames.Windows;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class District : MonoBehaviour
{
	public Cell[] cells;
	public BuildingPlace[] buildingPlaces;

	[HideInInspector] public DistrictData data;

	private void Start()
	{
		for (int i = 0; i < cells.Length; i++)
		{
			if (i == 0)
			{
				cells[i].prev = cells[cells.Length - 1];
			}
			else
			{
				cells[i].prev = cells[i - 1];
			}
			if (i == (cells.Length - 1))
			{
				cells[i].next = cells[0];
			}
			else
			{
				cells[i].next = cells[i + 1];
			}
		}
	}

	public void Init(DistrictData districtData)
	{
		data = districtData;

		//selection.SetColor(DistrictData.GetColor(color));

		if (!data.isActive)
		{
			data.OnActivated += OnActivated;
			//selection.gameObject.SetActive(false);
		}

		for (int i = 0; i < data.buildings.Length; i++)
		{
			if (i < buildingPlaces.Length)
			{
				buildingPlaces[i].Init(data.buildings[i]);
			}
		}
	}

	private void OnDestroy()
	{
		if (data != null)
		{
			data.OnActivated -= OnActivated;
			data = null;
		}
	}

	private void OnActivated(DistrictData districtData)
	{
		data.OnActivated -= OnActivated;

		//selection.gameObject.SetActive(true);
		//for (int i = 0; i < objectsToHide.Length; i++)
		//{
		//	objectsToHide[i].SetActive(false);
		//}
	}
}
