using ArtworkGames.DiceValley.Managers;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConstructionPopup : MonoBehaviour
{
	[SerializeField] private Image[] images;

	[SerializeField] private RectTransform content;
	[SerializeField] private Image progressFill;

	[Space]
	[SerializeField] private GameObject skipButton;
	[SerializeField] private TMP_Text skipLabel;

	[Space]
	[SerializeField] private GameObject inactiveSkipButton;
	[SerializeField] private TMP_Text inactiveSkipLabel;

	[HideInInspector] public BuildingPlace buildingPlace;

	private Vector2 screenSize;

	private BuildingData buildingProfile;
	private int skipPrice = 100;

	private void Awake()
	{
		screenSize = PopupsManager.GetSize();
	}

	private void Start()
	{
		buildingPlace.OnUnselect += OnBuildingPlaceUnselect;

		//buildingProfile = Profile.GetBuilding(buildingPlace.district.color, buildingPlace.district.id, buildingPlace.id);
		buildingProfile.OnConstructionStateChanged += OnBuildingConstructionStateChanged;
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

		buildingProfile.OnConstructionStateChanged -= OnBuildingConstructionStateChanged;
		buildingProfile.OnPropertyStateChanged -= OnBuildingPropertyStateChanged;
		buildingProfile = null;
	}

	private void OnCurrencyChanged()
	{
		bool canSkip = Profile.currency >= skipPrice;

		if (buildingProfile.propertyState != PropertyState.NotOwned)
			content.SetHeight(200.0f);
		else
			content.SetHeight(95.0f);

		skipButton.SetActive((buildingProfile.propertyState != PropertyState.NotOwned) && canSkip);
		inactiveSkipButton.SetActive((buildingProfile.propertyState != PropertyState.NotOwned) && !canSkip);
	}

	private void OnBuildingPlaceUnselect(BuildingPlace buildingPlace)
	{
		Destroy(gameObject);
	}

	private void OnBuildingConstructionStateChanged(BuildingData buildingProfile)
	{
		Destroy(gameObject);
	}

	private void OnBuildingPropertyStateChanged(BuildingData buildingProfile)
	{
		OnCurrencyChanged();
	}

	private void Update()
	{
		if (buildingPlace != null)
		{
			Vector3 popupPos = buildingPlace.construction.popupPoint.position;
			popupPos = GameManager.instance.gameCamera.camera.WorldToViewportPoint(popupPos);
			popupPos = new Vector3((popupPos.x - 0.5f) * screenSize.x, (popupPos.y - 0.5f) * screenSize.y, 0.0f);
			transform.localPosition = popupPos;

			float progress = (Time.realtimeSinceStartup - buildingProfile.constructionStartTime) / buildingProfile.constructionDuration;
			progressFill.fillAmount = progress;

			skipLabel.text = "SPEED UP\n" + skipPrice;
			inactiveSkipLabel.text = skipLabel.text;
		}
	}

	public void OnSkipButtonClick()
	{
		if (Profile.currency >= skipPrice)
		{
			Profile.currency -= skipPrice;
			//buildingPlace.SkipConstruction();
		}
	}
}
