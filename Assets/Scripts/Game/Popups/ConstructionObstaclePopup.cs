using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConstructionObstaclePopup : MonoBehaviour
{
	[SerializeField] private Image[] images;

	[Space]
	[SerializeField] private GameObject cleanButton;
	[SerializeField] private TMP_Text cleanLabel;

	[Space]
	[SerializeField] private GameObject inactiveCleanButton;
	[SerializeField] private TMP_Text inactiveCleanLabel;

	[HideInInspector] public BuildingPlace buildingPlace;

	private Vector2 screenSize;

	private BuildingData buildingProfile;
	private int cleanPrice = 100;

	private void Awake()
	{
		screenSize = PopupsManager.GetSize();
	}

	private void Start()
	{
		buildingPlace.OnUnselect += OnBuildingPlaceUnselect;

		//buildingProfile = Profile.GetBuilding(buildingPlace.district.color, buildingPlace.district.id, buildingPlace.id);
		buildingProfile.OnConstructionStateChanged += OnBuildingStateChanged;

		//Color color = DistrictData.GetColor(buildingPlace.district.color);
		//for (int i = 0; i < images.Length; i++)
		//{
		//	images[i].color = Color.white;// color;
		//}

		cleanLabel.text = "CLEAN\n" + cleanPrice;
		inactiveCleanLabel.text = cleanLabel.text;

		Profile.OnCurrencyChanged += OnCurrencyChanged;
		OnCurrencyChanged();

		Update();
	}

	private void OnDestroy()
	{
		Profile.OnCurrencyChanged -= OnCurrencyChanged;

		buildingPlace.OnUnselect -= OnBuildingPlaceUnselect;
		buildingPlace = null;

		buildingProfile.OnConstructionStateChanged -= OnBuildingStateChanged;
		buildingProfile = null;
	}

	private void OnCurrencyChanged()
	{
		bool canClean = Profile.currency >= cleanPrice;
		cleanButton.SetActive(canClean);
		inactiveCleanButton.SetActive(!canClean);
	}

	private void OnBuildingPlaceUnselect(BuildingPlace buildingPlace)
	{
		Destroy(gameObject);
	}

	private void OnBuildingStateChanged(BuildingData buildingProfile)
	{
		Destroy(gameObject);
	}

	private void Update()
	{
		if (buildingPlace != null)
		{
			//Vector3 popupPos = buildingPlace.constructionObstacle.popupPoint.position;
			//popupPos = GameManager.instance.gameCamera.camera.WorldToViewportPoint(popupPos);
			//popupPos = new Vector3((popupPos.x - 0.5f) * screenSize.x, (popupPos.y - 0.5f) * screenSize.y, 0.0f);
			//transform.localPosition = popupPos;
		}
	}

	public void OnCleanButtonClick()
	{
		if (Profile.currency >= cleanPrice)
		{
			Profile.currency -= cleanPrice;
			//buildingPlace.RemoveObstacle();
		}
	}
}
