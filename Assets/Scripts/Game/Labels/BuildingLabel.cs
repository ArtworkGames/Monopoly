using ArtworkGames.DiceValley.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingLabel : MonoBehaviour
{
	[SerializeField] private Image[] images;
	[SerializeField] private GameObject container;

	[Space]
	[SerializeField] private GameObject cleanIcon;
	[SerializeField] private GameObject constructionIcon;
	[SerializeField] private GameObject currencyIcon;
	[SerializeField] private GameObject loanIcon;

	[HideInInspector] public BuildingPlace buildingPlace;

	private Vector2 screenSize;

	private BuildingData buildingProfile;

	private void Awake()
	{
		screenSize = PopupsManager.GetSize();
	}

	private void Start()
	{
	}

	private void OnDestroy()
	{
	}

	private void Show()
	{
		container.SetActive(true);
	}

	private void Hide()
	{
		container.SetActive(false);
	}

	private void OnBuildingPlaceSelect(BuildingPlace buildingPlace)
	{
		Hide();
	}

	private void OnBuildingPlaceUnselect(BuildingPlace buildingPlace)
	{
		//if ((buildingProfile.constructionState == ConstructionState.NeedToClean) ||
		//	((buildingProfile.constructionState == ConstructionState.InProgress) &&
		//	(buildingProfile.propertyState != PropertyState.NotOwned)) ||
		//	(buildingProfile.constructionState == ConstructionState.Completed))
		//{
		//	Show();
		//}
		//else
		//{
		//	Hide();
		//}

		OnBuildingProductsChanged(buildingProfile);
	}

	private void OnBuildingConstructionStateChanged(BuildingData buildingProfile)
	{
		cleanIcon.SetActive(buildingProfile.constructionState == ConstructionState.NeedToClean);
		constructionIcon.SetActive((buildingProfile.constructionState == ConstructionState.InProgress) &&
			(buildingProfile.propertyState != PropertyState.NotOwned));
		currencyIcon.SetActive((buildingProfile.constructionState == ConstructionState.Completed) && 
			(buildingProfile.propertyState == PropertyState.NotOwned));
		loanIcon.SetActive((buildingProfile.constructionState == ConstructionState.Completed) && 
			(buildingProfile.propertyState == PropertyState.InPledge));

		if ((buildingProfile.constructionState == ConstructionState.Completed) &&
			(buildingProfile.propertyState == PropertyState.Owned))
		{
			Destroy(gameObject);
		}
	}

	private void OnBuildingPropertyStateChanged(BuildingData buildingProfile)
	{
		OnBuildingConstructionStateChanged(buildingProfile);
	}

	private void OnBuildingProductsChanged(BuildingData buildingProfile)
	{
		bool canShow = false;
		for (int i = 0; i < buildingProfile.products.Count; i++)
		{
			if (buildingProfile.products[i].state == ProductState.Completed)
			{
				canShow = true;
				break;
			}
		}

		if (canShow &&
			(buildingProfile.constructionState == ConstructionState.Completed) &&
			(buildingProfile.propertyState == PropertyState.Owned))
		{
			Show();
		}
		else
		{
			Hide();
		}
	}

	private void Update()
	{
		if (buildingPlace != null)
		{
			Vector3 popupPos = Vector3.zero;
			if (buildingProfile.constructionState == ConstructionState.NeedToClean)
			{
				//popupPos = buildingPlace.constructionObstacle.popupPoint.position;
			}
			else if (buildingProfile.constructionState == ConstructionState.InProgress)
			{
				popupPos = buildingPlace.construction.popupPoint.position;
			}
			else
			{
				popupPos = buildingPlace.building.popupPoint.position;
			}
			popupPos = GameManager.instance.gameCamera.camera.WorldToViewportPoint(popupPos);
			popupPos = new Vector3((popupPos.x - 0.5f) * screenSize.x, (popupPos.y - 0.5f) * screenSize.y, 0.0f);
			transform.localPosition = popupPos;
		}
	}
}
