using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using ArtworkGames.DiceValley.Managers;

public class ProductTickPopup : MonoBehaviour
{
	[SerializeField] private Transform pivot;
	[SerializeField] private Image image;
	[SerializeField] private Image mask;

	private BuildingPlace buildingPlace;
	private ProductData product;

	private int ticks;
	private int ticksCount;

	private Vector2 screenSize;

	private void Awake()
	{
		screenSize = PopupsManager.GetSize();
	}

	private void Start()
	{
		mask.fillAmount = 1.0f - (float)ticks / (float)ticksCount;

		pivot.localScale = Vector3.zero;
		pivot.DOScale(1.0f, 0.3f)
			.SetEase(Ease.OutBack)
			.OnComplete(() =>
			{
				mask.DOFillAmount(1.0f - (float)(ticks + 1) / (float)ticksCount, 1.0f)
					.SetEase(Ease.OutCubic)
					.OnComplete(() =>
					{
						if ((ticks + 1) == ticksCount)
						{
							FlyingIcons.ShowProduct(product, image.transform.position);
							image.enabled = false;
						}

						pivot.DOScale(0.0f, 0.3f)
							.SetEase(Ease.InBack)
							.OnComplete(() =>
							{
								Destroy(gameObject);
							});
					});
			});
	}

	private void OnDestroy()
	{
		buildingPlace = null;
		product = null;
	}

	public void SetBuildingPlace(BuildingPlace buildingPlace)
	{
		this.buildingPlace = buildingPlace;
		//product = buildingPlace.buildingData.products[0];

		ticks = product.ticks;
		ticksCount = product.ticksCount;

		image.sprite = Resources.Load<Sprite>("Products/" + product.type.ToString().ToLower() + "_icon");
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
}
