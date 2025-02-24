using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupsManager : MonoBehaviour
{
	private static PopupsManager instance;

	private void Awake()
	{
		instance = this;
	}

	private void OnDestroy()
	{
		instance = null;
	}

	public static ConstructionObstaclePopup ShowConstructionObstaclePopup(BuildingPlace buildingPlace)
	{
		GameObject popupObject = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Popups/ConstructionObstaclePopup"));
		popupObject.name = "ConstructionObstaclePopup";
		popupObject.transform.SetParent(instance.transform, false);

		ConstructionObstaclePopup popup = popupObject.GetComponent<ConstructionObstaclePopup>();
		popup.buildingPlace = buildingPlace;

		return popup;
	}

	public static ConstructionPopup ShowConstructionPopup(BuildingPlace buildingPlace)
	{
		GameObject popupObject = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Popups/ConstructionPopup"));
		popupObject.name = "ConstructionPopup";
		popupObject.transform.SetParent(instance.transform, false);

		ConstructionPopup popup = popupObject.GetComponent<ConstructionPopup>();
		popup.buildingPlace = buildingPlace;

		return popup;
	}

	public static BuyBuildingPopup ShowBuyBuildingPopup(BuildingPlace buildingPlace)
	{
		GameObject popupObject = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Popups/BuyBuildingPopup"));
		popupObject.name = "BuyBuildingPopup";
		popupObject.transform.SetParent(instance.transform, false);

		BuyBuildingPopup popup = popupObject.GetComponent<BuyBuildingPopup>();
		popup.buildingPlace = buildingPlace;

		return popup;
	}

	public static ProductTickPopup ShowProductTickPopup(BuildingPlace buildingPlace)
	{
		GameObject popupObject = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Popups/ProductTickPopup"));
		popupObject.name = "ProductTickPopup";
		popupObject.transform.SetParent(instance.transform, false);

		ProductTickPopup popup = popupObject.GetComponent<ProductTickPopup>();
		popup.SetBuildingPlace(buildingPlace);

		return popup;
	}

	public static BuildingPopup ShowBuildingPopup(BuildingPlace buildingPlace)
	{
		GameObject popupObject = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Popups/BuildingPopup"));
		popupObject.name = "BuildingPopup";
		popupObject.transform.SetParent(instance.transform, false);

		BuildingPopup popup = popupObject.GetComponent<BuildingPopup>();
		popup.SetBuildingPlace(buildingPlace);

		return popup;
	}

	public static Vector2 GetSize()
	{
		return ((RectTransform)instance.transform).GetSize();
	}
}
