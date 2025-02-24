using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingIcons : MonoBehaviour
{
	private static FlyingIcons _instance;

	[SerializeField] private Camera uiCamera;
	[SerializeField] private Transform storeButton;
	[SerializeField] private Transform experienceIcon;
	[SerializeField] private Transform energyIcon;
	[SerializeField] private Transform currencyIcon;

	[Space]
	[SerializeField] private GameObject sourceFlyingExperience;
	[SerializeField] private GameObject sourceFlyingEnergy;
	[SerializeField] private GameObject sourceFlyingCurrency;
	[SerializeField] private GameObject sourceFlyingProduct;

	private Vector2 screenSize;

	private void Awake()
	{
		_instance = this;
	}

	private void Start()
	{
		screenSize = ((RectTransform)transform).GetSize();
	}

	private void OnDestroy()
	{
		_instance = null;
	}

	public static void ShowExperience(Vector3 fromPos)
	{
		GameObject itemObject = Instantiate<GameObject>(_instance.sourceFlyingExperience, _instance.transform, false);
		itemObject.transform.localPosition = GetPos(fromPos);

		FlyingExperience item = itemObject.GetComponent<FlyingExperience>();
		item.destPos = GetPos(_instance.experienceIcon.position);
	}

	public static void ShowEnergy(Vector3 fromPos)
	{
		GameObject itemObject = Instantiate<GameObject>(_instance.sourceFlyingEnergy, _instance.transform, false);
		itemObject.transform.localPosition = GetPos(fromPos);

		FlyingEnergy item = itemObject.GetComponent<FlyingEnergy>();
		item.destPos = GetPos(_instance.energyIcon.position);
	}

	public static void ShowCurrency(Vector3 fromPos)
	{
		GameObject itemObject = Instantiate<GameObject>(_instance.sourceFlyingCurrency, _instance.transform, false);
		itemObject.transform.localPosition = GetPos(fromPos);

		FlyingCurrency item = itemObject.GetComponent<FlyingCurrency>();
		item.destPos = GetPos(_instance.currencyIcon.position);
	}

	public static void ShowProduct(ProductData productProfile, Vector3 fromPos)
	{
		GameObject itemObject = Instantiate<GameObject>(_instance.sourceFlyingProduct, _instance.transform, false);
		itemObject.transform.localPosition = GetPos(fromPos);
		itemObject.transform.localScale = Vector3.one * 0.7f;

		FlyingProduct item = itemObject.GetComponent<FlyingProduct>();
		item.productProfile = productProfile;
		item.destPos = GetPos(_instance.storeButton.position);
	}

	private static Vector3 GetPos(Vector3 globalPos)
	{
		Vector3 pos = globalPos;
		pos = _instance.uiCamera.WorldToViewportPoint(pos);
		pos = new Vector3((pos.x - 0.5f) * _instance.screenSize.x, (pos.y - 0.5f) * _instance.screenSize.y, 0.0f);
		return pos;
	}
}
