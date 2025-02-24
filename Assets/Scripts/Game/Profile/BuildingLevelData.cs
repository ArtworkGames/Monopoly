using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BuildingLevelData
{
	[NonSerialized]
	public BuildingData building;

	public int level;
	public int price;
	public int rentIncrease;
}
