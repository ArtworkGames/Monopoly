using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ManufactureData
{
	[NonSerialized]
	public BuildingData building;

	public int buildingLevel;
	public OldProductType type;
}
