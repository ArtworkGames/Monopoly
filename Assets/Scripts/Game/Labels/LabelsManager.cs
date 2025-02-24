using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LabelsManager : MonoBehaviour
{
	private static LabelsManager instance;

	private void Awake()
	{
		instance = this;
	}

	private void OnDestroy()
	{
		instance = null;
	}

	public static BuildingLabel ShowBuildingLabel(BuildingPlace buildingPlace)
	{
		GameObject labelObject = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Labels/BuildingLabel"));
		labelObject.name = "BuildingLabel";
		labelObject.transform.SetParent(instance.transform, false);

		BuildingLabel label = labelObject.GetComponent<BuildingLabel>();
		label.buildingPlace = buildingPlace;

		return label;
	}
}
