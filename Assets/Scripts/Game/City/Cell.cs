using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CellType
{
	Building, Chance, Roulette
}

public class Cell : MonoBehaviour
{
	public CellType type;
	public BuildingPlace buildingPlace;

	[HideInInspector] public Cell prev;
	[HideInInspector] public Cell next;
}
