using ArtworkGames.DiceValley.Managers;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuyBuildingPopup : MonoBehaviour
{
	[SerializeField] private Image[] images;

	[Space]
	[SerializeField] private GameObject buyButton;
	[SerializeField] private TMP_Text buyLabel;

	[Space]
	[SerializeField] private GameObject inactiveBuyButton;
	[SerializeField] private TMP_Text inactiveBuyLabel;

	[HideInInspector] public BuildingPlace buildingPlace;

	private Vector2 screenSize;

	private BuildingData buildingProfile;

	private void Awake()
	{
		screenSize = PopupsManager.GetSize();
	}

	private void Start()
	{
		buildingPlace.OnUnselect += OnBuildingPlaceUnselect;

		//buildingProfile = Profile.GetBuilding(buildingPlace.district.color, buildingPlace.district.id, buildingPlace.id);
		buildingProfile.OnPropertyStateChanged += OnBuildingPropertyStateChanged;

		//Color color = DistrictData.GetColor(buildingPlace.district.color);
		//for (int i = 0; i < images.Length; i++)
		//{
		//	images[i].color = Color.white;// color;
		//}

		Profile.OnCurrencyChanged += OnCurrencyChanged;
		OnCurrencyChanged();

		Update();
	}

	private void OnDestroy()
	{
		Profile.OnCurrencyChanged -= OnCurrencyChanged;

		buildingPlace.OnUnselect -= OnBuildingPlaceUnselect;
		buildingPlace = null;

		buildingProfile.OnPropertyStateChanged -= OnBuildingPropertyStateChanged;
		buildingProfile = null;
	}

	private void OnCurrencyChanged()
	{
		if (buildingProfile.propertyState == PropertyState.NotOwned)
		{
			buyLabel.text = "BUY\n" + buildingProfile.constructionPrice;
			inactiveBuyLabel.text = buyLabel.text;
		}
		else
		{
			buyLabel.text = "BUY OUT\n" + buildingProfile.constructionPrice;
			inactiveBuyLabel.text = buyLabel.text;
		}

		bool canClean = Profile.currency >= buildingProfile.constructionPrice;
		buyButton.SetActive(canClean);
		inactiveBuyButton.SetActive(!canClean);
	}

	private void OnBuildingPlaceUnselect(BuildingPlace buildingPlace)
	{
		Destroy(gameObject);
	}

	private void OnBuildingPropertyStateChanged(BuildingData buildingProfile)
	{
		if (buildingProfile.propertyState == PropertyState.NotOwned)
		{
			OnCurrencyChanged();
		}
		else
		{
			Destroy(gameObject);
		}
	}

	private void Update()
	{
		if (buildingPlace != null)
		{
			Vector3 popupPos = buildingPlace.building.popupPoint.position;
			popupPos = GameManager.instance.gameCamera.camera.WorldToViewportPoint(popupPos);
			popupPos = new Vector3((popupPos.x - 0.5f) * screenSize.x, (popupPos.y - 0.5f) * screenSize.y, 0.0f);
			transform.localPosition = popupPos;
		}
	}

	public void OnBuyButtonClick()
	{
		if (Profile.currency >= buildingProfile.constructionPrice)
		{
			Profile.currency -= buildingProfile.constructionPrice;

			if (buildingProfile.propertyState == PropertyState.InPledge)
			{
				Profile.loanCount -= 1;
				Profile.loan -= buildingProfile.constructionPrice;
			}
			buildingProfile.propertyState = PropertyState.Owned;
		}
	}
}
